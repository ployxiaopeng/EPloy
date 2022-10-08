using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    // 数据 以Ecs CommonAICpt 的组件存在 动态设置 自定义的数据继承 CommonAICpt即可
    public class BTNode : IReference
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; protected set; }
        /// <summary>
        /// 孩子节点列表
        /// </summary>
        public List<BTNode> childNodes { get; protected set; }

        public BTNode()
        {
            childNodes = new List<BTNode>();
            Clear();
        }

        /// <summary>
        /// 检查节点能否执行：包括是否激活，是否冷却完成，是否通过准入条件以及个性化检查
        /// </summary>
        public virtual bool Evaluate(CommonAICpt aiCpt)
        {
            return true;
        }
        /// <summary>
        /// 节点执行
        /// </summary>
        /// <returns></returns>
        public virtual BTResult Execute(CommonAICpt aiCpt)
        {
            return BTResult.Success;
        }
        /// <summary>
        /// 添加子节点函数
        /// </summary>
        /// <param name="bTNode"></param>
        public void AddChildNode(BTNode bTNode)
        {
            if (childNodes.Contains(bTNode))
            {
                Log.Warning("节点已经存在");
                return;
            }
            childNodes.Add(bTNode);
        }
        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="bTNode"></param>
        /// <param name="是否释放"></param>
        public void RemoveChildNote(BTNode bTNode, bool isRelease = false)
        {
            if (childNodes.Contains(bTNode))
            {
                childNodes.Remove(bTNode);
                if (isRelease) ReferencePool.Release(bTNode);
            }
            Log.Warning("节点不存在");
        }

        /// <summary>
        /// 节点清除
        /// </summary>
        public virtual void Clear()
        {
            foreach (var notes in childNodes)
            {
                notes.RemoveChildNote(notes, true);
            }
            childNodes.Clear();
            name = null;
        }
    }
}
