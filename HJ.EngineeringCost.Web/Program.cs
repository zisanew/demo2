using FreeSql;
using HJ.EngineeringCost.Web.Filter;
using HJ.EngineeringCost.Web.MapConfig;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // 配置应用设置
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var appConfig = new AppConfig();
            builder.Configuration.Bind(appConfig);

            InitSerilog(appConfig);

            // 从配置文件中获取端口
            builder.WebHost.UseUrls(appConfig.ApplicationUrl);

            // 配置服务
            builder.Services.AddSingleton(appConfig);
            builder.Services.AddSingleton(CreateFreeSql(appConfig));
            builder.Services.AddSingleton(Log.Logger);

            #region Mapster

            var assembly = Assembly.Load("HJ.EngineeringCost.Web");
            TypeAdapterConfig.GlobalSettings.Scan(assembly);
            builder.Services.AddSingleton<IMapper>(sp => new Mapper(TypeAdapterConfig.GlobalSettings));

            #endregion

            // 注册业务服务
            RegisterServices(builder.Services);

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = "EngineeringCostAuth"; // Cookie名称
            //        options.Cookie.HttpOnly = true;
            //        options.LoginPath = "/Account/Login"; // 登录页地址
            //        //options.AccessDeniedPath = "/Account/AccessDenied"; // 权限不足页地址
            //        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Cookie有效期
            //    });

            // 添加MVC支持
            builder.Services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<LoginFilter>();
            })
                .AddJsonOptions(options =>

                {
                    options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    // 添加自定义转换器
                    options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });

            var app = builder.Build();

            // 配置中间件
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "应用程序意外终止");
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void RegisterServices(IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.Namespace.EndsWith("Service")
                     && t.IsClass
                     && !t.IsAbstract
                     && t.IsPublic);

        services.AddSingleton(serviceTypes);
    }

    private static IFreeSql CreateFreeSql(AppConfig appConfig)
    {
        try
        {
            if (string.IsNullOrEmpty(appConfig.ConnectionString))
            {
                var errorMsg = $"数据库连接字符串不能为空";
                Log.Fatal(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            var freeSql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, appConfig.ConnectionString)
                .UseMonitorCommand(cmd => Log.Debug($"SQL: {cmd.CommandText}"))
#if DEBUG
                .UseAutoSyncStructure(true)
#endif
                .Build();

            Log.Information($"FreeSql 数据库连接创建成功");
            return freeSql;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "创建 FreeSql 数据库连接失败");
            throw;
        }
    }

    private static void InitSerilog(AppConfig appConfig)
    {
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        var infoLogDirectory = Path.Combine(logDirectory, "Info");
        var errorLogDirectory = Path.Combine(logDirectory, "Error");

        if (!Directory.Exists(infoLogDirectory))
            Directory.CreateDirectory(infoLogDirectory);
        if (!Directory.Exists(errorLogDirectory))
            Directory.CreateDirectory(errorLogDirectory);

        // 从配置文件读取Seq服务器地址
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()

            .WriteTo.File(
                path: Path.Combine(infoLogDirectory, "log.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30,
                shared: true
            )
            .WriteTo.File(
                path: Path.Combine(errorLogDirectory, "error.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30,
                shared: true,
                restrictedToMinimumLevel: LogEventLevel.Error
            )
            .WriteTo.Seq(appConfig.SeqServerUrl)
            .CreateLogger();
    }
}