using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ResLoader
    {
        private List<Res> mLoadedAssets = new List<Res>();

        public static List<Res> SharedLoadedAssets = new List<Res>();

        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var res = mLoadedAssets.Find(loadedAsset => loadedAsset.Name == assetName);

            if (res != null)
            {
                return res.Asset as T;
            }

            res = SharedLoadedAssets.Find(sharedLoadedRes => sharedLoadedRes.Name == assetName);

            if (res != null)
            {
                res.Retain();

                mLoadedAssets.Add(res);

                return res.Asset as T;
            }

            var asset = Resources.Load<T>(assetName);
            res = new Res(asset);
            res.Retain();
            mLoadedAssets.Add(res);
            SharedLoadedAssets.Add(res);

            return asset;
        }

        public void UnloadAll()
        {
            mLoadedAssets.ForEach(loadedAsset => { 
                loadedAsset.Release(); 
            });

            mLoadedAssets.Clear();
            mLoadedAssets = null;
        }
    }

    public class Res
    {
        public string Name
        {
            get
            {
                return Asset.name;
            }
        }

        public Res(Object asset)
        {
            Asset = asset;
        }

        public Object Asset { get; private set; }

        private int mReferenceCount = 0;

        public void Retain()
        {
            mReferenceCount++;
        }

        public void Release()
        {
            mReferenceCount--;
            if(mReferenceCount == 0)
            {
                Resources.UnloadAsset(Asset);

                ResLoader.SharedLoadedAssets.Remove(this);

                Asset = null;
            }
        }
    }
}

