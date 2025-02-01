using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    [Table("FoodAnalysis", Schema = "manager")]
    public class FoodAnalysis
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        public int TypeOfAnalysis { get; set; }
        public int State { get; set; }
        public string? Result { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
