using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.Tilemaps;

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

    [MenuItem("Tools/ExportSprite")]
    private static void ExportSprite()
    {
        string resourcesPath = Path.Combine(PathUtil.AssetsPath, "Resources/Sprite");
        var files = Directory.GetFiles(resourcesPath);
        foreach (var path in files)
        {
            if (path.EndsWith(".meta")) continue;
            string tarstr = path.Substring(path.LastIndexOf("Sprite"));
            tarstr = tarstr.Remove(tarstr.IndexOf('.'));
            Sprite[] spriteList = Resources.LoadAll<Sprite>(tarstr);
            if (spriteList.Length > 0)
            {
                string outPath = Application.dataPath + "/BuildResources/Sprite";
                foreach (var sprite in spriteList)
                {
                    Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
                    tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin, (int)sprite.rect.width, (int)sprite.rect.height));
                    tex.Apply();
                    System.IO.File.WriteAllBytes(outPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                    Debug.Log("SaveSprite to" + outPath);
                }
            }
        }
    }

    [MenuItem("Tools/ExportCurStageData")]
    private static void ExportStageData()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/LevelEditor.unity", UnityEditor.SceneManagement.OpenSceneMode.Single);
        MT.Editor.MTEditor.instance.ExportConfig();
        EditorUtility.DisplayDialog("", "当前楼层数据导出完成!", "确定");
    }
}
