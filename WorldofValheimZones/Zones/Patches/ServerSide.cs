using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace WorldofValheimZones
{
    /*
    public class ServerSide
    {
        public class ServerSideAreaInfo
        {
            public Vector3 pos;
            public bool square;
            public float range;
            public string configs;
            public string data;
            internal ServerSideAreaInfo(Vector3 _pos, bool _square, float _range, string _configs, string _data)
            {
                pos = _pos;
                square = _square;
                range = _range;
                configs = _configs;
                data = _data;
            }
        }
    }
    public class AreaInfo
    {
        public Vector3 pos;
        public bool square;
        public float range;
        public string configs;
        internal AreaInfo(Vector3 _pos, bool _square, float _range, string _configs)
        {
            pos = _pos;
            square = _square;
            range = _range;
            configs = _configs;
        }
    }
    static void WriteAreaData(List<string> list)
    {
        ServerSide.Clear();
        for (int i = 0; i< list.Count; i++)
        {
            if (list[i] != "" && list[i] != null && !list[i].StartsWith("/") && !list[i].StartsWith("#") && list[i] != string.Empty)
            {
                string[] array = list[i].Replace(" ", "").Split('|');
                Vector3 Vec = new Vector3(x: float.Parse(array[0]), y: float.Parse(array[1], ZBroastcast: float.Parse(array[2])));
                float range = float.Parse(array[3]);
                bool square = array[4].ToLower() == "square" ? true : false;
                string data = array[5];
                string configs = array[6];
                ServerSide.Add(new ServerSideAreaInfo(Vec, square, range, configs, data));
            }
        }
    }
    */
}
