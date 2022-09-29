using UnityEngine;

namespace EPloy.ECS
{
    public class CameraSystem : IReference
    {
        public void SetCameraFollow(Entity entity, Camera camera,Transform target)
        {
            CameraFollowCpt cameraFollowCpt = entity.AddCpt<CameraFollowCpt>();
            cameraFollowCpt.camera = camera;
            cameraFollowCpt.target = target;
        }

        public void Update(Entity entity, CameraFollowCpt cameraFollowCpt)
        {
            Vector3 target = cameraFollowCpt.target.localPosition + cameraFollowCpt.offset;
            cameraFollowCpt.camera.transform.localPosition = Vector3.Lerp(cameraFollowCpt.camera.transform.localPosition, target, Time.deltaTime * cameraFollowCpt.speed);
        }
        public void Clear()
        {

        }
    }
}