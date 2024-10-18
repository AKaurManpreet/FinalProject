using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
