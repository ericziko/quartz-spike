using Microsoft.AspNetCore.Builder.Internal;
using Quartz;
using quartz.worker;
using Quartzmin;

public class Program {

    public static void Main(string[] args) {
        
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        
        
        var services = builder.Services;
        services.AddHostedService<Worker>();
        services.AddQuartz(q => {
        // services.AddQuartzmin();
        
        });
        services.AddQuartzHostedService(options => {
            options.WaitForJobsToComplete = true;
        });
        
        var host = builder.Build();
        host.Run();
    }

}