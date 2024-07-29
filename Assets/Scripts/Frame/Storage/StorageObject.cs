using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Storage", menuName = "Storage/Storage_clump")]
public class StorageObject : ScriptableObject
{
    public List<ResourcesInfo> rsCheck = new List<ResourcesInfo>();
}

/// <summary>
/// 客户端资源更新前会进行检查请求
/// 如果客户端的版本码和服务器的不一致则需要更新
/// </summary>
[Serializable]
public class ResourcesInfo
{
    public string relaPath = "";
    public string version_code = "";
    public bool need_updata = true;
}

