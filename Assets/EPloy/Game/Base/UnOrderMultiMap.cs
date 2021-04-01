using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace EPloy
{
    /// <summary>
    /// 游戏框架多值字典类。 
    /// </summary>
    /// <typeparam name="TKey">指定多值字典的主键类型。</typeparam>
    /// <typeparam name="TValue">指定多值字典的值类型。</typeparam>
    public class UnOrderMultiMap<T, K> : IEnumerable<KeyValuePair<T, TypeLinkedList<K>>>, IEnumerable
    {
        private readonly Dictionary<T, TypeLinkedList<K>> dictionary = new Dictionary<T, TypeLinkedList<K>>();
        // 重用list队列
        private readonly Queue<TypeLinkedList<K>> queue = new Queue<TypeLinkedList<K>>();

        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        public void Add(T t, K k)
        {
            TypeLinkedList<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.dictionary[t] = list;
            }
            list.Add(k);
        }

        public KeyValuePair<T, TypeLinkedList<K>> First()
        {
            return this.dictionary.First();
        }

        private TypeLinkedList<K> FetchList()
        {
            if (this.queue.Count > 0)
            {
                TypeLinkedList<K> list = this.queue.Dequeue();
                list.Clear();
                return list;
            }
            return new TypeLinkedList<K>();
        }

        private void RecycleList(TypeLinkedList<K> list)
        {
            // 防止暴涨
            if (this.queue.Count > 150)
            {
                return;
            }
            list.Clear();
            this.queue.Enqueue(list);
        }

        public bool Remove(T t, K k)
        {
            TypeLinkedList<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.RecycleList(list);
                this.dictionary.Remove(t);
            }
            return true;
        }

        public bool Remove(T t)
        {
            TypeLinkedList<K> list = null;
            this.dictionary.TryGetValue(t, out list);
            if (list != null)
            {
                this.RecycleList(list);
            }
            return this.dictionary.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            TypeLinkedList<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return new K[0];
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public TypeLinkedList<K> this[T t]
        {
            get
            {
                TypeLinkedList<K> list;
                this.dictionary.TryGetValue(t, out list);
                return list;
            }
        }

        public K GetOne(T t)
        {
            TypeLinkedList<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list.First.Value;
            }
            return default(K);
        }

        public bool Contains(T t, K k)
        {
            TypeLinkedList<K> list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }

        public bool ContainsKey(T t)
        {
            return this.dictionary.ContainsKey(t);
        }

        public bool TryGetValue(T key, out TypeLinkedList<K> value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            foreach (KeyValuePair<T, TypeLinkedList<K>> keyValuePair in this.dictionary)
            {
                this.RecycleList(keyValuePair.Value);
            }
            this.dictionary.Clear();
        }

        #region  完全不懂 反正抄的
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator<KeyValuePair<T, TypeLinkedList<K>>> IEnumerable<KeyValuePair<T, TypeLinkedList<K>>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数。
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<T, TypeLinkedList<K>>>, IEnumerator
        {
            private Dictionary<T, TypeLinkedList<K>>.Enumerator m_Enumerator;

            internal Enumerator(Dictionary<T, TypeLinkedList<K>> dictionary)
            {
                if (dictionary == null)
                {
                    Log.Fatal("Dictionary is invalid.");
                    return;
                }

                m_Enumerator = dictionary.GetEnumerator();
            }

            /// <summary>
            /// 获取当前结点。
            /// </summary>
            public KeyValuePair<T, TypeLinkedList<K>> Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }

            /// <summary>
            /// 获取当前的枚举数。
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }

            /// <summary>
            /// 清理枚举数。
            /// </summary>
            public void Dispose()
            {
                m_Enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个结点。
            /// </summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext()
            {
                return m_Enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数。
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<KeyValuePair<T, TypeLinkedList<K>>>)m_Enumerator).Reset();
            }
        }

        #endregion
    }
}