using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy
{
    public static class Config
    {
        /// <summary>
        /// 对象池最大容量
        /// </summary>
        public static int ObjectPoolCapacity = 32;

        /// <summary>
        /// 对象池中对象过期事件
        /// </summary>
        public static int ObjectPoolExpireTime = 320;

        /// <summary>
        /// 对象池自动释放间隔
        /// </summary>
        public static int ObjectPoolReleaseTime = 320;
    }
}