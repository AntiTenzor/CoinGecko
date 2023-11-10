using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using CoinGecko.Clients;
using CoinGecko.Entities.Response;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Entities.Response.Shared;
using CoinGecko.Entities.Response.Simple;
using CoinGecko.Entities.Response.Exchanges;
using CoinGecko.Entities.Response.Derivatives;

namespace TryCoinGecko
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CoinGeckoClient client = CoinGeckoClient.Instance;

            Console.WriteLine("Pinging...");
            Ping resp = client.PingClient.GetPingAsync().Result;
            // (V3) To the Moon!
            Console.WriteLine("Ping response: {0}", resp.GeckoSays);
            Console.WriteLine();
            Console.ReadLine();

            Console.WriteLine("Get supported currencies...");
            SupportedCurrencies currs = client.SimpleClient.GetSupportedVsCurrencies().Result;
            // There are 61 supported currencies:
            Console.WriteLine("There are {0} supported currencies:", currs.Count);
            foreach (string curr in currs)
            {
                Console.WriteLine("  {0}", curr);
            }
            Console.WriteLine();
            Console.ReadLine();

            Console.WriteLine("Get supported exchanges...");
            List<Exchanges> exchanges = new List<Exchanges>(1024);
            {
                IReadOnlyList<Exchanges> exchangesTmp = client.ExchangesClient.GetExchanges(250, "0").Result;
                exchanges.AddRange(exchangesTmp);
                exchangesTmp = client.ExchangesClient.GetExchanges(250, "1").Result;
                exchanges.AddRange(exchangesTmp);
                exchangesTmp = client.ExchangesClient.GetExchanges(250, "2").Result;
                exchanges.AddRange(exchangesTmp);
                exchangesTmp = client.ExchangesClient.GetExchanges(250, "3").Result;
                exchanges.AddRange(exchangesTmp);
                exchangesTmp = client.ExchangesClient.GetExchanges(250, "4").Result;
                exchanges.AddRange(exchangesTmp);
            }
            Console.WriteLine("There are {0} supported exchanges:", exchanges.Count);
            foreach (Exchanges exch in exchanges)
            {
                if (exch.Description == null)
                    exch.Description = String.Empty;

                string desc = exch.Description.Substring(0, Math.Min(exch.Description.Length, 32));
                Console.WriteLine("  id:{0}; url:{1}; est:{2};   desc: {3}",
                    exch.Id, exch.Url, exch.YearEstablished?.ToString() ?? "NULL", desc);
            }
            // There are 943 supported exchanges:
            Console.WriteLine("There are {0} supported exchanges!!!", exchanges.Count);

            List<Exchanges> suppExchanges = new List<Exchanges>();
            // id:okex; url:https://www.okx.com; est:2017;
            List<Exchanges> et2 = (from e in exchanges where e.Id.Equals("okex") select e).ToList();
            Exchanges exchange_okex = (from e in et2 where e.Id.Equals("okex") select e).Single();

            // id:binance; url:https://www.binance.com/; est:2017;
            Exchanges exchange_binance = (from e in exchanges where e.Id.Equals("binance") select e).Single();
            // id:huobi; url:https://www.huobi.com; est:2013;
            Exchanges exchange_huobi = (from e in exchanges where e.Id.Equals("huobi") select e).Single();
            // id:bitfinex; url:https://www.bitfinex.com;
            Exchanges exchange_bitfinex = (from e in exchanges where e.Id.Equals("bitfinex") select e).Single();
            // 
            Exchanges exchange_bybitSpot = (from e in exchanges where e.Id.Equals("bybit_spot") select e).SingleOrDefault();
            // 
            Exchanges exchange_deribit = (from e in exchanges where e.Id.Equals("deribit") select e).SingleOrDefault();
            suppExchanges.AddRange(new[] { exchange_okex, exchange_binance, exchange_bitfinex });
            Console.WriteLine();
            Console.ReadLine();


            Console.WriteLine("Get supported DERIVATIVE exchanges...");
            List<DerivativesExchangesList> derivExchanges = new List<DerivativesExchangesList>(1024);
            {
                IReadOnlyList<DerivativesExchangesList> exchangesTmp = client.DerivativesClient.GetDerivativesExchangesList().Result;
                derivExchanges.AddRange(exchangesTmp);                
            }
            Console.WriteLine("There are {0} supported DERIVATIVE exchanges:", derivExchanges.Count);
            foreach (DerivativesExchangesList exch in derivExchanges)
            {
                Console.WriteLine("  id:{0}; name: {1}", exch.Id, exch.Name);
            }
            // There are 943 supported exchanges:
            Console.WriteLine("There are {0} supported DERIVATIVE exchanges!!!", exchanges.Count);










            Console.WriteLine("List all supported coins id, name and symbol (no pagination required)...");
            IReadOnlyList<CoinList> coins = client.CoinsClient.GetCoinList().Result;
            // There are 10_827 supported coins:
            Console.WriteLine("There are {0} supported coins. Only BITCOIN, TETHER will be displayed:", coins.Count);
            foreach (CoinList cl in coins)
            {
                if ((!cl.Id.StartsWith("bitcoin", StringComparison.InvariantCultureIgnoreCase)) &&
                    (!cl.Id.StartsWith("tether", StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                Console.WriteLine("  id:{0}; sym:{1}; name:{2}; ", cl.Id, cl.Symbol, cl.Name);
                if (cl.Platforms != null)
                {
                    foreach (var kvp in cl.Platforms)
                    {
                        Console.WriteLine("    key:{0}; val:{1}", kvp.Key, kvp.Value);
                    }
                }
            }
            Console.WriteLine();

            CoinList coinList_bitcoin = (from cl in coins
                                         where cl.Id.Equals("bitcoin", StringComparison.InvariantCultureIgnoreCase)
                                         select cl).Single();
            Console.WriteLine("  BITCOIN id:{0}; sym:{1}; name:{2}; ", coinList_bitcoin.Id, coinList_bitcoin.Symbol, coinList_bitcoin.Name);
            Console.WriteLine();
            Console.ReadLine();

            Console.WriteLine("Get BITCOIN tickers...");
            //TickerById bitcoinTickers = client.CoinsClient.GetTickerByCoinId(coinList_bitcoin.Id, new[] { "okex" }, null).Result;
            //TickerById bitcoinTickers = client.CoinsClient.GetTickerByCoinId(coinList_bitcoin.Id).Result;
            //binance,okex,bybit,bitfinex,deribit
            TickerById bitcoinTickers = client.CoinsClient.GetTickerByCoinId(coinList_bitcoin.Id,
                new[] { "binance", "okex", "bybit", "bitfinex", "deribit" }, null).Result;
            Console.WriteLine("There are {0} BITCOIN tickers. Only TETHER, USD, USD-COIN will be displayed:", bitcoinTickers.Tickers.Length);
            foreach (Ticker tick in bitcoinTickers.Tickers)
            {
                if ((!"USD".Equals(tick.Target, StringComparison.InvariantCultureIgnoreCase)) &&
                    (!"USDT".Equals(tick.Target, StringComparison.InvariantCultureIgnoreCase)) &&
                    (!"USDC".Equals(tick.Target, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                string last = tick.Last?.ToString("0.########################") ?? "NULL";
                Console.WriteLine("  exch:{0}; base:{1}; target:{2}; last:{3};   url:{4}",
                    tick.Market.Identifier, tick.Base, tick.Target, last, tick.TradeUrl);
            }
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
