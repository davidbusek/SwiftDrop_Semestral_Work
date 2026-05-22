using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SwiftDrop.Client
{
    class Program
    {
        // API key must match ApiKeys:CourierApi in the server's appsettings.json
        private const string ApiKey = "change-me-before-production";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("    SWIFTDROP TERMINAL CLIENT v1.0     ");
            Console.WriteLine("=======================================");

            Console.Write("\nEnter your Courier ID: ");
            if (!int.TryParse(Console.ReadLine(), out int courierId))
            {
                Console.WriteLine("[Error] Invalid Courier ID. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            var baseUrl = "https://localhost:7032/api/courierapi";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

            try
            {
                Console.WriteLine("\n[System] Connecting to Nexus...");
                var response = await client.GetAsync($"{baseUrl}/dashboard?courierId={courierId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("[Success] Data incoming...");
                    Console.WriteLine("\n=== RAW JSON PAYLOAD ===");

                    var parsedJson = JsonSerializer.Deserialize<JsonElement>(json);
                    var formattedJson = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(formattedJson);

                    Console.WriteLine("\n[Info] Copy an Order ID to advance its state.");
                }
                else
                {
                    Console.WriteLine($"[Error] Could not reach Nexus. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fatal] {ex.Message}");
                Console.WriteLine("Make sure the SwiftDrop ASP.NET project is running and the port is correct.");
            }

            Console.WriteLine("\nEnter Order ID to Advance State (or type 'exit' to quit): ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int orderId))
            {
                var postResponse = await client.PostAsync(
                    $"{baseUrl}/advance-state/{orderId}?courierId={courierId}", null);
                if (postResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Success] Order #{orderId} state advanced!");
                }
                else
                {
                    Console.WriteLine($"[Error] Code: {postResponse.StatusCode}");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
