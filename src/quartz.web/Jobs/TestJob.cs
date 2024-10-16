using Quartz;

namespace quartz.web.Jobs;

public class TestJob:IJob {
    public Task Execute(IJobExecutionContext context) {
        Console.WriteLine("Test Job");
        return Task.CompletedTask;
    }

}