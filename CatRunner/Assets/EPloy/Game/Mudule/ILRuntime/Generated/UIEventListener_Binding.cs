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
    unsafe class UIEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIEventListener);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_0);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("RemoveListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RemoveListener_1);

            field = type.GetField("onArgDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgDrag_0);
            app.RegisterCLRFieldSetter(field, set_onArgDrag_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgDrag_0, AssignFromStack_onArgDrag_0);
            field = type.GetField("onClickUp", flag);
            app.RegisterCLRFieldGetter(field, get_onClickUp_1);
            app.RegisterCLRFieldSetter(field, set_onClickUp_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClickUp_1, AssignFromStack_onClickUp_1);
            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_2);
            app.RegisterCLRFieldSetter(field, set_onClick_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_2, AssignFromStack_onClick_2);
            field = type.GetField("onArgBeginDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgBeginDrag_3);
            app.RegisterCLRFieldSetter(field, set_onArgBeginDrag_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgBeginDrag_3, AssignFromStack_onArgBeginDrag_3);
            field = type.GetField("onArgEndDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onArgEndDrag_4);
            app.RegisterCLRFieldSetter(field, set_onArgEndDrag_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_onArgEndDrag_4, AssignFromStack_onArgEndDrag_4);


        }


        static StackObject* Get_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @Go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::UIEventListener.Get(@Go);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* RemoveListener_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @Go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            global::UIEventListener.RemoveListener(@Go);

            return __ret;
        }


        static object get_onArgDrag_0(ref object o)
        {
            return ((global::UIEventListener)o).onArgDrag;
        }

        static StackObject* CopyToStack_onArgDrag_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onArgDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgDrag_0(ref object o, object v)
        {
            ((global::UIEventListener)o).onArgDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_onArgDrag_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData> @onArgDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIEventListener)o).onArgDrag = @onArgDrag;
            return ptr_of_this_method;
        }

        static object get_onClickUp_1(ref object o)
        {
            return ((global::UIEventListener)o).onClickUp;
        }

        static StackObject* CopyToStack_onClickUp_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onClickUp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClickUp_1(ref object o, object v)
        {
            ((global::UIEventListener)o).onClickUp = (System.Action<UnityEngine.GameObject>)v;
        }

        static StackObject* AssignFromStack_onClickUp_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.GameObject> @onClickUp = (System.Action<UnityEngine.GameObject>)typeof(System.Action<UnityEngine.GameObject>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIEventListener)o).onClickUp = @onClickUp;
            return ptr_of_this_method;
        }

        static object get_onClick_2(ref object o)
        {
            return ((global::UIEventListener)o).onClick;
        }

        static StackObject* CopyToStack_onClick_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_2(ref object o, object v)
        {
            ((global::UIEventListener)o).onClick = (System.Action<UnityEngine.GameObject>)v;
        }

        static StackObject* AssignFromStack_onClick_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.GameObject> @onClick = (System.Action<UnityEngine.GameObject>)typeof(System.Action<UnityEngine.GameObject>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIEventListener)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onArgBeginDrag_3(ref object o)
        {
            return ((global::UIEventListener)o).onArgBeginDrag;
        }

        static StackObject* CopyToStack_onArgBeginDrag_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onArgBeginDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgBeginDrag_3(ref object o, object v)
        {
            ((global::UIEventListener)o).onArgBeginDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_onArgBeginDrag_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData> @onArgBeginDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIEventListener)o).onArgBeginDrag = @onArgBeginDrag;
            return ptr_of_this_method;
        }

        static object get_onArgEndDrag_4(ref object o)
        {
            return ((global::UIEventListener)o).onArgEndDrag;
        }

        static StackObject* CopyToStack_onArgEndDrag_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onArgEndDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onArgEndDrag_4(ref object o, object v)
        {
            ((global::UIEventListener)o).onArgEndDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_onArgEndDrag_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData> @onArgEndDrag = (System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.GameObject, UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::UIEventListener)o).onArgEndDrag = @onArgEndDrag;
            return ptr_of_this_method;
        }



    }
}
