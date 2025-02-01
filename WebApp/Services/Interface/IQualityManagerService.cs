using Models.Request;
using Models.Response;

namespace Services.Interface
{
    public interface IQualityManagerService
    {
        Task<ProcessFoodResponse> Process(ProcessFoodRequest processFoodRequest);
        Task<string> GetStateBySerialNumber(string serailNumber);
        Task<ProcessFoodResponse> UpdateFoodAnalysis(string serialNumber, string analysisResult);
    }
}
