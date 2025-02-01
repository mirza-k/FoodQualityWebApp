namespace AnalysisEngine.Models.Messaging
{
    public class FoodAnalysisRequest
    {
        public string? SerialNumber { get; set; }
        public string? FoodName { get; set; }
        public int AnalysisType { get; set; }
    }
}
