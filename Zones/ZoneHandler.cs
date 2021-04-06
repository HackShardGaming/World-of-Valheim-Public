
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace WorldofValheimZones
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

        public class ZoneTypes
        {
            //What affect does the zone provide?
            public string Name = "Unknown";

            // PVP settings
            public bool PVP = false;
            public bool PVPEnforce = false;

            // Show position settings
            public bool ShowPosition = true;
            public bool PositionEnforce = false;

            // Other features in the future.
            public int ViewDistance = 30;
            public bool AutoHeal = false;
        }

        // ./addzone Neutral SquareZone Square 50 100
        // ./addzone Neutral CircleZone Circle 50 100
        // ./addzone Neutral CoordsZone Coords 100,100 -100,-100 100

        // Zones
        //   Name: 
        //   Priority:
        //   Range:
        //     Sphere+radius (Circle)
        //     Square+cords  (Square)
        //     Coords+FourCorners  (Coords) <- Not Implemented
        //   Height Range <- Not Implemented
        //   Zone Parameters
        public struct Zone
        {
            public int ID; // Use for id to maintain current zone
            public string Name;
            public string Type;
            public int Priority;
            public string Shape;//0 - circle, 1 - square, 2 - coords
            public Vector2 Position;
            public float Radius;
        }

        // Zone Details

        // Which zone are we currently in.
        // For now just the name of the zone.
        // Later 
        public static int CurrentZoneID = -2; // -2 initial, -1 wilderness, 0 up are zones

        // List of all the zones
        public static List<Zone> Zones = new List<Zone>();

        public static List<ZoneTypes> ZoneT = new List<ZoneTypes>();



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
        public static void _debug(ZoneTypes zt)
        {
            Debug.Log($"  Type: {zt.Name} -> [ {zt.PVP}, {zt.PVPEnforce}, {zt.ShowPosition}, {zt.PositionEnforce}, {zt.ViewDistance}, {zt.AutoHeal} ]");
        }

        public static void _debug(List<ZoneTypes> zt)
        {
            Debug.Log("Loaded Zone Type Data: ");
            Debug.Log("  Zone Type Cnt: " + zt.Count);

            using (List<ZoneTypes>.Enumerator enumerator = zt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    _debug(enumerator.Current);
                }
            }
        }

        public static void _debug()
        {
            _debug(ZoneT);
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
                switch (checkZone.Shape)
                {
                    case "square":
                        // Square check if you are in the boundaries
                        float boundary = checkZone.Radius;// / 2;
                        if (((checkZone.Position.x + boundary) > a.x) &&
                            ((checkZone.Position.x - boundary) < a.x) &&
                            ((checkZone.Position.y + boundary) > a.y) &&
                            ((checkZone.Position.y - boundary) < a.y))
                        {
                            occupiedZones.Add(checkZone);
                        }
                        break;
                    /*
                    case 2:
                        // Coords checks
                        break;
                    */
                    default:
                        //Default: We are a circle.
                        if (Vector2.Distance(a, checkZone.Position) <= checkZone.Radius)
                        {
                            occupiedZones.Add(checkZone);
                        }
                        break;
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
            return z[0];
        }

        public static ZoneTypes FindZoneType(string ztType)
        {
            //Debug.Log($"Searching for: {ztName}");
            return ZoneT.Find(a => a.Name == ztType) ?? new ZoneTypes();
            /*
            //ZoneTypes zt = ZoneT.Find(a => a.Name == ztName) ?? new ZoneTypes();
            //zt = ZoneT.Where(a => a.Name.Contains(ztName));

            foreach (ZoneTypes zt in ZoneT)
            {
                if (zt.Name.ToLower() == ztType)
                {
                    Debug.Log($"Found Zone: {ztType}");
                    _debug(zt);
                    return zt;
                }
            }

            return new ZoneTypes();
            */
        }

        public static bool Detect(Vector3 position, out bool changed, out Zone z, out ZoneTypes zt)
        {
            List<Zone> zlist = ListOccupiedZones(position);
            if (zlist.Count == 0)
            {
                // No Zones occupied (We are in the wilderness)
                z = new Zone();
                zt = new ZoneTypes();

                // Did we change to the wilderness?
                if (CurrentZoneID != -1)
                {
                    CurrentZoneID = -1;
                    zt = FindZoneType("wilderness");
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
                zt = new ZoneTypes();

                if (CurrentZoneID != z.ID)
                {
                    zt = FindZoneType(z.Type);//.ToLower());
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
            zip.Write(ZoneT.Count);
            foreach (ZoneTypes zt in ZoneT)
            {
                zip.Write(zt.Name);
                zip.Write(zt.PVP);
                zip.Write(zt.PVPEnforce);
                zip.Write(zt.ShowPosition);
                zip.Write(zt.PositionEnforce);
                zip.Write(zt.ViewDistance);
                zip.Write(zt.AutoHeal);
            }
            zip.Write(Zones.Count);
            foreach (Zone z in Zones)
            {
                zip.Write(z.ID);
                zip.Write(z.Name);
                zip.Write(z.Type);
                zip.Write(z.Priority);
                zip.Write(z.Shape);
                //zip.Write((int)z.type);
                zip.Write(z.Position.x);
                zip.Write(z.Position.y);
                zip.Write(z.Radius);
                //zip.Write(z.pvp);
            }
            return zip;
        }

        public static void Deserialize(ZPackage package)
        {
            ZoneT.Clear();
            int tnum = package.ReadInt();
            for (int i = 0; i < tnum; i++)
            {
                ZoneT.Add(new ZoneTypes
                    {
                    Name = package.ReadString(),
                    PVP = package.ReadBool(),
                    PVPEnforce = package.ReadBool(),
                    ShowPosition = package.ReadBool(),
                    PositionEnforce = package.ReadBool(),
                    ViewDistance = package.ReadInt(),
                    AutoHeal = package.ReadBool()
                    }
                    );
            }

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
                    Shape = package.ReadString(),
                    //type = (zoneType) package.ReadInt(),
                    Position = new Vector2(package.ReadSingle(), package.ReadSingle()),
                    Radius = package.ReadSingle(),
                    //pvp = package.ReadBool()
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


        // WorldofValheimZones.ServerSafeZonePath.Value
        public static void LoadZoneData(string ZonePath)
        {
            

            if (!File.Exists(ZonePath))
            {
                Debug.Log($"Creating zone file at {ZonePath}");
                string text = global::WorldofValheimZones.Properties.Resources.Default_zones;
                //string text = "# format: name type x z radius\nDefaultSafeZone safe 1 0.0 0.0 5.0 true";
                File.WriteAllText(ZonePath, text);
            }
            else
            {
                Debug.Log($"Loading zone file: {ZonePath}");
            }

            // Clean up the old zone data
            ZoneT.Clear();
            Zones.Clear();

            int pos = 0;
            foreach (string text2 in File.ReadAllLines(ZonePath))
            {
                if (!string.IsNullOrWhiteSpace(text2) && text2[0] != '#')
                {
                    string[] array2 = text2.Split(Array.Empty<char>());

                    // Check if it is a type
                    if (array2[0].ToLower() == "type:")
                    {
                        Debug.Log("Loading Type ...");
                        ZoneTypes zt = new ZoneTypes {Name = array2[1]};
                        
                        // Go through each argument to override defaults.

                        if(array2.Length >= 3)
                            zt.PVP = bool.Parse(array2[2]);

                        if (array2.Length >= 4)
                            zt.PVPEnforce = bool.Parse(array2[3]);

                        if (array2.Length >= 5)
                            zt.ShowPosition = bool.Parse(array2[4]);

                        if (array2.Length >= 6)
                            zt.PositionEnforce = bool.Parse(array2[5]);

                        if (array2.Length >= 7)
                            zt.ViewDistance = int.Parse(array2[6]);
                        
                        if (array2.Length >= 8)
                            zt.AutoHeal = bool.Parse(array2[7]);

                        ZoneT.Add(zt);

                    }
                    else
                    {
                        if (array2.Length != 7)
                        {
                            Debug.Log($"Zone '{text2}' is not correctly formatted.");
                        }
                        else
                        {

                            Debug.Log("Loading Zone ...");
                            Zone z = new Zone();
                            z.Name = array2[0];
                            z.Type = array2[1];
                            z.Priority = int.Parse(array2[2]);
                            z.Shape = array2[3];
                            //z.type = (zoneType) Enum.Parse(typeof(zoneType), array2[1]);
                            z.Position.x = float.Parse(array2[4]);
                            z.Position.y = float.Parse(array2[5]);
                            z.Radius = float.Parse(array2[6]);
                            //z.pvp = bool.Parse(array2[7]);
                            z.ID = pos;

                            Zones.Add(z);

                            pos++;
                        }
                    }
                }
            }
#if DEBUG
            _debug();
#endif
        }

    }
}
