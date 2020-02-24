using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace _19Cov
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    Console.Write("取得公開資訊中...");
                    var req = await http.GetAsync("https://od.cdc.gov.tw/eic/Age_County_Gender_19Cov.json");
                    if (!req.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"失敗！({(int)req.StatusCode}-{req.ReasonPhrase})");
                        return;
                    }
                    
                    Console.WriteLine("OK!");
                    var json = await req.Content.ReadAsStringAsync();
                    var bytes = await req.Content.ReadAsByteArrayAsync();

                    var 確診病例 = JsonSerializer.Deserialize<List<確診>>(json);
                    var 各縣市確診數 = 確診病例.GroupBy(x => x.縣市)
                        .Select(g => new
                        {
                            縣市 = g.Key,
                            病例數 = g.Sum(x => int.Parse(x.確定病例數))
                        });
                    foreach (var 群組 in 各縣市確診數)
                    {
                        Console.WriteLine($"{群組.縣市}： {群組.病例數} 例");
                    }
                    Console.WriteLine();
                    Console.WriteLine($"全台共： {確診病例.Sum(x => Convert.ToInt32(x.確定病例數))} 例");

                    File.WriteAllBytes(@"C:\temp\cov19.txt", bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"失敗！\n錯誤訊息如下：\n{ex}");
            }
        }
    }
}
