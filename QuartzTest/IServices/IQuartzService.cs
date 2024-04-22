using System.ComponentModel.DataAnnotations;
using Quartz;

namespace QuartzTest.IServices;

public interface IQuartzService
{
    #region AddJob
    Task AddJob(IJobDetail jobDetail, ITrigger trigger);
    Task AddCronJob<T>(JobKey jobKey, TriggerKey triggerKey, string cron, JobDataMap? jobDataMap = null, bool startNow = false) where T : IJob;
    Task AddSimpleJob<T>(JobKey jobKey, TriggerKey triggerKey, int interval, int repeatCount, JobDataMap? jobDataMap = null, bool startNow = false) where T : IJob;
    #endregion
    Task Start();
    Task Shutdown();
    Task RemoveJob(string jobName);
    Task PauseJob(string jobName);
    Task ResumeJob(string jobName);
    Task PauseJobsByGroup([MinLength(1)] string jobGroupName);
    Task ResumeJobsByGroup(string jobGroupName);
    Task<IEnumerable<string>> GetJobGroupNames();
    Task<IEnumerable<string>> GetJobNamesByGroup(string jobGroupName);
    Task<IEnumerable<string>> GetTriggerGroupNames();
    Task PauseTriggersByGroup(string triggerGroupName);
    Task<IEnumerable<string>> GetTriggerNamesByGroup(string triggerGroupName);
}