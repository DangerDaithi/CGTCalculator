using System;
using CGTCalculator.Processor;

namespace CGTRSUCalculator
{
    public static class CurrencyStringConvertor
    {
        public static Currency FromString(string currency)
        {
            return currency switch
            {
                "EUR" => Currency.EUR,
                "USD" => Currency.USD,
                "GBP" => Currency.GBP,
                _ => throw new NotSupportedException($"Currency {currency} not supported, available are " +
                                                     $"{Currency.EUR.ToString()}, {Currency.GBP.ToString()}, {Currency.USD.ToString()}")
            };
        }

        public static string ToString(Currency currency)
        {
            return currency switch
            {
                Currency.EUR => "EUR",
                Currency.USD => "USD",
                Currency.GBP => "GBP",
                _ => throw new NotSupportedException($"Currency {currency.ToString()} not supported, available are " +
                                                     $"{Currency.EUR.ToString()}, {Currency.GBP.ToString()}, {Currency.USD.ToString()}")
            };
        }
    }
}