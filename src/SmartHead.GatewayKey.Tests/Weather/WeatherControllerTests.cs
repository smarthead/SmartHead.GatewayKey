using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SmartHead.GatewayKey.Example;
using Xunit;

namespace SmartHead.GatewayKey.Tests.Weather
{
    public class WeatherControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private string _forbiddenKey = "forbidden";
        private string _unauthorizedKey = "";
        private string _authorizedKey = "authorized";

        private string _headerName = "Api-Key";
        
        private HttpClient _httpClient;

        public WeatherControllerTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData("forbidden", HttpStatusCode.Forbidden)]
        [InlineData("authorized", HttpStatusCode.OK)]
        [InlineData("", HttpStatusCode.Unauthorized)]
        [InlineData(null, HttpStatusCode.Unauthorized)]
        public async Task Get_ShouldReturn_Forbid_ForDeactivatedKey(string key, HttpStatusCode code)
        { 
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_httpClient.BaseAddress, "weather"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { _headerName, key }
                }
            };
            
            if(key == null)
                request.Headers.Clear();
            
            var response = await _httpClient
                .SendAsync(request);
            
            Assert.Equal(code, response.StatusCode);
        }
    }
}