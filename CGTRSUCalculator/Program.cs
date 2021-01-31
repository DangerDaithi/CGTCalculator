using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGTCalculator.Processor;
using CGTCalculator.Processor.CapitalGainsCalculator;
using CGTCalculator.Processor.ExchangeRatesClient;
using CGTCalculator.Processor.Logging;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace CGTRSUCalculator
{
    class Program
    {
        private static readonly ILogger Logger = ApplicationLoggerFactory.CreateLogger<Program>();
        
        private class RsuCgtCommandLineOptions
        {

            [Option("buy-price", HelpText = "The price the asset was bought (vested in case of RSU)", Required = true)]
            public double BuyPrice { get; set; } = 0.00;
            
            [Option("sell-price", HelpText = "The price the asset was sold", Required = true)]
            public double SellPrice { get; set; } = 0.00;
            
            [Option("sell-date", HelpText = "The date the asset was sold. Must be in format DD-MM-YYYY or DD/MM/YYYY", Required = true)]
            public string SellDate { get; set; } = null;

            [Option("number-of-assets-sold", HelpText = "The number of assets that were sold in the order. Default is 1.")]
            public int NumberOfAssetsSold { get; set; } = 1;
            
            [Option("convert-from-currency", HelpText = "The currency the order was completed in. Default is USD. Supported currencies are EUR, USD, GBP")]
            public string ConvertFrom { get; set; } = "USD";
            
            [Option("convert-to-currency", HelpText = "The currency to convert the capital gains result too. Default is EUR. Supported currencies are EUR, USD, GBP")]
            public string ConvertTo { get; set; } = "EUR";
            
            [Option("remaining-personal-threshold", HelpText = "The remaining personal threshold (defaults to 1270 if not specified)")]
            public double RemainingPersonalThreshold { get; set; } = 1270;
        }
        
        static async Task Main(string[] args)
        {
            Logger.LogInformation("Restricted Stock Unit Capital Gains Tax Calculator." +
                                  "\n All exchange rates are acquired from the European Central Bank via the free https://exchangeratesapi.io/ web service." +
                                  "\n Every reasonable attempt has been made to test the application however, this is not an official piece of software. " +
                                  "\n Use at your own discretion and risk." +
                                  "\n Always seek tax advice from a tax professional." +
                                  "\n See for more details https://www.revenue.ie/en/gains-gifts-and-inheritance/transfering-an-asset/how-to-calculate-cgt.aspx");

            var convertToo = Currency.UNKOWN;
            var remainingPersonalThreshold = 0.0;
            
            // parse args
            var orderDetails = new CapitalGainsOrderDetails();
            var argsParsedSuccess = false;
            Parser.Default.ParseArguments<RsuCgtCommandLineOptions>(args)
                .WithParsed(o =>
                {
                    orderDetails.BuyPrice = o.BuyPrice;
                    orderDetails.SellPrice = o.SellPrice;
                    orderDetails.SellDate = DateTime.Parse(o.SellDate);
                    orderDetails.Currency = CurrencyStringConvertor.FromString(o.ConvertFrom);
                    orderDetails.NumOfAssetsSold = o.NumberOfAssetsSold;
                    convertToo = CurrencyStringConvertor.FromString(o.ConvertTo);
                    remainingPersonalThreshold = o.RemainingPersonalThreshold;
                    argsParsedSuccess = true;
                });

            if (!argsParsedSuccess)
            {
                return;
            }

            PrintOrderDetailsToConsole(orderDetails, convertToo, remainingPersonalThreshold);

            var chargeableGainsCalculator = new RsuCapitalGainsCalculator(new ExchangeApiIoHttpClient(), remainingPersonalThreshold);
            var result = (RsuCalculatorResult)await chargeableGainsCalculator.CalculateChargeableGains(orderDetails, convertToo);
            
            PrintChargeableGainresultToConsole(result, convertToo);
        }

        private static void PrintChargeableGainresultToConsole(RsuCalculatorResult result, Currency convertToo)
        {
            var resultStringBuilder = new StringBuilder();
            resultStringBuilder.AppendLine($"Chargeable gain: {convertToo} {result.ChargeableGain}");
            resultStringBuilder.AppendLine($"Remaining personal threshold: {convertToo} {result.RemainingPersonalThreshold}");
            Logger.LogInformation(resultStringBuilder.ToString());
        }

        private static void PrintOrderDetailsToConsole(CapitalGainsOrderDetails orderDetails, Currency convertToo, double remainingPersonalThreshold)
        {
            // print order details to console
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Following order details parsed from command line:");
            stringBuilder.AppendLine($"Buy Price: {orderDetails.BuyPrice}");
            stringBuilder.AppendLine($"Sell Price: {orderDetails.SellPrice}");
            stringBuilder.AppendLine($"Sell Date: {orderDetails.SellDate}");
            stringBuilder.AppendLine($"Number of assets sold: {orderDetails.NumOfAssetsSold}");
            stringBuilder.AppendLine($"Transaction Currency: {orderDetails.Currency}");
            stringBuilder.AppendLine($"Capital gains will be converted too: {convertToo.ToString()}");
            stringBuilder.AppendLine($"Remaining personal threshold: {convertToo} {remainingPersonalThreshold}");
            Logger.LogInformation(stringBuilder.ToString());
        }
    }
}