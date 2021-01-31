using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CGTCalculator.Processor.ExchangeRatesClient;
using Xunit;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace CGTCalculator.Processor.Test.ExchangeRatesClient
{
    public class ExchangeApiIoHttpClientTest
    {
        [Fact]
        public async Task GetLatestExchangeRate_FromEuro_ToUsd_ReturnsValidExchangeRate()
        {
            var exchangeApiIoResponse = GetExchangeApiIoResponse("EUR", "USD", 0.88, DateTime.Now);
            var response = GetHttpResponseMessage(HttpStatusCode.OK, exchangeApiIoResponse);
            var handlerMock = GetHttpHandlerMock(response);
            var exchangeRateApiIoHttpClient = new ExchangeApiIoHttpClient(new HttpClient(handlerMock.Object));

            var actual = await exchangeRateApiIoHttpClient.GetExchangeRate(Currency.USD, Currency.EUR);
            Assert.Equal(exchangeApiIoResponse.Rates["EUR"], actual);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetExchangeRate_HistoricalRate_FromEuro_ToUsd_ReturnsValidExchangeRate()
        {
            var dateInThePast = new DateTime(2015, 05, 23);
            var exchangeApiIoResponse = GetExchangeApiIoResponse("EUR", "USD", 0.88, dateInThePast);
            var response = GetHttpResponseMessage(HttpStatusCode.OK, exchangeApiIoResponse);
            var handlerMock = GetHttpHandlerMock(response);
            var exchangeRateApiIoHttpClient = new ExchangeApiIoHttpClient(new HttpClient(handlerMock.Object));

            var actual = await exchangeRateApiIoHttpClient.GetExchangeRate(dateInThePast, Currency.USD, Currency.EUR);

            Assert.Equal(exchangeApiIoResponse.Rates["EUR"], actual);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }
        
        [Fact]
        public async Task GetLatestExchangeRate_ApiReturnsServerError_CgtCalculatorExceptionIsThrown()
        {
            var dateInThePast = new DateTime(2015, 05, 23);
            var exchangeApiIoResponse = GetExchangeApiIoResponse("EUR", "USD", 0.88, dateInThePast);
            var response = GetHttpResponseMessage(HttpStatusCode.InternalServerError, exchangeApiIoResponse);
            var handlerMock = GetHttpHandlerMock(response);
            var exchangeRateApiIoHttpClient = new ExchangeApiIoHttpClient(new HttpClient(handlerMock.Object));

            await Assert.ThrowsAsync<CgtCalculatorProcessorException>(() => exchangeRateApiIoHttpClient.GetExchangeRate(dateInThePast,Currency.USD, Currency.EUR));
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }
        
        [Fact]
        public async Task GetHistoricalExchangeRate_ApiReturnsServerError_CgtCalculatorExceptionIsThrown()
        {
            var exchangeApiIoResponse = GetExchangeApiIoResponse("EUR", "USD", 0.88, DateTime.Now);
            var response = GetHttpResponseMessage(HttpStatusCode.InternalServerError, exchangeApiIoResponse);
            var handlerMock = GetHttpHandlerMock(response);
            var exchangeRateApiIoHttpClient = new ExchangeApiIoHttpClient(new HttpClient(handlerMock.Object));

            await Assert.ThrowsAsync<CgtCalculatorProcessorException>(() => exchangeRateApiIoHttpClient.GetExchangeRate(Currency.USD, Currency.EUR));
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        private ExchangeApiIoResponse GetExchangeApiIoResponse(string from, string to, double exchangeRate,
            DateTime dateOfExchangeRate)
        {
            return new ExchangeApiIoResponse
            {
                Rates = new Dictionary<string, double>()
                {
                    {from, exchangeRate}
                },
                Date = dateOfExchangeRate.ToString(CultureInfo.InvariantCulture),
                Base = to
            };
        }

        private HttpResponseMessage GetHttpResponseMessage(HttpStatusCode httpStatusCode,
            ExchangeApiIoResponse exchangeApiIoResponse)
        {
            return new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = new StringContent(JsonConvert.SerializeObject(exchangeApiIoResponse)),
            };
        }
        
        private Mock<HttpMessageHandler> GetHttpHandlerMock(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            return handlerMock;
        }
    }
}