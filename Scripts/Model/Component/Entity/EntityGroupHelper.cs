//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.Entity;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 默认实体组辅助器。
    /// </summary>
    public class EntityGroupHelper : IEntityGroupHelper
    {
        public GameObject Group { get; protected set; }

        public EntityGroupHelper(string name, Transform parent)
        {
            Group = new GameObject(name);
            Group.transform.SetParent(parent, false);
        }
    }
}
