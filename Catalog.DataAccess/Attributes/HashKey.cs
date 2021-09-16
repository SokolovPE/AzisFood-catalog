using System;

namespace Catalog.DataAccess.Attributes
{
    /// <summary>
    /// Key of HashSet
    /// </summary>
    public class HashKey : Attribute
    {
        /// <summary>
        /// Key of HashSet
        /// </summary>
        public string Key { get; set; }
    }
}