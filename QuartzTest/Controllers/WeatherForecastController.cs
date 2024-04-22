using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzTest.IServices;
using QuartzTest.Jobs;
using QuartzTest.Models;
using QuartzTest.MqttTest;

namespace QuartzTest.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly MqttClientService _mqttClient;
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IQuartzService _quartzService;
    List<JobInfo> _jobInfos = new();
    List<TelemetryTask> _telemetryTasks = new();
    List<TelemetryPoint> _telemetryPoints = new();
    public WeatherForecastController(MqttClientService mqttClient, ILogger<WeatherForecastController> logger,
        IQuartzService quartzService)
    {
        _mqttClient = mqttClient;
        _logger = logger;
        _quartzService = quartzService;
        for (var i = 1; i < 100; i++)
        {
            _jobInfos.Add(new JobInfo($"采集点位{i}", new TriggerInfo("点位{i}触发器", "*/2 * * * * ?")));
            // _jobInfos = new List<JobInfo>()
            // {
            //     new JobInfo("采集点位1", new TriggerInfo("点位1触发器", "*/2 * * * * ?")),
            //     new JobInfo("采集点位2", new TriggerInfo("点位2触发器", "*/3 * * * * ?")),
            //     new JobInfo("采集点位3", new TriggerInfo("点位3触发器", "*/4 * * * * ?")),
            //     new JobInfo("采集点位4", new TriggerInfo("点位4触发器", "*/5 * * * * ?")),
            // };
        }

        for (var i = 0; i < 4; i++)
        {
            _telemetryTasks.Add(new TelemetryTask($"G{new Random().Next(1, 5)}", 5000, "", telemetryType: (TelemetryTypeEnum)new Random().Next(1, 4)));
        }

        for (int i = 0; i < 20; i++)
        {
            var index = (i + 1).ToString().PadLeft(2, '0');
            _telemetryPoints.Add(new TelemetryPoint($"点位{index}", $"p{index}", "plcGroup1", "plc1", "127.0.0.1", "int", RWTypeEnum.OnlyRead, "", "", "", "", "", _telemetryTasks[new Random().Next(0, 3)]));
        }
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)]
        })
            .ToArray();
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob()
    {
        foreach (var _jobInfo in _jobInfos)
        {
            var jobDataMap = new JobDataMap()
            {
                new KeyValuePair<string, object>("name", _jobInfo.Name),
                new KeyValuePair<string, object>("LastVal", "null"),
                new KeyValuePair<string, object>("CurVal", "null"),
                new KeyValuePair<string, object>("MqttTopic", _jobInfo.Name),
                new KeyValuePair<string, object>("TelemetryType", (TelemetryTypeEnum)new Random().Next(1,9)),
                new KeyValuePair<string, object>("RunCounts", 0)
            };
            await _quartzService.AddCronJob<TestJob>(new JobKey(_jobInfo.Name), new TriggerKey(_jobInfo.Triggers.FirstOrDefault()!.Name), _jobInfo.Triggers.FirstOrDefault()!.Cron, jobDataMap);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        await _quartzService.Start();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> PauseJob(string jobName)
    {
        await _quartzService.PauseJob(jobName);
        return Ok();
    }

    [HttpGet]
    public IActionResult Test123()
    {
        return Ok(_mqttClient._client!.IsConnected);
    }

    [HttpPost]
    public async Task<IActionResult> TestPublish(string message)
    {
        var res = await _mqttClient.Publish(message);
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTelemetryJob()
    {
        foreach (var _telemetryTask in _telemetryTasks)
        {
            await _quartzService.AddJob(_telemetryTask.CreateJobDetail(JobBuilder.Create<TelemetryPointJob>()), _telemetryTask.CreateTrigger());
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetJobGroupNames()
    {
        return Ok(await _quartzService.GetJobGroupNames());
    }

    [HttpPost]
    public async Task<IActionResult> PauseJobsByGroup(string jobGroupName)
    {
        await _quartzService.PauseJobsByGroup(jobGroupName);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ResumeJobsByGroup(string jobGroupName)
    {
        await _quartzService.ResumeJobsByGroup(jobGroupName);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> GetJobNamesByGroup(string jobGroupName)
    {
        return Ok(await _quartzService.GetJobNamesByGroup(jobGroupName));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTelemetryPointJobs()
    {
        foreach (var telemetryPoint in _telemetryPoints)
        {
            await _quartzService.AddJob(telemetryPoint.CreateJobDetail(JobBuilder.Create<TelemetryPointJob>()), telemetryPoint.CreateTrigger());
        }
        return Ok(true);
    }
}