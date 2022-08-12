using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class EPloy_Game_GameModule_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EPloy.Game.GameModule);
            args = new Type[]{};
            method = type.GetMethod("get_Timer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Timer_0);
            args = new Type[]{};
            method = type.GetMethod("get_Atlas", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Atlas_1);
            args = new Type[]{};
            method = type.GetMethod("get_Res", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Res_2);
            args = new Type[]{};
            method = type.GetMethod("get_ILRuntime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ILRuntime_3);

            field = type.GetField("ObjectPool", flag);
            app.RegisterCLRFieldGetter(field, get_ObjectPool_0);
            app.RegisterCLRFieldSetter(field, set_ObjectPool_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ObjectPool_0, AssignFromStack_ObjectPool_0);


        }


        static StackObject* get_Timer_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EPloy.Game.GameModule.Timer;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Atlas_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EPloy.Game.GameModule.Atlas;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Res_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EPloy.Game.GameModule.Res;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ILRuntime_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EPloy.Game.GameModule.ILRuntime;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_ObjectPool_0(ref object o)
        {
            return EPloy.Game.GameModule.ObjectPool;
        }

        static StackObject* CopyToStack_ObjectPool_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EPloy.Game.GameModule.ObjectPool;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ObjectPool_0(ref object o, object v)
        {
            EPloy.Game.GameModule.ObjectPool = (EPloy.Game.ObjectPoolMudule)v;
        }

        static StackObject* AssignFromStack_ObjectPool_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.Game.ObjectPoolMudule @ObjectPool = (EPloy.Game.ObjectPoolMudule)typeof(EPloy.Game.ObjectPoolMudule).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            EPloy.Game.GameModule.ObjectPool = @ObjectPool;
            return ptr_of_this_method;
        }



    }
}
