

using Newtonsoft.Json;
using System;

namespace ConsoleAppCallAPIs;

public class Program
{
    static async Task Main(string[] args)
    {
        ClientDemoService service = new();
        
        string fileId = "2rPwjW0679"; 
        Console.WriteLine($"Send request to API get {fileId}");

        await service.GetFileInfoAsync(fileId);

        var tokenInfo = await service.SignInAsync("Vinh2025", "");
        Console.WriteLine($"AccessToken: {tokenInfo.AccessToken}");

        Console.ReadLine();
    }
}
