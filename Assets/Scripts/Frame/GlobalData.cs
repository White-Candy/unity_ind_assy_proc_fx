using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sugar
{
    public static class GlobalData
    {
        public static string IP = "127.0.0.1";
        public static string Port = "8096";
        public static string Url = $"http://{IP}:{Port}";

        public static void SetUrl(string _ip, string _port)
        {
            Url = $"http://{_ip}:{_port}";
        }
    }
}
