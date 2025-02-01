using System.ComponentModel.DataAnnotations;

namespace AnalysisEngine.Infrastructure.Models
{
    public class AnalysisLog
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string? FoodName { get; set; }
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        public int TypeOfAnalysis { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Result { get; set; }
    }
}
