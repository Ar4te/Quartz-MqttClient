using Quartz;

namespace QuartzTest.Jobs;

public class TestJob : IJob
{
    public TestJob()
    {
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var map = context.JobDetail.JobDataMap;
        var jobName = map.GetString("name");
        Console.WriteLine($"执行Job【{jobName}】：{DateTime.Now}");
        await Task.Delay(1000);
    }
}