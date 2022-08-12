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
    unsafe class EPloy_Game_Net_NetChannel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EPloy.Game.Net.NetChannel);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_NetConnected", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NetConnected_0);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_NetClosed", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NetClosed_1);
            args = new Type[]{typeof(System.Action<System.Int32>)};
            method = type.GetMethod("set_NetMissHeartBeat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NetMissHeartBeat_2);
            args = new Type[]{typeof(System.Action<EPloy.Game.Net.NetErrorCode, System.Net.Sockets.SocketError, System.String>)};
            method = type.GetMethod("set_NetError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NetError_3);
            args = new Type[]{typeof(System.Action<EPloy.Game.Net.Packet>)};
            method = type.GetMethod("RegisterHandler", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterHandler_4);
            args = new Type[]{typeof(System.Func<EPloy.Game.Net.Packet>)};
            method = type.GetMethod("RegisterHeartBeat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterHeartBeat_5);
            args = new Type[]{};
            method = type.GetMethod("Dispose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Dispose_6);
            args = new Type[]{typeof(System.Net.IPAddress), typeof(System.Int32)};
            method = type.GetMethod("Connect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Connect_7);
            args = new Type[]{typeof(EPloy.Game.Net.Packet)};
            method = type.GetMethod("Send", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Send_8);


        }


        static StackObject* set_NetConnected_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetConnected = value;

            return __ret;
        }

        static StackObject* set_NetClosed_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetClosed = value;

            return __ret;
        }

        static StackObject* set_NetMissHeartBeat_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Int32> @value = (System.Action<System.Int32>)typeof(System.Action<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetMissHeartBeat = value;

            return __ret;
        }

        static StackObject* set_NetError_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<EPloy.Game.Net.NetErrorCode, System.Net.Sockets.SocketError, System.String> @value = (System.Action<EPloy.Game.Net.NetErrorCode, System.Net.Sockets.SocketError, System.String>)typeof(System.Action<EPloy.Game.Net.NetErrorCode, System.Net.Sockets.SocketError, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetError = value;

            return __ret;
        }

        static StackObject* RegisterHandler_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<EPloy.Game.Net.Packet> @handler = (System.Action<EPloy.Game.Net.Packet>)typeof(System.Action<EPloy.Game.Net.Packet>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterHandler(@handler);

            return __ret;
        }

        static StackObject* RegisterHeartBeat_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Func<EPloy.Game.Net.Packet> @handler = (System.Func<EPloy.Game.Net.Packet>)typeof(System.Func<EPloy.Game.Net.Packet>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterHeartBeat(@handler);

            return __ret;
        }

        static StackObject* Dispose_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Dispose();

            return __ret;
        }

        static StackObject* Connect_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @port = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Net.IPAddress @ipAddress = (System.Net.IPAddress)typeof(System.Net.IPAddress).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Connect(@ipAddress, @port);

            return __ret;
        }

        static StackObject* Send_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EPloy.Game.Net.Packet @packet = (EPloy.Game.Net.Packet)typeof(EPloy.Game.Net.Packet).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EPloy.Game.Net.NetChannel instance_of_this_method = (EPloy.Game.Net.NetChannel)typeof(EPloy.Game.Net.NetChannel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Send(@packet);

            return __ret;
        }



    }
}
