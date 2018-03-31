using System;
using Realms;

namespace Slink
{
    public class ExchangeTransaction
    {
        public string UUID { get; set; }
        public SlinkUser Person { get; set; }
        public double Latitidue { get; set; }
        public double Longitude { get; set; }
        public string AccessToken { get; set; }
        public Card Card { get; set; }
        public DateTimeOffset Time { get; set; }
        public bool Saved { get; set; }
    }
}