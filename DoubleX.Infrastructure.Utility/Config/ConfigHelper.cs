using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 配置文件辅助类
    /// </summary>
    public class ConfigHelper
    {
        #region 配置节点获取

        /// <summary>
        /// 获取信息节点
        /// </summary>
        /// <param name="sectionName">节点对象名</param>
        /// <returns>返回Obj</returns>
        public static object GetSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return null;
            }
            return ConfigurationManager.GetSection(sectionName);
        }

        /// <summary>
        /// 获取信息节点
        /// </summary>
        /// <typeparam name="TEntity">信息类型</typeparam>
        /// <param name="sectionName">节点对象名</param>
        /// <returns>返回TEntity</returns>
        public static TEntity GetSection<TEntity>(string sectionName) where TEntity : class, new()
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = (new TEntity()).GetType().Name;
            }
            var configObj = ConfigurationManager.GetSection(sectionName);
            if (configObj == null)
                configObj = default(TEntity);
            return (TEntity)configObj;
        }

        #endregion

        #region 获取分组集合

        /// <summary>
        /// 获取分组集合列表
        /// </summary>
        /// <typeparam name="TSection">节点类型</typeparam>
        /// <typeparam name="TGroup">集合组类型 默认ConfigGroupItem</typeparam>
        /// <typeparam name="TItem">集合项 默认ConfigListItem</typeparam>
        /// <param name="section">节点</param>
        /// <param name="groupKey">组Key 默认全部</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<TItem> GetGroups<TSection, TGroup, TItem>(TSection section, string groupKey = null)
            where TGroup : ConfigGroupItem, new()
            where TItem : ConfigListItem, new()
        {
            List<TItem> list = new List<TItem>();
            if (section == null)
            {
                return list;
            }
            var nodes = section as ConfigSection<ConfigGroup<TGroup>>;
            if (nodes != null)
            {
                var groupQuery = from i in nodes.Items.Cast<ConfigGroupItem>() select i;
                if (!string.IsNullOrWhiteSpace(groupKey))
                {
                    groupQuery = from i in groupQuery where i.Key.ToLower() == groupKey.ToLower() select i;
                }
                groupQuery.ToList().ForEach(x =>
                {
                    list.AddRange(x.Items.Cast<TItem>().ToList());
                });
            }
            return list;
        }

        /// <summary>
        /// 获取分组集合列表
        /// </summary>
        /// <typeparam name="TGroup">集合组类型 默认ConfigGroupItem</typeparam>
        /// <typeparam name="TItem">集合项 默认ConfigListItem</typeparam>
        /// <param name="sectionName">节点名称</param>
        /// <param name="groupKey">组Key 默认全部</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<TItem> GetGroups<TGroup, TItem>(string sectionName, string groupKey = null)
            where TGroup : ConfigGroupItem, new()
            where TItem : ConfigListItem, new()
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return new List<TItem>();
            }

            var section = GetSection(sectionName) as ConfigSection<ConfigGroup<TGroup>>;
            if (section != null)
            {
                return GetGroups<ConfigSection<ConfigGroup<TGroup>>, TGroup, TItem>(section, groupKey);
            }
            return new List<TItem>();
        }

        /// <summary>
        /// 获取分组集合列表
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="groupKey">组Key 默认全部</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<ConfigListItem> GetGroups(string sectionName, string groupKey = null)
        {
            return GetGroups<ConfigGroupItem, ConfigListItem>(sectionName, groupKey);
        }

        #endregion

        #region 获取集合列表

        /// <summary>
        /// 获取集合列表
        /// </summary>
        /// <typeparam name="TEntity">集合项</typeparam>
        /// <typeparam name="TSection">节点类型</typeparam>
        /// <param name="section">节点</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<TEntity> GetItems<TSection, TEntity>(TSection section) where TEntity : ConfigListItem, new()
        {
            if (section == null)
                return new List<TEntity>();

            var sectionModel = section as ConfigSection<ConfigList<TEntity>>;
            if (sectionModel != null)
            {
                return (from i in sectionModel.Items.Cast<TEntity>() select i).ToList();
            }
            return new List<TEntity>();
        }

        /// <summary>
        /// 获取集合列表
        /// </summary>
        /// <typeparam name="TEntity">集合项</typeparam>
        /// <param name="sectionName">节点名称</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<TEntity> GetItems<TEntity>(string sectionName) where TEntity : ConfigListItem, new()
        {
            List<TEntity> list = new List<TEntity>();
            var section = GetSection(sectionName) as ConfigSection<ConfigList<TEntity>>;
            if (section != null)
            {
                return GetItems<ConfigSection<ConfigList<TEntity>>, TEntity>(section);
            }
            return list;
        }

        /// <summary>
        /// 获取集合列表
        /// </summary>
        /// <typeparam name="TSection">节点类型</typeparam>
        /// <param name="section">节点</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<ConfigListItem> GetItems<TSection>(TSection section)
        {
            return GetItems<TSection, ConfigListItem>(section);
        }

        /// <summary>
        /// 获取集合列表
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static List<ConfigListItem> GetItems(string sectionName)
        {
            return GetItems<ConfigListItem>(sectionName);
        }

        #endregion

        #region 获取配置项

        /// <summary>
        /// 获取集合列表
        /// </summary>
        /// <typeparam name="TEntity">集合项</typeparam>
        /// <typeparam name="TSection">节点类型</typeparam>
        /// <param name="section">节点</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static TEntity GetItem<TSection, TEntity>(TSection section, string key) where
            TEntity : ConfigListItem, new()
        {
            if (section == null)
                return default(TEntity);

            if (string.IsNullOrWhiteSpace(key))
                return default(TEntity);

            var items = GetItems<TSection, TEntity>(section);

            if (items != null)
            {
                return items.Where(x => x.Key.ToLower() == key.ToLower()).Cast<TEntity>().FirstOrDefault();
            }
            return default(TEntity);
        }

        /// <summary>
        /// 获取组配置项
        /// </summary>
        /// <typeparam name="TSection">节点类型</typeparam>
        /// <typeparam name="TGroup">集合组类型 默认ConfigGroupItem</typeparam>
        /// <typeparam name="TItem">集合项 默认ConfigListItem</typeparam>
        /// <param name="section">节点</param>
        /// <param name="groupKey">组Key 默认全部</param>
        /// <param name="key">节点键</param>
        /// <returns>返回List[TEntity]集合列表</returns>
        public static TItem GetItem<TSection, TGroup, TItem>(TSection section, string groupKey, string key)
            where TGroup : ConfigGroupItem, new()
            where TItem : ConfigListItem, new()
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(TItem);

            var items = GetGroups<TSection, ConfigGroupItem, ConfigListItem>(section, groupKey);
            if (items != null)
            {
                return items.Where(x => x.Key.ToLower() == key.ToLower()).Cast<TItem>().FirstOrDefault();
            }
            return default(TItem);
        }

        #endregion
    }

    /// <summary>
    /// 节点对象
    /// </summary>
    /// <typeparam name="TEntity">集合类型</typeparam>
    public class ConfigSection<TEntity> : ConfigurationSection
    {
        protected static readonly ConfigurationProperty defaultProperty =
            new ConfigurationProperty(string.Empty, typeof(TEntity), null,
                                ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public virtual TEntity Items
        {
            get
            {

                return (TEntity)base[defaultProperty];
            }
        }
    }

    /// <summary>
    /// 分组集合
    /// </summary>
    public class ConfigGroup<TItem> : ConfigurationElementCollection where TItem : ConfigGroupItem, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigGroupItem();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigGroupItem)element).Key;
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName
        {
            get { return "Item"; }
        }
    }

    /// <summary>
    /// 分组对象(每个对象必须有Key 及 CofnigFileList)
    /// </summary>
    public class ConfigGroupItem : ConfigurationElement
    {
        protected static readonly ConfigurationProperty itemsProperty =
                new ConfigurationProperty(string.Empty, typeof(ConfigList<ConfigListItem>), null,
                                    ConfigurationPropertyOptions.IsDefaultCollection);

        public ConfigGroupItem() : base() { }

        [ConfigurationProperty("key")] //[ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return this["key"].ToString(); }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return this["value"].ToString(); }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public virtual ConfigList<ConfigListItem> Items
        {
            get
            {

                return (ConfigList<ConfigListItem>)base[itemsProperty];
            }
        }
    }

    /// <summary>
    /// 集合列表
    /// </summary>
    /// <typeparam name="TItem">集合对象/选项 类型</typeparam>
    public class ConfigList<TItem> : ConfigurationElementCollection where TItem : ConfigListItem, new()
    {
        // 基本上，所有的方法都只要简单地调用基类的实现就可以了。
        public ConfigList() : base(StringComparer.OrdinalIgnoreCase)// 忽略大小写
        {
        }

        // 下面二个方法中抽象类中必须要实现的。
        protected override ConfigurationElement CreateNewElement()
        {
            return new TItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TItem)element).Key;
        }

        // 其实关键就是这个索引器。但它也是调用基类的实现，只是做下类型转就行了。
        new public TItem this[string name]
        {
            get
            {
                return (TItem)base.BaseGet(name);
            }
        }

        // 说明：如果不需要在代码中修改集合，可以不实现Add， Clear， Remove
        public void Add(TItem model)
        {
            ConfigurationElement element = model as ConfigurationElement;
            this.BaseAdd(element);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    /// <summary>
    /// 集合对象(每个对象必须有Key Value 属性)
    /// </summary>
    public class ConfigListItem : ConfigurationElement
    {
        public ConfigListItem() { }

        [ConfigurationProperty("key")] //[ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return this["key"].ToString(); }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return this["value"].ToString(); }
            set { this["value"] = value; }
        }
    }
}
