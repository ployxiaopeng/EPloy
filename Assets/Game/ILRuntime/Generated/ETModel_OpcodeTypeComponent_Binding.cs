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
    unsafe class ETModel_OpcodeTypeComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.OpcodeTypeComponent);
            args = new Type[]{typeof(System.Type)};
            method = type.GetMethod("GetOpcode", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetOpcode_0);

            field = type.GetField("opcodeTypes", flag);
            app.RegisterCLRFieldGetter(field, get_opcodeTypes_0);
            app.RegisterCLRFieldSetter(field, set_opcodeTypes_0);
            field = type.GetField("typeMessages", flag);
            app.RegisterCLRFieldGetter(field, get_typeMessages_1);
            app.RegisterCLRFieldSetter(field, set_typeMessages_1);


        }


        static StackObject* GetOpcode_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Type @type = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.OpcodeTypeComponent instance_of_this_method = (ETModel.OpcodeTypeComponent)typeof(ETModel.OpcodeTypeComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetOpcode(@type);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_opcodeTypes_0(ref object o)
        {
            return ((ETModel.OpcodeTypeComponent)o).opcodeTypes;
        }
        static void set_opcodeTypes_0(ref object o, object v)
        {
            ((ETModel.OpcodeTypeComponent)o).opcodeTypes = (ETModel.DoubleMap<System.UInt16, System.Type>)v;
        }
        static object get_typeMessages_1(ref object o)
        {
            return ((ETModel.OpcodeTypeComponent)o).typeMessages;
        }
        static void set_typeMessages_1(ref object o, object v)
        {
            ((ETModel.OpcodeTypeComponent)o).typeMessages = (System.Collections.Generic.Dictionary<System.UInt16, System.Object>)v;
        }


    }
}
