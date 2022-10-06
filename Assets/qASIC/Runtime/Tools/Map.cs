using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace qASIC
{
    public class Map<T0, T1> : IEnumerable<KeyValuePair<T0, T1>>, IEnumerable
    {
        public Map()
        {
            Forward = new Indexer<T0, T1>(_forwardDictionary);
            Reverse = new Indexer<T1, T0>(_reverseDictionary);
        }

        public Map(Dictionary<T0, T1> dictionary)
        {
            _forwardDictionary = dictionary;
            _reverseDictionary = dictionary.ToDictionary(x => x.Value, x => x.Key);
        }

        private Dictionary<T0, T1> _forwardDictionary = new Dictionary<T0, T1>();
        private Dictionary<T1, T0> _reverseDictionary = new Dictionary<T1, T0>();

        public Indexer<T0, T1> Forward { get; private set; }
        public Indexer<T1, T0> Reverse { get; private set; }

        public int Count => _forwardDictionary.Count();
        public bool IsReadOnly => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<T0, T1>> GetEnumerator()
        {
            return _forwardDictionary.GetEnumerator();
        }

        public void Add(T0 forward, T1 reverse)
        {
            _forwardDictionary.Add(forward, reverse);
            _reverseDictionary.Add(reverse, forward);
        }

        public void RemoveForward(T0 key)
        {
            T1 reverseKey = _forwardDictionary[key];
            _forwardDictionary.Remove(key);
            _reverseDictionary.Remove(reverseKey);
        }

        public void RemoveReverse(T1 key)
        {
            T0 forwardKey = _reverseDictionary[key];
            _forwardDictionary.Remove(forwardKey);
            _reverseDictionary.Remove(key);
        }

        public void Clear()
        {
            _forwardDictionary.Clear();
            _reverseDictionary.Clear();
        }

        public bool Contains(KeyValuePair<T0, T1> item) =>
            _forwardDictionary.Contains(item);

        public class Indexer<T, t>
        {
            private readonly Dictionary<T, t> _dictionary;

            public Indexer(Dictionary<T, t> dictionary)
            {
                _dictionary = dictionary;
            }

            public t this[T index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            public bool Contains(T key)
            {
                return _dictionary.ContainsKey(key);
            }
        }
    }
}
