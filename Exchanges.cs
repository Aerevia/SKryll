using System;
using System.Collections.Generic;
using System.Text;

namespace SKryll
{
    public class Exchange
    {
        private Exchange(string value) { Value = value; }

        public string Value { get; set; }

        public static Exchange Binance { get { return new Exchange("Binance"); } }
        public static Exchange FTX { get { return new Exchange("FTX"); } }
        public static Exchange KuCoin { get { return new Exchange("KuCoin"); } }
        public static Exchange BinanceUS { get { return new Exchange("Binance US"); } }
        public static Exchange Bittrex { get { return new Exchange("Bittrex"); } }
        public static Exchange HitBTC { get { return new Exchange("HitBTC"); } }
        public static Exchange Liquid { get { return new Exchange("Liquid"); } }
    }
}
