using Quartz;

namespace QuartzTest.IServices;

public interface IQuartzService
{
    Task Start();
    Task Shutdown();
    Task AddJob<T>(string jobName, string cron, bool startNow = false) where T : IJob;
    Task RemoveJob(string jobName);
    Task PauseJob(string jobName);
    Task ResumeJob(string jobName);
}