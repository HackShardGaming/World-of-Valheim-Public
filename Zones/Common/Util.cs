using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace WorldofValheimZones
{

    public static class Util
    {
        public static List<Util.ConnectionData> Connections = new List<Util.ConnectionData>();

        public class ConnectionData
        {

            public ZRpc rpc;
        }
        
        public static bool isServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }

        public static void Broadcast(string text, string username = "server")
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        
        
        public static Util.ConnectionData GetServer()
        {
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Assert(Util.Connections.Count == 1);
            return Util.Connections[0];
        }
        

    }
}
