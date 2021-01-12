using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public class Init : MonoBehaviour
    {
        
        private void Start()
        {
            GameEntry.GameSystem.Add("Main", typeof(Init).Assembly);
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();

            Entity entity = GameEntry.Game.CreateEntity("我就是这样啊");

            GameEntry.Event.Subscribe(100, EvtTest);
            Debug.LogError(entity.Name);

            StartCoroutine(enumerator());
        }
        IEnumerator enumerator()
        {
            yield return new WaitForSeconds(2f);
            GameEntry.Event.Fire(ReferencePool.Acquire<TestEvt>());
        }
        public void EvtTest(EventArg eventArg)
        {
            Debug.Log("事件测试");
        }

        private void Update()
        {
            GameEntry.GameSystem.Update();
        }
    }


}
