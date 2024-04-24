using Microsoft.AspNetCore.Mvc;
using QuartzTest.IServices;
using QuartzTest.TestInfo;

namespace QuartzTest.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly IQuartzService _quartzService;

    public TestController(IQuartzService quartzService)
    {
        _quartzService = quartzService;
    }


    [HttpPost]
    public async Task<IActionResult> CreateTelemetryVariableJobs()
    {
        var mockData = TelemetryJobEntity.JobDetailAndTriggers;
        foreach (var item in mockData)
        {
            try
            {
                await _quartzService.AddJob(item.Key, item.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        return Ok();
    }
}
