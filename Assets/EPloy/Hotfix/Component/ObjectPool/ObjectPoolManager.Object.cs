﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using EPloy.ObjectPool;
using System;

namespace EPloy
{
    public partial class ObjectPoolComponent : Component
    {
        /// <summary>
        /// 默认对象池内部对象的实现
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        private sealed class Object : ObjectBase
        { 
            private int m_SpawnCount;

            /// <summary>
            /// 初始化内部对象的新实例。
            /// </summary>
            public Object()
            {
                m_SpawnCount = 0;
            }

            /// <summary>
            /// 获取对象是否正在使用。
            /// </summary>
            public bool IsInUse
            {
                get
                {
                    return m_SpawnCount > 0;
                }
            }

            /// <summary>
            /// 获取对象的获取计数。
            /// </summary>
            public int SpawnCount
            {
                get
                {
                    return m_SpawnCount;
                }
            }

            /// <summary>
            /// 设置产生数量
            /// </summary>
            /// <param name="spawned"></param>
            public void SetSpawned( bool spawned)
            {
                m_SpawnCount = spawned ? 1 : 0;
                if (spawned)
                {
                    OnSpawn();
                }
            }

            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <returns>对象。</returns>
            protected internal override void  OnSpawn()
            {
                m_SpawnCount++;
               m_LastUseTime = DateTime.Now;
            }

            /// <summary>
            /// 回收对象。
            /// </summary>
            protected internal override void OnUnspawn()
            {
                m_LastUseTime = DateTime.Now;
                m_SpawnCount--;
                if (m_SpawnCount < 0)
                {
                   Log.Fatal(Utility.Text.Format("Object '{0}' spawn count is less than 0.", Name));
                }
            }

            /// <summary>
            /// 清理内部对象。
            /// </summary>
            public override void Clear()
            {
                m_SpawnCount = 0;
            }
        }
    }
}