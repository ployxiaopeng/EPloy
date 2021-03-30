using System;
using GameFramework;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ETModel
{
    public class IDataRowBaseAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(DataRowBase);
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

        public class Adaptor : DataRowBase, CrossBindingAdaptorType
        {
            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appDomain;

            private IMethod ParseDataRowMethod;

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

            public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
            {
                if (this.ParseDataRowMethod == null)
                {
                    this.ParseDataRowMethod = instance.Type.GetMethod("ParseDataRow");
                }
                this.appDomain.Invoke(this.ParseDataRowMethod, instance, dataRowSegment);
                return true;
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
