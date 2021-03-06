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
    unsafe class ETModel_HotfixUIEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.HotfixUIEvent);
            args = new Type[]{};
            method = type.GetMethod("get_uIFunType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_uIFunType_0);
            args = new Type[]{};
            method = type.GetMethod("get_uIFormLogic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_uIFormLogic_1);

            field = type.GetField("EventId", flag);
            app.RegisterCLRFieldGetter(field, get_EventId_0);


        }


        static StackObject* get_uIFunType_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.HotfixUIEvent instance_of_this_method = (ETModel.HotfixUIEvent)typeof(ETModel.HotfixUIEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.uIFunType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_uIFormLogic_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.HotfixUIEvent instance_of_this_method = (ETModel.HotfixUIEvent)typeof(ETModel.HotfixUIEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.uIFormLogic;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_EventId_0(ref object o)
        {
            return ETModel.HotfixUIEvent.EventId;
        }


    }
}
