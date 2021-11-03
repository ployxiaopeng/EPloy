namespace EPloy
{
    public class MapCameraSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }

        private MapCpt mapCpt;
        private MapEntityCpt mapEntityCpt;

        public void Start()
        {
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
        }

        public void Update()
        {
            if (mapEntityCpt.role.HasComponent<MapCameraCpt>())
            {
                MapCameraCpt mapCameraCpt = mapEntityCpt.role.GetComponent<MapCameraCpt>();
                mapCameraCpt.camera.transform.position = mapCameraCpt.followPos + mapCameraCpt.offset;
            }
        }


        public void OnDestroy()
        {

        }
    }
}