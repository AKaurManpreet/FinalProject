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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public Expense Expense { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Expense = await _context.Expenses.FindAsync(id);
            if (Expense == null)
            {
                _logger.LogWarning($"Expense with ID {id} was not found.");
                return NotFound();
            }

            _logger.LogInformation($"Expense with ID {id} retrieved for editing.");
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

            Expense.UserId = userId;

            // Manually clear the model state error for UserId
            ModelState.Remove(nameof(Expense.UserId));

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid. Validation errors:");
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

            // Retrieve the existing Expense from the database
            var existingExpense = await _context.Expenses.FindAsync(Expense.Id);
            if (existingExpense == null)
            {
                _logger.LogWarning($"Attempted to update non-existent Expense with ID {Expense.Id}.");
                return NotFound();
            }

            // Update the properties of the existing Expense
            existingExpense.Description = Expense.Description;
            existingExpense.Amount = Expense.Amount;
            existingExpense.Date = Expense.Date;
            existingExpense.UserId = Expense.UserId;

            _context.Expenses.Update(existingExpense);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Expense with ID {existingExpense.Id} updated successfully.");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Expenses.Any(e => e.Id == existingExpense.Id))
                {
                    _logger.LogWarning($"Expense with ID {existingExpense.Id} no longer exists for update.");
                    return NotFound();
                }
                throw;
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
