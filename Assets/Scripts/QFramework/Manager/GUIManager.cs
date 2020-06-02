using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public enum UILayer
    {
        Bg,
        Common,
        Top
    }

    public class GUIManager : MonoBehaviour
    {
        /// <summary>
        /// 界面存储
        /// </summary>
        private static Dictionary<string, GameObject> Panel = new Dictionary<string, GameObject>();

        private static Dictionary<string, GameObject> PanelPool = new Dictionary<string, GameObject>();

        /// <summary>
        /// 层级控制器
        /// </summary>
        private static GameObject mPrivateUIRoot;

        private static Transform BgObject;

        private static Transform CommonObject;

        private static Transform TopObject;

        public static GameObject UIRoot
        {
            get
            {
                if (mPrivateUIRoot == null)
                {
                    var uirootPrefab = Resources.Load<GameObject>("UIRoot");
                    // Resources.UnloadAsset 无法卸载Prefab 只支持独立的资源 不能用在GameObject、Component、AssetBundle资源的卸载上
                    // Resources.UnloadUnusedAssets 只能卸载没有引用的资源 无法进行精确的对某个资源进行卸载 从工作机制来说 他会出发 GC.Collect 的调用 会造成游戏的卡顿
                    mPrivateUIRoot = GameObject.Instantiate(uirootPrefab);
                    DontDestroyOnLoad(mPrivateUIRoot);
                    mPrivateUIRoot.name = "UIRoot";
                    BgObject = mPrivateUIRoot.transform.GetChild(0);
                    CommonObject = mPrivateUIRoot.transform.GetChild(1);
                    TopObject = mPrivateUIRoot.transform.GetChild(2);
                }

                return mPrivateUIRoot;
            }
        }

        /// <summary>
        /// 设置UI适配
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="matchWidthOrHeight">宽高比</param>
        public static void SetResolution(float width, float height, float matchWidthOrHeight)
        {
            var canvasScaler = UIRoot.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(width, height);
            canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        }

        /// <summary>
        /// 生成UI界面
        /// </summary>
        /// <param name="panelName">界面名称</param>
        public static void LoadPanel(string panelName, UILayer layer)
        {
            //获取到指定对象
            GameObject panel;
            if (Panel.ContainsKey(panelName)) return;
            if (PanelPool.ContainsKey(panelName))
            {
                panel = Instantiate(PanelPool[panelName]);
                panel.name = panelName;
            }
            else
            {
                //生成设置父类
                var panelPrefab = Resources.Load<GameObject>(panelName);
                panel = Instantiate(panelPrefab);
                panel.name = panelName;

                PanelPool.Add(panelName, panelPrefab);
            }

            Panel.Add(panelName, panel);
            Transform targetLayer = null;
            //设置对象的层级
            switch (layer)
            {
                case UILayer.Bg:
                    targetLayer = BgObject;
                    break;
                case UILayer.Common:
                    targetLayer = CommonObject;
                    break;
                case UILayer.Top:
                    targetLayer = TopObject;
                    break;
            }

            if (!targetLayer) return;

            if (targetLayer.childCount > 0)
            {
                UnLoadPanel(targetLayer.GetChild(0).name);
            }
            panel.gameObject.transform.SetParent(targetLayer);

            //设置对象的位置
            var panelRectTrans = panel.transform as RectTransform;

            panelRectTrans.offsetMin = Vector2.zero;
            panelRectTrans.offsetMax = Vector2.zero;
            panelRectTrans.anchoredPosition3D = Vector3.zero;
            panel.transform.localScale = new Vector3(1, 1, 1);
            //panelRectTrans.anchorMin = Vector2.zero;
            //panelRectTrans.anchorMax = Vector2.zero;
        }

        /// <summary>
        /// 移除界面
        /// </summary>
        /// <param name="panelName">界面名称</param>
        public static void UnLoadPanel(string panelName)
        {
            if (Panel.ContainsKey(panelName))
            {
                Destroy(Panel[panelName]);
                Panel.Remove(panelName);
            }
        }
    }
}

