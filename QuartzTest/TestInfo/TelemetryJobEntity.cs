using Quartz;

namespace QuartzTest.TestInfo;

public partial class TelemetryJobEntity
{
    public TelemetryJobEntity()
    {

    }
    public TelemetryJobEntity(string id, string code, string name, string jobGroupCode, string jobGroupName, string triggerGroupName, string triggerGroupCode, bool isCronJob, string cronExpression = "*/5 * * * * ?", int interval = 5000, int repeatCount = -1, bool enable = true, string jobDataMapJson = "", int jobType = 1, string description = "", DateTime? lastRunTime = null)
    {
        ID = id;
        Code = code;
        Name = name;
        JobGroupCode = jobGroupCode;
        JobGroupName = jobGroupName;
        TriggerGroupName = triggerGroupName;
        TriggerGroupCode = triggerGroupCode;
        CronExpression = cronExpression;
        Interval = interval;
        RepeatCount = repeatCount;
        Enable = enable;
        JobDataMapJson = jobDataMapJson;
        JobType = jobType;
        Description = description;
        LastRunTime = lastRunTime;
    }
    public string ID { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string JobGroupCode { get; set; }
    public string JobGroupName { get; set; }
    public string TriggerGroupCode { get; set; }
    public string TriggerGroupName { get; set; }
    public bool IsCronJob { get; set; }
    public string CronExpression { get; set; }
    public int Interval { get; set; }
    public int RepeatCount { get; set; }
    public bool Enable { get; set; }
    public string JobDataMapJson { get; set; }
    public int JobType { get; set; }
    public string Description { get; set; }
    public DateTime? LastRunTime { get; set; }
}

public partial class TelemetryJobEntity
{
    public static List<TelemetryJobEntity> MockData => GetMockData();

    public static Dictionary<IJobDetail, ITrigger> JobDetailAndTriggers => CreateJobDetailAndTrigger();

    private static List<TelemetryJobEntity> GetMockData()
    {
        var mockData = new List<TelemetryJobEntity>()
        {
            new("job1", "jobCode1", "jobName1", "jobGroupCode1", "jobGroupName1", "triggerGroupName1", "triggerGroupCode1", false),
            new("job2", "jobCode2", "jobName2", "jobGroupCode2", "jobGroupName2", "triggerGroupName2", "triggerGroupCode2", false),
            new("job3", "jobCode3", "jobName3", "jobGroupCode3", "jobGroupName3", "triggerGroupName3", "triggerGroupCode3", false),
            new("job4", "jobCode4", "jobName4", "jobGroupCode4", "jobGroupName4", "triggerGroupName4", "triggerGroupCode4", false),
        };

        return mockData;
    }

