using System.ComponentModel.DataAnnotations;

namespace ToDo.Models.DTOs
{
    public class UpdatePercentageDTO
    {

        [Required]
        public int Id { get; set; }
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        public double Percentage { get; set; }
    }
}
