using MackerelClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace ToMackerel
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string mackerelApiKey = Environment.GetEnvironmentVariable("MACKEREL_API_KEY");
            string path = args.First();

            var info = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.ToLower().EndsWith("mockgui.cs"))
                .Where(f => !f.ToLower().EndsWith("ref.cs"))
                .SelectMany(f =>
                {
                    // Console.WriteLine(f + " --------");
                    return File.ReadAllLines(f).Select(l => new CodeInfo(Path.GetFileName(f), l));
                })
                .Aggregate((x, y) => new CodeInfo(x.TodoCount + y.TodoCount, x.NotImplCount + y.NotImplCount));
            Console.WriteLine(path + " -> " + JsonConvert.SerializeObject(info));

            var time = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var metrics = new List<ServiceMetricValue>
            {
                new ServiceMetricValue
                {
                    Name = "srcs.codeinfo.todo",
                    Value = info.TodoCount,
                    Time = time,
                },
                new ServiceMetricValue
                {
                    Name = "srcs.codeinfo.notimpl",
                    Value = info.NotImplCount,
                    Time = time,
                },
            };

            // XXX サービス接続で作ったクライアント認証設定構築してくれてない？
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("X-Api-Key", mackerelApiKey);
            var client = new mackerel_apiClient(http);
            var res = await client.PostServiceMetricAsync("SRCS", metrics);
            Console.WriteLine("PostServiceMetricAsync: " + JsonConvert.SerializeObject(res));
        }
    }

    class CodeInfo
    {
        public int TodoCount { get; }
        public int NotImplCount { get; }

        public CodeInfo(string filename, string line)
        {
            TodoCount = line.ToLower().Replace(" ", "").Contains("//todo") ? 1 : 0;
            NotImplCount = line.Contains("NotImplementedException") ? 1 : 0;

            if (TodoCount > 0 || NotImplCount > 0)
            {
                Console.WriteLine(filename + ": " + line);
            }
        }

        public CodeInfo(int todoCount, int notImplCount)
        {
            TodoCount = todoCount;
            NotImplCount = notImplCount;
        }
    }
}
