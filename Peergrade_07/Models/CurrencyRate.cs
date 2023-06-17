using System;

namespace IHW4.Models
{
    /// <summary>
    /// Информация о курсе валют
    /// </summary>
    public class CurrencyRate
    {
        /// <summary>
        /// Исходная валюта
        /// </summary>
        public string From { get; }
        
        /// <summary>
        /// Целевая валюта
        /// </summary>
        public string To { get; }
        
        /// <summary>
        /// Стоимость единицы исходной валюты в конечной
        /// </summary>
        public decimal Rate { get; }

        public CurrencyRate(string name1, string name2, decimal rate)
            => (From, To, Rate) = (name1, name2, rate);
    }
}