using System;

namespace Catalog.Sdk.Models;

/// <summary>
///     Data transfer object for category
/// </summary>
public record CategoryDto : CategoryRequestDto, IDto
{
    /// <summary>
    ///     Identifier of category
    /// </summary>
    public Guid Id { get; set; }
}