using GameFramework;
using GameFramework.WebRequest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    [ObjectSystem]
    public class WebRequestComponentAwakeSystem : AwakeSystem<WebRequestComponent>
    {
        public override void Awake(WebRequestComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class WebRequestComponentStartSystem : StartSystem<WebRequestComponent>
    {
        public override void Start(WebRequestComponent self)
        {
            self.Start();
        }
    }
    [ObjectSystem]
    public class WebRequestComponentUpdateSystem : UpdateSystem<WebRequestComponent>
    {
        public override void Update(WebRequestComponent self)
        {
            self.Update();
        }
    }
    public static class WebRequestComponentSystem
    {
        private static int HelperCount = 3;
        private static float Timeout = 30f;

        public static void Awake(this WebRequestComponent self)
        {
            self.WebRequestManager = GameFrameworkEntry.GetModule<IWebRequestManager>();
            if (self.WebRequestManager == null)
            {
                Log.Fatal("Web request manager is invalid.");
                return;
            }
            self.webRequestHelpers = new WebRequestHelper[HelperCount];
            self.WebRequestManager.Timeout = Timeout;
        }
        public static void Start(this WebRequestComponent self)
        {
            for (int i = 0; i < HelperCount; i++)
                self.AddWebRequestAgentHelper(i);
        }
        public static void Update(this WebRequestComponent self)
        {
            foreach (var helper in self.webRequestHelpers)
                helper.Update();
        }
    }
}