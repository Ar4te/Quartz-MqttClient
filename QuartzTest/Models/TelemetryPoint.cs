using Quartz;

namespace QuartzTest.Models;

public class TelemetryPoint
{
    public TelemetryPoint()
    {
        Id = Guid.NewGuid().ToString().Replace("-", "");
    }
    public TelemetryPoint(string telemetryTaskId, string name, string code, string plcGroupName, string plcName, string address, string dataType, RWTypeEnum rwType, string bussinessType, string writeExpression, string readExpression, string originValue, string currentValue) : this()
    {
        TelemetryTaskId = telemetryTaskId;
        Name = name;
        Code = code;
        PLCGroupName = plcGroupName;
        PLCName = plcName;
        Address = address;
        DataType = dataType;
        RWType = rwType;
        BussinessType = bussinessType;
        WriteExpression = writeExpression;
        ReadExpression = readExpression;
        OriginValue = originValue;
        CurrentValue = currentValue;
    }

    public TelemetryPoint(string name, string code, string plcGroupName, string plcName, string address, string dataType, RWTypeEnum rwType, string bussinessType, string writeExpression, string readExpression, string originValue, string currentValue, TelemetryTask telemetryTask) : this(telemetryTask.Id, name, code, plcGroupName, plcName, address, dataType, rwType, bussinessType, writeExpression, readExpression, originValue, currentValue)
    {
        TelemetryTask = telemetryTask;
    }

    public string Id { get; set; }
    public string TelemetryTaskId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string PLCGroupName { get; set; }
    public string PLCName { get; set; }
    public string Address { get; set; }
    public string DataType { get; set; }
    public RWTypeEnum RWType { get; set; }
    public string BussinessType { get; set; }
    public string WriteExpression { get; set; }
    public string ReadExpression { get; set; }
    public string OriginValue { get; set; }
    public string CurrentValue { get; set; }
    public TelemetryTask TelemetryTask { get; set; }
    public JobKey JobKey => new($"{Code}_{Name}", TelemetryTaskId);
    public TriggerKey TriggerKey => new($"T_{Code}_{Name}", TelemetryTaskId);

    public IJobDetail CreateJobDetail(JobBuilder jobBuilder)
    {
        var jobDataMap = new JobDataMap()
        {
            new KeyValuePair<string, object>("Address",Address),
            new KeyValuePair<string, object>("PLCName",PLCName),
            new KeyValuePair<string, object>("PLCGroupName",PLCGroupName),
            new KeyValuePair<string, object>("DataType",DataType),
            new KeyValuePair<string, object>("RWType",RWType),
            new KeyValuePair<string, object>("BussinessType",BussinessType),
            new KeyValuePair<string, object>("WriteExpression",WriteExpression),
            new KeyValuePair<string, object>("ReadExpression",ReadExpression),
            new KeyValuePair<string, object>("OriginValue",OriginValue),
            new KeyValuePair<string, object>("CurrentValue",CurrentValue),
        };
        var job = jobBuilder.WithIdentity(JobKey)
            .UsingJobData(jobDataMap)
            .WithDescription($"{Code}_{Name}采集任务")
            .Build();
        return job;
    }

    public ITrigger CreateTrigger()
    {
        var trigger = TriggerBuilder
            .Create()
            .ForJob(JobKey)
            .WithIdentity(TriggerKey)
            .WithSimpleSchedule(config =>
            {
                config
                .RepeatForever()
                .WithInterval(TimeSpan.FromMilliseconds(TelemetryTask.Interval));
            })
            .Build();

        return trigger;
    }
}
