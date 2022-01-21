using System;

namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for option
    /// </summary>
    public record OptionDto : OptionRequestDto, IDto
    {
        /// <summary>
        /// Identifier of ingredient
        /// </summary>
        public Guid Id { get; set; }
    }
}