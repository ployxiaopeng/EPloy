using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using EPloy.Res;

namespace EPloy
{
    /// <summary>
    /// 版本校验。
    /// </summary>
    public class DownModule : EPloyModule
    {

        private Queue<DownloadTask> DownloadTask;
        private Queue<DownloadAgent> DownloadAgent;

        public override void Awake()
        {
            DownloadTask = new Queue<DownloadTask>();
            DownloadAgent = new Queue<DownloadAgent>();
        }

        public override void Update()
        {
            ProcessRunningTasks();
        }

        public override void OnDestroy()
        {
            DownloadTask.Clear();
            DownloadAgent.Clear();
        }

 
        private void ProcessRunningTasks()
        {
            DownloadAgent current = DownloadAgent.Peek();
            while (current != null)
            {
                DownloadTask task = current.Task;
                if (!task.Done)
                {
                    current.Update();
                    current = DownloadAgent.Dequeue();
                    continue;
                }

                // LinkedListNode<ITaskAgent<T>> next = current.Next;
                // current.Value.Reset();
                // m_FreeAgents.Push(current.Value);
                // m_WorkingAgents.Remove(current);
                // ReferencePool.Release(task);
                // current = next;
            }
        }

        private void ProcessWaitingTasks(float elapseSeconds, float realElapseSeconds)
        {
            // LinkedListNode<T> current = m_WaitingTasks.First;
            // while (current != null && FreeAgentCount > 0)
            // {
            //     ITaskAgent<T> agent = m_FreeAgents.Pop();
            //     LinkedListNode<ITaskAgent<T>> agentNode = m_WorkingAgents.AddLast(agent);
            //     T task = current.Value;
            //     LinkedListNode<T> next = current.Next;
            //     StartTaskStatus status = agent.Start(task);
            //     if (status == StartTaskStatus.Done || status == StartTaskStatus.HasToWait || status == StartTaskStatus.UnknownError)
            //     {
            //         agent.Reset();
            //         m_FreeAgents.Push(agent);
            //         m_WorkingAgents.Remove(agentNode);
            //     }

            //     if (status == StartTaskStatus.Done || status == StartTaskStatus.CanResume || status == StartTaskStatus.UnknownError)
            //     {
            //         m_WaitingTasks.Remove(current);
            //     }

            //     if (status == StartTaskStatus.Done || status == StartTaskStatus.UnknownError)
            //     {
            //         ReferencePool.Release(task);
            //     }

            //     current = next;
            //}
        }

    }
}