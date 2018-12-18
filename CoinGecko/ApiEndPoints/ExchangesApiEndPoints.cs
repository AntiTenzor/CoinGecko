namespace CoinGecko.ApiEndPoints
{
    public static class ExchangesApiEndPoints
    {
        public const string Exchanges = "exchanges";
        public static string ExchangeById(string id) => "exchanges/"+id;
        public static string TickerById(string id) => "exchanges/"+id+"/tickers";
    }
}