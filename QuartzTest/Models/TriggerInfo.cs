namespace QuartzTest.Models;

public class TriggerInfo
{
    public string Name { get; private set; }
    public string Cron { get; private set; }

    public TriggerInfo(string name, string cron)
    {
        Name = name;
        Cron = cron;
    }
}