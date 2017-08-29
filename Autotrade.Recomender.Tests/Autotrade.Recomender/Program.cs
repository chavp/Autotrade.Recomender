using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            decimal? previese_price = null;
            decimal current_price = 0;
            decimal next_price = 0;
            string direction = "";
            string data_file = DateTime.Today.ToString("ddMMyyyy");

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
                    data_file = Path.Combine(DateTime.Today.ToString("ddMMyyyy")) + ".csv";
                    if (!File.Exists(data_file))
                    {
                        File.AppendAllText(data_file, "TIME, DIRECTION, CURRENT_PRICE" + Environment.NewLine);
                        previese_price = -1;
                    }
                    else if(previese_price == null)
                    {
                        using (var reader = new CsvReader(File.OpenText(data_file)))
                        {
                            reader.Configuration.IgnoreHeaderWhiteSpace = true;
                            var lastRecord = reader.GetRecords<PricingData>().LastOrDefault();
                            previese_price = lastRecord.CURRENT_PRICE;
                        }
                    }
                    var re = response.Result.Content.ReadAsAsync<CapfeedResult>().Result;
                    //var data = JsonConvert.DeserializeObject<dynamic>(re);
                    foreach (var item in re.result)
                    {
                        current_price = item.usdPrice;
                        var diffPrice = current_price - previese_price;
                        if (diffPrice > 0)
                        {
                            direction = "+";
                            if (previese_price == -1) direction = "*";
                            Console.Write(direction);
                            File.AppendAllText(data_file, getdata(direction, current_price));
                        }
                        else if (diffPrice < 0)
                        {
                            direction = "-";
                            Console.Write(direction);
                            File.AppendAllText(data_file, getdata(direction, current_price));
                        }

                        previese_price = current_price;
                        
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
        }

        private static string getdata(string direction, decimal current_price)
        {
            return string.Format(
            "{0}, {1}, {2}" + Environment.NewLine, 
            DateTime.Now.TimeOfDay, direction, current_price);
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

        public struct PricingData
        {
            public TimeSpan TIME { get; set; }
            public string DIRECTION { get; set; }
            public decimal CURRENT_PRICE { get; set; }
        }
    }
}
