using System.Net;
using System.Text;
using Cache.Client.Impl;
using NBomber.CSharp;

Console.WriteLine("Welcome to load tests");
var endPoint = CreateDefaultEndPoint();

var scenario = Scenario.Create("Load test scenario", async context =>
    {
        var random = new Random();
        
        var setKeyValueStep = await Step.Run("Set random key-value", context, async () =>
        {
            using var client = new SimpleTcpClient(endPoint);
            await client.ConnectAsync();
            var randomKey = $"anyKey{random.Next()}";
            var randomValue = $"anyValue{random.Next()}";
            var response = await client.SetAsync(randomKey, Encoding.UTF8.GetBytes(randomValue));

            return (response ==  @"OK\r\n") ? Response.Ok() : Response.Fail();
        });

        return Response.Ok();
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(10))
    .WithLoadSimulations(Simulation.Inject(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)));

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();

IPEndPoint CreateDefaultEndPoint()
{
    var ip = IPAddress.Parse("127.0.0.1");
    var endpoint = new IPEndPoint(ip, 9995);
        
    return endpoint;
}