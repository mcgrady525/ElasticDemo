using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticDemo.Entity;
using Elasticsearch.Net;

namespace ElasticDemo.NEST
{
    class Program
    {
        private static ElasticClient client = null;

        static Program()
        {
            //创建es连接，使用SingleNodeConnectionPool
            var uri = new Uri("http://192.168.1.40:9200");
            var pool = new SingleNodeConnectionPool(uri);
            var settings = new ConnectionSettings(pool).DefaultIndex("people");
            client = new ElasticClient(settings);
        }

        static void Main(string[] args)
        {
            //索引文档
            Indexing();

            //搜索文档
            //Searching();
            //SearchingInAllTypes();
            //SearchingInAllIndices();
            //SearchingInAllIndicesAllTypes();
            //SearchingByObjectInitializer();
            //SearchingByLowLevel();
            //SearchingMatchAll();

            //Aggregations();

            Console.ReadKey();
        }

        /// <summary>
        /// 索引文档
        /// </summary>
        static void Indexing()
        {
            ////endpoint：people/person/2
            //var person = new Person
            //{
            //    Id = 2,
            //    FirstName = "Martijn",
            //    LastName = "Laarman"
            //};

            var persons = new List<Person>
            {
                new Person
                {
                    Id= 1,
                    FirstName= "Zhang",
                    LastName= "San"
                },
                new Person
                {
                    Id= 2,
                    FirstName= "Li",
                    LastName= "Si"
                }
            };

            //var indexResponse = client.IndexDocument(person);
            var indexResponse = client.IndexMany(persons);
        }

        static void SearchingMatchAll()
        {
            var searchRS = client.Search<Person>(s=> s.MatchAll());

            var peoples = searchRS.Documents;
        }

        static void Searching()
        {
            //endpoint：people/person/_search
            var searchResponse = client.Search<Person>(s => s
                                    .From(0)
                                    .Size(10)
                                    .Query(q => q.Match(m => m.Field(f => f.FirstName).Query("Martijn"))));

            var people = searchResponse.Documents;
        }

        static void SearchingInAllTypes()
        {
            //endpoint：people/_search
            var searchResponse = client.Search<Person>(s => s
                                    .AllTypes()
                                    .From(0)
                                    .Size(10)
                                    .Query(q => q.Match(m => m.Field(f => f.FirstName).Query("Martijn"))));

            var people = searchResponse.Documents;
        }

        static void SearchingInAllIndices()
        {
            //endpoint：/_all/person/_search
            var searchResponse = client.Search<Person>(s => s
                                    .AllIndices()
                                    .From(0)
                                    .Size(10)
                                    .Query(q => q.Match(m => m.Field(f => f.FirstName).Query("Martijn"))));

            var people = searchResponse.Documents;
        }

        static void SearchingInAllIndicesAllTypes()
        {
            //endpoint：/_search
            var searchResponse = client.Search<Person>(s => s
                                    .AllIndices()
                                    .AllTypes()
                                    .From(0)
                                    .Size(10)
                                    .Query(q => q.Match(m => m.Field(f => f.FirstName).Query("Martijn"))));

            var people = searchResponse.Documents;
        }

        static void SearchingByObjectInitializer()
        {
            var searchRequest = new SearchRequest<Person>(Nest.Indices.All, Types.All)
            {
                From = 0,
                Size = 10,
                Query = new MatchQuery
                {
                    Field = Infer.Field<Person>(f => f.FirstName),
                    Query = "Martijn"
                }
            };

            var rs = client.Search<Person>(searchRequest);
        }

        static void SearchingByLowLevel()
        {
            var searchResponse = client.LowLevel.Search<SearchResponse<Person>>("people", "person", PostData.Serializable(new
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

            var responseJson = searchResponse;
        }

        static void Aggregations()
        {
            var rs = client.Search<Person>(s => s
                 .Size(0)
                 .Query(q => q.Match(m => m.Field(f => f.FirstName).Query("Martijn")))
                 .Aggregations(a => a.Terms("last_names", t => t.Field(f => f.LastName))));

            var termsAggregation = rs.Aggregations.Terms("last_names");
        }
    }
}
