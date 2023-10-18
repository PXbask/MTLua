using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static HotUpdate;

public class HotUpdate : MonoBehaviour
{
    byte[] m_readPathFileListData;
    byte[] m_serverFileListData;
    public bool IsFirstInstall
    {
        get
        {
            //判断只读目录是否存在版本文件
            bool existInReadable = File.Exists(Path.Combine(PathUtil.ReadablePath,APPConst.FILE_LIST_OUTPUT_NAME));
            //判断可读写目录是否存在版本文件
            bool existInRaedWritable = File.Exists(Path.Combine(PathUtil.ReadWritablePath, APPConst.FILE_LIST_OUTPUT_NAME));
            return existInReadable && !existInRaedWritable;
        }
    }

    public class DownLoadFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }
    //下载文件数量
    int downLoadCount;
    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerator DownLoadFile(DownLoadFileInfo info,Action<DownLoadFileInfo> complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if(webRequest.result==UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogErrorFormat("Error found when download:{0}", info.url);
            yield break;
            //TODO:重试
        }
        yield return new WaitForSeconds(0.2f);
        info.fileData = webRequest.downloadHandler;
        complete?.Invoke(info);
        webRequest.Dispose();
    }
    /// <summary>
    /// 下载多个文件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="complete"></param>
    /// <returns></returns>
    public IEnumerator DownLoadFile(List<DownLoadFileInfo> infos, Action<DownLoadFileInfo> complete,Action complete_allFiles)
    {
        foreach (var info in infos)
        {
            yield return DownLoadFile(info, complete);
        }
        complete_allFiles?.Invoke();
    }
    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="fileData"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private List<DownLoadFileInfo> GetFileList(string fileData,string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownLoadFileInfo> downLoadFiles= new List<DownLoadFileInfo>(files.Length);
        foreach (string file in files)
        {
            string[] info = file.Split('|');
            DownLoadFileInfo downLoadFileInfo = new DownLoadFileInfo();
            downLoadFileInfo.fileName= info[1];
            downLoadFileInfo.url = Path.Combine(path, info[1]);
            downLoadFiles.Add(downLoadFileInfo);
        }
        return downLoadFiles;
    }

    GameObject loadingObj;
    LoadingUI loadingUI;
    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("LoadingUI");
        loadingObj = Instantiate(go);
        loadingObj.transform.SetParent(this.transform);
        loadingUI = loadingObj.GetComponent<LoadingUI>();
        if (IsFirstInstall)
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate(); 
        }
    }

    private void ReleaseResources()
    {
        downLoadCount = 0;
        string url = Path.Combine(PathUtil.ReadablePath, APPConst.FILE_LIST_OUTPUT_NAME);
        DownLoadFileInfo fileInfo = new DownLoadFileInfo();
        fileInfo.url = url;
        fileInfo.fileName = APPConst.FILE_LIST_OUTPUT_NAME;
        StartCoroutine(DownLoadFile(fileInfo, OnDownLoadReadableFileListComplete));
    }

    private void OnDownLoadReadableFileListComplete(DownLoadFileInfo obj)
    {
        m_readPathFileListData = obj.fileData.data;
        List<DownLoadFileInfo> downLoadFileInfos = GetFileList(obj.fileData.text, PathUtil.ReadablePath);
        StartCoroutine(DownLoadFile(downLoadFileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        loadingUI.InitProgress(downLoadFileInfos.Count, "正在释放资源，不消耗流量...");
    }
    private void OnReleaseFileComplete(DownLoadFileInfo obj)
    {
        Debug.LogFormat("release file:{0}", obj.url);
        string writeFile = Path.Combine(PathUtil.ReadWritablePath,obj.fileName);
        FileUtil.WriteFile(writeFile, obj.fileData.data);
        downLoadCount++;
        loadingUI.UpdateProgress(downLoadCount);
    }
    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritablePath,APPConst.FILE_LIST_OUTPUT_NAME), m_readPathFileListData);
        CheckUpdate();
    }
    private void CheckUpdate()
    {
        string url = Path.Combine(APPConst.ReourcesURL, APPConst.FILE_LIST_OUTPUT_NAME);
        DownLoadFileInfo fileInfo= new DownLoadFileInfo();
        fileInfo.url = url;
        fileInfo.fileName = APPConst.FILE_LIST_OUTPUT_NAME;
        StartCoroutine(DownLoadFile(fileInfo, OnDownLoadServerFileListComplete));
    }
    private void OnDownLoadServerFileListComplete(DownLoadFileInfo obj)
    {
        downLoadCount = 0;
        m_serverFileListData = obj.fileData.data;
        List<DownLoadFileInfo> loadFileInfos = GetFileList(obj.fileData.text, APPConst.ReourcesURL);
        List<DownLoadFileInfo> needDownLoadFiles = new List<DownLoadFileInfo>();
        foreach (var info in loadFileInfos)
        {
            string localFile = Path.Combine(PathUtil.ReadWritablePath, info.fileName);
            if (!File.Exists(localFile))
            {
                info.url = Path.Combine(APPConst.ReourcesURL, info.fileName);
                needDownLoadFiles.Add(info);
            }
        }
        if (needDownLoadFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(needDownLoadFiles, OnUpdateFileComplete, OnUpdateAllFileComplete));
            loadingUI.InitProgress(needDownLoadFiles.Count, "正在更新...");
        }
        else
        {
            GameEnter();
        }
    }
    private void OnUpdateFileComplete(DownLoadFileInfo obj)
    {
        Debug.LogFormat("update file:{0}", obj.url);
        string writeFile = Path.Combine(PathUtil.ReadWritablePath, obj.fileName);
        FileUtil.WriteFile(writeFile, obj.fileData.data); 
        downLoadCount++;
        loadingUI.UpdateProgress(downLoadCount);
    }
    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritablePath, APPConst.FILE_LIST_OUTPUT_NAME), m_serverFileListData);
        GameEnter();
        loadingUI.InitProgress(0, "正在载入...");
    }
    private void GameEnter()
    {
        Manager.Event.Fire((int)GameEvent.GameInit);
        Destroy(loadingObj);
    }
    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject gameObject = obj as GameObject;
        Instantiate(gameObject, transform.position, Quaternion.identity, this.transform);
        gameObject.SetActive(true);
    }
}
