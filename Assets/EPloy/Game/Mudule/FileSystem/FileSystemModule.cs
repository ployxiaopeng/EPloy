using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;

namespace EPloy
{

    /// <summary>
    /// 文件系统组件。
    /// </summary>
    public class FileSystemModule : EPloyModule
    {
        private const string AndroidFileSystemPrefixString = "jar:";

        private Dictionary<string, FileSystem> FileSystems;

        /// <summary>
        /// 获取文件系统数量。
        /// </summary>
        public int Count
        {
            get
            {
                return FileSystems.Count;
            }
        }

        public override void Awake()
        {
            FileSystems = new Dictionary<string, FileSystem>(StringComparer.Ordinal);
        }

        public override void Update()
        {

        }

        /// <summary>
        /// 关闭并清理文件系统管理器。
        /// </summary>
        public override void OnDestroy()
        {
            while (FileSystems.Count > 0)
            {
                foreach (KeyValuePair<string, FileSystem> fileSystem in FileSystems)
                {
                    DestroyFileSystem(fileSystem.Value, false);
                    break;
                }
            }
        }

        /// <summary>
        /// 检查是否存在文件系统。
        /// </summary>
        /// <param name="fullPath">要检查的文件系统的完整路径。</param>
        /// <returns>是否存在文件系统。</returns>
        public bool HasFileSystem(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Log.Fatal("Full path is invalid.");
                return false;
            }

            return FileSystems.ContainsKey(Utility.Path.GetRegularPath(fullPath));
        }

        /// <summary>
        /// 获取文件系统。
        /// </summary>
        /// <param name="fullPath">要获取的文件系统的完整路径。</param>
        /// <returns>获取的文件系统。</returns>
        public IFileSystem GetFileSystem(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Log.Fatal("Full path is invalid.");
                return null;
            }

            FileSystem fileSystem = null;
            if (FileSystems.TryGetValue(Utility.Path.GetRegularPath(fullPath), out fileSystem))
            {
                return fileSystem;
            }

            return null;
        }

        /// <summary>
        /// 创建文件系统。
        /// </summary>
        /// <param name="fullPath">要创建的文件系统的完整路径。</param>
        /// <param name="access">要创建的文件系统的访问方式。</param>
        /// <param name="maxFileCount">要创建的文件系统的最大文件数量。</param>
        /// <param name="maxBlockCount">要创建的文件系统的最大块数据数量。</param>
        /// <returns>创建的文件系统。</returns>
        public IFileSystem CreateFileSystem(string fullPath, FileSystemAccess access, int maxFileCount, int maxBlockCount)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Log.Fatal("Full path is invalid.");
                return null;
            }

            if (access == FileSystemAccess.Unspecified)
            {
                Log.Fatal("Access is invalid.");
                return null;
            }

            if (access == FileSystemAccess.Read)
            {
                Log.Fatal("Access read is invalid.");
                return null;
            }

            fullPath = Utility.Path.GetRegularPath(fullPath);
            if (FileSystems.ContainsKey(fullPath))
            {
                Log.Fatal(Utility.Text.Format("File system '{0}' is already exist.", fullPath));
                return null;
            }

            FileSystemStream fileSystemStream = CreateFileSystemStream(fullPath, access, true);
            if (fileSystemStream == null)
            {
                Log.Fatal(Utility.Text.Format("Create file system stream for '{0}' failure.", fullPath));
                return null;
            }

            FileSystem fileSystem = FileSystem.Create(fullPath, access, fileSystemStream, maxFileCount, maxBlockCount);
            if (fileSystem == null)
            {
                Log.Fatal(Utility.Text.Format("Create file system '{0}' failure.", fullPath));
                return null;
            }

            FileSystems.Add(fullPath, fileSystem);
            return fileSystem;
        }

        /// <summary>
        /// 加载文件系统。
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径。</param>
        /// <param name="access">要加载的文件系统的访问方式。</param>
        /// <returns>加载的文件系统。</returns>
        public IFileSystem LoadFileSystem(string fullPath, FileSystemAccess access)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Log.Fatal("Full path is invalid.");
                return null;
            }

            if (access == FileSystemAccess.Unspecified)
            {
                Log.Fatal("Access is invalid.");
                return null;
            }

            fullPath = Utility.Path.GetRegularPath(fullPath);
            if (FileSystems.ContainsKey(fullPath))
            {
                Log.Fatal(Utility.Text.Format("File system '{0}' is already exist.", fullPath));
                return null;
            }

            FileSystemStream fileSystemStream = CreateFileSystemStream(fullPath, access, false);
            if (fileSystemStream == null)
            {
                Log.Fatal(Utility.Text.Format("Create file system stream for '{0}' failure.", fullPath));
                return null;
            }

            FileSystem fileSystem = FileSystem.Load(fullPath, access, fileSystemStream);
            if (fileSystem == null)
            {
                Log.Fatal(Utility.Text.Format("Load file system '{0}' failure.", fullPath));
                return null;
            }

            FileSystems.Add(fullPath, fileSystem);
            return fileSystem;
        }

        /// <summary>
        /// 销毁文件系统。
        /// </summary>
        /// <param name="fileSystem">要销毁的文件系统。</param>
        /// <param name="deletePhysicalFile">是否删除文件系统对应的物理文件。</param>
        public void DestroyFileSystem(IFileSystem fileSystem, bool deletePhysicalFile)
        {
            if (fileSystem == null)
            {
                Log.Fatal("File system is invalid.");
                return;
            }

            string fullPath = fileSystem.FullPath;
            ((FileSystem)fileSystem).Shutdown();
            FileSystems.Remove(fullPath);

            if (deletePhysicalFile && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        /// 获取所有文件系统集合。
        /// </summary>
        /// <returns>获取的所有文件系统集合。</returns>
        public IFileSystem[] GetAllFileSystems()
        {
            int index = 0;
            IFileSystem[] results = new IFileSystem[FileSystems.Count];
            foreach (KeyValuePair<string, FileSystem> fileSystem in FileSystems)
            {
                results[index++] = fileSystem.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有文件系统集合。
        /// </summary>
        /// <param name="results">获取的所有文件系统集合。</param>
        public void GetAllFileSystems(List<IFileSystem> results)
        {
            if (results == null)
            {
                Log.Fatal("Results is invalid.");
                return;
            }

            results.Clear();
            foreach (KeyValuePair<string, FileSystem> fileSystem in FileSystems)
            {
                results.Add(fileSystem.Value);
            }
        }

        private FileSystemStream CreateFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
        {
            if (fullPath.StartsWith(AndroidFileSystemPrefixString, StringComparison.Ordinal))
            {
                return new AndroidFileSystemStream(fullPath, access, createNew);
            }
            else
            {
                return new CommonFileSystemStream(fullPath, access, createNew);
            }
        }
    }
}
