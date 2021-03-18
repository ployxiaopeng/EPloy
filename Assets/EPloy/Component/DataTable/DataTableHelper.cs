﻿
using System;
using System.IO;
using System.Text;
using EPloy.Res;

namespace EPloy.Table
{
    /// <summary>
    /// 默认数据表辅助器。
    /// </summary>
    public class DataTableHelper : IDataTableHelper
    {
        private static readonly string BytesAssetExtension = ".bytes";

        //辅助器 所操作的数据类
        private DataTableBase DataTableBase;
        private LoadBinaryCallbacks LoadBinaryCallbacks;

        public DataTableHelper(DataTableBase dataTableBase)
        {
            DataTableBase = dataTableBase;
            LoadBinaryCallbacks = new LoadBinaryCallbacks(LoadBinarySuccessCallback, LoadBinaryFailureCallback);
        }

        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="dataTableAssetName">数据表资源名称。</param>
        /// <returns>是否读取数据表成功。</returns>
        public void ReadData(string dataTableAssetName)
        {
            GameEntry.Res.LoadBinary(dataTableAssetName, LoadBinaryCallbacks);
        }

        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="dataTableAssetName">数据表资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否读取数据表成功。</returns>
        public void ReadData(string dataTableAssetName, object userData)
        {
            GameEntry.Res.LoadBinary(dataTableAssetName, LoadBinaryCallbacks, userData);
        }

        public bool ParseData(byte[] dataTableBytes)
        {
            return ParseData(dataTableBytes, 0, dataTableBytes.Length, null);
        }

        public bool ParseData(byte[] dataTableBytes, object userData)
        {
            return ParseData(dataTableBytes, 0, dataTableBytes.Length, userData);
        }

        public bool ParseData(byte[] dataTableBytes, int startIndex, int length)
        {
            return ParseData(dataTableBytes, startIndex, length, null);
        }
        
        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTableBytes">要解析的数据表二进制流。</param>
        /// <param name="startIndex">数据表二进制流的起始位置。</param>
        /// <param name="length">数据表二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析数据表成功。</returns>
        public bool ParseData(byte[] dataTableBytes, int startIndex, int length, object userData)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(dataTableBytes, startIndex, length, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        {
                            int dataRowBytesLength = binaryReader.Read7BitEncodedInt32();
                            if (!DataTableBase.AddDataRow(dataTableBytes, (int)binaryReader.BaseStream.Position, dataRowBytesLength))
                            {
                                // Log.Warning("Can not parse data row bytes.");
                                return false;
                            }

                            binaryReader.BaseStream.Position += dataRowBytesLength;
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                // Log.Warning("Can not parse dictionary bytes with exception '{0}'.", exception.ToString());
                return false;
            }
        }

        private void LoadBinaryFailureCallback(string dataAssetName, LoadResStatus status, string errorMessage, object userData)
        {
            string appendErrorMessage = Utility.Text.Format("Load data failure, data asset name '{0}', status '{1}', error message '{2}'.", dataAssetName, status.ToString(), errorMessage);
            DataTableFailureEvt Evt = ReferencePool.Acquire<DataTableFailureEvt>();
            Evt.SetData(dataAssetName, appendErrorMessage, userData);
            GameEntry.Event.Fire(Evt);
            throw new EPloyException(appendErrorMessage);
        }

        private void LoadBinarySuccessCallback(string dataAssetName, byte[] dataBytes, float duration, object userData)
        {
            try
            {
                if (!DataTableBase.ParseData(dataBytes))
                {
                    throw new EPloyException(Utility.Text.Format("Load data failure in data provider helper, data asset name '{0}'.", dataAssetName));
                }

                DataTableSuccessEvt Evt = ReferencePool.Acquire<DataTableSuccessEvt>();
                Evt.SetData(dataAssetName, duration, userData);
                GameEntry.Event.Fire(Evt);
            }
            catch (Exception exception)
            {
                DataTableFailureEvt Evt = ReferencePool.Acquire<DataTableFailureEvt>();
                Evt.SetData(dataAssetName, exception.ToString(), userData);
                GameEntry.Event.Fire(Evt);
                throw;
            }
        }
    }
}
