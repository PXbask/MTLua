using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    None,
    EditorMode,
    PackageBundle,
    UpdateMode,
}
public enum GameEvent
{
    GameInit = 10000,
    StartLua, 
}
public class APPConst
{
    public const string ASSET_BUNDLE_EXTENSION = ".ab";
    public const string FILE_LIST_OUTPUT_NAME = "fileList.txt";
    public const string ReourcesURL = "http://localhost/AssetBundles";
    public static GameMode GameMode = GameMode.EditorMode;

    public static bool OpenLog = true;
}
