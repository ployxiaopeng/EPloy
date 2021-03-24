
namespace EPloy.Res
{
    internal sealed class LoadBinaryTask : LoadResTaskBase
    {
        private LoadBinaryCallbacks LoadBinaryCallbacks;

        public override bool IsScene
        {
            get
            {
                return false;
            }
        }

        public static LoadBinaryTask Create(ResInfo resInfo, string[] dependAssetNames, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            LoadBinaryTask loadBinaryTask = ReferencePool.Acquire<LoadBinaryTask>();
            // loadBinaryTask.Initialize(null, resInfo, dependAssetNames);
            loadBinaryTask.LoadBinaryCallbacks = loadBinaryCallbacks;
            return loadBinaryTask;
        }

        public override void Clear()
        {
            base.Clear();
            LoadBinaryCallbacks = null;
        }
    }
}
