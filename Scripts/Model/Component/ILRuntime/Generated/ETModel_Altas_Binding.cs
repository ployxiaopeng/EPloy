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
    unsafe class ETModel_Altas_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.Altas);

            field = type.GetField("Sprites", flag);
            app.RegisterCLRFieldGetter(field, get_Sprites_0);
            app.RegisterCLRFieldSetter(field, set_Sprites_0);


        }



        static object get_Sprites_0(ref object o)
        {
            return ((ETModel.Altas)o).Sprites;
        }
        static void set_Sprites_0(ref object o, object v)
        {
            ((ETModel.Altas)o).Sprites = (System.Collections.Generic.List<UnityEngine.Sprite>)v;
        }


    }
}
