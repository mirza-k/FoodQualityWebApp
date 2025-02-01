using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Models.Enum;
using Repositories.Interface;

namespace Repositories.Repository
{
    public class FoodAnalaysisRepository : IFoodAnalaysisRepository
    {
        private readonly AppDbContext _appDbContext;
        public FoodAnalaysisRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<FoodAnalysis> Add(FoodAnalysis foodAnalysis)
        {
            if (foodAnalysis == null) throw new ArgumentNullException(nameof(foodAnalysis));

            try
            {
                await _appDbContext.FoodAnalysis.AddAsync(foodAnalysis);
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // add logging in future
                throw new InvalidOperationException("Error saving FoodAnalysis", ex);
            }

            return foodAnalysis;
        }

        public async Task<FoodAnalysis?> GetFoodAnalaysisBySerialNumber(string serialNumber)
        {
            return await _appDbContext.FoodAnalysis.Where(x => x.SerialNumber == serialNumber).FirstOrDefaultAsync();
        }

        public async Task<FoodAnalysis> Update(string serialNumber, string result)
        {
            var foodAnalysis = await GetFoodAnalaysisBySerialNumber(serialNumber);
            if (foodAnalysis == null) throw new ArgumentNullException(nameof(foodAnalysis));

            foodAnalysis.Result = result;
            foodAnalysis.State = (int)AnalysisState.COMPLETED;
            foodAnalysis.EditDate = DateTime.Now;

            try
            {
                _appDbContext.FoodAnalysis.Update(foodAnalysis);
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // add logging in future
                throw new InvalidOperationException("Error saving FoodAnalysis", ex);
            }

            return foodAnalysis;
        }
    }
}
