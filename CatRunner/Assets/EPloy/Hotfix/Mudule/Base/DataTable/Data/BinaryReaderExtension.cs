using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EPloy.Hotfix.Table
{
    public static class BinaryReaderExtension
    {
        public static Color32 ReadColor32(this BinaryReader binaryReader)
        {
            return new Color32(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
        }

        public static Color ReadColor(this BinaryReader binaryReader)
        {
            return new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static DateTime ReadDateTime(this BinaryReader binaryReader)
        {
            return new DateTime(binaryReader.ReadInt64());
        }

        public static Quaternion ReadQuaternion(this BinaryReader binaryReader)
        {
            return new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static Rect ReadRect(this BinaryReader binaryReader)
        {
            return new Rect(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static Vector2 ReadVector2(this BinaryReader binaryReader)
        {
            return new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader binaryReader)
        {
            return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static Vector4 ReadVector4(this BinaryReader binaryReader)
        {
            return new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public static List<string> ReadListstring(this BinaryReader binaryReader)
        {
            List<string> listStr = new List<string>();
            try
            {
                short cont = binaryReader.ReadInt16();
                while (cont > 0)
                {
                    listStr.Add(binaryReader.ReadString());
                    cont -= 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return listStr;
        }

        public static List<int> ReadListint(this BinaryReader binaryReader)
        {
            List<int> listInt = new List<int>();
            try
            {
                short cont = binaryReader.ReadInt16();
                while (cont > 0)
                {
                    int Num = binaryReader.ReadInt32();
                    listInt.Add(Num);
                    cont -= 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return listInt;
        }

        public static List<float> ReadListfloat(this BinaryReader binaryReader)
        {
            List<float> listfloat = new List<float>();
            try
            {
                short cont = binaryReader.ReadInt16();
                while (cont > 0)
                {
                    byte[] bytes = binaryReader.ReadBytes(4);
                    float Num = System.BitConverter.ToSingle(bytes, 0);
                    listfloat.Add(Num);
                    cont -= 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return listfloat;
        }
    }
}
