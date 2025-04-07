using System;
using System.Collections.Generic;
using System.Text;
using GameFramework;

namespace Dungeon.BlackBoardSystem
{
    public static class StringExtensions
    {
        /// https://en.wikipedia.org/wiki/Fowler–Noll–Vo_hash_function
        public static int ComputeFNV1aHash(this string input)
        {
            const uint prime = 0x01000193;
            uint hash = 0x811C9DC5;
            foreach (byte c in Encoding.UTF8.GetBytes(input))
            {
                hash ^= c;
                hash *= prime;
            }
            return (int)hash;
        }
    }

    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        readonly string name;
        readonly int hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;

        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }

    [Serializable]
    public class Blackboard
    {
        Dictionary<string, BlackboardKey> keyRegistry = new();
        Dictionary<BlackboardKey, object> entries = new();

        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action)
        {
            if (action == null) return;
            PassedActions.Add(action);
        }

        public void ClearActions() => PassedActions.Clear();

        public string Debug()
        {
            StringBuilder builder = new();
            
            foreach (var entry in entries)
            {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    builder.AppendLine($"Key: {entry.Key}, Value: {value}");
                }
            }

            return builder.ToString();
        }

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            entries[key] = new BlackboardEntry<T>(key, value);
        }

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if (string.IsNullOrEmpty(keyName)) return default;

            if (!keyRegistry.TryGetValue(keyName, out BlackboardKey key))
            {
                key = new BlackboardKey(keyName);
                keyRegistry[keyName] = key;
            }

            return key;
        }

        public bool CheckIfKeyExists(string keyName)
        {
            if(string.IsNullOrEmpty(keyName)) return false;

            return keyRegistry.ContainsKey(keyName);
        }

        public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);

        public void Remove(BlackboardKey key) => entries.Remove(key);
    }
}