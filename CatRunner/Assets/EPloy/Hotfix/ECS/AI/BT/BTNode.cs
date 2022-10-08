using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    // ���� ��Ecs CommonAICpt ��������� ��̬���� �Զ�������ݼ̳� CommonAICpt����
    public class BTNode : IReference
    {
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string name { get; protected set; }
        /// <summary>
        /// ���ӽڵ��б�
        /// </summary>
        public List<BTNode> childNodes { get; protected set; }

        public BTNode()
        {
            childNodes = new List<BTNode>();
            Clear();
        }

        /// <summary>
        /// ���ڵ��ܷ�ִ�У������Ƿ񼤻�Ƿ���ȴ��ɣ��Ƿ�ͨ��׼�������Լ����Ի����
        /// </summary>
        public virtual bool Evaluate(CommonAICpt aiCpt)
        {
            return true;
        }
        /// <summary>
        /// �ڵ�ִ��
        /// </summary>
        /// <returns></returns>
        public virtual BTResult Execute(CommonAICpt aiCpt)
        {
            return BTResult.Success;
        }
        /// <summary>
        /// ����ӽڵ㺯��
        /// </summary>
        /// <param name="bTNode"></param>
        public void AddChildNode(BTNode bTNode)
        {
            if (childNodes.Contains(bTNode))
            {
                Log.Warning("�ڵ��Ѿ�����");
                return;
            }
            childNodes.Add(bTNode);
        }
        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="bTNode"></param>
        /// <param name="�Ƿ��ͷ�"></param>
        public void RemoveChildNote(BTNode bTNode, bool isRelease = false)
        {
            if (childNodes.Contains(bTNode))
            {
                childNodes.Remove(bTNode);
                if (isRelease) ReferencePool.Release(bTNode);
            }
            Log.Warning("�ڵ㲻����");
        }

        /// <summary>
        /// �ڵ����
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
