using Infrastructure.Models;
using Models.Request;

namespace Repositories.Interface
{
    public interface IFoodAnalaysisRepository
    {
        Task<FoodAnalysis> Add(FoodAnalysis foodAnalysis);
        Task<FoodAnalysis?> GetFoodAnalaysisBySerialNumber(string serialNumber);
        Task<FoodAnalysis> Update(string serialNumber, string result);
    }
}
