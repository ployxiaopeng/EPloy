using EPloy.ObjectPool;
using System;
using System.Collections.Generic;

namespace EPloy
{
    public partial class ObjectPoolComponent : Component
    {
        /// <summary>
        /// 默认对象池的实现
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        private class ObjectPool : ObjectPoolBase
        {
            private  UnOrderMultiMap<string, Object> m_Objects;
            private  Dictionary<object, Object> m_ObjectMap;
            private  List<Object> m_CanReleaseObjects;//未使用能释放的对象
            private  List<Object> m_ToReleaseObjects;//确定能释放的对象

            private Type m_ObjectType;
            private int m_Capacity;
            private float m_ExpireTime;
            private float m_AutoReleaseTime;


            /// <summary>
            /// 对象类型。
            /// </summary>
            public override Type ObjectType
            {
                get
                {
                    return m_ObjectType;
                }
            }

            /// <summary>
            /// 对象的数量。
            /// </summary>
            public override int Count
            {
                get
                {
                    return m_ObjectMap.Count;
                }
            }

            /// <summary>
            /// 对象池的容量。
            /// </summary>
            public override int Capacity
            {
                get
                {
                    return m_Capacity;
                }
                set
                {
                    if (value < 0)
                    {
                        throw new EPloyException("Capacity is invalid.");
                    }

                    if (m_Capacity == value)
                    {
                        return;
                    }

                    m_Capacity = value;
                    Release();
                }
            }

            /// <summary>
            /// 自动释放间隔
            /// </summary>
            public override float AutoReleaseTime
            {
                get
                {
                    return m_AutoReleaseTime;
                }

                set
                {
                    if (value < 0f)
                    {
                        throw new EPloyException("AutoReleaseTime is invalid.");
                    }

                    if (m_AutoReleaseTime == value)
                    {
                        return;
                    }

                    m_AutoReleaseTime = value;
                    Release();
                }
            }

            /// <summary>
            /// 对象过期秒数。
            /// </summary>
            public override float ExpireTime
            {
                get
                {
                    return m_ExpireTime;
                }

                set
                {
                    if (value < 0f)
                    {
                        throw new EPloyException("ExpireTime is invalid.");
                    }

                    if (ExpireTime == value)
                    {
                        return;
                    }

                    m_ExpireTime = value;
                    Release();
                }
            }

            /// <summary>
            /// 设置对象池基本数据
            /// </summary>
            public override void Initialize(Type objectType, string name, float autoReleaseTime, int capacity, float expireTime)
            {
                m_Objects = new UnOrderMultiMap<string, Object>();
                m_ObjectMap = new Dictionary<object, Object>();
                m_CanReleaseObjects = new List<Object>();
                m_ToReleaseObjects = new List<Object>();
                m_ObjectType = objectType;
                m_Name = name;
                m_AutoReleaseTime = autoReleaseTime;
                Capacity = capacity;
                ExpireTime = expireTime;
            }

            /// <summary>
            /// 创建对象。
            /// </summary>
            /// <param name="obj">对象。</param>
            /// <param name="spawned">对象是否已被获取。</param>
            public override void Register(ObjectBase obj, bool spawned)
            {
                if (obj == null)
                {
                    throw new EPloyException("Object is invalid.");
                }

                Object internalObject = ReferencePool.Acquire<Object>();
                internalObject.Initialize(obj.Name, obj);
                internalObject.SetSpawned(spawned);
                m_Objects.Add(obj.Name, internalObject);
                m_ObjectMap.Add(obj.Target, internalObject);

                if (Count > m_Capacity)
                {
                    Release();
                }
            }

            /// <summary>
            /// 检查对象。
            /// </summary>
            /// <param name="name">对象名称。</param>
            /// <returns>要检查的对象是否存在。</returns>
            public override bool CanSpawn(string name)
            {
                List<Object> objectRange = default(List<Object>);
                if (m_Objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object internalObject in objectRange)
                    {
                        if (!internalObject.IsInUse)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <param name="name">对象名称。</param>
            /// <returns>要获取的对象。</returns>
            public override ObjectBase Spawn(string name)
            {
                List<Object> objectRange = default(List<Object>);
                if (m_Objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object internalObject in objectRange)
                    {
                        if (!internalObject.IsInUse)
                        {
                            return internalObject;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// 回收对象。
            /// </summary>
            /// <param name="target">要回收的对象。</param>
            public override void Unspawn(object target)
            {
                if (target == null)
                {
                    throw new EPloyException("Target is invalid.");
                }

                Object internalObject = GetObject(target);
                if (internalObject != null)
                {
                    internalObject.OnUnspawn();
                    if (Count > m_Capacity && internalObject.SpawnCount <= 0)
                    {
                        Release();
                    }
                }
                else
                {
                    throw new EPloyException(string.Format("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.", new TypeNamePair(target.GetType(), Name).ToString(), target.GetType().FullName, target.ToString()));
                }
            }

            /// <summary>
            /// 释放可释放对象。
            /// </summary>
            public override void Release()
            {
                DateTime expireTime = DateTime.MinValue;
                if (m_ExpireTime < float.MaxValue)
                {
                    expireTime = DateTime.Now.AddSeconds(-m_ExpireTime);
                }

                m_AutoReleaseTime = 0f;
                GetCanReleaseObjects(m_CanReleaseObjects);
                List<Object> toReleaseObjects = ConfirmReleaseObjectFiltrate(m_CanReleaseObjects, expireTime);
                if (toReleaseObjects == null || toReleaseObjects.Count <= 0)
                {
                    return;
                }

                foreach (Object toReleaseObject in toReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }
            }

            /// <summary>
            /// 释放所有未使用对象。
            /// </summary>
            public override void ReleaseAllUnused()
            {
                m_AutoReleaseTime = 0f;
                GetCanReleaseObjects(m_CanReleaseObjects);
                foreach (Object toReleaseObject in m_CanReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }
            }

            /// <summary>
            /// 释放对象。
            /// </summary>
            /// <param name="target">要释放的对象。</param>
            /// <returns>释放对象是否成功。</returns>
            public bool ReleaseObject(object target)
            {
                if (target == null)
                {
                    throw new EPloyException("Target is invalid.");
                }

                Object internalObject = GetObject(target);
                if (internalObject == null)
                {
                    throw new EPloyException("Can not release object which is not found.");
                }

                if (internalObject.IsInUse)
                {
                    return false;
                }

                m_Objects.Remove(internalObject.Name, internalObject);
                m_ObjectMap.Remove(internalObject.Target);
                ReferencePool.Release(internalObject);
                return true;
            }

            /// <summary>
            /// 获取所有对象信息。
            /// </summary>
            /// <returns>所有对象信息。</returns>
            public override ObjectInfo[] GetAllObjectInfos()
            {
                List<ObjectInfo> results = new List<ObjectInfo>();
                foreach (KeyValuePair<string,List<Object>> objectRanges in m_Objects.Dictionary())
                {
                    foreach (Object internalObject in objectRanges.Value)
                    {
                        results.Add(new ObjectInfo(internalObject.Name,internalObject.LastUseTime, internalObject.SpawnCount));
                    }
                }
                return results.ToArray();
            }

            private Object GetObject(object target)
            {
                if (target == null)
                {
                    throw new EPloyException("Target is invalid.");
                }

                Object  internalObject = null;
                if (m_ObjectMap.TryGetValue(target, out internalObject))
                {
                    return internalObject;
                }
                return null;
            }

            private void GetCanReleaseObjects(List<Object> results)
            {
                if (results == null)
                {
                    throw new EPloyException("Results is invalid.");
                }

                results.Clear();
                foreach (KeyValuePair<object, Object> objectInMap in m_ObjectMap)
                {
                    Object internalObject = objectInMap.Value;
                    if (internalObject.IsInUse)
                    {
                        continue;
                    }
                    results.Add(internalObject);
                }
            }

            private List<Object> ConfirmReleaseObjectFiltrate(List<Object> canReleaseObjects, DateTime expireTime)
            {
                m_ToReleaseObjects.Clear();

                if (expireTime > DateTime.MinValue)
                {
                    for (int i = canReleaseObjects.Count - 1; i >= 0; i--)
                    {
                        if (canReleaseObjects[i].LastUseTime <= expireTime)
                        {
                            m_ToReleaseObjects.Add(canReleaseObjects[i]);
                            canReleaseObjects.RemoveAt(i);
                            continue;
                        }
                    }
                }
                return m_ToReleaseObjects;
            }

            public override void Clear()
            {
                foreach (KeyValuePair<object, Object> objectInMap in m_ObjectMap)
                {
                    ReferencePool.Release(objectInMap.Value);
                }
                m_Objects.Clear();
                m_ObjectMap.Clear();
                m_CanReleaseObjects.Clear();
                m_ToReleaseObjects.Clear();
            }
  
        }
    }
}
