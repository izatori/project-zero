giusing System.Text.RegularExpressions;
using Core.Domain.Abstractions;

namespace Core.Domain.Entities;

/// <summary>
/// Product aggregate root.
/// </summary>
public class Product : AggregateRoot<Guid>
{
    // Private constructor for EF and deserialization
    private Product(Guid id, string name, string fileName, double price, string description) : base(id)
    {
        Name = name;
        FileName = fileName;
        Price = Math.Round(price, 2);
        Description = description;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
    
    public string Name { get; private set; }
    
    public string FileName { get; private set; }
    
    public double Price { get; private set; }
    
    public string Description { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }
    
    public bool IsActive { get; private set; }

    /// <summary>
    /// Factory method to create a new Product.
    /// Contains all business logic for product creation.
    /// </summary>
    public static Product Create(string name, string fileName, double price, string description)
    {
        ValidateName(name);
        ValidateFileName(fileName);
        ValidatePrice(price);
        
        var product = new Product(Guid.NewGuid(), name, fileName, price, description);
        
        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, name, fileName, price, description));
        
        return product;
    }

    /// <summary>
    /// Update product information.
    /// </summary>
    public void Update(string? name, string? fileName, double? price, string? description)
    {
        if (name is null && fileName is null && price is null && description is null)
            throw new ArgumentException("At least one property must be provided to update");

        if (name is not null)
        {
            ValidateName(name);
            Name = name;
        }

        if (fileName is not null)
        {
            ValidateFileName(fileName);
            FileName = fileName.ToLower();
        }

        if (price is not null)
        {
            ValidatePrice(price.Value);
            Price = Math.Round(price.Value, 2);
        }

        if (description is not null)
            Description = description;

        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new ProductUpdatedEvent(Id, Name, FileName, Price, Description));
    }

    /// <summary>
    /// Deactivate the product.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }
        
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new ProductDeactivatedEvent(Id));
    }
    /// <summary>
    /// Validates the product name.
    /// </summary>
    private static void ValidateName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Name cannot be empty", nameof(fileName));
        }
    }

    /// <summary>
    /// Validates the file name.
    /// </summary>
    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File-Name cannot be empty", nameof(fileName));
        }

        if (!fileName.ToLower().EndsWith(".jpg") && !fileName.ToLower().EndsWith(".jpeg") 
                                                 && !fileName.ToLower().EndsWith(".png"))
        {
            throw new ArgumentException("File-Name must end with '.jpg' or '.jpeg' or '.png'",  nameof(fileName));
        }

        if (fileName.Contains(' '))
        {
            throw new ArgumentException("File-Name cannot contain spaces", nameof(fileName));
        }
        
        if (!Regex.IsMatch(fileName, @"^[a-zA-Z0-9_-]+\.(jpe?g|png)$", RegexOptions.IgnoreCase))
        {
            throw new ArgumentException("File-Name must only contain letters, numbers, - and _ followed by a valid image extension", nameof(fileName));
        }
    }

    /// <summary>
    /// Validates the product price.
    /// </summary>
    private static void ValidatePrice(double price)
    {
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }
    }
}

/// <summary>
/// Domain event raised when a product is created.
/// </summary>
public class ProductCreatedEvent : DomainEvent
{
    public ProductCreatedEvent(Guid id, string name, string fileName, double price, string description)
    {
        ProductId = id;
        Name = name;
        FileName = fileName;
        Price = price;
        Description = description;
    }
    
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string FileName { get; private set; }
    public double Price { get; private set; }
    public string Description { get; private set; }
}

/// <summary>
/// Domain event raised when a product is updated.
/// </summary>
public class ProductUpdatedEvent : DomainEvent
{
    public ProductUpdatedEvent(Guid id, string name, string fileName, double price, string description)
    {
        ProductId = id;
        Name = name;
        FileName = fileName;
        Price = price;
        Description = description;
    }
    
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string FileName { get; private set; }
    public double Price { get; private set; }
    public string Description { get; private set; }
}

/// <summary>
/// Domain event raised when a product is deactivated.
/// </summary>
public class ProductDeactivatedEvent : DomainEvent
{
    public ProductDeactivatedEvent(Guid id)
    {
        ProductId = id;
    }
    
    public Guid ProductId { get; private set; }
}