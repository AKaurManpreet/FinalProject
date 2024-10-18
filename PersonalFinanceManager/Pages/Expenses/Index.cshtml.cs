using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using System.Security.Claims;

namespace PersonalFinanceManager.Pages.Expenses
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Expense> Expenses { get; set; } = new List<Expense>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            Expenses = await _context.Expenses
                                     .Where(e => e.UserId == userId)
                                     .ToListAsync();
        }
    }
}
