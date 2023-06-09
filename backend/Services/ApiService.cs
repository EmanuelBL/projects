using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;
using System;
using backend.Models;
using backend.Errors;
using Newtonsoft.Json;
using System.Linq;

namespace backend.Services;

public class ApiService : IApiService
{
    public HttpClient HttpClient;
    string Url;
    string Symbol;
    IList<Coin> CoinList;
    string[]? Coins;
    string Name;
    decimal? Price;
    string Money;
    HttpResponseMessage Response;

    public ApiService(IConfiguration c)
    {
        Symbol = c.GetValue<string>("symbols");
        Url = c.GetValue<string>("url");
        Coins = Symbol.Split(",");
        Money = c.GetValue<string>("money");
    }

    public async Task<IList<Coin>> GetCoins(string apiKey)
    {

        Url +=
            $"/cryptocurrency/quotes/latest?CMC_PRO_API_KEY={apiKey}&symbol={Symbol}&convert=USD";

        try
        {
            HttpClient = new();
            Response = await HttpClient.GetAsync(Url);

            if (Response.IsSuccessStatusCode)
            {
                CoinList = new List<Coin>();
                string responseBody = await Response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseBody))
                {
                    CoinList = GetCoinsList(responseBody);
                }
                return CoinList;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(Response, "Error en la solicitud Http");
        }
        catch (Exception ex)
        {
            throw new ApiException(Response, "Error inesperado", ex);
        }
        finally
        {
            HttpClient.Dispose();
        }
    }
    private IList<Coin> GetCoinsList(string responseBody)
    {
        dynamic coinData = JsonConvert.DeserializeObject<dynamic>(responseBody);

        foreach (var item in Coins)
        {
            Name = coinData.data[item]?[0].name.ToString();// se utiliza el indice cero ya que se examino el Json de la respuesta y ese era el path 

            if (Name != null)
            {
                Price = decimal.Parse(
                    coinData.data[item][0].quote[Money].price.Value.ToString()// se utiliza el indice cero ya que se examino el Json de la respuesta y ese era el path
                );
                CoinList.Add(
                    new Coin
                    {
                        Name = Name,
                        Symbol = item,
                        Price = Price
                    }
                );
            }
        }
        return CoinList;

    }
    public async Task<IList<Coin>> ConvertCryptoCurrency(
        string apiKey,
        string fromSymbol,
        decimal amount
    )
    {
        try
        {
            HttpClient = new();
            CoinList = new List<Coin>();
            Url +=
                $"/tools/price-conversion?CMC_PRO_API_KEY={apiKey}&amount={amount}&symbol={fromSymbol}&convert=";

            foreach (var item in Coins)
            {
                Url = Url.Replace("convert=", $"convert={item}");
                Response = await HttpClient.GetAsync(Url);

                if (Response.IsSuccessStatusCode)
                {
                    string responseBody = await Response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        dynamic coinData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                        Name = coinData.data[0]?.name?.ToString();
                        if (Name != null)
                        {
                            Price = decimal.Parse(coinData.data[0].quote[item].price.Value.ToString());
                            CoinList.Add(
                                new Coin
                                {
                                    Name = Name,
                                    Symbol = item,
                                    Price = Price
                                }
                            );
                        }
                    }
                }
                Url = Url.Replace($"convert={item}", "convert=");
            }
            return CoinList.Count > 0
                ? CoinList.Where(coin => coin.Symbol != fromSymbol).ToList()
                : null;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(Response, "Error en la solicitud Http");
        }
        catch (Exception ex)
        {
            throw new ApiException(Response, "Error inesperado", ex); ;
        }
        finally
        {
            HttpClient.Dispose();
        }

    }
}
