using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Autotrade.Recomender.Tests
{
    [TestClass]
    public class UnitTest1
    {
        string feed_ETH = "http://capfeed.com/json?c=ETH";
        [TestMethod]
        public void TestMethod1()
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

                foreach (var item in re.result)
                {
                    Console.WriteLine(item.today_time);
                }
            }
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
        public TimeSpan today_time
        {
            get
            {
                return new TimeSpan(time);
            }
        }
        public decimal usdPrice { get; set; }
        public decimal btcPrice { get; set; }
        public decimal usdVolume { get; set; }
        public decimal mktcap { get; set; }
        public decimal supply { get; set; }
        public decimal change24 { get; set; }
    }
}
