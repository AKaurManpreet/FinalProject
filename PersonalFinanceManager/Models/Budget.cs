using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public decimal Amount { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
