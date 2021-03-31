using GameFramework;
using GameFramework.Network;

namespace ETModel
{
    [ObjectSystem]
    public class NetWorkComponentAwakeSystem : AwakeSystem<NetworkComponent>
    {
        public override void Awake(NetworkComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class NetWorkComponentStartSystem : StartSystem<NetworkComponent>
    {
        public override void Start(NetworkComponent self)
        {
            self.Start();
        }
    }
    public static class NetWorkComponentSystem
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this NetworkComponent self)
        {
            self.NetworkManager = GameFrameworkEntry.GetModule<INetworkManager>();
            if (self.NetworkManager == null)
            {
                Log.Fatal("Network manager is invalid.");
                return;
            }
        }

        public static void Start(this NetworkComponent self)
        {
            try
            {
                //消息识别码组件
                ModelGame.Scene.AddComponent<OpcodeTypeComponent>();
                //消息分发组件
                ModelGame.Scene.AddComponent<MessageDispatcherComponent>();
            }
            catch (System.Exception e)
            {
                Log.Error("初始化ET网络组件时出现异常：{0}", e);
            }

        }
    }
}