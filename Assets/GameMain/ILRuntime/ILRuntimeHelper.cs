using Google.Protobuf;
using ILRuntime.Runtime.Enviorment;
// using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace EPloy
{
    public static class ILRuntimeHelper
    {
        public static void InitILRuntime(AppDomain appDomain)
        {
            //TODO:注册重定向方法


            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();

            //GF用
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, ILTypeInstance>();

            //ET用
            appDomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterMethodDelegate<ushort, object>();
            appDomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appDomain.DelegateManager.RegisterMethodDelegate<ushort, MemoryStream>();

            //PB用
            appDomain.DelegateManager.RegisterFunctionDelegate<IMessageAdaptor.Adaptor>();
            appDomain.DelegateManager.RegisterMethodDelegate<IMessageAdaptor.Adaptor>();


            //注册委托
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((action) =>
            {
                return new UnityAction(() =>
                {
                    ((Action)action)();
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<float>>((action) =>
            {
                return new UnityAction<float>((a) =>
                {
                    ((Action<float>)action)(a);
                });
            });
     
            appDomain.DelegateManager.RegisterDelegateConvertor<EventHandler<ILTypeInstance>>((act) =>
            {
                return new EventHandler<ILTypeInstance>((sender, e) =>
                {
                    ((Action<object, ILTypeInstance>)act)(sender, e);
                });
            });


            //注册CLR绑定代码
            //CLRBindings.Initialize(appDomain);

            //TODO:注册跨域继承适配器
            appDomain.RegisterCrossBindingAdaptor(new IMessageAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new IDisposableAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new ICoroutineAdapter());

            //TODO:注册值类型绑定
            appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);
        }
    }
}
