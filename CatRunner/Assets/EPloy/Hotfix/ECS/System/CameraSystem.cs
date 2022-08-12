using UnityEngine;

namespace EPloy.Hotfix
{
    public class CameraSystem : IReference
    {
        public void SetCameraFollow(EntityRole entityRole, Camera camera)
        {
            entityRole.cameraFollowCpt = HotFixMudule.GameScene.GetCpt<CameraFollowCpt>(entityRole);
            entityRole.cameraFollowCpt.camera = camera;
        }

        public void Update(EntityRole entityRole, CameraFollowCpt cameraFollowCpt)
        {
            Vector3 target = entityRole.roleCpt.rolePos + cameraFollowCpt.offset;
            cameraFollowCpt.camera.transform.localPosition = Vector3.Lerp(cameraFollowCpt.camera.transform.localPosition, target, Time.deltaTime * cameraFollowCpt.speed);
        }
        public void Clear()
        {

        }
    }
}