namespace Models.Messaging
{
    public class FoodAnalysisMessageRequest
    {
        public string? SerialNumber { get; set; }
        public string? FoodName { get; set; }
        public int AnalysisType { get; set; }
    }
}
