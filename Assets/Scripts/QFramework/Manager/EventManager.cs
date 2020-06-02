using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QFramework
{
    public static class EventManager
    {

        public class MsgRecord
        {
            public string Name;
            public Action<object> OnMsgReceived;
        }

        /// <summary>
        /// 消息储存
        /// </summary>
        public static Dictionary<string, List<Delegate>> RegisteredMsgs = new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// 消息注册方法
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void Register<T>(string msgName, Action<T> onMsgReceived)
        {

            List<Delegate> actions = null;

            if (RegisteredMsgs.ContainsKey(msgName))
            {
                RegisteredMsgs[msgName].Add(onMsgReceived);
            }
            else
            {

                actions = new List<Delegate>();

                actions.Add(onMsgReceived);
                RegisteredMsgs.Add(msgName, actions);
            }
        }

        /// <summary>
        /// 消息注册方法
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void Register(string msgName, Action onMsgReceived)
        {

            List<Delegate> actions = null;

            if (RegisteredMsgs.ContainsKey(msgName))
            {
                RegisteredMsgs[msgName].Add(onMsgReceived);
            }
            else
            {

                actions = new List<Delegate>();

                actions.Add(onMsgReceived);
                RegisteredMsgs.Add(msgName, actions);
            }
        }

        /// <summary>
        /// 注销某个方法
        /// </summary>
        /// <param name="msgName">注销的键名</param>
        public static void UnRegister<T>(string msgName, Action<T> onMsgReceived)
        {
            List<Delegate> actions = null;

            if (RegisteredMsgs.TryGetValue(msgName, out actions))
            {
                actions.Remove(onMsgReceived);
                if(actions.Count == 0)
                {
                    RegisteredMsgs.Remove(msgName);
                }
            }
        }

        /// <summary>
        /// 注销某个方法
        /// </summary>
        /// <param name="msgName">注销的键名</param>
        public static void UnRegister(string msgName, Action onMsgReceived)
        {
            List<Delegate> actions = null;

            if (RegisteredMsgs.TryGetValue(msgName, out actions))
            {
                actions.Remove(onMsgReceived);
                if (actions.Count == 0)
                {
                    RegisteredMsgs.Remove(msgName);
                }
            }
        }

        /// <summary>
        /// 解除所有方法
        /// </summary>
        public static void AllUnRegister()
        {
            RegisteredMsgs.Clear();
        }

        /// <summary>
        /// 有参的发送方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public static void Send<T>(string msgName, T data)
        {
            if (RegisteredMsgs.ContainsKey(msgName))
            {
                List<Delegate> actions = null;
                RegisteredMsgs.TryGetValue(msgName, out actions);

                foreach(var act in actions)
                {
                    act.DynamicInvoke(data);
                }
            }
        }

        /// <summary>
        /// 无参的发送方法
        /// </summary>
        /// <param name="msgName"></param>
        public static void Send(string msgName)
        {
            if (RegisteredMsgs.ContainsKey(msgName))
            {
                List<Delegate> actions = null;
                RegisteredMsgs.TryGetValue(msgName, out actions);

                foreach (var act in actions)
                {
                    act.DynamicInvoke();
                }
            }
        }

    }
}


