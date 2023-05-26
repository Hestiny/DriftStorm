using DriftStorm;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameMaker : Editor
{
    [MenuItem("AssetBundle/Build")]
    static void BuildAssetBunle()
    {
        //Application.streamingAssetsPath
        string dir = "Assets/StreamingAssets";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //BuildTarget.StandaloneWindows64 选择构建平台
        //BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //LZMA
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android); //LZ4
        //BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64); //不压缩        
    }

    [MenuItem("工具栏/清除数据", priority = -100)]
    public static void ClearData()
    {
        DataCtrl.Instance.Clear();
    }
}
