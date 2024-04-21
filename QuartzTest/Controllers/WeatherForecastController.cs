using Microsoft.AspNetCore.Mvc;
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
            await _quartzService.AddJob<TestJob>(_jobInfo.Name, _jobInfo.Triggers.FirstOrDefault()!.Cron);
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
}