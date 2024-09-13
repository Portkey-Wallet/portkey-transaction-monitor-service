using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Monitor.Common;

public static class HttpHelper
{
    public static async Task<string> HttpPost(string url, string jsonData)
    {
        HttpClient client = new HttpClient();
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            Console.WriteLine("HttpPost Error: " + response.StatusCode);
        }

        return "";
    }
    
    public static async Task<string> HttpGet(string url)
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
    
    
}