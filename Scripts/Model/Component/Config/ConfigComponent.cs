//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Config;
using GameFramework.Resource;
using System.IO;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 配置组件。
    /// </summary>
    public sealed class ConfigComponent : Component
    {
        private const int DefaultPriority = 0;
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// 获取配置数量。
        /// </summary>
        public int ConfigCount
        {
            get
            {
                return ConfigManager.ConfigCount;
            }
        }

        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <param name="configName">配置名称。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="loadType">配置加载方式。</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType)
        {
            LoadConfig(configName, configAssetName, loadType, DefaultPriority, null);
        }

        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <param name="configName">配置名称。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="loadType">配置加载方式。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, int priority)
        {
            LoadConfig(configName, configAssetName, loadType, priority, null);
        }

        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <param name="configName">配置名称。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="loadType">配置加载方式。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, object userData)
        {
            LoadConfig(configName, configAssetName, loadType, DefaultPriority, userData);
        }

        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <param name="configName">配置名称。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="loadType">配置加载方式。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, int priority, object userData)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Log.Error("Config name is invalid.");
                return;
            }

            ConfigManager.LoadConfig(configAssetName, loadType, priority, new LoadConfigInfo(configName, userData));
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="text">要解析的配置文本。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(string text)
        {
            return ConfigManager.ParseConfig(text);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="text">要解析的配置文本。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(string text, object userData)
        {
            return ConfigManager.ParseConfig(text, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="bytes">要解析的配置二进制流。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(byte[] bytes)
        {
            return ConfigManager.ParseConfig(bytes);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="bytes">要解析的配置二进制流。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(byte[] bytes, object userData)
        {
            return ConfigManager.ParseConfig(bytes, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="stream">要解析的配置二进制流。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(Stream stream)
        {
            return ConfigManager.ParseConfig(stream);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="stream">要解析的配置二进制流。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseConfig(Stream stream, object userData)
        {
            return ConfigManager.ParseConfig(stream, userData);
        }

        /// <summary>
        /// 检查是否存在指定配置项。
        /// </summary>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasConfig(string configName)
        {
            return ConfigManager.HasConfig(configName);
        }

        /// <summary>
        /// 移除指定配置项。
        /// </summary>
        /// <param name="configName">要移除配置项的名称。</param>
        public void RemoveConfig(string configName)
        {
            ConfigManager.RemoveConfig(configName);
        }

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        public void RemoveAllConfigs()
        {
            ConfigManager.RemoveAllConfigs();
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName)
        {
            return ConfigManager.GetBool(configName);
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName, bool defaultValue)
        {
            return ConfigManager.GetBool(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName)
        {
            return ConfigManager.GetInt(configName);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName, int defaultValue)
        {
            return ConfigManager.GetInt(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName)
        {
            return ConfigManager.GetFloat(configName);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName, float defaultValue)
        {
            return ConfigManager.GetFloat(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName)
        {
            return ConfigManager.GetString(configName);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName, string defaultValue)
        {
            return ConfigManager.GetString(configName, defaultValue);
        }
    }
}
