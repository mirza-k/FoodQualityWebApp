namespace AnalysisEngine.Repositories.Interface
{
    public interface IAnalysisRepository
    {
        Task Add(string serialNumber, string name, string result, int analysisType);
    }
}
