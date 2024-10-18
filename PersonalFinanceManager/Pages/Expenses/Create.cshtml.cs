using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceManager.Pages.Expenses
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public Expense Expense { get; set; }
        
        public string UserId { get; set; }

        public IActionResult OnGet()
        {
            UserId = _userManager.GetUserId(User);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                _logger.LogError("User is not authenticated.");
                return RedirectToPage("./Index");
            }

            // Set UserId in Expense before validation
            Expense.UserId = userId;
            _logger.LogInformation($"UserId set for Expense: {Expense.UserId}");

            // Manually clear the model state error for UserId
            ModelState.Remove(nameof(Expense.UserId));

            // Validate the model state after setting UserId
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    _logger.LogWarning($"Validation for {modelState.Key}: value: {modelState.Value?.AttemptedValue}");
                    foreach (var error in modelState.Value.Errors)
                    {
                        var attemptedValue = modelState.Value.AttemptedValue ?? "null";
                        _logger.LogWarning($"Validation Error for {modelState.Key}: {error.ErrorMessage} but attempted value: {attemptedValue}");
                    }
                }
                return Page();
            }

            _context.Expenses.Add(Expense);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error saving expense: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Unable to save expense due to a database error. Please try again.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error saving expense: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }
        }


    }
}
