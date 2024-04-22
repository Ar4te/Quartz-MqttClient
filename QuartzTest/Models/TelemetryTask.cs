using Quartz;
using QuartzTest.MqttTest;

namespace QuartzTest.Models;

public class TelemetryTask
{
    #region Constructors
    private TelemetryTask()
    {
        Id = Guid.NewGuid().ToString().Replace("-", "");
    }

    private TelemetryTask(string code = "Code", string name = "Name", TelemetryTypeEnum telemetryType = TelemetryTypeEnum.ChangeTelemetry, bool enable = false) : this()
    {
        Code = $"{code}_{Id}";
        Name = $"{name}_{Id}";
        Enable = enable;
        TelemetryType = telemetryType;
    }

    public TelemetryTask(string groupName, int interval, string pushTarget, string code = "Code", string name = "Name", TelemetryTypeEnum telemetryType = TelemetryTypeEnum.ChangeTelemetry, bool enable = false, string description = "") : this(code, name, telemetryType, enable)
    {
        GroupName = groupName;
        Interval = interval;
        PushTarget = pushTarget;
        Description = description;
    }
    #endregion

    #region Properties
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string GroupName { get; set; }
    public TelemetryTypeEnum TelemetryType { get; set; }
    public int Interval { get; set; }
    public string PushTarget { get; set; }
    public bool Enable { get; set; }
    public string Description { get; set; }
    #endregion

    #region Methods
    //public CronExpression CronExpression()
    //{
    //    CronExpression cronExpression = null;
    //    if (Interval <= 0) throw new InvalidDataException($"{nameof(Interval)} must bigger than zero");
    //    var interval = Interval / 1000;
    //    if (interval < 60)
    //    {
    //        cronExpression = new CronExpression($"*/{interval} * * * * ?");
    //    }
    //    else if (interval >= 60 && interval < 60 * 60)
    //    {
    //        var minutes = interval / 60;
    //        cronExpression = new CronExpression($"* */{minutes} * * * ?");
    //    }
    //    else if (interval >= 60 * 60 && interval <= 60 * 60 * 24)
    //    {
    //        var hours = interval / (60 * 60);
    //        cronExpression = new CronExpression($"* * */{hours} * * ?");
    //    }
    //    else if (interval >= 60 * 60 * 24)
    //    {
    //        var days = interval / (60 * 60 * 24);
    //        cronExpression = new CronExpression($"* * * */{days} * ?");
    //    }
    //    else
    //    {

    //    }
    //    return cronExpression;
    //}

    public JobKey JobKey => new(Code, GroupName);
    public TriggerKey TriggerKey => new($"T_{Code}", $"T_{GroupName}");
    public IJobDetail CreateJobDetail(JobBuilder jobBuilder)
    {
        var jobDetail = jobBuilder
            .WithIdentity(JobKey)
            .UsingJobData("Name", Name)
            .UsingJobData("TelemetryType", Name)
            .WithDescription(Description)
            .Build();

        return jobDetail;
    }
    public ITrigger CreateTrigger()
    {
        var interval = Interval / 1000;
        var trigger = TriggerBuilder
            .Create()
            .ForJob(JobKey)
            .WithIdentity(TriggerKey)
            .WithSimpleSchedule(config =>
            {
                config
                .RepeatForever()
                .WithInterval(TimeSpan.FromMilliseconds(Interval));
            })
            .Build();

        return trigger;
    }
    #endregion
}

public enum RWTypeEnum
{
    OnlyRead = 1,
    OnlyWrite,
    ReadWrite
}