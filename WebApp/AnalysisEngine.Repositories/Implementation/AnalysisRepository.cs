using AnalysisEngine.Infrastructure.Data;
using AnalysisEngine.Repositories.Interface;

namespace AnalysisEngine.Repositories.Implementation
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly AnalysisEngineDbContext _analysisEngineDbContext;
        public AnalysisRepository(AnalysisEngineDbContext analysisEngineDbContext)
        {
            _analysisEngineDbContext = analysisEngineDbContext;
        }
        public async Task Add(string serialNumber, string name, string result, int analysisType)
        {
            await _analysisEngineDbContext.AnalysisLog.AddAsync(new Infrastructure.Models.AnalysisLog { CreateDate = DateTime.Now, FoodName = name, SerialNumber = serialNumber, Result = result, TypeOfAnalysis = analysisType });
            await _analysisEngineDbContext.SaveChangesAsync();
        }
    }
}
