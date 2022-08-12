using System;
using System.IO;
using UnityEngine;

namespace EPloy.SystemFile
{
    /// <summary>
    /// 安卓文件系统流。
    /// </summary>
    public sealed class AndroidFileSystemStream : FileSystemStream
    {
        private static readonly string SplitFlag = "!/assets/";
        private static readonly int SplitFlagLength = SplitFlag.Length;
        private static readonly AndroidJavaObject s_AssetManager = null;
        private static readonly IntPtr s_InternalReadMethodId = IntPtr.Zero;
        private static readonly jvalue[] s_InternalReadArgs = null;

        private readonly AndroidJavaObject m_FileStream;
        private readonly IntPtr m_FileStreamRawObject;

        static AndroidFileSystemStream()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer == null)
            {
                Log.Fatal("Unity player is invalid.");
                return;
            }

            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (currentActivity == null)
            {
                Log.Fatal("Current activity is invalid.");
                return;
            }

            AndroidJavaObject assetManager = currentActivity.Call<AndroidJavaObject>("getAssets");
            if (assetManager == null)
            {
                Log.Fatal("Asset manager is invalid.");
                return;
            }

            s_AssetManager = assetManager;

            IntPtr inputStreamClassPtr = AndroidJNI.FindClass("java/io/InputStream");
            s_InternalReadMethodId = AndroidJNIHelper.GetMethodID(inputStreamClassPtr, "read", "([BII)I");
            s_InternalReadArgs = new jvalue[3];

