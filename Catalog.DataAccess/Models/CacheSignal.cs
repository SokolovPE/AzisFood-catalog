using System;

namespace Catalog.DataAccess.Models
{
    public class CacheSignal
    {
        public DateTime SendTime { get; set; }
        public string Source { get; set; }

        public CacheSignal(string source)
        {
            SendTime = DateTime.Now;
            Source = source;
        }
    }
}