using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Controllers;

[ApiController]
[Route("api/[Controller]/[action]")]
public class TestController:ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> log()
    {
        _logger.LogDebug("1");
        _logger.LogError("2");
        _logger.LogInformation("3");
        _logger.LogTrace("4");
        _logger.LogWarning("5");

        return Ok();
    }
}