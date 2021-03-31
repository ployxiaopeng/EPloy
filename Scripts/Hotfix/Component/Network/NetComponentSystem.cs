using ETModel;
using System;


namespace ETHotfix
{

    public static class NetComponentSystem
    {
        public static void HotfixStart(this NetworkComponent self)
        {
            try
            {
                //消息识别码组件
                HotfixOpcodeType(ModelGame.Scene.GetComponent<OpcodeTypeComponent>());
                //消息分发组件
                HotfixMessageDispatcher(ModelGame.Scene.GetComponent<MessageDispatcherComponent>(),
                ModelGame.Scene.GetComponent<OpcodeTypeComponent>());
            }
            catch (Exception e)
            {
                Log.Error("初始化热更网络组件时出现异常：{0}", e);
            }
        }

        private static void HotfixOpcodeType(OpcodeTypeComponent opcodeType)
        {
            foreach (Type type in GameEntry.ILRuntime.GetHotfixTypes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attrs.Length == 0) continue;
                MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                if (messageAttribute == null) continue;
                opcodeType.opcodeTypes.Add(messageAttribute.Opcode, type);
                opcodeType.typeMessages.Add(messageAttribute.Opcode, Activator.CreateInstance(type));
            }
        }

        private static void HotfixMessageDispatcher(MessageDispatcherComponent messageDispatcher, OpcodeTypeComponent opcodeType)
        {
            foreach (Type type in GameEntry.ILRuntime.GetHotfixTypes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0) continue;
                Log.Error("有的话也待定");
                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = opcodeType.GetOpcode(messageType);
                if (opcode == 0)
                {
                    Log.Error($"消息opcode为0: {messageType.Name}");
                    continue;
                }
                messageDispatcher.RegisterHandler(opcode, iMHandler);
            }
        }
    }
}