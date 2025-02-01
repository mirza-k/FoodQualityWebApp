using AnalysisEngine.Infrastructure.Data;
using AnalysisEngine.Repositories.Implementation;
using AnalysisEngine.Repositories.Interface;
using AnalysisEngine.Service.Implementation;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Enum;
using Models.Request;
using Repositories.Interface;
using Repositories.Repository;
using Services.Automapper;
using Services.Implementation;
using Services.Interface;
using Testcontainers.RabbitMq;
using Xunit;

namespace Tests.IntegrationTests
{
    public class IntegrationTests : IAsyncLifetime
    {
        private readonly RabbitMqContainer _rabbitMqContainer;
        private ServiceProvider _serviceProvider;
        private IServiceScopeFactory _serviceScopeFactory;

        public IntegrationTests()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                .WithImage("rabbitmq:3-management")
                .WithCleanUp(true)
                .WithPortBinding(5672, 5672)
                .WithPortBinding(15672, 15672)
                .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
                .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
                .WithName("rabbitmq")
                .Build();
        }
        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();

            var services = new ServiceCollection();

            // configure databases
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("WebAppDb"));

            services.AddDbContext<AnalysisEngineDbContext>(options =>
                options.UseInMemoryDatabase("AnalysisEngineDb"));

            services.AddAutoMapper(typeof(MappingProfile));

            // mock quality manager messaging buses
            services.AddSingleton<IMessagingBus>(sp => new Services.Messaging.MessagingBus("localhost"));

            // mock analysis engine messaging buses
            services.AddSingleton<AnalysisEngine.Service.Interface.IMessagingBus>(
                sp => new AnalysisEngine.Service.Implementation.MessagingBus("localhost"));

            // Register services and listeners
            services.AddTransient<IQualityManagerService, QualityManagerService>();
            services.AddTransient<IFoodAnalaysisRepository, FoodAnalaysisRepository>();
            services.AddTransient<IAnalysisRepository, AnalysisRepository>();
            services.AddSingleton<AnalysisEngineListener>();
            services.AddSingleton<QualityManagerListener>();
            services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());
            _serviceProvider = services.BuildServiceProvider();
            _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        [Fact]
        public async Task FullMessagingWorkflow_ShouldProcessMessageAndUpdateDatabase()
        {
            var qualityManagerService = _serviceProvider.GetRequiredService<IQualityManagerService>();

            var analysisEngineListener = _serviceProvider.GetRequiredService<AnalysisEngineListener>();
            var qualityManagerListener = _serviceProvider.GetRequiredService<QualityManagerListener>();

            var dbContext = _serviceProvider.GetRequiredService<AppDbContext>();

            // Start listeners
            var analysisListenerTask = Task.Run(() => analysisEngineListener.StartListening());
            var qualityListenerTask = Task.Run(() => qualityManagerListener.StartListening());

            var request = new ProcessFoodRequest
            {
                Name = "TestFood",
                SerialNumber = "12345",
                TypeOfAnalysis = (int) AnalysisType.MICROBIOLOGICAL_ANALAYSIS
            };

            var response = await qualityManagerService.Process(request);

            // wait for messages to be processed
            await Task.Delay(5000);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var processedEntity = await scopedDbContext.FoodAnalysis.FirstOrDefaultAsync(f => f.SerialNumber == "12345");

                Assert.NotNull(processedEntity);
                Assert.Equal("12345", processedEntity.SerialNumber);
                Assert.Equal((int)AnalysisState.COMPLETED, processedEntity.State);
            }
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync();
        }

    }
}
