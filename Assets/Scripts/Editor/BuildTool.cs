using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
public class BuildTool : MonoBehaviour
{
    [MenuItem("Tools/Build Bundle/Windows")]
    static void BuildWindowsBundle()
    {
        Build(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/Build Bundle/Android")]
    static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }
    [MenuItem("Tools/Build Bundle/iOS")]
    static void BuildiOSBundle()
    {
        Build(BuildTarget.iOS);
    }
    private static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        List<string> assetBundleInfos = new List<string>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.EndsWith(".meta")) continue;
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            string fileName = PathUtil.GetStandardPath(file);
            string assetName = PathUtil.GetUnityPath(fileName);
            Debug.Log("file" + fileName);
            assetBundle.assetNames = new string[] { assetName };
            string bundleName = file.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";
            assetBundleBuilds.Add(assetBundle);
            //添加文件依赖信息
            //Asset name|bundle name|dependence files
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = string.Format("{0}|{1}", assetName, assetBundle.assetBundleName);
            if(dependenceInfo.Count > 0)
            {
                bundleInfo = string.Format("{0}|{1}",bundleInfo,string.Join("|", dependenceInfo));
            }
            assetBundleInfos.Add(bundleInfo);
        }

        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + APPConst.FILE_LIST_OUTPUT_NAME, assetBundleInfos);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
        AssetDatabase.Refresh();
    }
    public static List<string> GetDependence(string path)
    {
        List<string> result = new List<string>();
        string[] files = AssetDatabase.GetDependencies(path);
        result = files.Where(file => !file.EndsWith(".cs") && !file.Equals(path)).ToList();
        return result;
    }
}
