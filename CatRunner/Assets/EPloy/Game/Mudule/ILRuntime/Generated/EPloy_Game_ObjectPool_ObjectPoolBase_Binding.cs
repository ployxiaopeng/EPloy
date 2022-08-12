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
    unsafe class EPloy_Game_ObjectPool_ObjectPoolBase_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EPloy.Game.ObjectPool.ObjectPoolBase);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("Unspawn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Unspawn_0);
            args = new Type[]{};
            method = type.GetMethod("ReleaseAllUnused", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReleaseAllUnused_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Spawn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Spawn_2);
            args = new Type[]{typeof(EPloy.Game.ObjectPool.ObjectBase), typeof(System.Boolean)};
            method = type.GetMethod("Register", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Register_3);
            args = new Type[]{};
            method = type.GetMethod("get_AutoReleaseTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AutoReleaseTime_4);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_AutoReleaseTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AutoReleaseTime_5);
            args = new Type[]{};
            method = type.GetMethod("get_Capacity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Capacity_6);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_Capacity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Capacity_7);
            args = new Type[]{};
            method = type.GetMethod("get_ExpireTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ExpireTime_8);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_ExpireTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ExpireTime_9);


        }


        static StackObject* Unspawn_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @target = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Unspawn(@target);

            return __ret;
        }

        static StackObject* ReleaseAllUnused_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReleaseAllUnused();

            return __ret;
        }

        static StackObject* Spawn_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Spawn(@name);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Register_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @spawned = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectBase @obj = (EPloy.Game.ObjectPool.ObjectBase)typeof(EPloy.Game.ObjectPool.ObjectBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Register(@obj, @spawned);

            return __ret;
        }

        static StackObject* get_AutoReleaseTime_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AutoReleaseTime;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_AutoReleaseTime_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AutoReleaseTime = value;

            return __ret;
        }

        static StackObject* get_Capacity_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Capacity;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_Capacity_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Capacity = value;

            return __ret;
        }

        static StackObject* get_ExpireTime_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ExpireTime;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_ExpireTime_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.ObjectPool.ObjectPoolBase instance_of_this_method = (EPloy.Game.ObjectPool.ObjectPoolBase)typeof(EPloy.Game.ObjectPool.ObjectPoolBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ExpireTime = value;

            return __ret;
        }



    }
}
