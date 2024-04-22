using Quartz;
using QuartzTest.MqttTest;

namespace QuartzTest.Jobs;

[PersistJobDataAfterExecution]
public class TelemetryPointJob : IJob
{
    private readonly MqttClientService _mqttClientService;

    public TelemetryPointJob(MqttClientService mqttClientService)
    {
        _mqttClientService = mqttClientService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"{context.JobDetail.Key.Name}:{DateTime.Now}");
        return Task.CompletedTask;
    }
}
