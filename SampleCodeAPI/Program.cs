using API_JeeSale.Services;
using AutomationService.Consumers;
using DPSinfra.ConnectionCache;
using DPSinfra.Kafka;
using DPSinfra.Logger;
using DPSinfra.Notifier;
using DPSinfra.Redis;
using DPSinfra.Vault;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using SampleCodeAPI.Services;
using StackExchange.Redis;
using VaultSharp.V1.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DPSInfra
#region Add Vault and get Vault for secret in another services 
// get Secret key from Vault
var serviceConfig = builder.Configuration.GetVaultConfig();
var vaultClient = builder.Services.addVaultService(serviceConfig); // vaultClient là Client của Vault đã được thêm vào trước
Secret<SecretData> kafkaSecret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "jwt", mountPoint: "kv").Result;
IDictionary<string, object> kafkaData = kafkaSecret.Data.Data;

var accessSecret = kafkaData["access_secret"].ToString();
builder.Configuration["Jwt:access_secret"] = accessSecret;

//config cho kafka
Secret<SecretData> kafkaSecret2 = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "kafka", mountPoint: "kv").Result;
IDictionary<string, object> kafkaData2 = kafkaSecret2.Data.Data;
builder.Configuration["KafkaConfig:username"] = kafkaData2["username"].ToString();
builder.Configuration["KafkaConfig:password"] = kafkaData2["password"].ToString();

//config cho minio
Secret<SecretData> minioSecret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "minio", mountPoint: "kv").Result;
IDictionary<string, object> minioData = minioSecret.Data.Data;
builder.Configuration["MinioConfig:MinioAccessKey"] = minioData["access_key"].ToString();
builder.Configuration["MinioConfig:MinioSecretKey"] = minioData["secret_key"].ToString();

//config cho connection catche
builder.Configuration["Jwt:internal_secret"] = kafkaData["internal_secret"].ToString();
#endregion
//add provider mới cho logger 
builder.Services.AddLogging(builder =>
{
    builder.addAsyncLogger(p => new AsyncLoggerProvider(p.GetService<IProducer>()));
});

builder.Services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddHttpClient();

builder.Services.AddScoped<MinioObject>();

// add Kafka
builder.Services.addKafkaService();
//add Notification (phải đặt sau khi add Kafka)
builder.Services.addNotificationService();

builder.Services.AddMemoryCache();
builder.Services.addConnectionCacheService();

//add Services sử dụng cho api version
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader =
    ApiVersionReader.Combine(
       new HeaderApiVersionReader("x-api-version"),
       new QueryStringApiVersionReader("version"));
});

//add Service sử dụng redis
var config = new ConfigurationOptions
{
    AbortOnConnectFail = false,
    Ssl = false,
    Password = builder.Configuration["Redis:REDIS_PASS"],
};
config.EndPoints.Add(builder.Configuration["Redis:REDIS_HOST"], int.Parse(builder.Configuration["Redis:REDIS_PORT"].ToString()));
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddTransient<IRedisService, RedisService>();
builder.Services.AddTransient<INotifyService, NotifyService>();
builder.Services.AddTransient<IHostedService, Automation_Consumer1>();
builder.Services.AddTransient<IConnectionService, ConnectionService>();
#endregion

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Giữ nguyên tên thuộc tính
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowOrigin");

app.UseAuthentication();

app.Run();
