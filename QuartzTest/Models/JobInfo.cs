namespace QuartzTest.Models;

public class JobInfo
{
    public string Name { get; private set; }
    public ICollection<TriggerInfo> Triggers { get; set; }

    public JobInfo(string name)
    {
        Name = name;
        Triggers = new List<TriggerInfo>();
    }

    public JobInfo(string name, TriggerInfo triggerInfo)
        : this(name)
    {
        Triggers.Add(triggerInfo);
    }
}