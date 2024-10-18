using Xunit;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.Tests.Data
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void CanInsertBudgetIntoDatabase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var budget = new Budget { Name = "Test Budget", Amount = 1000 };
                context.Budgets.Add(budget);
                context.SaveChanges();

                var savedBudget = context.Budgets.Find(budget.Id);
                Assert.NotNull(savedBudget);
                Assert.Equal("Test Budget", savedBudget.Name);
            }
        }
    }
}
