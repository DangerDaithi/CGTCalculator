using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CGTCalculator.Processor.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CGTCalculator.Processor.ExchangeRatesClient
{
    public class ExchangeApiIoHttpClient : IExchangeRateClient
    {
       
        private static readonly ILogger Logger = ApplicationLoggerFactory.CreateLogger<ExchangeApiIoHttpClient>();

        private readonly HttpClient _client;

        /// <summary>
        /// Using the free web api provided by https://exchangeratesapi.io/, no authentication tokens required,
        /// see https://github.com/exchangeratesapi/exchangeratesapi for details
        /// </summary>
        private const string BaseExchangeRatesApiUrl = "https://api.exchangeratesapi.io/";

        public ExchangeApiIoHttpClient(HttpClient client = null)
        {
            _client = client ?? new HttpClient();
        }
        
        public async Task<double> GetExchangeRate(Currency from, Currency to)
        {
            return await GetExchangeRateInternal($"{BaseExchangeRatesApiUrl}latest?base={from}&symbols={to.ToString()}", to);
        }

        public async Task<double> GetExchangeRate(DateTime historicalDate, Currency from, Currency to)
        {
            // url should look like https://api.exchangeratesapi.io/2010-01-12?base=USD&symbols=EUR
            return await GetExchangeRateInternal(
                $"{BaseExchangeRatesApiUrl}{historicalDate:yyyy-MM-dd}?base={from}&symbols={to.ToString()}", to);
        }
        
        private async Task<double> GetExchangeRateInternal(string url, Currency to)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                Logger.LogInformation($"Exchange rates received: {responseBody}");
                var exchangeResponse = JsonConvert.DeserializeObject<ExchangeApiIoResponse>(responseBody);
                return exchangeResponse.Rates[to.ToString()];
            }
            catch (Exception e)
            {
                throw new CgtCalculatorProcessorException(
                    $"Failed to retrieve exchange rate from {BaseExchangeRatesApiUrl}", e);
            }
        }

       
    }
}