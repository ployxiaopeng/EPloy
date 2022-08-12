using Google.Protobuf;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace EPloy.Game
{
    public static class ILRuntimeHelper
    {
        public static void InitILRuntime(AppDomain appDomain)
        {

            //注册委托
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((action) =>
            {
                return new UnityAction(() => { ((Action)action)(); });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<float>>((action) =>
            {
                return new UnityAction<float>((a) => { ((Action<float>)action)(a); });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<EventHandler<ILTypeInstance>>((act) =>
            {
                return new EventHandler<ILTypeInstance>((sender, e) =>
                {
                    ((Action<object, ILTypeInstance>)act)(sender, e);
                });
            });            appDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });
            // 注册类型
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appDomain.DelegateManager.RegisterMethodDelegate<Net.Packet>();
            appDomain.DelegateManager.RegisterFunctionDelegate<Net.Packet>();
            appDomain.DelegateManager.RegisterMethodDelegate<Net.NetErrorCode, System.Net.Sockets.SocketError, String>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, ILTypeInstance>();
            appDomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterMethodDelegate<ushort, object>();
            appDomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appDomain.DelegateManager.RegisterMethodDelegate<ushort, MemoryStream>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector2, object>();
            //资源加载
            appDomain.DelegateManager.RegisterMethodDelegate<string, float>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, EPloy.Game.Res.LoadResStatus, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, string, int, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<string>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, object, float, object>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, byte[], float>();

            //PB用
            appDomain.DelegateManager.RegisterFunctionDelegate<IMessageAdaptor.Adaptor>();
            appDomain.DelegateManager.RegisterMethodDelegate<IMessageAdaptor.Adaptor>();
            //虚拟列表用
            appDomain.DelegateManager.RegisterFunctionDelegate<RectTransform>();
            appDomain.DelegateManager.RegisterMethodDelegate<int, RectTransform>();
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject, UnityEngine.EventSystems.PointerEventData>();


            //TODO:注册跨域继承适配器
            appDomain.RegisterCrossBindingAdaptor(new IMessageAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new ICoroutineAdapter());
            //appDomain.RegisterCrossBindingAdaptor(new IDisposableAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());

            //TODO:注册值类型绑定
            appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);

            //注册CLR绑定代码 坑一定要放在最后
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
        }
    }
}
