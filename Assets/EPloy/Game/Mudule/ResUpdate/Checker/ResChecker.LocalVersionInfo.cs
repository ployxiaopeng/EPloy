using System;
using System.IO;
using System.Text;

namespace EPloy.Res
{
    internal sealed partial class ResChecker
    {
        /// <summary>
        /// 本地资源状态信息。
        /// </summary>
        public struct LocalVersionInfo
        {
            public bool Exist
            {
                get;
                private set;
            }

            public bool UseFileSystem
            {
                get
                {
                    return !string.IsNullOrEmpty(FileSystemName);
                }
            }

            public string FileSystemName
            {
                get;
                private set;
            }

            public LoadType LoadType
            {
                get;
                private set;
            }

            public int Length
            {
                get;
                private set;
            }

            public int HashCode
            {
                get;
                private set;
            }


            public LocalVersionInfo(string fileSystemName, LoadType loadType, int length, int hashCode)
            {
                Exist = true;
                FileSystemName = fileSystemName;
                LoadType = loadType;
                Length = length;
                HashCode = hashCode;
            }

        }
    }
}
