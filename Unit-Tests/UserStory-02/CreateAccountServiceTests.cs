using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-02 (CreateAccount). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing accounts) directly, so every model/run is exercised against
// this same fixed fixture rather than whatever arbitrary data each model would otherwise invent.
public class CreateAccountServiceTests
{
    private static List<Account> CreateExistingAccountsFixture()
    {
        return new List<Account>
        {
            new Account
            {
                AccountId = 1,
                Email = "existing.user@example.com",
                PasswordHash = "already-hashed-value",
                DisplayName = "Existing User",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                AccountId = 2,
                Email = "another.user@example.com",
                PasswordHash = "another-hashed-value",
                DisplayName = null,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static ICreateAccountService CreateService(IEnumerable<Account> existingAccounts)
    {
        return new CreateAccountService(existingAccounts);
    }

    [Fact]
    public void CreateAccount_WithValidInput_ReturnsNonNullAccount()
    {
        var service = CreateService(new List<Account>());

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateAccount_WithValidInput_PreservesEmailAndDisplayName()
    {
        var service = CreateService(new List<Account>());

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        Assert.Equal("new.user@example.com", result.Email);
        Assert.Equal("New User", result.DisplayName);
    }

    [Fact]
    public void CreateAccount_WithValidInput_AssignsPositiveAccountId()
    {
        var service = CreateService(new List<Account>());

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        Assert.True(result.AccountId > 0);
    }

    [Fact]
    public void CreateAccount_WithValidInput_SetsRecentCreatedAt()
    {
        var service = CreateService(new List<Account>());
        var before = DateTime.UtcNow;

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void CreateAccount_WithValidInput_DoesNotStorePasswordInPlainText()
    {
        var service = CreateService(new List<Account>());

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        Assert.False(string.IsNullOrWhiteSpace(result.PasswordHash));
        Assert.NotEqual("Password123!", result.PasswordHash);
    }

    [Fact]
    public void CreateAccount_WithNullDisplayName_DoesNotThrow()
    {
        var service = CreateService(new List<Account>());

        var exception = Record.Exception(() => service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: null));

        Assert.Null(exception);
    }

    [Fact]
    public void CreateAccount_AssignsAccountIdNotCollidingWithExistingAccounts()
    {
        var existingAccounts = CreateExistingAccountsFixture();
        var existingIds = existingAccounts.Select(a => a.AccountId).ToHashSet();
        var service = CreateService(existingAccounts);

        var result = service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "New User");

        Assert.DoesNotContain(result.AccountId, existingIds);
    }

    [Fact]
    public void CreateAccount_CalledTwice_ReturnsDifferentAccountIds()
    {
        var service = CreateService(new List<Account>());

        var first = service.CreateAccount(
            email: "first.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "First User");

        var second = service.CreateAccount(
            email: "second.user@example.com",
            password: "Password123!",
            confirmPassword: "Password123!",
            displayName: "Second User");

        Assert.NotEqual(first.AccountId, second.AccountId);
    }

    [Fact]
    public void CreateAccount_WhenPasswordsDoNotMatch_ThrowsArgumentException()
    {
        var service = CreateService(new List<Account>());

        Assert.ThrowsAny<ArgumentException>(() => service.CreateAccount(
            email: "new.user@example.com",
            password: "Password123!",
            confirmPassword: "DifferentPassword456!",
            displayName: "New User"));
    }

    [Theory]
    [InlineData(null, "Password123!", "Password123!")]
    [InlineData("", "Password123!", "Password123!")]
    [InlineData("   ", "Password123!", "Password123!")]
    [InlineData("new.user@example.com", null, "Password123!")]
    [InlineData("new.user@example.com", "", "Password123!")]
    [InlineData("new.user@example.com", "   ", "Password123!")]
    [InlineData("new.user@example.com", "Password123!", null)]
    [InlineData("new.user@example.com", "Password123!", "")]
    [InlineData("new.user@example.com", "Password123!", "   ")]
    public void CreateAccount_WithMissingRequiredField_ThrowsArgumentException(
        string? email, string? password, string? confirmPassword)
    {
        var service = CreateService(new List<Account>());

        Assert.ThrowsAny<ArgumentException>(() => service.CreateAccount(
            email: email!,
            password: password!,
            confirmPassword: confirmPassword!,
            displayName: "New User"));
    }
}
