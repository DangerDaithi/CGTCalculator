using System.Threading.Tasks;
using CGTCalculator.Processor;
using CGTCalculator.Processor.CapitalGainsCalculator;
using CGTCalculator.Processor.ExchangeRatesClient;

namespace CGTRSUCalculator
{
    public class RsuCapitalGainsCalculator : ICapitalGainsCalculator
    {
        private readonly IExchangeRateClient _exchangeRateClient;
        private readonly double _remainingPersonalThreshold;
        private const double RemainingPersonalThresholdDefault = 1270;

        public RsuCapitalGainsCalculator(IExchangeRateClient exchangeRateClient,
            double remainingPersonalThreshold = RemainingPersonalThresholdDefault)
        {
            _exchangeRateClient = exchangeRateClient;
            _remainingPersonalThreshold = remainingPersonalThreshold;
        }

        public async Task<CapitalGainsCalculatorResult> CalculateChargeableGains(CapitalGainsOrderDetails orderDetails, Currency convertTo)
        {
            var sellVestDifference = orderDetails.SellPrice - orderDetails.BuyPrice;
            var totalProfit = sellVestDifference * orderDetails.NumOfAssetsSold;
            var exchangeRateOnDayOfOrder = await
                _exchangeRateClient.GetExchangeRate(orderDetails.SellDate, orderDetails.Currency, convertTo);
            var chargeableGain = totalProfit * exchangeRateOnDayOfOrder;
            return new RsuCalculatorResult()
            {
                ChargeableGain = chargeableGain,
                RemainingPersonalThreshold = _remainingPersonalThreshold - chargeableGain
            };
        }
    }
}