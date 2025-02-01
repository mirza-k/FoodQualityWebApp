using Microsoft.Extensions.DependencyInjection;
using Models.Messaging;
using Services.Interface;
using System.Text.Json;

namespace Services.Implementation
{
    public class QualityManagerListener
    {
        private readonly IMessagingBus _messagingBus;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const string FoodAnalysingResultQueue = "food_analysing_result";

        public QualityManagerListener(IMessagingBus messagingBus, IServiceScopeFactory serviceScopeFactory)
        {
            _messagingBus = messagingBus;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public void StartListening()
        {
            _messagingBus.Subscribe(FoodAnalysingResultQueue, async message =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var qualityManagerService = scope.ServiceProvider.GetRequiredService<IQualityManagerService>();

                var foodAnalysingResult = JsonSerializer.Deserialize<FoodAnalysisMessageResponse>(message);
                if (foodAnalysingResult == null) throw new ArgumentNullException();

                await qualityManagerService.UpdateFoodAnalysis(foodAnalysingResult.SerialNumber, foodAnalysingResult.AnalysisResult);
            });
        }
    }
}
