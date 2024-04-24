using Newtonsoft.Json;
using Quartz;
using QuartzTest.MqttTest;

namespace QuartzTest.TestInfo;

[PersistJobDataAfterExecution]
public class TelemetryVariableJob : IJob
{
    private readonly MqttClientService _mqttClientService;

    public TelemetryVariableJob(MqttClientService mqttClientService)
    {
        _mqttClientService = mqttClientService;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        var map = context.JobDetail.JobDataMap;
        var tmp = map.Get("Variables");
        var tmp3 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(JsonConvert.SerializeObject(tmp));
        var tmp2 = map.TryGetInt("LastExecIndex", out int lastExecIndex);
        if (tmp2 && lastExecIndex != 0)
        {
            var array = tmp3!.Values.ToArray();
            lastExecIndex++;
            lastExecIndex = array.Length < lastExecIndex || lastExecIndex < 0 ? 1 : lastExecIndex;
            var variable = array[lastExecIndex - 1];
            map.Put("LastExecIndex", lastExecIndex);
            var enable = variable.TryGetValue("Enable", out var enable1) && enable1 is bool _enable && _enable;
            var isUpload = variable!.TryGetValue("IsUpload", out var isUpload1) && isUpload1 is bool _isUpload && _isUpload;
            if (enable && isUpload)
            {
                //await _mqttClientService.Publish(variable.GetValueOrDefault("ID")?.ToString()!, variable.GetValueOrDefault("ID")?.ToString()!);
                await _mqttClientService.Publish(context.JobDetail.Key.Name, $"Index:[{lastExecIndex - 1}],{variable.GetValueOrDefault("ID")?.ToString()!}");
                Console.WriteLine($"{context.JobDetail.Key.Name},Current Index:[{lastExecIndex - 1}],{variable.GetValueOrDefault("ID")?.ToString()!}");
                //Console.WriteLine($"UnConJob:{map.GetString("Name")} Need Upload");
                return;
            }

            Console.WriteLine($"UnConJob:{map.GetString("Name")}");
            await _mqttClientService.Publish(tmp3.FirstOrDefault().Key, tmp3.FirstOrDefault().Key);
        }
        else
        {
            var variable = tmp3!.Values.FirstOrDefault();
            var enable = variable!.TryGetValue("Enable", out var enable1) && enable1 is bool _enable && _enable;
            var isUpload = variable!.TryGetValue("IsUpload", out var isUpload1) && isUpload1 is bool _isUpload && _isUpload;
            if (enable && isUpload)
            {
                await _mqttClientService.Publish(variable.GetValueOrDefault("ID")?.ToString()!, tmp3.FirstOrDefault().Key);
                Console.WriteLine($"ConJob:{map.GetString("Name")} Need Upload");
            }
            else
            {
                Console.WriteLine($"ConJob:{map.GetString("Name")}");
            }
        }
    }
}
