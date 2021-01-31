using System;

namespace CGTCalculator.Processor.CapitalGainsCalculator
{
    public class CapitalGainsOrderDetails
    {
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public DateTime SellDate { get; set; }
        public Currency Currency { get; set; } = Currency.USD;
        public int NumOfAssetsSold { get; set; } = 1;
    }
}