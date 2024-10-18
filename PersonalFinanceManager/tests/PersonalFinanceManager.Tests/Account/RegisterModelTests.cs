using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Areas.Identity.Pages.Account;

namespace PersonalFinanceManager.Tests.Account
{
    public class RegisterModelTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;

        public RegisterModelTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null
            );
        }

        [Fact]
        public async Task OnPostAsync_ValidModel_RedirectsToReturnUrl()
        {
            // Arrange
            var registerModel = new RegisterModel(_userManagerMock.Object, _signInManagerMock.Object)
            {
                Input = new RegisterModel.InputModel
                {
                    Email = "test@example.com",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                }
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await registerModel.OnPostAsync("/");

            // Assert
            var redirectToPageResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal("/", redirectToPageResult.Url);
        }

        [Fact]
        public async Task OnPostAsync_InvalidPassword_ReturnsError()
        {
            // Arrange
            var registerModel = new RegisterModel(_userManagerMock.Object, _signInManagerMock.Object)
            {
                Input = new RegisterModel.InputModel
                {
                    Email = "test@example.com",
                    Password = "short",
                    ConfirmPassword = "short"
                }
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too short" }));

            // Act
            var result = await registerModel.OnPostAsync("/");

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Contains(registerModel.ModelState[""].Errors, e => e.ErrorMessage == "Password is too short");
        }
    }
}
