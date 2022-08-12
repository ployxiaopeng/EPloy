using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EPloy.ECS
{
    public class Entity : IReference
    {
        public string Name { get; private set; }
        public long Id { get; set; }

        /// <summary>
        /// 实体是否释放
        /// </summary>
        public bool IsRelease
        {
            get { return Id == -1; }
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        public virtual void Awake(long id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 实体清理
        /// </summary>
        public virtual void Clear()
        {
            Id = -1;
        }
    }
}
