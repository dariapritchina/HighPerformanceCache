using System.Net;
using Cache.Client.Impl;
using NBomber.CSharp;

Console.WriteLine("Welcome to load tests");
var endPoint = CreateDefaultEndPoint();

var scenario = Scenario.Create("Load test scenario", async context =>
    {
        var createStep = await Step.Run("create", context, async () =>
        {
            using var client = new SimpleTcpClient(endPoint);
            await client.ConnectAsync();
            await client.SetAsync("anyKey", "SET MYKEY MYVALUE"u8.ToArray());

            return Response.Ok();
        });

        return Response.Ok();
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(10))
    .WithLoadSimulations(Simulation.Inject(1000, TimeSpan.FromTicks(1), TimeSpan.FromSeconds(30)));

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();

IPEndPoint CreateDefaultEndPoint()
{
    var ip = IPAddress.Parse("127.0.0.1");
    var endpoint = new IPEndPoint(ip, 9995);
        
    return endpoint;
}