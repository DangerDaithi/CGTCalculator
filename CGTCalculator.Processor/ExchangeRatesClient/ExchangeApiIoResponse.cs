using System.Collections.Generic;

namespace CGTCalculator.Processor.ExchangeRatesClient
{
    public class ExchangeApiIoResponse
    {
        public Dictionary<string, double> Rates { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
    }
}