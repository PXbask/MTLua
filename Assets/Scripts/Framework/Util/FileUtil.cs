using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileUtil
{
    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string path)
    {
        FileInfo fileInfo= new FileInfo(path);
        return fileInfo.Exists;
    }
    public static void WriteFile(string path, byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        //文件夹的路径
        string dir = path.Substring(0, path.LastIndexOf('/')); 
        if(!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        FileInfo file = new FileInfo(path);
        if(file.Exists)
        {
            file.Delete(); 
        }
        try
        {
            using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);  
        }
    }
}
