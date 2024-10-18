using System.Threading.Tasks;
using Xunit;
using Moq;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManager.Tests.Services
{
    public class BudgetServiceTests
    {
        private readonly ApplicationDbContext _context;

        public BudgetServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddBudget_SuccessfullyAddsBudget()
        {
            // Arrange
            var budget = new Budget { Name = "Test Budget", Amount = 500 };
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Budgets.FirstOrDefaultAsync(b => b.Name == "Test Budget");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Budget", result.Name);
            Assert.Equal(500, result.Amount);
        }
    }
}
