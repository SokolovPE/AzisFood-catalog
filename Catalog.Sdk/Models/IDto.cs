using System;

namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Some data transfer object
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// Identifier of object
        /// </summary>
        public Guid Id { get; set; }
    }
}