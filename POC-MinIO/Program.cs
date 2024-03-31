using Minio;

//var endpoint = "play.min.io";
//var accessKey = "yQ2D5SL3QRhUo4JeuDiX";
//var secretKey = "g3BzP09RTlBEEAEcyy9pZgRJCWcg5hkWXwZ29Uok";

var endpoint = "172.17.0.2:9000";
var accessKey = "KQJx6CfWkjJrEdLc5sCw";
var secretKey = "FmwhlZ8pKEOKvibGPjaKsBzm2nujMHX2zUqsmd4I";

var builder = WebApplication.CreateBuilder(args);

// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
builder.Services.AddMinio(configureClient => configureClient.WithEndpoint(endpoint).WithCredentials(accessKey, secretKey).WithSSL(false));

// Add services to the container.

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
