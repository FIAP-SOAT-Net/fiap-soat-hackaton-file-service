using Amazon.Runtime;
using Amazon.S3;
using Fiap.Soat.Hackaton.FileService.Api.Endpoints;
using MongoDB.Driver;
using Fiap.Soat.Hackaton.FileService.Infrastructure.Repositories;
using Fiap.Soat.Hackaton.FileService.Infrastructure.Services;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;

var builder = WebApplication.CreateBuilder(args);

var s3Section = builder.Configuration.GetSection("S3");
var s3Config = new AmazonS3Config
{
    ServiceURL = s3Section["ServiceUrl"],
    ForcePathStyle = bool.TryParse(s3Section["ForcePathStyle"], out bool fps) && fps,
    AuthenticationRegion = s3Section["Region"] ?? "us-east-1"
};

var credentials = new BasicAWSCredentials(s3Section["AccessKey"] ?? "test", s3Section["SecretKey"] ?? "test");

_ = builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(credentials, s3Config));

_ = builder.Services.AddScoped<IFileController, FileController>();
_ = builder.Services.AddScoped<IFileRepository, FileRepository>();
_ = builder.Services.AddScoped<IFileStorage, AwsS3Storage>();
_ = builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
_ = builder.Services.AddScoped<IUploadFileHandler, UploadFileHandler>();
_ = builder.Services.AddScoped<IGetFileHandler, GetFileHandler>();
_ = builder.Services.AddScoped<IListFilesHandler, ListFilesHandler>();
_ = builder.Services.AddScoped<IDeleteFileHandler, DeleteFileHandler>();

_ = builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

// Add services to the container.
_ = builder.Services.AddOpenApi();

// MongoDB Configuration
string mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
                               ?? throw new InvalidOperationException("MongoDB connection string not configured");
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(builder.Configuration["MongoDb:Database"] ?? "fileservice");

_ = builder.Services.AddSingleton<IMongoClient>(mongoClient);
_ = builder.Services.AddSingleton(mongoDatabase);


// Health Checks
_ = builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

_ = app.UseHttpsRedirection();
_ = app.MapHealthChecks("/healthcheck");

FileEndpoints.Map(app);

await app.RunAsync();

public partial class Program
{
}
