using Core.Domain.Entities;

namespace Core.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ReturnsProduct()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Widget", product.Name);
        Assert.Equal("widget.jpg", product.FileName);
        Assert.Equal(19.99, product.Price);
        Assert.Equal("A handy widget", product.Description);
        Assert.True(product.IsActive);
        Assert.Null(product.UpdatedAt);
        Assert.InRange(product.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() => Product.Create(name!, "widget.jpg", 19.99, "desc"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyFileName_ThrowsArgumentException(string? fileName)
    {
        Assert.Throws<ArgumentException>(() => Product.Create("Widget", fileName!, 19.99, "desc"));
    }

    [Theory]
    [InlineData("widget.txt")]
    [InlineData("widget")]
    [InlineData("widget.jpeg ")]
    public void Create_WithInvalidFileName_ThrowsArgumentException(string fileName)
    {
        Assert.Throws<ArgumentException>(() => Product.Create("Widget", fileName, 19.99, "desc"));
    }

    [Theory]
    [InlineData("widget file.jpg")]
    [InlineData("widget!.jpg")]
    [InlineData("widget.jpg.exe")]
    public void Create_WithInvalidCharacters_ThrowsArgumentException(string fileName)
    {
        Assert.Throws<ArgumentException>(() => Product.Create("Widget", fileName, 19.99, "desc"));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void Create_WithNegativePrice_ThrowsArgumentException(double price)
    {
        Assert.Throws<ArgumentException>(() => Product.Create("Widget", "widget.jpg", price, "desc"));
    }

    [Fact]
    public void Create_RaisesProductCreatedEvent()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");

        var domainEvent = Assert.Single(product.GetDomainEvents());
        var created = Assert.IsType<ProductCreatedEvent>(domainEvent);

        Assert.Equal(product.Id, created.ProductId);
        Assert.Equal("Widget", created.Name);
        Assert.Equal("widget.jpg", created.FileName);
        Assert.Equal(19.99, created.Price);
        Assert.Equal("A handy widget", created.Description);
    }

    [Fact]
    public void Update_WithAllNull_ThrowsArgumentException()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        Assert.Throws<ArgumentException>(() => product.Update(null, null, null, null));
    }

    [Fact]
    public void Update_WithValidData_UpdatesProductAndRaisesEvent()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        product.Update("Super Widget", "super.jpg", 24.99, "An upgraded widget");

        Assert.Equal("Super Widget", product.Name);
        Assert.Equal("super.jpg", product.FileName);
        Assert.Equal(24.99, product.Price);
        Assert.Equal("An upgraded widget", product.Description);
        Assert.NotNull(product.UpdatedAt);

        var domainEvent = Assert.Single(product.GetDomainEvents());
        Assert.IsType<ProductUpdatedEvent>(domainEvent);
    }

    [Fact]
    public void Update_WithOnlyName_UpdatesOnlyName()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        product.Update(name: "Renamed", fileName: null, price: null, description: null);

        Assert.Equal("Renamed", product.Name);
        Assert.Equal("widget.jpg", product.FileName);
        Assert.Equal(19.99, product.Price);
        Assert.Equal("A handy widget", product.Description);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void Update_WithInvalidFileName_ThrowsArgumentException()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        Assert.Throws<ArgumentException>(() => product.Update(null, "bad file name", null, null));
    }

    [Fact]
    public void Update_WithNegativePrice_ThrowsArgumentException()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        Assert.Throws<ArgumentException>(() => product.Update(null, null, -5, null));
    }

    [Fact]
    public void Deactivate_WhenActive_SetsInactiveAndRaisesEvent()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.ClearDomainEvents();

        product.Deactivate();

        Assert.False(product.IsActive);
        Assert.NotNull(product.UpdatedAt);

        var domainEvent = Assert.Single(product.GetDomainEvents());
        Assert.IsType<ProductDeactivatedEvent>(domainEvent);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_DoesNotRaiseEvent()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        product.Deactivate();
        product.ClearDomainEvents();

        product.Deactivate();

        Assert.Empty(product.GetDomainEvents());
    }

    [Fact]
    public void TwoProducts_WithDifferentIds_AreNotEqual()
    {
        var product = Product.Create("Widget", "widget.jpg", 19.99, "A handy widget");
        var other = Product.Create("Gadget", "gadget.jpg", 29.99, "A shiny gadget");

        Assert.NotEqual(product.Id, other.Id);
        Assert.NotEqual(product, other);
    }
}
