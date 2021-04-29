using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using EPloy.Res;

namespace EPloy.Download
{
    /// <summary>
    /// 版本校验。
    /// </summary>
    public class DownLoadModule : EPloyModule
    {
        private const float Timeout = 30f;
        private const int OneMegaBytes = 1024 * 1024;
        private TypeLinkedList<DownloadTask> DownloadTasks;
        private Stack<DownloadAgent> FreeDownloadAgents;
        private TypeLinkedList<DownloadAgent> WorkingDownloadAgents;
        public int FreeAgentCount
        {
            get
            {
                return FreeDownloadAgents.Count;
            }
        }

        public override void Awake()
        {
            DownloadTasks = new TypeLinkedList<DownloadTask>();
            FreeDownloadAgents = new Stack<DownloadAgent>();
            WorkingDownloadAgents = new TypeLinkedList<DownloadAgent>();
        }

        public override void Update()
        {
            ProcessRunningTasks();
            ProcessWaitingTasks();
        }

        public override void OnDestroy()
        {
            DownloadTasks.Clear();
            WorkingDownloadAgents.Clear();
        }

        public void AddDownload(string downloadPath, string downloadUri, DownloadCallBack downloadCallBack, object userData = null)
        {
            if (downloadCallBack == null)
            {
                Log.Error("downloadCallBack is null");
                return;
            }
            DownloadTask downloadTask = DownloadTask.Create(downloadPath, downloadUri, OneMegaBytes, Timeout, downloadCallBack, userData);
            DownloadTasks.AddLast(downloadTask);
        }

        public void RemoveDownload(int SerialId)
        {

        }
        private void ProcessRunningTasks()
        {
            LinkedListNode<DownloadAgent> current = WorkingDownloadAgents.First;
            while (current != null)
            {
                DownloadTask task = current.Value.Task;
                if (!task.Done)
                {
                    current.Value.Update();
                    current = current.Next;
                    continue;
                }

                LinkedListNode<DownloadAgent> next = current.Next;
                current.Value.Reset();
                FreeDownloadAgents.Push(current.Value);
                WorkingDownloadAgents.Remove(current);
                task.Dispose();
                current = next;
            }
        }

        private void ProcessWaitingTasks()
        {
            LinkedListNode<DownloadTask> current = DownloadTasks.First;
            while (current != null && FreeAgentCount > 0)
            {
                DownloadAgent agent = FreeDownloadAgents.Pop();
                LinkedListNode<DownloadAgent> agentNode = WorkingDownloadAgents.AddLast(agent);
                DownloadTask task = current.Value;
                LinkedListNode<DownloadTask> next = current.Next;
                DownloadTaskStatus status = agent.Start(task);
                if (status == DownloadTaskStatus.Done || status == DownloadTaskStatus.Error)
                {
                    agent.Reset();
                    FreeDownloadAgents.Push(agent);
                    WorkingDownloadAgents.Remove(agentNode);
                    DownloadTasks.Remove(current);
                    task.Dispose();
                }
                current = next;
            }
        }

    }
}