using System;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ETModel
{
    public class IGameEventArgsAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(GameEventArgs);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adaptor);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor : GameEventArgs, CrossBindingAdaptorType
        {
            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appDomain;

            private IMethod OnClear;

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance)
            {
                this.appDomain = appDomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get
                {
                    return instance;
                }
            }

            public override int Id => throw new NotImplementedException();

            public override void Clear()
            {
                if(OnClear==null) OnClear= this.OnClear = instance.Type.GetMethod("OnInit");
                this.appDomain.Invoke(this.OnClear, instance);
            }

            public override string ToString()
            {
                IMethod m = this.appDomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                return instance.Type.FullName;
            }
        }
    }
}
