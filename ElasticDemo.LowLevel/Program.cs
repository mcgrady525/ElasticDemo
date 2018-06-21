using ElasticDemo.Entity;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDemo.LowLevel
{
    class Program
    {
        private static ElasticLowLevelClient client = null;
        
        static Program()
        {
            //创建es连接
            var settings = new ConnectionConfiguration(new Uri("http://192.168.1.40:9200")).RequestTimeout(TimeSpan.FromMinutes(2));
            client = new ElasticLowLevelClient(settings);
        }


        static void Main(string[] args)
        {
            //索引文档
            Indexing();

            //批量索引
            //BulkIndexing();

            //基本搜索
            //Searching();

            Console.ReadKey();
        }

        static void Indexing()
        {
            var person = new Person
            {
                FirstName = "Martijn",
                LastName = "Laarman"
            };

            var indexResponse = client.Index<BytesResponse>("people", "person", "1", PostData.Serializable(person));
            byte[] responseBytes = indexResponse.Body;
        }

        static void BulkIndexing()
        {
            var peoples = new object[]
            {
                new { index = new { _index = "people", _type = "person", _id = "1"  }},
                new { FirstName = "Martijn", LastName = "Laarman" },
                new { index = new { _index = "people", _type = "person", _id = "2"  }},
                new { FirstName = "Greg", LastName = "Marzouka" },
                new { index = new { _index = "people", _type = "person", _id = "3"  }},
                new { FirstName = "Russ", LastName = "Cam" },
            };

            var indexResponse = client.Bulk<StringResponse>(PostData.MultiJson(peoples));
            string responseStream = indexResponse.Body;
        }

        static void Searching()
        {
            var searchResponse = client.Search<StringResponse>("people", "person", PostData.Serializable(new
            {
                from = 0,
                size = 10,
                query = new
                {
                    match = new
                    {
                        field = "firstName",
                        query = "Martijn"
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
        }
    }
}
