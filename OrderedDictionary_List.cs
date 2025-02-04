//Copyright 2025 Daniil Glagolev
//Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Collections.OrderedDictionary
{
    public partial class OrderedDictionary<TKey, TValue> : IList<KeyValuePair<TKey, TValue>>, IReadOnlyList<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets or sets the element at the specified index as a key-value pair.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The key-value pair at the specified index.</returns>
        KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
        {
            get => new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
            set
            {
                if (value.Key == null) throw new ArgumentNullException(nameof(value.Key));

                var currentKey = _keys[index];
                if (_indexMap.Comparer.Equals(value.Key, currentKey))
                {
                    _values[index] = value.Value;
                }
                else
                {
                    if (_indexMap.ContainsKey(value.Key)) throw new ArgumentException("Key already exists", nameof(value.Key));

                    RemoveAt(index);
                    Insert(index, value.Key, value.Value);
                }
            }
        }

        /// <summary>
        /// Inserts the specified key-value pair at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="item">The key-value pair to insert.</param>
        void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item) => Insert(index, item.Key, item.Value);

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        void IList<KeyValuePair<TKey, TValue>>.RemoveAt(int index) => RemoveAt(index);
        
        private void Insert(int index, TKey key, TValue value)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));

            if (_indexMap.ContainsKey(key)) throw new ArgumentException("Key already exists", nameof(key));

            _keys.Insert(index, key);
            _values.Insert(index, value);

            for (var i = index; i < _keys.Count; i++) _indexMap[_keys[i]] = i;
        }
    }
}
