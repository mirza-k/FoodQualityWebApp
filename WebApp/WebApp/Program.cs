using AnalysisEngine.Infrastructure.Data;
using AnalysisEngine.Repositories.Implementation;
using AnalysisEngine.Repositories.Interface;
using AnalysisEngine.Service.Implementation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Repositories.Repository;
using Services.Automapper;
using Services.Implementation;
using Services.Interface;
using WebApp.Middleware;
using WebApp.Validator;

var builder = WebApplication.CreateBuilder(args);

var rabbitMqHost = builder.Configuration.GetValue<string>("RabbitMQ:Host", "rabbitmq");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IQualityManagerService, QualityManagerService>();
builder.Services.AddTransient<IFoodAnalaysisRepository, FoodAnalaysisRepository>();
builder.Services.AddTransient<IMessagingBus>(sp =>
 new Services.Messaging.MessagingBus(rabbitMqHost));
builder.Services.AddSingleton<QualityManagerListener>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ProcessFoodRequestValidator>();

// In real-world, this should not be the case. We should have different two microservices
// that are separated and did not know anything about each other
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppDb"))
);

// Second microservice config
builder.Services.AddDbContext<AnalysisEngineDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AnalysisEngineDb"))
);
builder.Services.AddTransient<IAnalysisRepository, AnalysisRepository>();
builder.Services.AddTransient<AnalysisEngine.Service.Interface.IMessagingBus>(sp =>
    new AnalysisEngine.Service.Implementation.MessagingBus(rabbitMqHost)); 
builder.Services.AddSingleton<AnalysisEngineListener>();

var app = builder.Build();

// Initialize listeners
var qualityManagerListener = app.Services.GetRequiredService<QualityManagerListener>();
qualityManagerListener.StartListening();

var analysisEnginelistener = app.Services.GetRequiredService<AnalysisEngineListener>();
analysisEnginelistener.StartListening();


// Run Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var webAppDb = services.GetRequiredService<AppDbContext>();
        var analysisEngineDb = services.GetRequiredService<AnalysisEngineDbContext>();

        webAppDb.Database.Migrate();
        analysisEngineDb.Database.Migrate();

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
