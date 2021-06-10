using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPIProject1.Tests
{
    [TestClass]
    public class WeatherForecastControllerTest
    {
        private readonly WebApplicationFactory<APIProjectWithUnitTests.Startup> _factory;
        private readonly Mock<HttpMessageHandler> _httpRequestExceptionMessageHandler = new Mock<HttpMessageHandler>();
        private readonly HttpClient _clientWithMockHttpHandler;

        public WeatherForecastControllerTest()
        {
            _factory = new WebApplicationFactory<APIProjectWithUnitTests.Startup>();

            _httpRequestExceptionMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("This is just a test"));

            _clientWithMockHttpHandler = _factory.WithWebHostBuilder(builder =>
               builder.ConfigureServices(services =>
               {
               })).CreateClient();
        }

        [TestMethod]
        public async Task Get_Success()
        {
            var response = await _clientWithMockHttpHandler.GetAsync($" WeatherForecast");

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var body = await response.Content.ReadAsStringAsync();
            body.Should().NotBeNullOrEmpty();
        }
    }
}
