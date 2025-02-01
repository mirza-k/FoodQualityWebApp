using AutoMapper;
using Infrastructure.Models;
using Models.Enum;
using Models.Messaging;
using Models.Request;
using Models.Response;
using Repositories.Interface;
using Services.Interface;
using System.Text.Json;

namespace Services.Implementation
{
    public class QualityManagerService : IQualityManagerService
    {
        private readonly IFoodAnalaysisRepository _foodRepository;
        private readonly IMessagingBus _messagingBus;
        private readonly IMapper _mapper;
        private string FoodAnalysingCreateQueue = "food_analysing_create";
        public QualityManagerService(IFoodAnalaysisRepository foodRepository, IMapper mapper, IMessagingBus messagingBus)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
            _messagingBus = messagingBus;
        }

        public async Task<ProcessFoodResponse> Process(ProcessFoodRequest processFoodRequest)
        {
            try
            {
                // add new entity
                FoodAnalysis entity = _mapper.Map<FoodAnalysis>(processFoodRequest);
                entity.CreateDate = DateTime.Now;
                entity.State = (int)AnalysisState.IN_PROGRESS;

                FoodAnalysis result = await _foodRepository.Add(entity);
                ProcessFoodResponse response = _mapper.Map<ProcessFoodResponse>(result);

                // prepare message and publish
                var request = new FoodAnalysisMessageRequest { FoodName = entity.Name, SerialNumber = entity.SerialNumber, AnalysisType = entity.TypeOfAnalysis };
                var message = JsonSerializer.Serialize(request);
                _messagingBus.Publish(FoodAnalysingCreateQueue, message);

                return response;
            }
            catch (Exception)
            {
                // add logging in future
                throw;
            }
        }

        public async Task<string> GetStateBySerialNumber(string serialNumber)
        {
            var result = await _foodRepository.GetFoodAnalaysisBySerialNumber(serialNumber);

            if (result == null)
            {
                return $"Could not find any food analysis with provided serial number {serialNumber}.";
            }

            return ResolveAnalysis((AnalysisState)result.State, result.Result);
        }

        public async Task<ProcessFoodResponse> UpdateFoodAnalysis(string serialNumber, string analysisResult)
        {
            var result = await _foodRepository.Update(serialNumber, analysisResult);
            ProcessFoodResponse response = _mapper.Map<ProcessFoodResponse>(result);
            return response;
        }

        private string ResolveAnalysis(AnalysisState analysisState, string? analaysisResult)
        {
            return analysisState == AnalysisState.IN_PROGRESS ? "Food analysing still in progress." : $"Processing complete: {analaysisResult}.";
        }
    }
}
