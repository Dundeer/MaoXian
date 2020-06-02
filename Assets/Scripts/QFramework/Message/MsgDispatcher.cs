
namespace QFramework
{
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public static class MsgDispatcher
    {
        /// <summary>
        /// 消息捕捉器
        /// </summary>
        private class LogicMsgHandler
        {
            public readonly IMSReceiver Receiver;
            public readonly Action<object[]> Callback;

            public LogicMsgHandler(IMSReceiver receiver, Action<object[]> callback)
            {
                Receiver = receiver;
                Callback = callback;
            }
        }

        /// <summary>
        /// 每个消息名字未获一组消息捕捉器
        /// </summary>
        static readonly Dictionary<string, List<LogicMsgHandler>> mMsgHandlerDict = new Dictionary<string, List<LogicMsgHandler>>();

        public static void RegisterLogicMsg(this IMSReceiver self, string msgName, Action<object[]> callback)
        {
            if (string.IsNullOrEmpty(msgName))
            {
                System.Console.WriteLine("RegisterMsg:" + msgName + " is Null or Empty");
                return;
            }

            if(null == callback)
            {
                System.Console.WriteLine("RegisterMsg:" + msgName + " callback is Null");
                return;
            }

            if (!mMsgHandlerDict.ContainsKey(msgName))
            {
                mMsgHandlerDict[msgName] = new List<LogicMsgHandler>();
            }

            var handlers = mMsgHandlerDict[msgName];

            foreach(var handler in handlers)
            {
                if(handler.Receiver == self && handler.Callback == callback)
                {
                    System.Console.WriteLine("RegisterMsg:" + msgName + " ayready Register");
                    return;
                }
            }

            handlers.Add(new LogicMsgHandler(self, callback));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msgName"></param>
        /// <param name="paramList"></param>
        public static void SendLogicMsg(this IMsgSender sender, string msgName, params object[] paramList)
        {
            if (string.IsNullOrEmpty(msgName))
            {
                System.Console.WriteLine("SendMsg is Null or Empty");
                return;
            }

            if (!mMsgHandlerDict.ContainsKey(msgName))
            {
                System.Console.WriteLine("SendMsg is UnRegister");
                return;
            }

            var handlers = mMsgHandlerDict[msgName];
            var handlerCount = handlers.Count;

            for(var index = handlerCount - 1; index >= 0; index--)
            {
                var handler = handlers[index];

                if(handler.Receiver != null)
                {
                    System.Console.WriteLine("SendLogicMsg:" + msgName + " Succeed");
                    handler.Callback(paramList);
                }
                else
                {
                    handlers.Remove(handler);
                }
            }
        }

    }
}

