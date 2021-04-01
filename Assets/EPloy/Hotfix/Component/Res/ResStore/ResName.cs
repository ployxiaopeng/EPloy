using System;
using System.Collections.Generic;

namespace EPloy.Res
{

    /// <summary>
    /// 资源名称。
    /// </summary>
    internal struct ResName // : IComparable, IComparable<ResName>, IEquatable<ResName>
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
        /// 获取变体名称。
        /// </summary>
        public string Variant
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取扩展名称。
        /// </summary>
        public string Extension
        {
            get;
            private set;
        }
        private string CachedFullName;

        /// <summary>
        /// 初始化资源名称的新实例。
        /// </summary>
        /// <param name="name">资源名称。</param>
        /// <param name="variant">变体名称。</param>
        /// <param name="extension">扩展名称。</param>
        public ResName(string name, string variant, string extension)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Fatal("Resource name is invalid.");
            }

            if (string.IsNullOrEmpty(extension))
            {
                Log.Fatal("Resource extension is invalid.");
            }

            Name = name;
            Variant = variant;
            Extension = extension;
            CachedFullName = null;
        }



        public string FullName
        {
            get
            {
                if (CachedFullName == null)
                {
                    CachedFullName = Variant != null ? Utility.Text.Format("{0}.{1}.{2}", Name, Variant, Extension) : Utility.Text.Format("{0}.{1}", Name, Extension);
                }

                return CachedFullName;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        public override int GetHashCode()
        {
            if (Variant == null)
            {
                return Name.GetHashCode() ^ Extension.GetHashCode();
            }

            return Name.GetHashCode() ^ Variant.GetHashCode() ^ Extension.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is ResName) && Equals((ResName)obj);
        }

        public bool Equals(ResName value)
        {
            return string.Equals(Name, value.Name, StringComparison.Ordinal) && string.Equals(Variant, value.Variant, StringComparison.Ordinal) && string.Equals(Extension, value.Extension, StringComparison.Ordinal);
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

            result = string.CompareOrdinal(Variant, resName.Variant);
            if (result != 0)
            {
                return result;
            }

            return string.CompareOrdinal(Extension, resName.Extension);
        }
    }

    /// <summary>
    /// 资源名称比较器。
    /// </summary>
    internal sealed class ResNameComparer //: IComparer<ResName>, IEqualityComparer<ResName>
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

