using System;
using GameFramework.Fsm;
using GameFramework.Procedure;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ETModel
{
    public class IProcedureBaseAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ProcedureBase);
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

        public class Adaptor : ProcedureBase, CrossBindingAdaptorType
        {
            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appDomain;

            private IMethod OnEnterMethod;
            private IMethod OnInitMethod;
            private IMethod OnLeaveMethod;
            private IMethod OnUpdateMethod;

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

            protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
            {
                if (this.OnInitMethod == null)
                {
                    this.OnInitMethod = instance.Type.GetMethod("OnInit");
                }
                this.appDomain.Invoke(this.OnInitMethod, instance, procedureOwner);
            }
            protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
            {
                if (this.OnEnterMethod == null)
                {
                    this.OnEnterMethod = instance.Type.GetMethod("OnEnter");
                }
                this.appDomain.Invoke(this.OnEnterMethod, instance, procedureOwner);
            }
            protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
            {
                if (this.OnLeaveMethod == null)
                {
                    this.OnLeaveMethod = instance.Type.GetMethod("OnLeave");
                }
                this.appDomain.Invoke(this.OnLeaveMethod, instance, procedureOwner, isShutdown);
            }
            protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
            {
                if (this.OnUpdateMethod == null)
                {
                    this.OnUpdateMethod = instance.Type.GetMethod("OnUpdate");
                }
                this.appDomain.Invoke(this.OnUpdateMethod, instance, procedureOwner, elapseSeconds, realElapseSeconds);
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
