using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QFramework
{
    /// <summary>
    /// 控制才GUI输出类
    /// 包括FPS， 内存使用情况，日志GUI输出
    /// </summary>
    public class QConsole : Singleton<QConsole>
    {
        
        struct ConsoleMessage
        {
            public readonly string message;
            public readonly string stackTrace;
            public readonly LogType type;

            public ConsoleMessage(string message, string stackTrace, LogType type)
            {
                this.message = message;
                this.stackTrace = stackTrace;
                this.type = type;
            }

        }

        /// <summary>
        /// Update回调
        /// </summary>
        public delegate void OnUpdateCallback();

        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        public OnUpdateCallback onUpdateCallback = null;
        public OnGUICallback onGuiCallback = null;

        /// <summary>
        /// FPS计算器
        /// </summary>
        //private QFPSCounter fpsCounter = null;
        /// <summary>
        /// 内存监视器
        /// </summary>
        //private QMemoryDetector memoryDetector = null;

        private bool showGUI = true;
        List<ConsoleMessage> entries = new List<ConsoleMessage>();
        Vector2 scrollPos;
        bool scrollToBottom = true;
        bool collapse;
        bool mTouching = false;

        const int margin = 20;
        Rect windowRect = new Rect(margin + Screen.width * 0.5f, margin, Screen.width * 0.5f - (2 * margin), Screen.height - (2 * margin));

        GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console");
        GUIContent collapseLabel = new GUIContent("Callapse", "Hide repeated messages.");
        GUIContent scrollToBottomLabel = new GUIContent("ScrollToBottom", "Scroll bar always at bottom");

        private QConsole()
        {
            //this.fpsCounter = new QFPSCounter(this);
            //this.memoryDetector = new QMemoryDetector(this);


        }
    }
}


