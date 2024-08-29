
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class FileHelper
{
    /// <summary>
    /// 获取文件在中指定扩展名的文件信息
    /// </summary>
    /// <param name="path"></param>
    public static FileInfo[] GetDirectoryFileInfo(string path, string extension)
    {
        FileInfo[] filesInfo = new FileInfo[] {};
        if (Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);
            filesInfo = info.GetFiles("*." + extension, SearchOption.AllDirectories).ToArray();
        }
        return filesInfo;
    }
}

/// <summary>
/// 更新包
/// </summary>
public class UpdatePackage
{
    public string relativePath;
    public List<ResourcesInfo> filesInfo = new List<ResourcesInfo>();
}