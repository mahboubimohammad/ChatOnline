
using ChatOnlineApi.Application.Services;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Infrastructure.Base;
using ChatOnlineApi.Infrastructure.Configuration;
using ChatOnlineApi.Infrastructure.Persistence.Contexts;
using ChatOnlineApi.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- File Upload Configuration ----------

var fileUploadSettings = builder.Configuration
    .GetSection("FileUpload")
    .Get<FileUploadOptions>();

if (fileUploadSettings == null ||
    fileUploadSettings.MaxFileSizeMB <= 0 ||
    fileUploadSettings.MaxRequestSizeMB <= 0 ||
    string.IsNullOrWhiteSpace(fileUploadSettings.UploadPath) ||
    fileUploadSettings.AllowedExtensions == null ||
    fileUploadSettings.AllowedExtensions.Count == 0)
{
    throw new InvalidOperationException(
        "FileUpload configuration is invalid or missing.");
}

if (fileUploadSettings.MaxRequestSizeMB < fileUploadSettings.MaxFileSizeMB)
{
    throw new InvalidOperationException(
        "MaxRequestSizeMB cannot be smaller than MaxFileSizeMB.");
}

builder.Services.Configure<FileUploadOptions>(
    builder.Configuration.GetSection("FileUpload"));

var maxRequestSize =
    fileUploadSettings.MaxRequestSizeMB * 1024L * 1024L;

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxRequestSize;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestSize;
});

// ---------- Serilog ----------

builder.Logging.ClearProviders();

var seriLogSettings = builder.Configuration
    .GetSection("SeriLog");

var insertLog = seriLogSettings.GetValue<bool>("InsertLog");
var insertInfoLog = seriLogSettings.GetValue<bool>("InsertInfoLog");
var logFilePath = seriLogSettings["LogFilePath"] ?? "Logs/";
var logFileName = seriLogSettings["LogFileName"] ?? "ChatOnlineApi";
var logFileSizeMB = seriLogSettings.GetValue<int>("LogFileSizeMB");

if (logFileSizeMB <= 0)
{
    logFileSizeMB = 10;
}

var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Is(
        insertInfoLog
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override(
        "Microsoft",
        Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore",
        Serilog.Events.LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console();

if (insertLog)
{
    loggerConfig.WriteTo.File(
        path: Path.Combine(logFilePath, $"{logFileName}-.log"),
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: logFileSizeMB * 1024L * 1024L,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 30);
}

Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog(Log.Logger);

Log.Warning("Application starting...");

// ---------- Configurations ----------

builder.Services.Configure<LogOptions>(
    builder.Configuration.GetSection("SeriLog"));

// ---------- DbContext ----------

var connectionString = builder.Configuration
    .GetConnectionString("ChatOnlineDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ChatOnlineDb connection string is not configured.");
}

builder.Services.AddDbContext<ChatOnlineDbContext>(
    options => options.UseSqlServer(connectionString));

// ---------- Generic Repository & Unit Of Work ----------

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<
    IUnitOfWork,
    UnitOfWork<ChatOnlineDbContext>>();

// ---------- Specific Repositories ----------

var repositoryAssembly = typeof(ConversationRepository).Assembly;

var repositoryTypes = repositoryAssembly.GetTypes()
    .Where(type =>
        type.IsClass &&
        !type.IsAbstract &&
        type.Namespace ==
        "ChatOnlineApi.Infrastructure.Persistence.Repositories");

foreach (var repositoryType in repositoryTypes)
{
    var repositoryInterface = repositoryType.GetInterfaces()
        .FirstOrDefault(interfaceType =>
            interfaceType.Namespace ==
            "ChatOnlineApi.Domain.Interfaces");

    if (repositoryInterface != null)
    {
        builder.Services.AddScoped(
            repositoryInterface,
            repositoryType);
    }
}

// ---------- Application Services ----------

var serviceAssembly = typeof(ConversationService).Assembly;

var serviceTypes = serviceAssembly.GetTypes()
    .Where(type =>
        type.IsClass &&
        !type.IsAbstract &&
        type.Namespace ==
        "ChatOnlineApi.Application.Services" &&
        type.Name != "MessageAttachmentService");

foreach (var serviceType in serviceTypes)
{
    var serviceInterface = serviceType.GetInterfaces()
        .FirstOrDefault(interfaceType =>
            interfaceType.Namespace ==
            "ChatOnlineApi.Application.Contracts");

    if (serviceInterface != null)
    {
        builder.Services.AddScoped(
            serviceInterface,
            serviceType);
    }
}

// ---------- JWT Configuration ----------

var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["JwtKey"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT key must be configured and contain at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "JWT issuer is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT audience is not configured.");
}

var jwtKeySecurity = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtKey));

// ---------- JWT Authentication ----------

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,
                RequireExpirationTime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtKeySecurity,

                ClockSkew = TimeSpan.Zero
            };
    });

// ---------- Authorization ----------

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BackofficeBearerPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// ---------- Controllers & Localization ----------

builder.Services.AddLocalization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// ---------- Swagger ----------

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("backoffice", new OpenApiInfo
    {
        Title = "Chat Online Api",
        Version = "1.0"
    });

    var xmlFile = Path.Combine(
        AppContext.BaseDirectory,
        "ChatOnlineApi.xml");

    c.IncludeXmlComments(xmlFile, true);

    // Bearer Authentication
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme."
        });

    // Authorization Requirement
    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// ---------- Build App ----------

var app = builder.Build();

// ---------- Localization ----------

var supportedCultures = new[] { "fa-IR" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("fa-IR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// ---------- Middleware ----------
app.UseSerilogRequestLogging();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// ---------- Swagger ----------
var swaggerConfig = builder.Configuration
    .GetSection("Swagger");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        swaggerConfig["BackofficeSwaggerAddress"]!,
        "backoffice Api 1.0");

    c.InjectStylesheet(
        swaggerConfig["SwaggerCssAddress"]!);
});

// ---------- Static Files ----------
app.UseStaticFiles();

// ---------- Controllers ----------
app.MapControllers();

app.Run();