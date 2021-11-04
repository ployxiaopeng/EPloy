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
    unsafe class EPloy_Altas_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EPloy.Altas);

            field = type.GetField("Sprites", flag);
            app.RegisterCLRFieldGetter(field, get_Sprites_0);
            app.RegisterCLRFieldSetter(field, set_Sprites_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Sprites_0, AssignFromStack_Sprites_0);


        }



        static object get_Sprites_0(ref object o)
        {
            return ((EPloy.Altas)o).Sprites;
        }

        static StackObject* CopyToStack_Sprites_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EPloy.Altas)o).Sprites;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Sprites_0(ref object o, object v)
        {
            ((EPloy.Altas)o).Sprites = (System.Collections.Generic.List<UnityEngine.Sprite>)v;
        }

        static StackObject* AssignFromStack_Sprites_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Sprite> @Sprites = (System.Collections.Generic.List<UnityEngine.Sprite>)typeof(System.Collections.Generic.List<UnityEngine.Sprite>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EPloy.Altas)o).Sprites = @Sprites;
            return ptr_of_this_method;
        }



    }
}
