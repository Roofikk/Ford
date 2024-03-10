using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    internal class RetrieveArray<T>
    {
        public ICollection<T> Items { get; set; }

        public RetrieveArray(ICollection<T> items)
        {
            Items = items;
        }
    }
}
