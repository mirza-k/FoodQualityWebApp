namespace Models.Request
{
    public class ProcessFoodRequest
    {
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public int TypeOfAnalysis { get; set; }
    }
}
