
namespace EPloy.Game.Res
{
    public struct ReadWriteResInfo
    {
        public bool UseFileSystem
        {
            get
            {
                return string.IsNullOrEmpty(FileSystemName);
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

        public ReadWriteResInfo(string fileSystemName, LoadType loadType, int length, int hashCode)
        {
            FileSystemName = fileSystemName;
            LoadType = loadType;
            Length = length;
            HashCode = hashCode;
        }
    }
}
