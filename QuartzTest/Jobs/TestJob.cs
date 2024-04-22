using Quartz;
using QuartzTest.MqttTest;

namespace QuartzTest.Jobs;

[PersistJobDataAfterExecution]
public class TestJob : IJob
{
    private readonly MqttClientService _mqttClient;

    public TestJob(MqttClientService mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var telemetryVal = TelemetryVal();
        var jobName = context.JobDetail.Key.Name;
        var jobDataMap = context.JobDetail.JobDataMap;
        var telemetryType = (TelemetryTypeEnum)jobDataMap.GetInt("TelemetryType");
        var lastVal = jobDataMap.GetString("LastVal");
        var curVal = jobDataMap.GetString("CurVal");
        var runCounts = jobDataMap.GetInt("RunCounts");
        Interlocked.Increment(ref runCounts);
        jobDataMap.Put("RunCounts", runCounts);
        var needPublish = telemetryType switch
        {
            TelemetryTypeEnum.ChangeTelemetry => DealChangeTelemetryData(jobDataMap, telemetryVal, curVal ?? ""),
            TelemetryTypeEnum.TriggerTelemetry => DealTriggerTelemetryData(jobDataMap, telemetryVal, curVal ?? ""),
            TelemetryTypeEnum.SubscribeTelemetry => DealSubscribeTelemetryData(jobDataMap, telemetryVal, curVal ?? ""),
            _ => false
        };
        string valStr = $",采集值【{telemetryVal}】,当前值【{curVal}】,上次值【{lastVal}】";
        string mqttMessage = needPublish ? $"【{jobName}】:上报类型【{telemetryType}】{valStr}" : $"【{jobName}】:上报类型【无需上报】{valStr}";

        bool mqttPublish = false;
        if (jobDataMap.GetString("MqttTopic") is string mqttTopic && !string.IsNullOrEmpty(mqttTopic?.Trim()) && needPublish)
        {
            mqttPublish = await _mqttClient.Publish(mqttTopic, mqttMessage);
        }

        string needPublishStr = needPublish ? (mqttPublish ? $"Mqtt发布{mqttMessage}成功:" : $"Mqtt发布{mqttMessage}失败:") : $"Mqtt无需发布{mqttMessage}";
        Console.WriteLine($"第【{runCounts}】次执行【{jobName}】完成,{needPublishStr}{DateTime.Now}");
    }

    public string TelemetryVal()
    {
        var _random = new Random();
        lock (_random)
        {
            return (_random.NextDouble() * 10 - 5).ToString();
        }
    }

    public bool DealChangeTelemetryData(JobDataMap jobDataMap, string telemetryVal, string curVal)
    {
        bool needPublish = telemetryVal != curVal;
        jobDataMap.Put("LastVal", curVal);
        jobDataMap.Put("CurVal", telemetryVal);

        return needPublish;
    }

    public bool DealTriggerTelemetryData(JobDataMap jobDataMap, string telemetryVal, string curVal)
    {
        jobDataMap.Put("LastVal", curVal);
        jobDataMap.Put("CurVal", telemetryVal);
        return true;
    }

    public bool DealSubscribeTelemetryData(JobDataMap jobDataMap, string telemetryVal, string curVal)
    {
        return DealTriggerTelemetryData(jobDataMap, telemetryVal, curVal);
    }
}