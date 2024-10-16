using Quartz;

namespace quartz.web.Jobs;

public class TestJob:IJob {

    private ILogger<TestJob> _logger;
    public TestJob(ILogger<TestJob> logger) {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context) {
        _logger.LogInformation("Test Job");
        return Task.CompletedTask;
    }

}