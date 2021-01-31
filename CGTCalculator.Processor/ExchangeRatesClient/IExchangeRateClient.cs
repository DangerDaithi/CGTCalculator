using System;
using System.Threading.Tasks;

namespace CGTCalculator.Processor.ExchangeRatesClient
{
    /// <summary>
    /// An exchange rate client where the base currency is already known
    /// </summary>
    public interface IExchangeRateClient
    {
        /// <summary>
        /// Get the latest currency exchange rate
        /// </summary>
        /// <param name="from">the currency to convert from</param>
        /// <param name="to">the currency to convert too</param>
        /// <returns>the currency exchange</returns>
        Task<double> GetExchangeRate(Currency from, Currency to);

        /// <summary>
        /// Get the currency exchange rate for a given date in the past
        /// </summary>
        /// <param name="historicalDate">the historical date to get exchange rate</param>
        /// <param name="from">the currency to convert from</param>
        /// <param name="to">the currency to convert too</param>
        /// <returns>the currency exchange</returns>
        Task<double> GetExchangeRate(DateTime historicalDate, Currency from, Currency to);
    }
}