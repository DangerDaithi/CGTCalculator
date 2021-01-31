using System;
using System.Threading.Tasks;

namespace CGTCalculator.Processor.CapitalGainsCalculator
{
    public interface ICapitalGainsCalculator
    {
        Task<CapitalGainsCalculatorResult> CalculateChargeableGains(CapitalGainsOrderDetails orderDetails, Currency convertTo);
    }
}