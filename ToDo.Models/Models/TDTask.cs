using System.ComponentModel.DataAnnotations;

namespace ToDo.Models.Models
{
    public class TDTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Range(0, 100,ErrorMessage = "Percentage value must be between 0 to 100.")]
        public double CompletionPercentage { get; set; } = 0;
        public DateTime ExpirationDate { get; set; }

        public string Status { get; set; } = "InProgress";
    }
}
