using Core.Application.Abstractions;

namespace Core.Application.Tests.Abstractions;

public class ApplicationExceptionsTests
{
    [Fact]
    public void EntityNotFoundException_ContainsEntityNameAndId()
    {
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var exception = new EntityNotFoundException("User", id);

        Assert.Contains("User", exception.Message);
        Assert.Contains(id.ToString(), exception.Message);
        Assert.IsAssignableFrom<Core.Application.Abstractions.ApplicationException>(exception);
    }

    [Fact]
    public void ValidationException_WithErrors_ExposesErrors()
    {
        var exception = new ValidationException("Invalid input", new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        });

        Assert.Equal("Invalid input", exception.Message);
        Assert.Contains("Email", exception.Errors.Keys);
        Assert.Equal("Email is required", exception.Errors["Email"].Single());
    }

    [Fact]
    public void ValidationException_WithoutErrors_InitializesEmptyErrors()
    {
        var exception = new ValidationException("Invalid input");

        Assert.NotNull(exception.Errors);
        Assert.Empty(exception.Errors);
    }
}