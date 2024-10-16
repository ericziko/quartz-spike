using System.Collections.Specialized;
using System.Diagnostics;
using Quartz;
using Quartz.Impl;
using quartz.web.Jobs;
using Quartzmon;

namespace quartz.web;

public static class Program {

    public static async Task Main(string[] args) {
        // await StdSchedulerFactory.GetDefaultScheduler().Result.Start();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        // builder.Configuration.AddXmlFile("app.config",false);
        var services = builder.Services;
        services.AddRouting();
        services.AddMvc();


        services.AddQuartz(q => {
            // q.SchedulerName = "ez-scheduler";
            q.CheckConfiguration = true;
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
        services.AddQuartzmon();

        var app = builder.Build();
        app.UseRouting();


        // var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        // var scheduler = await schedulerFactory.GetScheduler();
        // var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        // var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var scheduler = DemoScheduler.Create().Result;
        app.UseQuartzmon(new QuartzmonOptions {
            Scheduler = scheduler,
            VirtualPathRoot = "/quartzmon",
        });

        var job = JobBuilder.Create<TestJob>()
            .WithIdentity("TestJobName", "group1")
            .WithDescription("Test Job Description")
            .StoreDurably()
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("myTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => { x.WithIntervalInSeconds(1).RepeatForever(); }).Build();
        await scheduler.ScheduleJob(job, trigger);

        app.MapGet("/", () => "Hello World!");

        StartQuartzmonInWebBrowser();

        await scheduler.Start();
        await app.RunAsync();
    }

    private static void StartQuartzmonInWebBrowser() {
        Process.Start(new ProcessStartInfo {
            FileName = "http://localhost:5066/quartzmon",
            UseShellExecute = true
        });
    }

}