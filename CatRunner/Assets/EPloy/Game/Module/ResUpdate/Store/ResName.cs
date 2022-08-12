using System;
using System.Collections.Generic;

namespace EPloy.Res
{

    /// <summary>
    /// 资源名称。
    /// </summary>
    public struct ResName : IComparable, IComparable<ResName>, IEquatable<ResName>
    {
        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源扩展。
        /// </summary>
        public string Extension
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化资源名称的新实例。
        /// </summary>
        /// <param name="name">资源名称。</param>
        public ResName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Fatal("Resource name is invalid.");
            }

            Name = name;
            Extension = "";
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is ResName) && Equals((ResName)obj);
        }

        public bool Equals(ResName value)
        {
            return string.Equals(Name, value.Name, StringComparison.Ordinal);
        }

        public static bool operator ==(ResName a, ResName b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ResName a, ResName b)
        {
            return !(a == b);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (!(value is ResName))
            {
                Log.Fatal("Type of value is invalid.");
                return 1;
            }

            return CompareTo((ResName)value);
        }

        public int CompareTo(ResName resName)
        {
            int result = string.CompareOrdinal(Name, resName.Name);
            if (result != 0)
            {
                return result;
            }
            return 0;
        }
    }

    /// <summary>
    /// 资源名称比较器。
    /// </summary>
    public sealed class ResNameComparer : IComparer<ResName>, IEqualityComparer<ResName>
    {
        public int Compare(ResName x, ResName y)
        {
            return x.CompareTo(y);
        }

        public bool Equals(ResName x, ResName y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ResName obj)
        {
            return obj.GetHashCode();
        }
    }
}

