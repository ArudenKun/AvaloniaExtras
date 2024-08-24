using System.Collections;
using System.Collections.Generic;

namespace AvaloniaExtras.Collections;

/// <summary>
/// A wrapper for double backing store for the keys and values
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IDoubleDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    /// <summary>Gets or sets the element with the specified value.</summary>
    /// <param name="val">The value of the element to get or set.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="val" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="val" /> is not found.</exception>
    /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    /// <returns>The element with the specified key.</returns>
    TKey this[TValue val] { get; set; }

    /// <summary>Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified value.</summary>
    /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the value; otherwise, <see langword="false" />.</returns>
    bool ContainsValue(TValue value);

    /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="value">The value of the element to remove.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="value" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    bool Remove(TValue value);

    /// <summary>Gets the key associated with the specified value.</summary>
    /// <param name="value">The value whose key to get.</param>
    /// <param name="key">When this method returns, the key associated with the specified value, if the value is found; otherwise, the default key for the type of the <paramref name="key" /> parameter. This parameter is passed uninitialized.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified value; otherwise, <see langword="false" />.</returns>
    bool TryGetKey(TValue value, out TKey key);
}

/// <inheritdoc />
public class DoubleDictionary<TKey, TValue> : IDoubleDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _keyToValue = new();
    private readonly Dictionary<TValue, TKey> _valueToKey = new();

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get => _keyToValue[key];
        set
        {
            if (_keyToValue.TryGetValue(key, out var oldVal))
            {
                _valueToKey.Remove(oldVal);
            }

            _keyToValue[key] = value;
            _valueToKey[value] = key;
        }
    }

    /// <summary>Gets or sets the element with the specified value.</summary>
    /// <param name="val">The value of the element to get or set.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="val" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="val" /> is not found.</exception>
    /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    /// <returns>The element with the specified key.</returns>
    public TKey this[TValue val]
    {
        get => _valueToKey[val];
        set
        {
            if (_valueToKey.TryGetValue(val, out var oldVal))
            {
                _keyToValue.Remove(oldVal);
            }

            _valueToKey[val] = value;
            _keyToValue[value] = val;
        }
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys => _keyToValue.Keys;

    /// <inheritdoc />
    public ICollection<TValue> Values => _valueToKey.Keys;

    /// <inheritdoc />
    public int Count => _keyToValue.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        _keyToValue[key] = value;
        _valueToKey[value] = key;
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _keyToValue[item.Key] = item.Value;
        _valueToKey[item.Value] = item.Key;
    }

    /// <inheritdoc />
    public void Clear()
    {
        _keyToValue.Clear();
        _valueToKey.Clear();
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) =>
        _keyToValue.ContainsKey(item.Key) && _valueToKey.ContainsKey(item.Value);

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => _keyToValue.ContainsKey(key);

    /// <summary>Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified value.</summary>
    /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the value; otherwise, <see langword="false" />.</returns>
    public bool ContainsValue(TValue value) => _valueToKey.ContainsKey(value);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _keyToValue.GetEnumerator();

    /// <inheritdoc />
    public bool Remove(TKey key)
    {
        if (!_keyToValue.Remove(key, out var val))
            return false;
        _valueToKey.Remove(val);
        return true;
    }

    /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="value">The value of the element to remove.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="value" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    public bool Remove(TValue value)
    {
        if (!_valueToKey.Remove(value, out var key))
            return false;

        _keyToValue.Remove(key);
        return true;
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_keyToValue.TryGetValue(key, out value))
        {
            return true;
        }

        value = default!;
        return false;
    }

    /// <summary>Gets the key associated with the specified value.</summary>
    /// <param name="value">The value whose key to get.</param>
    /// <param name="key">When this method returns, the key associated with the specified value, if the value is found; otherwise, the default key for the type of the <paramref name="key" /> parameter. This parameter is passed uninitialized.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified value; otherwise, <see langword="false" />.</returns>
    public bool TryGetKey(TValue value, out TKey key)
    {
        if (_valueToKey.TryGetValue(value, out key))
        {
            return true;
        }

        key = default!;
        return false;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _keyToValue.GetEnumerator();
}
