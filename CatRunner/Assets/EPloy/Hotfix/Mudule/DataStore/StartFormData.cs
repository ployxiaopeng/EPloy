using EPloy.Hotfix;
using EPloy.Hotfix.Table;

namespace EPloy.Hotfix
{
    public class StartFormData : DataStoreBase
    {
        public int txtId = 0;

        public override void Create()
        {
            txtId = 100000;
        }
    }
}