using Quartz;
using QuartzTest.IServices;
using QuartzTest.MqttTest;
using QuartzTest.Services;

namespace QuartzTest;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "QuartzTest";
        var builder = WebApplication.CreateBuilder(args);

        //builder.Host.ConfigureSilkierQuartzHost();
        // Add services to the container.

        builder.Services.AddControllers();
        //builder.Services.AddControllersWithViews();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddQuartz(config => { config.UseMicrosoftDependencyInjectionJobFactory(); });

        builder.Services.AddTransient<IQuartzService, QuartzService>();
        builder.Services.AddSingleton(new MqttClientService());
        //var t = builder.Services.BuildServiceProvider().GetRequiredService<ISchedulerFactory>();
        //builder.Services.AddSilkierQuartz(options =>
        //{
        //    options.VirtualPathRoot = "/quartz";
        //    options.UseLocalTime = true;
        //    options.DefaultDateFormat = "yyyy-MM-dd";
        //    options.DefaultTimeFormat = "HH:mm:ss";
        //    options.CronExpressionOptions = new CronExpressionDescriptor.Options()
        //    {
        //        DayOfWeekStartIndexZero = false
        //    };
        //},
        //authencationOptions =>
        //{
        //    authencationOptions.AccessRequirement = SilkierQuartzAuthenticationOptions.SimpleAccessRequirement.AllowAnonymous;
        //});
        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        //app.UseStaticFiles();
        //app.UseRouting();
        //app.UseAuthentication();
        app.UseAuthorization();
        //app.UseSilkierQuartz();

        //app.UseQuartzJob<InjectSampleJob>(() =>
        //{
        //    var result = new List<TriggerBuilder>();
        //    result.Add(TriggerBuilder.Create()
        //        .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));
        //    return result;
        //});
        app.MapControllers();

        app.Run();
    }
}