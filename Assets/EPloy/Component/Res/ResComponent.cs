
using EPloy.Res;


namespace EPloy
{
    [System]
    public class ResComponentUpdateSystem : UpdateSystem<ResComponent>
    {
        public override void Update(ResComponent self)
        {
            self.Update();
        }
    }

    public partial class ResComponent : Component
    {
        private ResLoader resLoader;

        protected override void Init()
        {
            base.Init();
            resLoader = ResLoader.CreateResLoader();
        }

        public void Update()
        {
            resLoader.Update();
        }

        public void OnDestroy()
        {
            resLoader.OnDestroy();
        }
    }
}
