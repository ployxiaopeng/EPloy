//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ETModel;
using GameFramework;
using GameFramework.DataTable;
using System;
using UnityEngine;

namespace ETHotfix
{

    public static class EntityComponentSystem
    {
        private static int EntityIdRecord = 0;

        private static IDataTable<DREntity> _dataEntity = null;
        private static IDataTable<DREntity> DateEntity
        {
            get
            {
                if (_dataEntity == null) _dataEntity = GameEntry.DataTable.GetDataTable<DREntity>();
                return _dataEntity;
            }
        }
        public static DREntity GetEntity(this int self)
        {
            return DateEntity.GetDataRow(self);
        }

        private static EntityHotfixComponent Entity = null;

        public static async ETVoid HotfixAwake(this EntityComponent self)
        {

        }

        /// <summary>
        ///隐藏所有实体 重置实体Id 
        /// </summary>
        /// <param name="self"></param>
        public static void HoxfixHideAllLoadedEntities(this EntityComponent self)
        {
            if (Entity == null) Entity = GameEntry.Extension.GetComponent<EntityHotfixComponent>();
            Entity.HideAllLoadedEntities();
            self.HideAllLoadedEntities();
            EntityIdRecord = 0;
        }
        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entity"></param>
        public static void HideEntity(this EntityComponent self, EntityLogic entity)
        {
            self.HideEntity(entity);
        }

        /// <summary>
        /// 显示角色
        /// </summary>
        /// <param name="self"></param>
        /// <param name="effectId"></param>
        /// <param name="parentLevel"></param>
        public static void ShowHero(this EntityComponent self, RoleData roleData)
        {
            self.ShowEntity(roleData, typeof(RoleLogic));
        }

        ///// <summary>
        ///// 显示怪物
        ///// </summary>
        ///// <param name="self"></param>
        ///// <param name="effectId"></param>
        ///// <param name="parentLevel"></param>
        //public static void ShowMonster(this EntityComponent self, HeroMonsterData entityMonster, Transform parentLevel)
        //{
        //    DREntity drEntity = DateEntity.GetDataRow(entityMonster.DREntityId);
        //    if (drEntity == null)
        //    {
        //        Log.Error("没有找到这个怪物 ID: " + entityMonster.DREntityId);
        //        return;
        //    }
        //    entityMonster.SetDREntityData(drEntity, parentLevel);
        //    self.ShowEntity(entityMonster, typeof(Monster));
        //}

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="self"></param>
        /// <param name="data"></param>
        private static void ShowEntity(this EntityComponent self, HotfixEntityData data, Type type)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }
            //待定 设置一个界限 EntityIdRecord = 1;            
            EntityIdRecord += 1;
            data.EntityId = EntityIdRecord;
            Entity.OnShow(data.EntityId, data);
            self.ShowEntity(EntityIdRecord, AssetUtility.GetEntityAsset(data.DREntity.AssetName), data.DREntity.AssetType, Constant.AssetPriority.EffectAsset, type.ToString());
        }
    }
}