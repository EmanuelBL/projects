using System;
using backend.Services;
using backend.Errors;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class CoinCurrencyController : ControllerBase
    {
        private readonly IApiService Service;

        public CoinCurrencyController(IApiService apiService)
        {
            Service = apiService;
            //string apiKey = "9454bad2-5287-4f52-bbb4-d518a6339544";
        }

        [HttpGet]
        [Route("getcoins/{apiKey}")]
        public async Task<IActionResult> GetCoins(string apiKey)
        {
            try
            {
                // Obtener las últimas cotizaciones de las monedas
                var CurrencyCoins = await Service.GetCoins(apiKey);
                if (CurrencyCoins != null)
                    return StatusCode(200, CurrencyCoins);

                return StatusCode(409, "No se ha podido establecer la consulta");
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.Response.StatusCode, $"Ha ocurrido un error en la Api : {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"Ha ocurrido un Error en tiempo de ejecucion : {ex.Message}"
                );
            }
        }

        [HttpGet]
        [Route("getconversion/{apiKey}/{fromSymbol}/{amount}")]
        public async Task<IActionResult> ConvertCryptoCurrency(string apiKey, string fromSymbol, decimal amount)
        {
            try
            {
                var convertedCoins = await Service.ConvertCryptoCurrency(apiKey, fromSymbol, amount);

                if (convertedCoins != null)
                    return StatusCode(200, convertedCoins);

                return StatusCode(409, "No se ha podido establecer la consulta");
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.Response.StatusCode, $"Ha ocurrido un error en la Api : {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"Ha ocurrido un Error en tiempo de ejecucion : {ex.Message}"
                );
            }
        }
    }
}
