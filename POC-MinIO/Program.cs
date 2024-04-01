using Minio;

using POC_MinIO.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var minioOptions = builder.Configuration.GetRequiredSection("MinioOptions").Get<MinioOptions>();

builder.Services.AddMinio(configureClient =>
{
    configureClient.WithEndpoint(minioOptions?.EndPoint)
                   .WithCredentials(minioOptions?.AccessKey, minioOptions?.SecretKey)
                   .WithSSL(false);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
