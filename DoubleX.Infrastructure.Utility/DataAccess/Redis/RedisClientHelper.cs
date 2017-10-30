using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using System.Collections.Concurrent;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// Redis客户端辅助类
    /// 注：redis 可以存储的对象，为适应统一结果，本框架将redis值统一成json字符串存储
    /// </summary>
    public class RedisHelper
    {
        #region 对象属性

        /// <summary>
        /// Ridis连接字符串
        /// </summary>
        public string RedisUri { get; set; }

        //private static readonly ConcurrentDictionary<string, IRedisClient> ConnectionCache = new ConcurrentDictionary<string, IRedisClient>();
        //string locker = "locker";

        #endregion

        #region 构造函数

        public RedisHelper() { }

        public RedisHelper(string redisUri)
        {
            this.RedisUri = redisUri;
        }

        #endregion

        /// <summary>
        /// 获取连接客户端
        /// </summary>
        /// <returns></returns>
        public IRedisClient GetClient()
        {
            //if (!ConnectionCache.ContainsKey(RedisUri))
            //{
            //    ConnectionCache[RedisUri] = CreateClient(RedisUri);
            //}
            //return ConnectionCache[RedisUri];
            return CreateClient(RedisUri);
        }

        /// <summary>
        /// 获取Redis(根据RedisKey)
        /// </summary>
        /// <param name="key">RedisKey</param>
        /// <returns>返回object</returns>
        public object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        /// 获取Redis(根据RedisKey)
        /// </summary>
        /// <typeparam name="TEntity">返回的类型</typeparam>
        /// <param name="key">RedisKey</param>
        /// <returns>返回的类型对象</returns>
        public TEntity Get<TEntity>(string key)
        {
            //lock (locker)
            //{
            //    using (var client = GetClient())
            //    {
            //        return client.Get<TEntity>(key);
            //    }
            //}
            using (var client = GetClient())
            {
                return client.Get<TEntity>(key);
            }
        }

        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        public void Set(string key, object obj)
        {
            SetCache<object>(key, obj);
        }

        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        public void Set<TEntity>(string key, TEntity obj)
        {
            SetCache<TEntity>(key, obj);
        }

        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        /// <param name="expirationDateTime">过期时间(绝对即：指定在XXX时候过期)</param>
        public void Set(string key, object obj, DateTime expirationDateTime)
        {
            SetCache<object>(key, obj, absoluteExpiration: expirationDateTime);
        }
        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        /// <param name="expirationDateTime">过期时间(绝对即：指定在XXX时候过期)</param>
        public void Set<TEntity>(string key, TEntity entity, DateTime expirationDateTime)
        {
            SetCache<TEntity>(key, entity, absoluteExpiration: expirationDateTime);
        }

        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        /// <param name="slidingExpirationTimeSpan">过期时间(相对即：多少时间内未使用过期)</param>
        public void Set(string key, object obj, TimeSpan slidingExpirationTimeSpan)
        {
            SetCache<object>(key, obj, slidingExpiration: slidingExpirationTimeSpan);
        }

        /// <summary>
        /// 设置Redis
        /// </summary>
        /// <param name="key">键Key</param>
        /// <param name="obj">内容对象</param>
        /// <param name="slidingExpirationTimeSpan">过期时间(相对即：多少时间内未使用过期)</param>
        public void Set<TEntity>(string key, TEntity entity, TimeSpan slidingExpirationTimeSpan)
        {
            SetCache<TEntity>(key, entity, slidingExpiration: slidingExpirationTimeSpan);
        }

        /// <summary>
        /// 移除Redis(根据RedisKey)
        /// </summary>
        /// <param name="key">键Key</param>
        public void Remove(string key)
        {
            using (var client = GetClient())
            {
                client.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有Redis
        /// </summary>
        public void Clear()
        {
            using (var client = GetClient())
            {
                client.FlushAll();
            }
        }

        #region 私有方法

        /// <summary>
        /// 获取RedisClient
        /// </summary>
        /// <param name="redisUri"></param>
        /// <returns></returns>
        private IRedisClient CreateClient(string redisUri)
        {
            if (VerifyHelper.IsEmpty(redisUri))
                throw new Exception("RedisUri is Null");

            PooledRedisClientManager manage = CreateManage(redisUri);
            if (manage == null)
                throw new Exception("RedisUri is Null");

            IRedisClient client = manage.GetClient();
            if (client == null)
                throw new Exception("RedisUri is Null");

            return client;
        }

        /// <summary>
        /// 管理Redis连接程
        /// </summary>
        /// <param name="redisUri"></param>
        /// <returns></returns>
        private static PooledRedisClientManager CreateManage(string redisUri)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(redisUri))
                    throw new Exception("RedisUri is Null");

                List<KeyValuePair<string, string>> list = StringHelper.GetToListKeyValue(redisUri);

                if (list == null || (list != null && list.Count == 0))
                    throw new Exception("RedisUri is Null");

                var reads = list.FirstOrDefault(x => String.Equals(x.Key, "Reads", StringComparison.CurrentCultureIgnoreCase)).Value.Split(',');
                var writes = list.FirstOrDefault(x => String.Equals(x.Key, "Writes", StringComparison.CurrentCultureIgnoreCase)).Value.Split(',');
                int maxWritePoolSize = IntHelper.Get(list.FirstOrDefault(x => VerifyHelper.IsEqualString(x.Key, "MaxWritePoolSize")).Value);
                int maxReadPoolSize = IntHelper.Get(list.FirstOrDefault(x => VerifyHelper.IsEqualString(x.Key, "MaxReadPoolSize")).Value);
                bool autoStart = BoolHelper.Get(list.FirstOrDefault(x => VerifyHelper.IsEqualString(x.Key, "AutoStart")).Value);
                long defaultDb = LongHelper.Get(list.FirstOrDefault(x => VerifyHelper.IsEqualString(x.Key, "DefaultDb")).Value);

                PooledRedisClientManager manage = new PooledRedisClientManager(reads, writes, new RedisClientManagerConfig
                {
                    MaxWritePoolSize = maxWritePoolSize,
                    MaxReadPoolSize = maxReadPoolSize,
                    AutoStart = autoStart,
                    DefaultDb = defaultDb
                });
                return manage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 设置缓存(absoluteExpiration:过期时间; slidingExpiration:给定活动时间，该时间内未活动，则过期; filesPath:文件依赖)
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="key">根据缓存Key</param>
        /// <param name="obj">缓存内容</param>
        /// <param name="absoluteExpiration">绝对(指定时间)过期</param>
        /// <param name="slidingExpiration">相对(使用间隔)过期</param>
        private void SetCache<TEntity>(string key, TEntity obj, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            //lock (locker)
            //{
            //    using (var client = GetClient())
            //    {
            //        if (absoluteExpiration != null)
            //        {
            //            client.Set<TEntity>(key, obj, absoluteExpiration.Value);
            //        }
            //        else if (slidingExpiration != null)
            //        {
            //            client.Set<TEntity>(key, obj, slidingExpiration.Value);
            //        }
            //        else
            //        {
            //            client.Set<TEntity>(key, obj);
            //        }
            //    }
            //}
            using (var client = GetClient())
            {
                if (absoluteExpiration != null)
                {
                    client.Set<TEntity>(key, obj, absoluteExpiration.Value);
                }
                else if (slidingExpiration != null)
                {
                    client.Set<TEntity>(key, obj, slidingExpiration.Value);
                }
                else
                {
                    client.Set<TEntity>(key, obj);
                }
            }

        }
        #endregion

    }
}
