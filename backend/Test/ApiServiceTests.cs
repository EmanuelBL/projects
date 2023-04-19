using NUnit.Framework;
using Moq;
using System.Net;
using Newtonsoft.Json;
using Moq.Protected;
using backend.Models;
using backend.Services;


/// <summary>
/// /No anda
/// </summary>


[TestFixture]
public class ApiServiceTests
{
    private Mock<IConfiguration> _configurationMock;
    private ApiService _apiService;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x.GetValue<string>("symbols")).Returns("BTC,ETH");
        _configurationMock.Setup(x => x.GetValue<string>("url")).Returns("https://api.example.com");
        _configurationMock.Setup(x => x.GetValue<string>("money")).Returns("USD");

        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        _apiService = new ApiService(_configurationMock.Object)
        {
            HttpClient = httpClient
        };
    }

    [Test]
    public async Task GetCoins_ReturnsListOfCoins_WhenResponseIsSuccess()
    {
       
        var expectedCoins = new List<Coin>
        {
            new Coin { Symbol = "BTC", Name = "Bitcoin", Price = 50000 },
            new Coin { Symbol = "ETH", Name = "Ethereum", Price = 2000 }
        };

        var responseContent = new
        {
            status = new { timestamp = "", error_code = 0, error_message = (string?)null },
            data = new Dictionary<string, Coin>
            {
                { "BTC", expectedCoins[0] },
                { "ETH", expectedCoins[1] }
            }
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(responseContent))
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        
        var result = await _apiService.GetCoins("API_KEY");

      
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedCoins.Count, result.Count);
        Assert.AreEqual(expectedCoins[0].Symbol, result[0].Symbol);
        Assert.AreEqual(expectedCoins[0].Name, result[0].Name);
        Assert.AreEqual(expectedCoins[0].Price, result[0].Price);
        Assert.AreEqual(expectedCoins[1].Symbol, result[1].Symbol);
        Assert.AreEqual(expectedCoins[1].Name, result[1].Name);
        Assert.AreEqual(expectedCoins[1].Price, result[1].Price);
    }

    [Test]
    public async Task GetCoins_ReturnsNull_WhenResponseIsNotSuccess()
    {
        
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

      
        var result = await _apiService.GetCoins("API_KEY");

      
        Assert.IsNull(result);
    }
}