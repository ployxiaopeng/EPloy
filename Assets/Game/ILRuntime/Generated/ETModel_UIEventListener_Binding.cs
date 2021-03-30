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
    unsafe class ETModel_UIEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.UIEventListener);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_0);

            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_0);
            app.RegisterCLRFieldSetter(field, set_onClick_0);
            field = type.GetField("onClickDown", flag);
            app.RegisterCLRFieldGetter(field, get_onClickDown_1);
            app.RegisterCLRFieldSetter(field, set_onClickDown_1);
            field = type.GetField("onClickUp", flag);
            app.RegisterCLRFieldGetter(field, get_onClickUp_2);
            app.RegisterCLRFieldSetter(field, set_onClickUp_2);


        }


        static StackObject* Get_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @Go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = ETModel.UIEventListener.Get(@Go);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_onClick_0(ref object o)
        {
            return ((ETModel.UIEventListener)o).onClick;
        }
        static void set_onClick_0(ref object o, object v)
        {
            ((ETModel.UIEventListener)o).onClick = (ETModel.UIEventListener.UIListenerDelegate)v;
        }
        static object get_onClickDown_1(ref object o)
        {
            return ((ETModel.UIEventListener)o).onClickDown;
        }
        static void set_onClickDown_1(ref object o, object v)
        {
            ((ETModel.UIEventListener)o).onClickDown = (ETModel.UIEventListener.UIListenerDelegate)v;
        }
        static object get_onClickUp_2(ref object o)
        {
            return ((ETModel.UIEventListener)o).onClickUp;
        }
        static void set_onClickUp_2(ref object o, object v)
        {
            ((ETModel.UIEventListener)o).onClickUp = (ETModel.UIEventListener.UIListenerDelegate)v;
        }


    }
}
