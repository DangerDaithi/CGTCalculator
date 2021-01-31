using System;
using CGTCalculator.Processor;
using Xunit;

namespace CGTRSUCalculator.Test
{
    public class CurrencyStringConvertorTest
    {
        [Fact]
        public void FromString_UsingSupportedCurrencyStrings_ConversionIsSuccessful()
        {
            Assert.Equal(Currency.EUR, CurrencyStringConvertor.FromString("EUR"));
            Assert.Equal(Currency.USD, CurrencyStringConvertor.FromString("USD"));
            Assert.Equal(Currency.GBP, CurrencyStringConvertor.FromString("GBP"));
        }
        
        [Fact]
        public void FromString_UsingUnSupportedCurrencyString_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => CurrencyStringConvertor.FromString("Imperial credit"));
        }
        
        [Fact]
        public void ToString_UsingSupportedCurrency_ConversionIsSuccessful()
        {
            Assert.Equal("EUR", CurrencyStringConvertor.ToString(Currency.EUR));
            Assert.Equal("USD", CurrencyStringConvertor.ToString(Currency.USD));
            Assert.Equal("GBP", CurrencyStringConvertor.ToString(Currency.GBP));
        }
        
        [Fact]
        public void ToString_UsingUnSupportedCurrency_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => CurrencyStringConvertor.ToString(Currency.UNKOWN));
        }
    }
}