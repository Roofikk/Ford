using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    internal class RetrieveArray<T>
    {
        public IEnumerable<T> Items { get; set; }

        public RetrieveArray(IEnumerable<T> items)
        {
            Items = items;
        }
    }
}
