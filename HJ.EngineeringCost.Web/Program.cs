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

            // ����Ӧ������
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var appConfig = new AppConfig();
            builder.Configuration.Bind(appConfig);

            InitSerilog(appConfig);

            // �������ļ��л�ȡ�˿�
            builder.WebHost.UseUrls(appConfig.ApplicationUrl);

            // ���÷���
            builder.Services.AddSingleton(appConfig);
            builder.Services.AddSingleton(CreateFreeSql(appConfig));
            builder.Services.AddSingleton(Log.Logger);

            #region Mapster

            var assembly = Assembly.Load("HJ.EngineeringCost.Web");
            TypeAdapterConfig.GlobalSettings.Scan(assembly);
            builder.Services.AddSingleton<IMapper>(sp => new Mapper(TypeAdapterConfig.GlobalSettings));

            #endregion

            // ע��ҵ�����
            RegisterServices(builder.Services);

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = "EngineeringCostAuth"; // Cookie����
            //        options.Cookie.HttpOnly = true;
            //        options.LoginPath = "/Account/Login"; // ��¼ҳ��ַ
            //        //options.AccessDeniedPath = "/Account/AccessDenied"; // Ȩ�޲���ҳ��ַ
            //        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Cookie��Ч��
            //    });

            // ���MVC֧��
            builder.Services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<LoginFilter>();
            })
                .AddJsonOptions(options =>

                {
                    options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    // ����Զ���ת����
                    options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });

            var app = builder.Build();

            // �����м��
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
            Log.Fatal(ex, "Ӧ�ó���������ֹ");
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
                var errorMsg = $"���ݿ������ַ�������Ϊ��";
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

            Log.Information($"FreeSql ���ݿ����Ӵ����ɹ�");
            return freeSql;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "���� FreeSql ���ݿ�����ʧ��");
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

        // �������ļ���ȡSeq��������ַ
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