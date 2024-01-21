namespace CIYW.Models;

public class KeyValue<K, V>
{
    public K Key { get; private set; }
    public V Value { get; set; }
    public KeyValue(K key)
    {
        Key = key;
    }
    public KeyValue(K key, V value)
    {
        Key = key;
        Value = value;
    }
}
