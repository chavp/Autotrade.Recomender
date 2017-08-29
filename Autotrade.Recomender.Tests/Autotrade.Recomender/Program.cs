using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Autotrade.Recomender
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal previes_price = 0;
            decimal current_price = 0;
            decimal next_price = 0;
            string direction = "";
            while (true)
            {
                var client = new HttpClient();

                // New code:
                client.BaseAddress = new Uri("http://capfeed.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync("json?c=ETH");
                response.Wait();
                if (response.IsCompleted)
                {
                    var re = response.Result.Content.ReadAsAsync<CapfeedResult>().Result;
                    //var data = JsonConvert.DeserializeObject<dynamic>(re);
                    direction = ".";
                    foreach (var item in re.result)
                    {
                        current_price = item.usdPrice;
                        if(current_price > previes_price)
                        {
                            direction = "+";
                            Console.Write(direction);
                        }
                        else if (current_price < previes_price)
                        {
                            direction = "-";
                            Console.Write(direction);
                        }
                        
                        previes_price = current_price;
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
        }

        public struct CapfeedResult
        {
            //"mktcap": "32694478411", "supply": "94299480", "change24": "0.7" } ]}
            public bool success { get; set; }
            public string message { get; set; }
            public List<Symbol> result { get; set; }
        }

        public struct Symbol
        {
            public int position { get; set; }
            public string name { get; set; }
            public long time { get; set; }
            public decimal usdPrice { get; set; }
            public decimal btcPrice { get; set; }
            public decimal usdVolume { get; set; }
            public decimal mktcap { get; set; }
            public decimal supply { get; set; }
            public decimal change24 { get; set; }
        }
    }
}
