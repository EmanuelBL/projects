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
    HttpClient HttpClient;
    string Url;
    string Symbol;
    IList<Coin> CoinList;
    string[]? Coins;
    string Name;
    decimal? Price;
    HttpResponseMessage Response;
    string Money;

    public ApiService(IConfiguration c)
    {
        Symbol = c.GetValue<string>("symbols");
        Url = c.GetValue<string>("url");
        Coins = Symbol.Split(",");
        HttpClient = new();
        Money = c.GetValue<string>("money");
    }

    public async Task<IList<Coin>> GetCoins(string apiKey)
    {
        Url +=
            $"/cryptocurrency/quotes/latest?CMC_PRO_API_KEY={apiKey}&symbol={Symbol}&convert=USD";

        try
        {
            Response = await HttpClient.GetAsync(Url);

            if (Response.IsSuccessStatusCode)
            {
                CoinList = new List<Coin>();
                string responseBody = await Response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseBody))
                {
                    dynamic coinData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    foreach (var item in Coins)
                    {
                        Name = coinData.data[item][0].name.ToString();
                        Price = decimal.Parse(
                            coinData.data[item][0].quote[Money].price.Value.ToString()
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
                    return CoinList;
                }
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(Response, "Error en la solicitud Http", ex);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            HttpClient.Dispose();
        }
    }

    public async Task<IList<Coin>> ConvertCryptoCurrency(
        string apiKey,
        string fromSymbol,
        decimal amount
    )
    {
        try
        {
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

                        Name = coinData.data[0].name.ToString();
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
                Url = Url.Replace($"convert={item}", "convert=");
            }
            return CoinList.Count > 0
                ? CoinList.Where(coin => coin.Symbol != fromSymbol).ToList()
                : null;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(Response, "Error en la solicitud Http", ex);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            HttpClient.Dispose();
        }
    }
}
