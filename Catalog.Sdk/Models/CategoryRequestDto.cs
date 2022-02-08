using System;

namespace Catalog.Sdk.Models;

/// <summary>
///     Request data transfer object for category
/// </summary>
public record CategoryRequestDto : IRequestDto
{
    /// <summary>
    ///     Title of product
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Possible subcategories
    /// </summary>
    public Guid[] SubCategories { get; set; }

    /// <summary>
    ///     Order of category
    /// </summary>
    public int Order { get; set; }
}