    private static Dictionary<IJobDetail, ITrigger> CreateJobDetailAndTrigger()
    {
        var dict = new Dictionary<IJobDetail, ITrigger>();
        MockData.ForEach(mock =>
        {
            var jobVariables = JobVariableRelation.MockData.Where(jvr => jvr.TelemetryJobId == mock.ID);
            if (jobVariables?.Any() == true)
            {
                var variables = Variable.MockData.Where(v => jobVariables.Any(jv => jv.VariableId == v.ID)).GroupBy(jv => jv.DeviceId).ToArray();

                if (variables?.Any() == true)
                {
                    for (var i = 0; i < variables.Length; i++)
                    {
                        // concurrence
                        if (variables[i].Key.Last().ToString() == "1")
                        {
                            var _variables = variables[i].ToArray();
                            //foreach (var variable in variables[i].ToList())
                            for (var j = 0; j < _variables.Length; j++)
                            {
                                var variable = _variables[j];
                                var jobDataMap = new JobDataMap()
                                {
                                    new("Name", $"{mock.Name}_{j + 1}")
                                };
                                var mapItem = new Dictionary<string, object>
                                {
                                    {
                                        variable.ID,
                                        new Dictionary<string, object>()
                                        {
                                            { "ID", variable.ID },
                                            { "Name", variable.Name },
                                            { "VariableId", variable.VariableId },
                                            { "Description", variable.Description },
                                            { "Method", variable.Method },
                                            { "Address", variable.Address },
                                            { "Type", variable.Type },
                                            { "Endian", variable.Endian },
                                            { "ReadExpression", variable.ReadExpression },
                                            { "WriteExpression", variable.WriteExpression },
                                            { "IsUpload", variable.IsUpload },
                                            { "IsTrigger", variable.IsTrigger },
                                            { "Permission", variable.Permission },
                                            { "Alias", variable.Alias },
                                            { "Index", variable.Index },
                                            { "DeviceId", variable.DeviceId },
                                            { "CreateTime", variable.CreateTime },
                                            { "CreateBy", variable.CreateBy },
                                            { "UpdateTime", variable.UpdateTime },
                                            { "PLCGroupId", variable.PLCGroupId },
                                            { "BusinessType", variable.BusinessType },
                                            { "Enable", variable.Enable },
                                            { "CmdPeriod", variable.CmdPeriod },
                                            { "Period", variable.Period },
                                            { "StringLength", variable.StringLength },
                                        }
                                    }
                                };
                                jobDataMap.Put("Variables", mapItem);
                                var jobDetail = JobBuilder.Create<TelemetryVariableJob>()
                                    .WithIdentity(new JobKey(variable.VariableId, mock.JobGroupCode))
                                    .UsingJobData(jobDataMap)
                                    .WithDescription(mock.Description)
                                    .Build();
                                var triggerBuild = TriggerBuilder.Create()
                                    .ForJob(jobDetail)
                                    .WithIdentity(new TriggerKey($"T_{variable.VariableId}", mock.TriggerGroupCode));

                                triggerBuild = mock.IsCronJob
                                    ? triggerBuild.WithCronSchedule(mock.CronExpression)
                                    : triggerBuild.WithSimpleSchedule(config =>
                                    {
                                        config.WithInterval(TimeSpan.FromMilliseconds(mock.Interval))
                                            .WithRepeatCount(mock.RepeatCount);
                                    });
                                var trigger = triggerBuild.Build();
                                dict.Add(jobDetail, trigger);

                            }
                        }
                        else
                        {
                            var mapItem = new Dictionary<string, object>();
                            var _variables = variables[i].ToList();
                            _variables.Sort((a, b) => a.Index.CompareTo(b.Index));
                            var jobDataMap = new JobDataMap()
                            {
                                new("Name", mock.Name),
                                new("LastExecIndex", -999)
                            };
                            foreach (var variable in _variables)
                            {
                                mapItem.Add(variable.ID, new Dictionary<string, object>()
                                {
                                    { "ID", variable.ID },
                                    { "Name", variable.Name },
                                    { "VariableId", variable.VariableId },
                                    { "Description", variable.Description },
                                    { "Method", variable.Method },
                                    { "Address", variable.Address },
                                    { "Type", variable.Type },
                                    { "Endian", variable.Endian },
                                    { "ReadExpression", variable.ReadExpression },
                                    { "WriteExpression", variable.WriteExpression },
                                    { "IsUpload", variable.IsUpload },
                                    { "IsTrigger", variable.IsTrigger },
                                    { "Permission", variable.Permission },
                                    { "Alias", variable.Alias },
                                    { "Index", variable.Index },
                                    { "DeviceId", variable.DeviceId },
                                    { "CreateTime", variable.CreateTime },
                                    { "CreateBy", variable.CreateBy },
                                    { "UpdateTime", variable.UpdateTime },
                                    { "PLCGroupId", variable.PLCGroupId },
                                    { "BusinessType", variable.BusinessType },
                                    { "Enable", variable.Enable },
                                    { "CmdPeriod", variable.CmdPeriod },
                                    { "Period", variable.Period },
                                    { "StringLength", variable.StringLength },
                                });
                            }
                            jobDataMap.Put("Variables", mapItem);
                            var jobDetail = JobBuilder.Create<TelemetryVariableJob>()
                                   .WithIdentity(new JobKey(mock.Code, mock.JobGroupCode))
                                   .UsingJobData(jobDataMap)
                                   .WithDescription(mock.Description)
                                   .DisallowConcurrentExecution()
                                   .Build();
                            var triggerBuild = TriggerBuilder.Create()
                                .ForJob(jobDetail)
                                .WithIdentity(new TriggerKey($"T_{mock.Code}", mock.TriggerGroupCode));

                            triggerBuild = mock.IsCronJob
                                ? triggerBuild.WithCronSchedule(mock.CronExpression)
                                : triggerBuild.WithSimpleSchedule(config =>
                                {
                                    config.WithInterval(TimeSpan.FromMilliseconds(mock.Interval))
                                        .WithRepeatCount(mock.RepeatCount);
                                });
                            var trigger = triggerBuild.Build();
                            dict.Add(jobDetail, trigger);
                        }
                    }

                }

                //var jobDetail = JobBuilder.Create<TelemetryVariableJob>()
                //    .WithIdentity(new JobKey(mock.Code, mock.JobGroupCode))
                //    .UsingJobData(jobDataMap)
                //    .WithDescription(mock.Description)
                //    .Build();
                //var triggerBuild = TriggerBuilder.Create()
                //    .ForJob(jobDetail)
                //    .WithIdentity(new TriggerKey($"T_{mock.Code}", mock.TriggerGroupCode));

                //triggerBuild = mock.IsCronJob
                //    ? triggerBuild.WithCronSchedule(mock.CronExpression)
                //    : triggerBuild.WithSimpleSchedule(config =>
                //    {
                //        config.WithInterval(TimeSpan.FromMilliseconds(mock.Interval))
                //            .WithRepeatCount(mock.RepeatCount);
                //    });
                //var trigger = triggerBuild.Build();
                //dict.Add(jobDetail, trigger);
            }
        });

        return dict;
    }
}