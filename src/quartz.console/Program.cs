// See https://aka.ms/new-console-template for more information

using Quartz;
using Quartz.Impl;

public static class Program {
    private static async Task Main(string[] args)
    {
        // Grab the Scheduler instance from the Factory
        var factory = new StdSchedulerFactory();
        var scheduler = await factory.GetScheduler();

        // and start it off
        await scheduler.Start();

        // some sleep to show what's happening
        await Task.Delay(TimeSpan.FromSeconds(10));

        // and last shut down the scheduler when you are ready to close your program
        await scheduler.Shutdown();
    }
}