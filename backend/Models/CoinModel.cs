using System;
using Newtonsoft.Json;

namespace backend.Models
{
    public class Coin
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal? Price { get; set; }
    }
}
