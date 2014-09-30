using System;
using System.Collections.Generic;
using System.Collections;

namespace LitJson
{
    public interface IOrderedDictionary : IDictionary, ICollection, IEnumerable
    {
        Object this[int index] { get; set; }

        IDictionaryEnumerator GetEnumerator();
        void Insert(int index, Object key, Object value);
        void RemoveAt(int index);

    }
}
