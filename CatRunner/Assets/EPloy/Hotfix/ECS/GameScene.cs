using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// EcsGame场景
    /// </summary>
    public partial class GameScene : IHotfixModule
    {

        public override void Awake()
        {

        }

        public void ECSActivate()
        {
            CreateSystem();
            HotFixMudule.RegisterUpdate(ECSUpdate);
        }

        public void ECSDisable()
        {
            foreach (var entity in entityRoles)
            {
                ReferencePool.Release(entity);
            }
            entityRoles.Clear();
            HotFixMudule.RemoveUpdate(ECSUpdate);
        }

        public void EnterMap(int mapId, int playerId)
        {
            entityMap = CreateEntity<EntityMap>("map");
            entityMap.mapCpt = GetCpt<MapCpt>(entityMap);
            entityMap.mapCpt.mapId = mapId;
            entityMap.mapCpt.PlayerId = playerId;
            mapSys.Start(entityMap, entityMap.mapCpt);
        }

        public void ExitMap()
        {

        }

        public void ECSUpdate()
        {
            for (int i = 0; i < entityRoles.Count; i++)
            {
                EntityRole entityRole = entityRoles[i];
                aICommandSys.Updata(entityRole);
                CameraFollowUpdate(entityRole);
                MoveUpdate(entityRole);
            }
        }

        private void CameraFollowUpdate(EntityRole entityRole)
        {
            if (entityRole.cameraFollowCpt == null) return;
            cameraSys.Update(entityRole, entityRole.cameraFollowCpt);
        }

        private void MoveUpdate(EntityRole entityRole)
        {
            if (entityRole.moveCpt == null) return;
            moveSys.Update(entityRole, entityRole.moveCpt);
        }

        public override void OnDestroy()
        {
            ECSDisable();
        }
    }
}