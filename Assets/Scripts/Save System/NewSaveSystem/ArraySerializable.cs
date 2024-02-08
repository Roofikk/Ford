using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2
{
    public class ArraySerializable<T>
    {
        public ICollection<T> Items { get; set; }

        public ArraySerializable(ICollection<T> items)
        {
            Items = items;
        }
    }
}
