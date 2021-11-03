using UnityEngine;

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
                if (mapCameraCpt.isInit)
                {
                    Vector3 pos = Vector3.Lerp(mapCameraCpt.transform.position,
                        mapCameraCpt.target, Time.deltaTime * 20f);
                    mapCameraCpt.transform.position = pos;
                }
                else
                {
                    mapCameraCpt.transform.position = mapCameraCpt.target;
                    mapCameraCpt.isInit = true;
                }
            }
        }

        public void OnDestroy()
        {

        }
    }
}