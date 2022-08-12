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
    unsafe class EPloy_Game_Res_LoadBinaryCallbacks_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EPloy.Game.Res.LoadBinaryCallbacks);

            args = new Type[]{typeof(System.Action<System.String, System.Byte[], System.Single>), typeof(System.Action<System.String, EPloy.Game.Res.LoadResStatus, System.String>)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, EPloy.Game.Res.LoadResStatus, System.String> @loadBinaryFailureCallback = (System.Action<System.String, EPloy.Game.Res.LoadResStatus, System.String>)typeof(System.Action<System.String, EPloy.Game.Res.LoadResStatus, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<System.String, System.Byte[], System.Single> @loadBinarySuccessCallback = (System.Action<System.String, System.Byte[], System.Single>)typeof(System.Action<System.String, System.Byte[], System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new EPloy.Game.Res.LoadBinaryCallbacks(@loadBinarySuccessCallback, @loadBinaryFailureCallback);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
