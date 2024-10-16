using System.Diagnostics;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using quartz.web.Jobs;
using Quartzmon;

namespace quartz.web;

public static class Program {

    public static async Task Main(string[] args) {

        StdSchedulerFactory.GetDefaultScheduler().Result.Start();
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        services.AddRouting();
        services.AddMvc();
        services.AddQuartz(q => {
            q.SchedulerName = "ez-scheduler";
        });
        services.AddQuartzHostedService(options => {
            options.WaitForJobsToComplete = true;
        });
        services.AddQuartzmon();
        
        var app = builder.Build();
        var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();
        
        app.UseRouting();
        app.UseQuartzmon(new QuartzmonOptions {
            Scheduler = scheduler,
            VirtualPathRoot = "/quartzmon"
        });

        var job = JobBuilder.Create<TestJob>()
            .WithIdentity("TestJobName", "group1")
            .WithDescription("Test Job Description")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("myTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => {
                x.WithIntervalInSeconds(1).RepeatForever();
            }).Build();
        await scheduler.ScheduleJob(job, trigger);
        
        app.MapGet("/", () => "Hello World!");
        
        StartQuartzmonInWebBrowser();
        
        await app.RunAsync();
    }

    private static void StartQuartzmonInWebBrowser() {
        Process.Start(new ProcessStartInfo {
            FileName = "http://localhost:5066/quartzmon",
            UseShellExecute = true
        });
    }

    // public static void ConfigureMicrosoftLogger()
    // {
    //     var loggerFactory = LoggerFactory.Create(builder =>
    //     {
    //         builder
    //             .SetMinimumLevel(LogLevel.Debug)
    //             .AddSimpleConsole(options =>
    //             {
    //                 options.IncludeScopes = true;
    //                 options.SingleLine = true;
    //                 options.TimestampFormat = "hh:mm:ss ";
    //             });
    //     });
    //     LogProvider.SetLogProvider(loggerFactory);
    // }

}