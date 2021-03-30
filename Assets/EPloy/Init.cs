using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public class Init : MonoBehaviour
    {
        internal static Init Instance = null;

        public bool isEditorRes = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Start()
        {
            GameEntry.GameSystem.Add("Main", typeof(Init).Assembly);
            GameEntry.FileSystem = GameEntry.Game.AddComponent<FileSystemComponent>();
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();

            Entity entity = GameEntry.Game.CreateEntity("Game01");
            GameEntry.Event.Subscribe(EventId.TestEvt, EvtTest);
            StartCoroutine(enumerator());
        }
        IEnumerator enumerator()
        {
            yield return new WaitForSeconds(2f);
            GameEntry.Event.Fire(ReferencePool.Acquire<TestEvt>());
            GameEntry.UI.OpenUIForm(UIName.txstUIForm, GroupName.Default, null);
        }
        public void EvtTest(EventArg eventArg)
        {
            Log.Info("事件测试");
        }

        private void Update()
        {
            GameEntry.GameSystem.Update();
        }
    }


}
