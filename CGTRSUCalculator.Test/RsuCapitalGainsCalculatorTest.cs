using System;
using System.Threading.Tasks;
using CGTCalculator.Processor;
using CGTCalculator.Processor.CapitalGainsCalculator;
using CGTCalculator.Processor.ExchangeRatesClient;
using Moq;
using Moq.Protected;
using Xunit;

namespace CGTRSUCalculator.Test
{
    public class RsuCapitalGainsCalculatorTest
    {
        private const double CurrencyExchangeRate = 0.9094;

        [Fact]
        public async Task CalculateChargeableGains_UsingValidOrderDetailsWithHistoricalSellDate_ReturnsValidRsuChargeableGainResult()
        {
            var exchangeRateClientMock = new Mock<IExchangeRateClient>();
            exchangeRateClientMock.Setup(client => 
                client.GetExchangeRate(It.IsAny<DateTime>(), It.IsAny<Currency>(), It.IsAny<Currency>()))
                .Returns(Task.FromResult(CurrencyExchangeRate));
            
            var rsuCgtCalculator = new RsuCapitalGainsCalculator(exchangeRateClientMock.Object);

            var orderDetails = new CapitalGainsOrderDetails()
            {
                BuyPrice = 89.56,
                SellPrice = 98.21,
                SellDate = new DateTime(2019, 09, 23),
                NumOfAssetsSold = 21,
                Currency = Currency.USD
            };

            var actual = (RsuCalculatorResult)await rsuCgtCalculator.CalculateChargeableGains(orderDetails, Currency.EUR);
            Assert.Equal(165.19251, actual.ChargeableGain, 4);
            Assert.Equal(1104.80749, actual.RemainingPersonalThreshold, 4);
        }
        
        [Fact]
        public async Task CalculateChargeableGains_UsingValidOrderDetailsWithHistoricalSellDate_SetPersonalThreshold_ReturnsValidRsuChargeableGainResult()
        {
            const int remainingPersonalThreshold = 500;
            var exchangeRateClientMock = new Mock<IExchangeRateClient>();
            exchangeRateClientMock.Setup(client => 
                    client.GetExchangeRate(It.IsAny<DateTime>(), It.IsAny<Currency>(), It.IsAny<Currency>()))
                .Returns(Task.FromResult(CurrencyExchangeRate));
            
            var rsuCgtCalculator = new RsuCapitalGainsCalculator(exchangeRateClientMock.Object, remainingPersonalThreshold);

            var orderDetails = new CapitalGainsOrderDetails()
            {
                BuyPrice = 89.56,
                SellPrice = 98.21,
                SellDate = new DateTime(2019, 09, 23),
                NumOfAssetsSold = 21,
                Currency = Currency.USD
            };

            var actual = (RsuCalculatorResult)await rsuCgtCalculator.CalculateChargeableGains(orderDetails, Currency.EUR);
            Assert.Equal(165.19251, actual.ChargeableGain, 4);
            Assert.Equal(334.80749, actual.RemainingPersonalThreshold, 4);
        }
    }
}