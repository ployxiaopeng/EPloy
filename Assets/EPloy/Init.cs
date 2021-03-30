using System.Collections;
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
            GameEntry.GameSystem.Add(Config.HotFixDllName, typeof(Init).Assembly);

            GameEntry.FileSystem = GameEntry.Game.AddComponent<FileSystemComponent>();
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.DataStore = GameEntry.Game.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();

            StartCoroutine(enumerator());
        }
        IEnumerator enumerator()
        {
            yield return new WaitForSeconds(1f);
            GameEntry.UI.OpenUIForm(UIName.StartUI, GroupName.Default, null);
        }

        private void Update()
        {
            GameEntry.GameSystem.Update();
        }
    }
}
