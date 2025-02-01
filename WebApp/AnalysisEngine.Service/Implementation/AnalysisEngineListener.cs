using AnalysisEngine.Models.Messaging;
using AnalysisEngine.Repositories.Interface;
using AnalysisEngine.Service.Interface;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AnalysisEngine.Service.Implementation
{
    public class AnalysisEngineListener
    {
        private readonly IMessagingBus _messagingBus;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private string subscribeQueue = "food_analysing_create";
        private string publishQueue = "food_analysing_result";
        public AnalysisEngineListener(IMessagingBus messagingBus, IServiceScopeFactory serviceScopeFactory)
        {
            _messagingBus = messagingBus;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void StartListening()
        {
            _messagingBus.Subscribe(subscribeQueue, async message =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var _analysisRepository = scope.ServiceProvider.GetRequiredService<IAnalysisRepository>();

                var parsedMessage = JsonSerializer.Deserialize<FoodAnalysisRequest>(message);
                if (parsedMessage == null) throw new ArgumentNullException();

                await Task.Delay(2000);

                var analysisResult = "Microorganisms are within limits";
                await _analysisRepository.Add(parsedMessage.SerialNumber, parsedMessage.FoodName, analysisResult, parsedMessage.AnalysisType);

                FoodAnalysisResponse response = new FoodAnalysisResponse { AnalysisResult = analysisResult, SerialNumber = parsedMessage.SerialNumber };
                var responseMessage = JsonSerializer.Serialize(response);
                _messagingBus.Publish(publishQueue, responseMessage);
            });
        }
    }
}
