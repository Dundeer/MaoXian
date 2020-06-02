using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum EnvironmentMode
    {
        /// <summary>
        /// 开发
        /// </summary>
        Developing,
        /// <summary>
        /// 测试
        /// </summary>
        Test,
        /// <summary>
        /// 发布
        /// </summary>
        Production
    }

    /// <summary>
    /// 手机发布平台
    /// </summary>
    public enum PhonePlatfrom
    {
        /// <summary>
        /// 华为
        /// </summary>
        Huwei,
        /// <summary>
        /// oppo
        /// </summary>
        Oppo
    }

    /// <summary>
    /// 包类型
    /// </summary>
    public enum PackageType
    {
        Black,
        White
    }

    public abstract class MainManager : MonoBehaviour
    {

        public EnvironmentMode Mode;
        public PhonePlatfrom phonePlatform;
        public PackageType packageType;

        public static EnvironmentMode mShareMode;
        public static PhonePlatfrom SharePhonePlatform;
        public static PackageType SharePackageType;
        public static bool mModeSetted = false;


        // Start is called before the first frame update
        void Start()
        {

            if (!mModeSetted)
            {
                mShareMode = Mode;
                SharePhonePlatform = phonePlatform;
                SharePackageType = packageType;
                mModeSetted = true;
            }

            switch (Mode)
            {
                case EnvironmentMode.Developing:
                    LaunchInDevelopingMode();
                    break;
                case EnvironmentMode.Test:
                    LaunchInTestMode();
                    break;
                case EnvironmentMode.Production:
                    LaunchInProductionMode();
                    break;
            }
        }


        protected abstract void LaunchInDevelopingMode();

        protected abstract void LaunchInProductionMode();

        protected abstract void LaunchInTestMode();

    }
}

