using Newtonsoft.Json;
using System.Net.Http.Json;

namespace ConsoleAppCallAPIs;

public class ClientDemoService
{
    private static readonly HttpClient client = new();
    private readonly string apiUrl = "https://localhost:44393/api";
    public ClientDemoService()
    {
        client.BaseAddress = new Uri(apiUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task GetFileInfoAsync(string fileId)
    {
        string url = $"{apiUrl}/Files/{fileId}";
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode(); // Throws an exception if not a success status code

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET Response: {responseBody}");
    }

    public async Task<TokenInfo> SignInAsync(string userName, string passWord)
    {
        // Example POST request with JSON data
        var userData = new { userName, passWord };
        string jsonContent = JsonConvert.SerializeObject(userData);
        StringContent content = new(jsonContent, System.Text.Encoding.UTF8, "application/json");

        string url = $"{apiUrl}/User/SignIn";

        HttpResponseMessage postResponse = await client.PostAsync(url, content);
        postResponse.EnsureSuccessStatusCode();

        var responseBody = await postResponse.Content.ReadFromJsonAsync<TokenInfo>();        
        return responseBody ?? new TokenInfo();
    }

    public class TokenInfo 
    {
        public string AccessToken { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
