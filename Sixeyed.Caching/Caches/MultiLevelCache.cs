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
        public ICache Level1 { get; protected set; }
        public ICache Level2 { get; protected set; }
        public bool Synchronizable { get; protected set; }
        public TimeSpan Level1Expiration { get; protected set; }

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
            this.Level1 = level1;
            this.Level2 = level2;
            this.Level1Expiration = level1Expiration;
            this.Synchronizable = synchronizable;
            Initialise();
        }

        public CacheType CacheType
        {
            get { return this.Level2.CacheType; }
        }

        public Serializer Serializer { get; set; }

        public void Initialise()
        {
            this.Level1.Initialise();
            this.Level2.Initialise();
        }

        public void Set(string key, object value, TimeSpan validFor, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this.Level2.Set(key, value, validFor, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        public void Set(string key, object value, DateTime expiresAt, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this.Level2.Set(key, value, expiresAt, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        public void Set(string key, object value, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            this.Level2.Set(key, value, serializationFormat);
            this.SetAtLevel1(key, value, serializationFormat);
        }

        protected void SetAtLevel1(string key, object value, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            // TODO validate that level1 expiration is lower or equal than level2
            this.Level1.Set(key, value, this.Level1Expiration, serializationFormat);
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
                return this.Level1.Get(type, key, serializationFormat);
            }
            catch (CacheKeyNotFoundException)
            {
                var value = this.Level2.Get(type, key, serializationFormat);

                this.Level1.Set(key, value, this.Level1Expiration, serializationFormat);

                return value;
            }
        }

        public Dictionary<string, object> GetAll(Type type, SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return this.Level2.GetAll(type, serializationFormat);
        }

        public Dictionary<string, T> GetAll<T>(SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return this.Level2.GetAll<T>(serializationFormat);
        }

        private void ValidateSynchronizable()
        {
            if (!this.Synchronizable)
                throw new InvalidOperationException("Operation is not supported because first level is not distributed and cannot be syncronized");
        }

        public void Remove(string key)
        {
            ValidateSynchronizable();
            this.Level2.Remove(key);
            this.Level1.Remove(key);
        }

        public void RemoveAll()
        {
            ValidateSynchronizable();
            this.Level2.RemoveAll();
            this.Level1.RemoveAll();
        }

        public void RemoveAll(string keyPrefix)
        {
            ValidateSynchronizable();
            this.Level2.RemoveAll(keyPrefix);
            this.Level1.RemoveAll(keyPrefix);
        }

        public bool Exists(string key)
        {
            return this.Level1.Exists(key) || this.Level2.Exists(key);
        }
    }
}