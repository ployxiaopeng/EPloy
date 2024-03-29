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
    unsafe class EPloy_Game_ILRuntimeModule_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EPloy.Game.ILRuntimeModule);
            args = new Type[]{};
            method = type.GetMethod("get_HotfixUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_HotfixUpdate_0);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_HotfixUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_HotfixUpdate_1);
            args = new Type[]{};
            method = type.GetMethod("get_GetHotfixTypes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_GetHotfixTypes_2);


        }


        static StackObject* get_HotfixUpdate_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ILRuntimeModule instance_of_this_method = (EPloy.Game.ILRuntimeModule)typeof(EPloy.Game.ILRuntimeModule).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HotfixUpdate;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_HotfixUpdate_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ILRuntimeModule instance_of_this_method = (EPloy.Game.ILRuntimeModule)typeof(EPloy.Game.ILRuntimeModule).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HotfixUpdate = value;

            return __ret;
        }

        static StackObject* get_GetHotfixTypes_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ILRuntimeModule instance_of_this_method = (EPloy.Game.ILRuntimeModule)typeof(EPloy.Game.ILRuntimeModule).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetHotfixTypes;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
