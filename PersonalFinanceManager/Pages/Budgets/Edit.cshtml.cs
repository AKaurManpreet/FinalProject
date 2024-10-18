using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceManager.Pages.Budgets
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
        public Budget Budget { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Budget = await _context.Budgets.FindAsync(id);
            if (Budget == null)
            {
                _logger.LogWarning($"Budget with ID {id} was not found.");
                return NotFound();
            }

            _logger.LogInformation($"Budget with ID {id} retrieved for editing.");
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

            Budget.UserId = userId;

            // Manually clear the model state error for UserId
            ModelState.Remove(nameof(Budget.UserId));

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

            // Retrieve the existing Budget from the database
            var existingBudget = await _context.Budgets.FindAsync(Budget.Id);
            if (existingBudget == null)
            {
                _logger.LogWarning($"Attempted to update non-existent Budget with ID {Budget.Id}.");
                return NotFound();
            }

            // Update the properties of the existing Budget
            existingBudget.Category = Budget.Category;
            existingBudget.Amount = Budget.Amount;
            existingBudget.UserId = Budget.UserId;

            _context.Budgets.Update(existingBudget);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Budget with ID {existingBudget.Id} updated successfully.");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Budgets.Any(b => b.Id == existingBudget.Id))
                {
                    _logger.LogWarning($"Budget with ID {existingBudget.Id} no longer exists for update.");
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error saving budget: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Unable to save budget due to a database error. Please try again.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error saving budget: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }
        }
    }
}
