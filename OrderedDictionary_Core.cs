//Copyright 2025 Daniil Glagolev
//Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections.OrderedDictionary
{
    /// <summary>
    /// Represents an ordered dictionary that maintains insertion order.
    /// Implements IDictionary, IReadOnlyDictionary, and IList of KeyValuePair.
    /// </summary>
    public partial class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        #region Fields

        private readonly Dictionary<TKey, int> _indexMap;
        private readonly List<TKey> _keys;
        private readonly List<TValue> _values;

        #region Properties

        /// <summary>
        /// Gets a read-only collection of keys.
        /// </summary>
        public ICollection<TKey> Keys => _keys.AsReadOnly();

        /// <summary>
        /// Gets a read-only collection of values.
        /// </summary>
        public ICollection<TValue> Values => _values.AsReadOnly();

        /// <summary>
        /// Gets the number of key/value pairs contained in the OrderedDictionary.
        /// </summary>
        public int Count => _keys.Count;

        /// <summary>
        /// Gets a value indicating whether the OrderedDictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the OrderedDictionary class.
        /// </summary>
        public OrderedDictionary()
        {
            _indexMap = new Dictionary<TKey, int>();
            _keys = new List<TKey>();
            _values = new List<TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the OrderedDictionary class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        public OrderedDictionary(int capacity)
        {
            _indexMap = new Dictionary<TKey, int>(capacity);
            _keys = new List<TKey>(capacity);
            _values = new List<TValue>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the OrderedDictionary class with the specified key comparer.
        /// </summary>
        /// <param name="comparer">The equality comparer to use for the keys.</param>
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            _indexMap = new Dictionary<TKey, int>(comparer);
            _keys = new List<TKey>();
            _values = new List<TValue>();
        }

        #endregion

        #region []

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public TValue this[TKey key]
        {
            get => _values[_indexMap[key]];
            set
            {
                if (_indexMap.TryGetValue(key, out var index)) _values[index] = value;
                else Add(key, value);
            }
        }

        /// <summary>
        /// Gets or sets the key/value pair at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The key/value pair at the specified index.</returns>
        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

                return new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
            }
            set
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

                var oldKey = _keys[index];

                if (!EqualityComparer<TKey>.Default.Equals(oldKey, value.Key) && _indexMap.ContainsKey(value.Key)) 
                    throw new ArgumentException("An element with the same key already exists.", nameof(value));

                _indexMap.Remove(oldKey);

                _keys[index] = value.Key;
                _values[index] = value.Value;

                _indexMap[value.Key] = index;
            }
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds the specified key and value to the OrderedDictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            if (_indexMap.ContainsKey(key)) throw new ArgumentException("Key already exists", nameof(key));

            _indexMap.Add(key, _keys.Count);
            _keys.Add(key);
            _values.Add(value);
        }

        /// <summary>
        /// Adds the specified key and value to the OrderedDictionary.
        /// </summary>
        /// <param name="item">The key and value to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        #endregion

        #region Clear/Copy

        /// <summary>
        /// Removes all keys and values from the OrderedDictionary.
        /// </summary>
        public void Clear()
        {
            _indexMap.Clear();
            _keys.Clear();
            _values.Clear();
        }
        
        /// <summary>
        /// Copies the elements of the OrderedDictionary to an array, starting at a particular index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count) throw new ArgumentException("Target array is too small");

            for (var i = 0; i < _keys.Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        #endregion

        #region Contains

        /// <summary>
        /// Determines whether the OrderedDictionary contains the specified key and value.
        /// </summary>
        /// <param name="item">The key and value to locate.</param>
        /// <returns>true if the key and value are found; otherwise, false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <summary>
        /// Determines whether the OrderedDictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the OrderedDictionary contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(TKey key) => _indexMap.ContainsKey(key);

        #endregion

        #region Enumerator

        /// <summary>
        /// Returns an enumerator that iterates through the OrderedDictionary.
        /// </summary>
        /// <returns>An enumerator for the OrderedDictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var i = 0; i < _keys.Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        #region Remove
        
        /// <summary>
        /// Removes the element with the specified key from the OrderedDictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {
            if (!_indexMap.TryGetValue(key, out var index)) return false;

            RemoveEntry(key, index);
            return true;
        }

        /// <summary>
        /// Removes the specified key and value from the OrderedDictionary.
        /// </summary>
        /// <param name="item">The key and value to remove.</param>
        /// <returns>true if the key and value are successfully removed; otherwise, false.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!_indexMap.TryGetValue(item.Key, out var index)) return false;

            if (!EqualityComparer<TValue>.Default.Equals(_values[index], item.Value)) return false;

            RemoveEntry(item.Key, index);
            return true;
        }
        
        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

            var key = _keys[index];
            RemoveEntry(key, index);
        }

        private void RemoveEntry(TKey key, int index)
        {
            _indexMap.Remove(key);
            _keys.RemoveAt(index);
            _values.RemoveAt(index);

            for (var i = index; i < _keys.Count; i++)
            {
                var currentKey = _keys[i];
                _indexMap[currentKey] = i;
            }
        }
        
        #endregion

        #region Get

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the key.</param>
        /// <returns>true if the OrderedDictionary contains an element with the key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_indexMap.TryGetValue(key, out var index))
            {
                value = _values[index];
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Returns the zero-based index of the specified key and value.
        /// </summary>
        /// <param name="item">The key and value to locate.</param>
        /// <returns>The index if found; otherwise, -1.</returns>
        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            if (!_indexMap.TryGetValue(item.Key, out var index)) return -1;

            return EqualityComparer<TValue>.Default.Equals(_values[index], item.Value) ? index : -1;
        }

        /// <summary>
        /// Returns the zero-based index of the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The index if found; otherwise, -1.</returns>
        public int IndexOf(TKey key) => _indexMap.GetValueOrDefault(key, -1);

        #endregion
    }
}
