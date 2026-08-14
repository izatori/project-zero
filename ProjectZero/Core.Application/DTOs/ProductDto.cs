namespace Core.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string FileName,
    double Price,
    string Description,
    DateTime CreationDate,
    DateTime LastUpdateDate,
    bool IsActive);