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
    unsafe class EPloy_UIEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EPloy.UIEventListener);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_0);

            field = type.GetField("onArgBeginDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgBeginDrag_0);
            app.RegisterCLRFieldSetter(field, set_onArgBeginDrag_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgBeginDrag_0, AssignFromStack_onArgBeginDrag_0);
            field = type.GetField("onArgDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgDrag_1);
            app.RegisterCLRFieldSetter(field, set_onArgDrag_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgDrag_1, AssignFromStack_onArgDrag_1);
            field = type.GetField("onArgEndDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgEndDrag_2);
            app.RegisterCLRFieldSetter(field, set_onArgEndDrag_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgEndDrag_2, AssignFromStack_onArgEndDrag_2);
            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_3);
            app.RegisterCLRFieldSetter(field, set_onClick_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_3, AssignFromStack_onClick_3);
            field = type.GetField("onClickDown", flag);
            app.RegisterCLRFieldGetter(field, get_onClickDown_4);
            app.RegisterCLRFieldSetter(field, set_onClickDown_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClickDown_4, AssignFromStack_onClickDown_4);
            field = type.GetField("onClickUp", flag);
            app.RegisterCLRFieldGetter(field, get_onClickUp_5);
            app.RegisterCLRFieldSetter(field, set_onClickUp_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClickUp_5, AssignFromStack_onClickUp_5);


        }


        static StackObject* Get_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @Go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = EPloy.UIEventListener.Get(@Go);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_onArgBeginDrag_0(ref object o)
        {
            return ((EPloy.UIEventListener)o).onArgBeginDrag;
        }

        static StackObject* CopyToStack_onArgBeginDrag_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onArgBeginDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgBeginDrag_0(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onArgBeginDrag = (EPloy.UIEventListener.UIArgListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onArgBeginDrag_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIArgListenerDelegate @onArgBeginDrag = (EPloy.UIEventListener.UIArgListenerDelegate)typeof(EPloy.UIEventListener.UIArgListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onArgBeginDrag = @onArgBeginDrag;
            return ptr_of_this_method;
        }

        static object get_onArgDrag_1(ref object o)
        {
            return ((EPloy.UIEventListener)o).onArgDrag;
        }

        static StackObject* CopyToStack_onArgDrag_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onArgDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgDrag_1(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onArgDrag = (EPloy.UIEventListener.UIArgListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onArgDrag_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIArgListenerDelegate @onArgDrag = (EPloy.UIEventListener.UIArgListenerDelegate)typeof(EPloy.UIEventListener.UIArgListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onArgDrag = @onArgDrag;
            return ptr_of_this_method;
        }

        static object get_onArgEndDrag_2(ref object o)
        {
            return ((EPloy.UIEventListener)o).onArgEndDrag;
        }

        static StackObject* CopyToStack_onArgEndDrag_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onArgEndDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgEndDrag_2(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onArgEndDrag = (EPloy.UIEventListener.UIArgListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onArgEndDrag_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIArgListenerDelegate @onArgEndDrag = (EPloy.UIEventListener.UIArgListenerDelegate)typeof(EPloy.UIEventListener.UIArgListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onArgEndDrag = @onArgEndDrag;
            return ptr_of_this_method;
        }

        static object get_onClick_3(ref object o)
        {
            return ((EPloy.UIEventListener)o).onClick;
        }

        static StackObject* CopyToStack_onClick_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_3(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onClick = (EPloy.UIEventListener.UIListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onClick_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIListenerDelegate @onClick = (EPloy.UIEventListener.UIListenerDelegate)typeof(EPloy.UIEventListener.UIListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onClickDown_4(ref object o)
        {
            return ((EPloy.UIEventListener)o).onClickDown;
        }

        static StackObject* CopyToStack_onClickDown_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onClickDown;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClickDown_4(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onClickDown = (EPloy.UIEventListener.UIListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onClickDown_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIListenerDelegate @onClickDown = (EPloy.UIEventListener.UIListenerDelegate)typeof(EPloy.UIEventListener.UIListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onClickDown = @onClickDown;
            return ptr_of_this_method;
        }

        static object get_onClickUp_5(ref object o)
        {
            return ((EPloy.UIEventListener)o).onClickUp;
        }

        static StackObject* CopyToStack_onClickUp_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.UIEventListener)o).onClickUp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClickUp_5(ref object o, object v)
        {
            ((EPloy.UIEventListener)o).onClickUp = (EPloy.UIEventListener.UIListenerDelegate)v;
        }

        static StackObject* AssignFromStack_onClickUp_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EPloy.UIEventListener.UIListenerDelegate @onClickUp = (EPloy.UIEventListener.UIListenerDelegate)typeof(EPloy.UIEventListener.UIListenerDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((EPloy.UIEventListener)o).onClickUp = @onClickUp;
            return ptr_of_this_method;
        }



    }
}
