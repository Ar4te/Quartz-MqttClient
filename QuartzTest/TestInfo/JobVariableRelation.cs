using Quartz;

namespace QuartzTest.TestInfo;

public partial class JobVariableRelation
{
    public JobVariableRelation()
    {
        ID = Guid.NewGuid().ToString().Replace("-", "");
    }

    public JobVariableRelation(string telemetryJobId, string variableId, bool isFlag = false, bool enable = false) :
        this()
    {
        TelemetryJobId = telemetryJobId;
        VariableId = variableId;
        IsFlag = isFlag;
        Enable = enable;
    }

    public string ID { get; set; }
    public string TelemetryJobId { get; set; }
    public string VariableId { get; set; }
    public bool IsFlag { get; set; }
    public bool Enable { get; set; }
}

public partial class JobVariableRelation
{
    public static List<JobVariableRelation> MockData => GetMockData();

    private static List<JobVariableRelation> GetMockData()
    {
        var mockData = new List<JobVariableRelation>()
        {
            new("job1", "variable1", false, true),
            new("job1", "variable5", false, true),
            new("job1", "variable6", false, true),
            new("job2", "variable2", false, true),
            new("job1", "variable3", false, true),
            new("job2", "variable4", false, true),
            new("job3", "variable7", false, true),
            new("job3", "variable8", false, true),
            new("job3", "variable9", false, true),
            new("job3", "variable10", false, true),
        };
        return mockData;
    }

    //private static Dictionary<IJobDetail, ITrigger> CreateJobDetailAndTrigger()
    //{
    //    var dict = new Dictionary<IJobDetail, ITrigger>();
    //    MockData.ForEach(mock =>
    //    {
    //        var telemetryJob = TelemetryJobEntity.MockData.FirstOrDefault(tje => tje.ID == mock.TelemetryJobId);
    //        var variable = Variable.MockData.FirstOrDefault(v => v.ID == mock.VariableId);
    //        if (telemetryJob != null)
    //        {
    //            var jobDataMap = new JobDataMap()
    //            {
    //                new("Name", telemetryJob.Name)
    //            };
    //            if (variable != null)
    //            {
    //                jobDataMap.PutAll(new Dictionary<string, object>()
    //                {
    //                    { "VariableID", variable.ID },
    //                    { "VariableName", variable.Name },
    //                    { "VariableCode", variable.VariableId },
    //                    { "VariableDescription", variable.Description },
    //                    { "VariableMethod", variable.Method },
    //                    { "VariableAddress", variable.Address },
    //                    { "VariableType", variable.Type },
    //                    { "VariableEndian", variable.Endian },
    //                    { "VariableReadExpression", variable.ReadExpression },
    //                    { "VariableWriteExpression", variable.WriteExpression },
    //                    { "VariableIsUpload", variable.IsUpload },
    //                    { "VariableIsTrigger", variable.IsTrigger },
    //                    { "VariablePermission", variable.Permission },
    //                    { "VariableAlias", variable.Alias },
    //                    { "VariableIndex", variable.Index },
    //                    { "VariableDeviceId", variable.DeviceId },
    //                    { "VariableCreateTime", variable.CreateTime },
    //                    { "VariableCreateBy", variable.CreateBy },
    //                    { "VariableUpdateTime", variable.UpdateTime },
    //                    { "VariablePLCGroupId", variable.PLCGroupId },
    //                    { "VariableBusinessType", variable.BusinessType },
    //                    { "VariableEnable", variable.Enable },
    //                    { "VariableCmdPeriod", variable.CmdPeriod },
    //                    { "VariablePeriod", variable.Period },
    //                    { "VariableStringLength", variable.StringLength },
    //                });
    //            }

    //            var jobDetail = JobBuilder.Create<TelemetryVariableJob>()
    //                .WithIdentity(new JobKey(telemetryJob.Code, telemetryJob.JobGroupCode))
    //                .UsingJobData(jobDataMap)
    //                .WithDescription(telemetryJob.Description)
    //                .Build();
    //            var triggerBuild = TriggerBuilder.Create()
    //                .ForJob(jobDetail)
    //                .WithIdentity(new TriggerKey($"T_{telemetryJob.Code}", telemetryJob.TriggerGroupCode));

    //            triggerBuild = telemetryJob.IsCronJob
    //                ? triggerBuild.WithCronSchedule(telemetryJob.CronExpression)
    //                : triggerBuild.WithSimpleSchedule(config =>
    //                {
    //                    config.WithInterval(TimeSpan.FromMilliseconds(telemetryJob.Interval))
    //                        .WithRepeatCount(telemetryJob.RepeatCount);
    //                });
    //            var trigger = triggerBuild.Build();
    //            dict.Add(jobDetail, trigger);
    //        }
    //    });

    //    return dict;
    //}
}