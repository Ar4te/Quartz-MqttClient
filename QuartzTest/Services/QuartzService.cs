using Quartz;
using QuartzTest.IServices;

namespace QuartzTest.Services;

public class QuartzService : IQuartzService
{
    private readonly ISchedulerFactory _schedulerFactory;
    //private readonly IScheduler _scheduler;
    public QuartzService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
        //_scheduler = schedulerFactory.GetScheduler().Result;
        //Shutdown();
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

    public async Task AddJob<T>(string jobName, string cron, bool startNow = false) where T : IJob
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(jobName);
        var job = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .UsingJobData("name", jobName)
            .Build();

        var trigger = TriggerBuilder.Create()
            .ForJob(jobKey)
            .WithIdentity(jobName)
            //.StartNow()
            .WithCronSchedule(cron)
            .Build();
        //var triggerBuilder = TriggerBuilder.Create()
        //    .ForJob(jobKey)
        //    .WithIdentity(jobName);
        //if (startNow) triggerBuilder.StartNow();
        //triggerBuilder.WithCronSchedule(cron);
        ////if(triggerType.Equals(SchedulerType.Normal)) triggerBuilder.WithCalendarIntervalSchedule(opt =>
        ////{
        ////    opt.WithInterval()
        ////})
        //triggerBuilder.WithSimpleSchedule(action =>
        //{

        //});
        //triggerBuilder.Build();

        await scheduler.ScheduleJob(job, trigger);
        //await PauseJob(jobName);
        Console.WriteLine($"Add job {jobName}");
    }

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
}

public enum SchedulerType
{
    Cron,
    Normal
}