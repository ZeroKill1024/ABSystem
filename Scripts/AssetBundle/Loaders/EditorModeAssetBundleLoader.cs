﻿#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;
using Uzen.AB;

/// <summary>
/// 编辑器模式下用的加载器
/// </summary>
public class EditorModeAssetBundleLoader : AssetBundleLoader
{
    class ABInfo : AssetBundleInfo
    {
        public override Object mainObject
        {
            get
            {
                string newPath = AssetBundlePathResolver.instance.GetEditorModePath(bundleName);
                Object mainObject = AssetDatabase.LoadMainAssetAtPath(newPath);
                return mainObject;
            }
        }
    }

    public override void Load()
    {
        if (bundleInfo == null)
        {
            this.state = LoadState.State_Complete;
            this.bundleInfo = bundleManager.CreateBundleInfo(this, new ABInfo());
            this.bundleInfo.isReady = true;
            this.bundleInfo.onUnloaded = OnBundleUnload;
        }

        bundleManager.StartCoroutine(this.LoadResource());
    }

    private void OnBundleUnload(AssetBundleInfo abi)
    {
        this.bundleInfo = null;
        this.state = LoadState.State_None;
    }

    IEnumerator LoadResource()
    {
        yield return new WaitForEndOfFrame();
        this.Complete();
    }
}
#endif