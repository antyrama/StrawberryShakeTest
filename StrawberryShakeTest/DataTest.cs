using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using StrawberryShake.Serialization;
using StrawberryShakeTest.Pim;
using System.Net;

namespace StrawberryShakeTest
{
    public class DataTest
    {
        [Fact]
        public async Task Test()
        {
            // arrange
            var response = EmbeddedFile.GetContent("InterceptedResponseBody.json");
            var instance = CreateClientInstance(response);

            // act
            var shakeResult = await instance.GetShapesByTenant.ExecuteAsync("whatever");
            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(response);

            // assert
            var shakeProduct = shakeResult.Data!.Shape.GetMany!.First(x => x.Identifier == "product");
            var shakeComponents = shakeProduct.Components!.First(x => x.Id == "data-from-sap");

            var jsonProduct = jsonResult.Data.Shape.GetMany.First(x => x.Identifier == "product");
            var jsonComponents = jsonProduct.Components.First(x => x.Id == "data-from-sap");

            shakeComponents.Should().BeEquivalentTo(jsonComponents);
        }

        private static IPimClient CreateClientInstance(string response)
        {
            var handlerMock = CreateMessageHandlerMock(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var services = new ServiceCollection();
            services.AddSingleton(factoryMock.Object);
            services.AddSerializer(new JsonSerializer("JSON"));
            services.AddPimClient();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IPimClient>();
        }

        private static Mock<HttpMessageHandler> CreateMessageHandlerMock(string response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response),
                })
                .Verifiable();

            handlerMock.Protected()
                .Setup(
                    "Dispose",
                    ItExpr.IsAny<bool>()
                )
                .Verifiable();
            return handlerMock;
        }
    }
}