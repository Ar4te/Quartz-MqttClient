using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzTest.IServices;

namespace QuartzTest.Services;

public class QuartzService : IQuartzService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Start()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (!scheduler.IsStarted)
        {
            await scheduler.Start();
        }
    }

    public async Task Shutdown()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (!scheduler.IsShutdown)
        {
            await scheduler.Shutdown();
        }
    }

    #region AddJob
    public async Task AddJob(IJobDetail jobDetail, ITrigger trigger)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ScheduleJob(jobDetail, trigger);
        Console.WriteLine($"Added job [{jobDetail.Key.Group}:{jobDetail.Key.Name}]");
    }

    public async Task AddCronJob<T>(JobKey jobKey, TriggerKey triggerKey, string cron, JobDataMap? jobDataMap = null, bool startNow = false) where T : IJob
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var job = CreateJobBuilder<T>(jobKey, jobDataMap).Build();
        var trigger = CreateCronTriggerBuilder(jobKey, triggerKey, cron, startNow).Build();
        await scheduler.ScheduleJob(job, trigger);
        Console.WriteLine($"Added cron job {job.Key.Name}");
    }

    public async Task AddSimpleJob<T>(JobKey jobKey, TriggerKey triggerKey, int interval, int repeatCount, JobDataMap? jobDataMap = null, bool startNow = false) where T : IJob
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var job = CreateJobBuilder<T>(jobKey, jobDataMap).Build();
        var trigger = CreateSimpleTriggerBuilder(jobKey, triggerKey, interval, repeatCount, startNow).Build();
        await scheduler.ScheduleJob(job, trigger);
        Console.WriteLine($"Added simple job {job.Key.Name}");
    }
    #endregion

    #region Private

    #region JobBuilder
    private static JobBuilder CreateJobBuilder<T>(JobKey jobKey, JobDataMap? jobDataMap = null, string jobDescription = "") where T : IJob
    {
        var jobBuilder = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .WithDescription(jobDescription);

        if (jobDataMap != null)
        {
            jobBuilder.UsingJobData(jobDataMap);
        }

        return jobBuilder;
    }
    #endregion

    #region TriggerBuilder
    private static TriggerBuilder CreateCronTriggerBuilder(JobKey jobKey, TriggerKey triggerKey, string cronExpression, bool startNow = false)
    {
        var triggerBuilder = CreateTriggerBuilder(jobKey, triggerKey, startNow)
            .WithCronSchedule(cronExpression);
        return triggerBuilder;
    }

    private static TriggerBuilder CreateSimpleTriggerBuilder(JobKey jobKey, TriggerKey triggerKey, int interval, int repeatCount, bool startNow = false)
    {
        var triggerBuilder = CreateTriggerBuilder(jobKey, triggerKey, startNow)
            .WithSimpleSchedule(config =>
            {
                config.WithInterval(TimeSpan.FromMilliseconds(interval))
                      .WithRepeatCount(repeatCount);
            });
        return triggerBuilder;
    }

    /// <summary>
    /// 创建触发器构造器
    /// </summary>
    /// <param name="jobKey"><see cref="JobKey"></param>
    /// <param name="triggerKey"><see cref="TriggerKey"></param>
    /// <param name="startNow">Job with such trigger will start now if <see cref="true"/></param>
    /// <returns></returns>
    private static TriggerBuilder CreateTriggerBuilder(JobKey jobKey, TriggerKey triggerKey, bool startNow = false)
    {
        var triggerBuilder = TriggerBuilder
           .Create()
           .ForJob(jobKey)
           .WithIdentity(triggerKey);
        if (startNow)
        {
            triggerBuilder.StartNow();
        }

        return triggerBuilder;
    }
    #endregion

    #endregion

    public async Task RemoveJob(string jobName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

        var jobKey = new JobKey(jobName);
        await scheduler.DeleteJob(jobKey);
        Console.WriteLine($"Delete job {jobName}");
    }

    public async Task PauseJob(string jobName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(jobName);

        await scheduler.PauseJob(jobKey);
        Console.WriteLine($"Pause job {jobName}");
    }

    public async Task ResumeJob(string jobName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(jobName);

        await scheduler.ResumeJob(jobKey);
        Console.WriteLine($"Resume job {jobName}");
    }

    public async Task PauseJobsByGroup([MinLength(1), NotNull] string jobGroupName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        try
        {
            await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(jobGroupName));
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message, ex);
        }
    }

    public async Task PauseTriggersByGroup(string triggerGroupName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals(triggerGroupName));
    }

    public async Task<IEnumerable<string>> GetJobGroupNames()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        return await scheduler.GetJobGroupNames();
    }

    public async Task<IEnumerable<string>> GetTriggerGroupNames()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        return await scheduler.GetTriggerGroupNames();
    }

    public async Task ResumeJobsByGroup(string jobGroupName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(jobGroupName));
        Console.WriteLine($"Resume jobs {jobGroupName}");
    }

    public async Task<IEnumerable<string>> GetJobNamesByGroup(string jobGroupName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName));
        return jobKeys.Select(jk => jk.Name);
    }

    public async Task<IEnumerable<string>> GetTriggerNamesByGroup(string triggerGroupName)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(triggerGroupName));
        return jobKeys.Select(jk => jk.Name);
    }

    public async Task StartJob(string jobName, string jobGroupName = "")
    {
        var jobKey = new JobKey(jobName, jobGroupName);
        var scheduler = await _schedulerFactory.GetScheduler();
        var triggers = await scheduler.GetTriggersOfJob(jobKey);
        foreach (var trigger in triggers)
        {
            await scheduler.ResumeTrigger(trigger.Key);
        }
    }
}