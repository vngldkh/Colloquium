using IHW4.DBManagers;
using Microsoft.AspNetCore.Mvc;

namespace IHW4.Controllers
{
    /// <summary>
    /// Контроллер.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class CurrencyController : Controller
    {
        /// <summary>
        /// Получение информации о курсе валют
        /// </summary>
        /// <returns> Результат запроса </returns>
        [HttpGet("rates")]
        public IActionResult Get()
        {
            var rates = CurrencyDBManager.GetList();
            return new OkObjectResult(rates);
        }
        
        /// <summary>
        /// Преобразование валюты
        /// </summary>
        /// <returns> Результат запроса </returns>
        [HttpPost("exchange")]
        public IActionResult Post(string from, string to, decimal value)
        {
            if (from == null || to == null)
            {
                return new BadRequestObjectResult("Недостаточно данных");
            }

            var cr = CurrencyDBManager.GetRate(from, to);
            if (cr == null)
            {
                return new NotFoundObjectResult("Информация о курсе отсутствует");
            }
            return new OkObjectResult(value * cr.Rate);
        }
    }
}
