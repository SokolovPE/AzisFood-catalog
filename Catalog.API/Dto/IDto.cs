namespace Catalog.Dto
{
    /// <summary>
    /// Some data transfer object.
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// Identifier of object.
        /// </summary>
        public string Id { get; set; }
    }
}