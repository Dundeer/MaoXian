﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading;

namespace QFramework
{
    public class QLog : Singleton<QLog>
    {

        /// <summary>
        /// 日志等级，为不同输出配置用
        /// </summary>
        public enum LogLevel
        {
            LOG = 0,
            WARNING,
            ASSERT,
            ERROR,
            MAX
        }

        /// <summary>
        /// 日志数据类
        /// </summary>
        public class LogData
        {
            public string Log { get; set; }
            public string Track { get; set; }
            public LogLevel Level { get; set; }
        }

        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        /// <summary>
        /// UI输出日志等级，只要大于等于这个等级的日志，就会输出到屏幕
        /// </summary>
        public LogLevel uiOutputLogLevel = LogLevel.LOG;
        /// <summary>
        /// 文本输出日志等级，只要大于等于这个等级的日志，就会输出到文本
        /// </summary>
        public LogLevel fileOutputLogLevel = LogLevel.MAX;
        /// <summary>
        /// unity日志和日志输出等级的映射
        /// </summary>
        private Dictionary<LogType, LogLevel> logTypeLevelDict = null;
        /// <summary>
        /// OnGUI回调
        /// </summary>
        public OnGUICallback onGUICallback = null;
        /// <summary>
        /// 日志输出列表
        /// </summary>
        private List<ILogOutput> logOutputList = null;
        private int mainThreadID = -1;

        /// <summary>
        /// Unity的Debug.Assert()在发布版本有问题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="info">输出信息</param>
        public static void Assert(bool condition, string info)
        {
            if (condition)
                return;
            Debug.LogError(info);
        }

        private QLog()
        {
            Application.logMessageReceived += LogCallback;
            Application.logMessageReceivedThreaded += LogMultiThreadCallback;

            this.logTypeLevelDict = new Dictionary<LogType, LogLevel>
            {
                {LogType.Log, LogLevel.LOG },
                {LogType.Warning, LogLevel.WARNING },
                {LogType.Assert, LogLevel.ASSERT },
                {LogType.Error, LogLevel.ERROR },
                {LogType.Exception, LogLevel.ERROR }
            };

            this.uiOutputLogLevel = LogLevel.LOG;
            this.fileOutputLogLevel = LogLevel.ERROR;
            this.mainThreadID = Thread.CurrentThread.ManagedThreadId;
            this.logOutputList = new List<ILogOutput>
            {

            };

            //App.Instance.onGUI += OnGUI;
            //App.Instance.onDestroy += OnDestroy;
        }

        void OnGUI()
        {
            if (this.onGUICallback != null)
                this.onGUICallback();
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= LogCallback;
            Application.logMessageReceivedThreaded -= LogMultiThreadCallback;
        }

        void LogCallback(string log, string track, LogType type)
        {
            if (this.mainThreadID == Thread.CurrentThread.ManagedThreadId)
                Output(log, track, type);
        }

        void LogMultiThreadCallback(string log, string track, LogType type)
        {
            if (this.mainThreadID != Thread.CurrentThread.ManagedThreadId)
                Output(log, track, type);
        }

        void Output(string log, string track, LogType type)
        {
            LogLevel level = this.logTypeLevelDict[type];
            LogData logData = new LogData
            {
                Log = log,
                Track = track,
                Level = level
            };
            for(int i = 0; i < this.logOutputList.Count; ++i)
            {
                this.logOutputList[i].Log(logData);
            }
        }
        
    }
}


