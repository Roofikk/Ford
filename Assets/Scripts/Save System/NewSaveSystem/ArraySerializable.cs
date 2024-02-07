using System.Collections.Generic;

public class ArraySerializable<T>
{
    public ICollection<T> Items { get; set; }

    public ArraySerializable(ICollection<T> items)
    {
        Items = items;
    }
}
