using UnityEngine;

namespace EPloy
{
    public class CameraSystem : ISystem
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
            if (mapEntityCpt.role.HasComponent<CameraCpt>())
            {
                CameraCpt cameraCpt = mapEntityCpt.role.GetComponent<CameraCpt>();
                cameraCpt.transform.position = cameraCpt.target;
                if (mapCpt.map.HasComponent<MapCreateCpt>())
                {
                    MapCreateCpt mapCreateCpt = mapCpt.map.GetComponent<MapCreateCpt>();
                    mapCreateCpt.SetCreate(cameraCpt.transform.position);
                }

                return;
            }
        }

        public void OnDestroy()
        {

        }
    }
}