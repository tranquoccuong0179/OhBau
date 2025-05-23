using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using OhBau.API.Constants;
using OhBau.API;
using Microsoft.OpenApi.Any;
using OhBau.Model.Enum;
using Serilog;
using OhBau.API.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VNPayService.Config;
using Microsoft.Extensions.Options;
using VNPayService;
using EmailService.Config;
using Microsoft.Extensions.DependencyInjection;
using EmailService.Service;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();
builder.Services.AddScoped(typeof(GenericCacheInvalidator<>));
//VNPay Config
var vnPaySection = builder.Configuration.GetSection("VNPayConfig");
builder.Services.Configure<VNPayConfig>(vnPaySection);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<VNPayConfig>>().Value);
builder.Services.AddScoped<IVnPayService,VNPayService.VNPayService>();

//Email Sender
var emailSection = builder.Configuration.GetSection("EmailSetting");
builder.Services.Configure<EmailSetting>(emailSection);
builder.Services.AddSingleton(reslover => reslover.GetRequiredService<IOptions<EmailSetting>>().Value);
var emailSetting = emailSection.Get<EmailSetting>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

//Serilog Config
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabase();
builder.Services.AddUnitOfWork();
builder.Services.AddCustomServices();
builder.Services.AddJwtValidation();
builder.Services.AddHttpClientServices();
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var account = new CloudinaryDotNet.Account(
        configuration["Cloudinary:CloudName"],
        configuration["Cloudinary:ApiKey"],
        configuration["Cloudinary:Secret"]);
    return account;
}      
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OhBau's API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Enter 'Bearer' [space] and then your token in the text input below.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            new string[] { }
        },
    };
    c.AddSecurityRequirement(securityRequirement);
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.MapType<RoleEnum>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(RoleEnum))
               .Select(name => new OpenApiString(name) as IOpenApiAny)
               .ToList()
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsConstant.PolicyName,
        policy =>
        {
            policy.WithOrigins("https://ohbau.cloud", "http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
var app = builder.Build();

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/test-telegram-error", () =>
{
    Log.Error("Đây là lỗi thử nghiệm gửi về Telegram");
    throw new Exception("Đây là Exception test gửi về Telegram");
});

app.UseCors(CorsConstant.PolicyName);

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

