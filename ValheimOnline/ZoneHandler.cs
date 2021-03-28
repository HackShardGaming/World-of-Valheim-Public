
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace ValheimOnline
{
    // 
    // The state of the client to follow
    // upon connecting, will sync with the server and enforce the settings provided.
    // 

    // Example to add client class for RPC
    //
    //   Register the RPC call on the client
    //     This is done in the OnNewConnection Method
    //
    //     peer.m_rpc.Register<ZPackage>("ZoneHandler", new Action<ZRpc, ZPackage>(ZoneHandler.RPC) );
    //
    //   Tell the server to call the RPC
    //     This is done in the SendPeerInfo Method
    //
    //      Debug.Log("S2C ZoneHandler (SendPeerInfo)");
    //      ZoneHandler._debug();
    //      rpc.Invoke("ZoneHandler", new object[] {
    //          ZoneHandler.Serialize()
    //      });
    //

    public static class ZoneHandler
    {
        // Convert zoneType to a txt list so users can customize
        // Example:
        // none
        // safe pvp=off
        // battle pvp=on
        // heal heal.period = 5
        // chaos stamina.regain = 100
        // prevent build
        // Hide kill messages
        /*
        public enum zoneType: byte
        {
            custom = 0,
            safe = 1,
            battle = 2,
            none = 3
        }
        */

        // ZoneSettings is the configuration for the zone.
        // Try to only handle the struct and not individual since it will update A LOT

        public class ZoneSettings
        {
            //What affect does the zone provide?
            public bool pvp = false;
            //public bool enforce = false;
            //public bool viewDistance = false;
            //public bool autoheal = false;
        }

        // ./addzone Neutral Reisucks Square 50 100
        // ./addzone Neutral Reirocks Circle 50 100
        // ./addzone Neutral Rei Coords 100,100 -100,-100 100

        // Zones
        //   Name: 
        //   Priority:
        //   Range:
        //     Sphere+radius (Circle)
        //     Square+cords  (Square)
        //   Zone Parameters
        public struct Zone
        {
            public int ID;
            public string Name;
            public string Type;
            public int Priority;
            public Vector2 Position;
            public float Radius;
            public bool pvp;
            //public ZoneSettings Settings;
        }

        // Zone Details

        // Which zone are we currently in.
        // For now just the name of the zone.
        // Later 
        public static int CurrentZoneID = -1;

        // List of all the zones
        public static List<Zone> Zones = new List<Zone>();

        // Generic debug output
        // Do not change name to debug. Will break "debug()" function in class.
#if DEBUG
        public static void _debug(Zone z)
        {
            Debug.Log($"   {z.ID} ({z.Name}, {z.Type}, {z.Priority}, {z.Position}, {z.Radius})");
        }

        public static void _debug(List<Zone> z)
        {
            Debug.Log("Loaded Zone Data: ");
            Debug.Log("  Zone Cnt: " + z.Count);

            using (List<Zone>.Enumerator enumerator = z.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    _debug(enumerator.Current);
                }
            }
        }

        public static void _debug()
        {
            _debug(Zones);
        }
#endif

        //List all the zones were are currently occupy
        public static List<Zone> ListOccupiedZones(Vector3 point)
        {
            Vector2 a = new Vector2(point.x, point.z);
            List<Zone> occupiedZones = new List<Zone>();

            foreach (Zone checkZone in Zones)
            {
                if (Vector2.Distance(a, checkZone.Position) <= checkZone.Radius)
                {
                    occupiedZones.Add(checkZone);
                }
            }
            return occupiedZones;
        }

        // Output the zone that we will use in the current area.
        // Will go through and find which one has the highest priority.
        public static Zone TopZone(List<Zone> z)
        {
            // Sort the Zone list and output the one on top.
            z.Sort((Zone a, Zone b) => a.Priority.CompareTo(b.Priority));
            // Remove the first zone and go through and compare.
            /*
            Zone occupiedZones = ZoneList;
            foreach (Zone checkZone in ZoneList)
            {
                if (occupiedZones.type < checkZone.type)
                {
                    occupiedZones = checkZone;
                }
            }
            */
            return z[0];
        }

        public static bool Detect(Vector3 position, out bool changed, out Zone z)
        {
            List<Zone> zlist = ListOccupiedZones(position);
            if (zlist.Count == 0)
            {
                // No Zones occupied (We are in the wilderness)
                z = new Zone();

                // Did we change to the wilderness?
                if (CurrentZoneID != -1)
                {
                    CurrentZoneID = -1;
                    changed = true;
                }
                else
                {
                    changed = false;
                }

                return false;

            }
            else
            {
                // We are in a zone
                z = TopZone(zlist);

                if (CurrentZoneID != z.ID)
                {
                    CurrentZoneID = z.ID;
                    changed = true;
                }
                else
                {
                    changed = false;
                }

                return true;

            }
        }


        public static ZPackage Serialize()
        {
            ZPackage zip = new ZPackage();
            zip.Write(Zones.Count);
            foreach (Zone z in Zones)
            {
                zip.Write(z.ID);
                zip.Write(z.Name);
                zip.Write(z.Type);
                zip.Write(z.Priority);
                //zip.Write((int)z.type);
                zip.Write(z.Position.x);
                zip.Write(z.Position.y);
                zip.Write(z.Radius);
                zip.Write(z.pvp);
            }
            return zip;
        }

        public static void Deserialize(ZPackage package)
        {
            Zones.Clear();
            int num = package.ReadInt();
            for (int i = 0; i < num; i++)
            {
                Zones.Add(new Zone
                {
                    ID = package.ReadInt(),
                    Name = package.ReadString(),
                    Type = package.ReadString(),
                    Priority = package.ReadInt(),
                    //type = (zoneType) package.ReadInt(),
                    Position = new Vector2(package.ReadSingle(), package.ReadSingle()),
                    Radius = package.ReadSingle(),
                    pvp = package.ReadBool()
                });
            }
        }


        // RPC function class. This is the class that you register to receive rpc data.
        public static void RPC(ZRpc rpc, ZPackage data)
        {
            Debug.Log("S2C Zone (RPC Call)");
            Debug.Assert(!ZNet.instance.IsServer());
#if DEBUG
            Debug.Log("Before");
            _debug();
#endif
            Deserialize(data);
#if DEBUG
            Debug.Log("After");
            _debug();
#endif
        }


        // ValheimOnline.ServerSafeZonePath.Value
        public static void LoadZoneData(string ZonePath)
        {
            if (!File.Exists(ZonePath))
            {
                Debug.Log($"Creating zone file at {ZonePath}");
                string text = "# format: name type x z radius\nDefaultSafeZone safe 1 0.0 0.0 5.0 true";
                File.WriteAllText(ZonePath, text);
            }

            int pos = 0;
            foreach (string text2 in File.ReadAllLines(ZonePath))
            {
                if (!string.IsNullOrWhiteSpace(text2) && text2[0] != '#')
                {
                    string[] array2 = text2.Split(Array.Empty<char>());
                    if (array2.Length != 7)
                    {
                        Debug.Log($"Zone {text2} is not correctly formatted.");
                    }
                    else
                    {
                        
                        Zone z = new Zone();
                        z.Name = array2[0];
                        z.Type = array2[1];
                        z.Priority = int.Parse(array2[2]);
                        //z.type = (zoneType) Enum.Parse(typeof(zoneType), array2[1]);
                        z.Position.x = float.Parse(array2[3]);
                        z.Position.y = float.Parse(array2[4]);
                        z.Radius = float.Parse(array2[5]);
                        z.pvp = bool.Parse(array2[6]);
                        z.ID = pos;

                        Zones.Add(z);

                        pos++;
                    }
                }
            }
            _debug();
        }

    }
}
