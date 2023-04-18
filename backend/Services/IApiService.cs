using System;
using backend.Models;

namespace backend.Services
{
    public interface IApiService
    {
        public Task<IList<Coin>> GetCoins(string apiKey);
        public Task<IList<Coin>> ConvertCryptoCurrency(
            string apikey,
            string fromSymbol,
            decimal amount
        );
    }
}
