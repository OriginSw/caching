using System;
using System.Collections.Generic;
using Sixeyed.Caching.Serialization;

namespace Sixeyed.Caching.Caches
{
    /// <summary>
    /// Provides two levels of cache.
    /// You can use a small fast (may be local) cache backed up by a larger slower (may be distributed) cache
    /// Operates by checking the fastest, level 1 cache first; if it hits, the process proceeds at high speed. Otherwise, the next cache (level 2) is checked.
    /// </summary>
    public class MultiLevelCache : ICache
    {
        protected ICache _level1;
        protected ICache _level2;
        protected bool _synchronizable;
        protected TimeSpan _level1Expiration;

        /// <param name="level1">ICache implementation for level 1</param>
        /// <param name="level2">ICache implementation for level 2</param>
        /// <param name="level1Expiration">Level 1 default expiration. Higher may be faster but you must keep it low if you want avoid that level 1 reach level 2 size</param>
        /// <param name="synchronizable">Indicates if caches are synchronizable. E.g. caches are not synchronizable when level 2 is distributed but level 1 is local.</param>
        public MultiLevelCache(
            ICache level1,
            ICache level2,
            TimeSpan level1Expiration,
            bool synchronizable = false)
        {
            this._level1 = level1;
            this._level2 = level2;
            this._level1Expiration = level1Expiration;
            this._synchronizable = synchronizable;
            Initialise();
        }

        public CacheType CacheType
        {
            get { return this._level2.CacheType; }
        }

        public Serializer Serializer { get; set; }

        public void Initialise()
        {
            this._level1.Initialise();
            this._level2.Initialise();
        }

        public void Set(string key, object value, TimeSpan validFor, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this._level2.Set(key, value, validFor, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        public void Set(string key, object value, DateTime expiresAt, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this._level2.Set(key, value, expiresAt, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        public void Set(string key, object value, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this._level2.Set(key, value, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        protected void SetAtLevel1(string key, object value, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            // TODO validate that level1 expiration is lower or equal than level2
            this._level1.Set(key, value, this._level1Expiration, serializationFormat);
        }

        public T Get<T>(string key, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            var value = Get(typeof(T), key, serializationFormat); ;

            if (value == null)
                return default(T);

            return (T)value;
        }

        public object Get(Type type, string key, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            try
            {
                return this._level1.Get(type, key, serializationFormat);
            }
            catch (CacheKeyNotFoundException)
            {
                var value = this._level2.Get(type, key, serializationFormat);

                this._level1.Set(key, value, this._level1Expiration, serializationFormat);

                return value;
            }
        }

        public Dictionary<string, object> GetAll(Type type, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return this._level2.GetAll(type, serializationFormat);
        }

        public Dictionary<string, T> GetAll<T>(SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return this._level2.GetAll<T>(serializationFormat);
        }

        private void ValidateSynchronizable()
        {
            if (!this._synchronizable)
                throw new InvalidOperationException("Operation is not supported because first level is not distributed and cannot be syncronized");
        }

        public void Remove(string key)
        {
            ValidateSynchronizable();
            this._level2.Remove(key);
            this._level1.Remove(key);
        }

        public void RemoveAll()
        {
            ValidateSynchronizable();
            this._level2.RemoveAll();
            this._level1.RemoveAll();
        }

        public void RemoveAll(string keyPrefix)
        {
            ValidateSynchronizable();
            this._level2.RemoveAll(keyPrefix);
            this._level1.RemoveAll(keyPrefix);
        }

        public bool Exists(string key)
        {
            return this._level1.Exists(key) || this._level2.Exists(key);
        }
    }
}