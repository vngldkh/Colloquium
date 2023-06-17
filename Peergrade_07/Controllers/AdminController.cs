using System.Diagnostics;
using IHW4.DBManagers;
using IHW4.Models;
using Microsoft.AspNetCore.Mvc;

namespace IHW4.Controllers
{
    /// <summary>
    /// Контроллер администратора.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class AdminController : Controller
    {
        /// <summary>
        /// Добавление/изменение курса валют
        /// </summary>
        /// <returns> Результат запроса </returns>
        [HttpPost("update")]
        public IActionResult Post(string from, string to, decimal rate)
        {
            if (from == null || to == null || rate == 0)
            {
                return new BadRequestObjectResult("Недостаточно данных");
            }

            var cr = new CurrencyRate(from, to, rate);
            return CurrencyDBManager.AddCurrencyRate(cr) switch
            {
                -1 => new BadRequestObjectResult("Во время добавления произошла неизвестная ошибка"),
                1 => CurrencyDBManager.UpdateRate(cr)
                    ? new OkObjectResult("Курс был обновлён")
                    : new BadRequestObjectResult("Во время обновления произошла ошибка"),
                _ => new OkObjectResult("Курс был успешно добавлен")
            };
        }
    }
}
