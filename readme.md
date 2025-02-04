# OrderedDictionary<TKey, TValue>

The `OrderedDictionary<TKey, TValue>` is a generic collection that maintains the insertion order of its elements. It implements the `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`, and `IList<KeyValuePair<TKey, TValue>>` interfaces, providing both key-based lookup and index-based access. This makes it especially useful in environments like Unity with Mono, where the standard `OrderedDictionary` is not available.

## Features

- **Maintains Insertion Order:** Elements are stored in the order they are added.
- **Dual Access:** Retrieve items by key or by index.
- **Interfaces Implemented:** Supports standard dictionary and list interfaces.
- **Unity Compatibility:** Serves as an alternative in environments (like Unity with Mono) that lack a built-in ordered dictionary.

## Complexity (Big-O Notation)

- **Add:** O(1).
- **Remove:** O(n) worst-case due to re-indexing of the underlying list.
- **Lookup by key:** O(1).
- **Lookup by index:** O(1).
- **Insert (by index):** O(n) worst-case.
- **Clear:** O(n)

## Usage Example

```csharp
using Collections.OrderedDictionary;

var dict = new OrderedDictionary<string, int>();

// Add items
dict.Add("apple", 1);
dict["banana"] = 2;

// Access items by key
int appleValue = dict["apple"];
Console.WriteLine(appleValue); // Output: 1

// Update value by key
dict["banana"] = 3;
Console.WriteLine(dict["banana"]); // Output: 3

```

## Conclusion

The `OrderedDictionary<TKey, TValue>` offers a robust solution for maintaining insertion order while allowing fast key-based lookups and index-based access. This makes it an ideal choice for projects, particularly in environments like Unity with Mono, where a built-in ordered dictionary is unavailable.