            AndroidJNI.DeleteLocalRef(inputStreamClassPtr);
            currentActivity.Dispose();
            unityPlayer.Dispose();
        }

        /// <summary>
        /// 初始化安卓文件系统流的新实例。
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径。</param>
        /// <param name="access">要加载的文件系统的访问方式。</param>
        /// <param name="createNew">是否创建新的文件系统流。</param>
        public AndroidFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Log.Fatal("Full path is invalid.");
                return;
            }

            if (access != FileSystemAccess.Read)
            {
                Log.Fatal(UtilText.Format("'{0}' is not supported in AndroidFileSystemStream.", access.ToString()));
                return;
            }

            if (createNew)
            {
                Log.Fatal("Create new is not supported in AndroidFileSystemStream.");
                return;
            }

            int position = fullPath.LastIndexOf(SplitFlag, StringComparison.Ordinal);
            if (position < 0)
            {
                Log.Fatal("Can not find split flag in full path.");
                return;
            }

            string fileName = fullPath.Substring(position + SplitFlagLength);
            m_FileStream = InternalOpen(fileName);
            if (m_FileStream == null)
            {
                Log.Fatal(UtilText.Format("Open file '{0}' from Android asset manager failure.", fullPath));
                return;
            }

            m_FileStreamRawObject = m_FileStream.GetRawObject();
        }

        /// <summary>
        /// 获取或设置文件系统流位置。
        /// </summary>
        protected override internal long Position
        {
            get
            {
                Log.Fatal("Get position is not supported in AndroidFileSystemStream.");
                return 0;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// 获取文件系统流长度。
        /// </summary>
        protected override internal long Length
        {
            get
            {
                return InternalAvailable();
            }
        }

        /// <summary>
        /// 设置文件系统流长度。
        /// </summary>
        /// <param name="length">要设置的文件系统流的长度。</param>
        protected override internal void SetLength(long length)
        {
            Log.Fatal("SetLength is not supported in AndroidFileSystemStream.");
        }

        /// <summary>
        /// 定位文件系统流位置。
        /// </summary>
        /// <param name="offset">要定位的文件系统流位置的偏移。</param>
        /// <param name="origin">要定位的文件系统流位置的方式。</param>
        protected override internal void Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.End)
            {
                Seek(Length + offset, SeekOrigin.Begin);
                return;
            }

            if (origin == SeekOrigin.Begin)
            {
                InternalReset();
            }

            while (offset > 0)
            {
                long skip = InternalSkip(offset);
                if (skip < 0)
                {
                    return;
                }

                offset -= skip;
            }
        }

        /// <summary>
        /// 从文件系统流中读取一个字节。
        /// </summary>
        /// <returns>读取的字节，若已经到达文件结尾，则返回 -1。</returns>
        protected override internal int ReadByte()
        {
            return InternalRead();
        }

        /// <summary>
        /// 从文件系统流中读取二进制流。
        /// </summary>
        /// <param name="buffer">存储读取文件内容的二进制流。</param>
        /// <param name="startIndex">存储读取文件内容的二进制流的起始位置。</param>
        /// <param name="length">存储读取文件内容的二进制流的长度。</param>
        /// <returns>实际读取了多少字节。</returns>
        protected override internal int Read(byte[] buffer, int startIndex, int length)
        {
            byte[] result = null;
            int bytesRead = InternalRead(length, out result);
            Array.Copy(result, 0, buffer, startIndex, bytesRead);
            return bytesRead;
        }

        /// <summary>
        /// 向文件系统流中写入一个字节。
        /// </summary>
        /// <param name="value">要写入的字节。</param>
        protected override internal void WriteByte(byte value)
        {
            Log.Fatal("WriteByte is not supported in AndroidFileSystemStream.");
        }

        /// <summary>
        /// 向文件系统流中写入二进制流。
        /// </summary>
        /// <param name="buffer">存储写入文件内容的二进制流。</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置。</param>
        /// <param name="length">存储写入文件内容的二进制流的长度。</param>
        protected override internal void Write(byte[] buffer, int startIndex, int length)
        {
            Log.Fatal("Write is not supported in AndroidFileSystemStream.");
        }

        /// <summary>
        /// 将文件系统流立刻更新到存储介质中。
        /// </summary>
        protected override internal void Flush()
        {
            Log.Fatal("Flush is not supported in AndroidFileSystemStream.");
        }

        /// <summary>
        /// 关闭文件系统流。
        /// </summary>
        protected override internal void Close()
        {
            InternalClose();
            m_FileStream.Dispose();
        }

        private AndroidJavaObject InternalOpen(string fileName)
        {
            return s_AssetManager.Call<AndroidJavaObject>("open", fileName);
        }

        private int InternalAvailable()
        {
            return m_FileStream.Call<int>("available");
        }

        private void InternalClose()
        {
            m_FileStream.Call("close");
        }

        private int InternalRead()
        {
            return m_FileStream.Call<int>("read");
        }

        private int InternalRead(int length, out byte[] result)
        {
#if UNITY_2019_2_OR_NEWER
#pragma warning disable CS0618
#endif
            IntPtr resultPtr = AndroidJNI.NewByteArray(length);
#if UNITY_2019_2_OR_NEWER
#pragma warning restore CS0618
#endif
            int offset = 0;
            int bytesLeft = length;
            while (bytesLeft > 0)
            {
                s_InternalReadArgs[0] = new jvalue() { l = resultPtr };
                s_InternalReadArgs[1] = new jvalue() { i = offset };
                s_InternalReadArgs[2] = new jvalue() { i = bytesLeft };
                int bytesRead = AndroidJNI.CallIntMethod(m_FileStreamRawObject, s_InternalReadMethodId, s_InternalReadArgs);
                if (bytesRead <= 0)
                {
                    break;
                }

                offset += bytesRead;
                bytesLeft -= bytesRead;
            }

#if UNITY_2019_2_OR_NEWER
#pragma warning disable CS0618
#endif
            result = AndroidJNI.FromByteArray(resultPtr);
#if UNITY_2019_2_OR_NEWER
#pragma warning restore CS0618
#endif
            AndroidJNI.DeleteLocalRef(resultPtr);
            return offset;
        }

        private void InternalReset()
        {
            m_FileStream.Call("reset");
        }

        private long InternalSkip(long offset)
        {
            return m_FileStream.Call<long>("skip", offset);
        }
    }
}
