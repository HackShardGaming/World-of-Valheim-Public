using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Linq;
using Steamworks;
using System.Globalization;
using System.Collections;



namespace WorldofValheimZones
{

    public static class Util
    {
        public static List<Util.ConnectionData> Connections = new List<Util.ConnectionData>();

        public static IEnumerator ZoneHandler2(ZRpc rpc)
        {
            rpc.Invoke("ZoneHandler", new object[] { 
                ZoneHandler.Serialize(rpc.GetSocket().GetHostName())
            }) ;
            yield return new WaitForSeconds(1);
        }
        public static IEnumerator Client2(ZRpc rpc)
        {
            rpc.Invoke("Client", new object[] {
                Client.Serialize(rpc.GetSocket().GetHostName())
            });
            yield return new WaitForSeconds(1);
        }
        public static void InsertChatMessage(string Message)
        {
            Chat.instance.AddString($"<color=grey><b>[{ModInfo.Title}]</b></color> {Message}");
        }
        public class ConnectionData
        {

            public ZRpc rpc;
        }
        public static float RestrictionCheckFloatReturn(string restriction)
        {
            string PlayerSteamID = SteamUser.GetSteamID().ToString();
            Player p = Player.m_localPlayer;
            // Are we in a zone? if so select that zone.
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
             }
            string key = "";
            string admins = "";
            // Lets set our admins and keys..
            admins = zt.Admins;
            key = zt.Configurations;
            // Lets see if the user is actually an admin in the zone first..
            if (admins.Contains(WorldofValheimZones.MySteamID))
            {
                // Ok they are an admin. Therefore, do not initialize the change...
                return 1;
            }
            if (key.ToLower().Contains(restriction))
            {
                string s = key.ToLower();
                string restrictioncheck = restriction + "(";
                int indexStart = s.IndexOf(restrictioncheck) + restrictioncheck.Length;
                string test = "";
                for (int i = indexStart; i < indexStart + 20; i++)
                {
                    if (s[i] == ')') break;
                    test += s[i];
                }
                float multiplier = 1;
                multiplier = Convert.ToSingle(test, new CultureInfo("en-US"));
                return multiplier;
            }
            else
                return 1;
        }
        public static float RestrictionCheckFloatReturnCharacter(Character __instance,string restriction)
        {
            Character p = __instance;
            string CharacterSteamID = (ZNet.instance.GetPeer(__instance.GetZDOID().m_userID)).m_socket.GetHostName();
            // Are we in a zone? if so select that zone.
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            string admins = "";
            // Lets set our admins and keys..
            admins = zt.Admins;
            key = zt.Configurations;
            // Lets see if the user is actually an admin in the zone first..
            if (admins.Contains(CharacterSteamID))
            {
                // Ok they are an admin. Therefore, do not initialize the change...
                return 1;
            }
            if (key.ToLower().Contains(restriction))
            {
                string s = key.ToLower();
                string restrictioncheck = restriction + "(";
                int indexStart = s.IndexOf(restrictioncheck) + restrictioncheck.Length;
                string test = "";
                for (int i = indexStart; i < indexStart + 20; i++)
                {
                    if (s[i] == ')') break;
                    test += s[i];
                }
                float multiplier = 1;
                multiplier = Convert.ToSingle(test, new CultureInfo("en-US"));
                return multiplier;
            }
            else
                return 1;
        }
        public static float RestrictionCheckFloatReturnNone(Character __instance, string restriction)
        {
            Character p = __instance;
            // Are we in a zone? if so select that zone.
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            string admins = "";
            // Lets set our admins and keys..
            admins = zt.Admins;
            key = zt.Configurations;
            // Lets see if the user is actually an admin in the zone first..
            if (key.ToLower().Contains(restriction))
            {
                string s = key.ToLower();
                string restrictioncheck = restriction + "(";
                int indexStart = s.IndexOf(restrictioncheck) + restrictioncheck.Length;
                string test = "";
                for (int i = indexStart; i < indexStart + 20; i++)
                {
                    if (s[i] == ')') break;
                    test += s[i];
                }
                float multiplier = 1;
                multiplier = Convert.ToSingle(test, new CultureInfo("en-US"));
                return multiplier;
            }
            else
                return 1;
        }
        public static bool RestrictionCheckCharacter(Character __instance, string restriction)
        {
            Character p = __instance;
            string CharacterSteamID = (ZNet.instance.GetPeer(__instance.GetZDOID().m_userID)).m_socket.GetHostName();
            // Are we in a zone? if so select that zone.
            if (ZoneHandler.Zones.Count() == 0)
            {
                return false;
            }
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            string admins = "";
            // Lets set our admin list and keys...
            admins = zt.Admins;
            key = zt.Configurations;
            // Lets check and see if the user is actually an admin in the zone.
            if (admins.Contains(CharacterSteamID))
            {
                return false;
            }
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }

        public static bool RestrictionCheckTerrain(TerrainComp __instance, string restriction)
        {
            TerrainComp p = __instance;
            // Are we in a zone? if so select that zone.
            if (ZoneHandler.Zones.Count() == 0)
            {
                return false;
            }
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            string admins = "";
            // Lets set our admin list and keys...
            admins = zt.Admins;
            key = zt.Configurations;
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }
        public static bool RestrictionCheckNone(Character __instance, string restriction)
        {
            Character p = __instance;
            // Are we in a zone? if so select that zone.
            if (ZoneHandler.Zones.Count() == 0)
            {
                return false;
            }
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            // Lets set our admin list and keys...
            key = zt.Configurations;
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }
        public static bool RestrictionCheck(string restriction)
        {
            Player p = Player.m_localPlayer;
            // Are we in a zone? if so select that zone.
            if (ZoneHandler.Zones.Count() == 0)
            {
                return false;
            }
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
            }
            string key = "";
            string admins = "";
            // Lets set our admin list and keys...
            admins = zt.Admins;
            key = zt.Configurations;
            // Lets check and see if the user is actually an admin in the zone.
            if (admins.Contains(WorldofValheimZones.MySteamID))
            {
                return false;
            }
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }
        public static void DoAreaEffect(Vector3 pos)
        {
            if (WorldofValheimZones.EffectTick <= 0)
            {
                WorldofValheimZones.EffectTick = 120;
                GameObject znet = ZNetScene.instance.GetPrefab("vfx_lootspawn");
                GameObject obj = UnityEngine.Object.Instantiate(znet, pos, Quaternion.identity);
                DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance();
                worldTextInstance.m_worldPos = pos;
                worldTextInstance.m_gui = UnityEngine.Object.Instantiate<GameObject>(DamageText.instance.m_worldTextBase, DamageText.instance.transform);
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<Text>();
                DamageText.instance.m_worldTexts.Add(worldTextInstance);
                worldTextInstance.m_textField.color = Color.cyan;
                worldTextInstance.m_textField.fontSize = 24;
                worldTextInstance.m_textField.text = "PRIVATE AREA";
                worldTextInstance.m_timer = -2f;
            }
        }
        public static void DoAreaEffectW(Vector3 pos)
        {
            if (WorldofValheimZones.EffectTick <= 0)
            {
                WorldofValheimZones.EffectTick = 120;
                GameObject znet = ZNetScene.instance.GetPrefab("vfx_lootspawn");
                GameObject obj = UnityEngine.Object.Instantiate(znet, pos, Quaternion.identity);
                DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance();
                worldTextInstance.m_worldPos = pos;
                worldTextInstance.m_gui = UnityEngine.Object.Instantiate<GameObject>(DamageText.instance.m_worldTextBase, DamageText.instance.transform);
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<Text>();
                DamageText.instance.m_worldTexts.Add(worldTextInstance);
                worldTextInstance.m_textField.color = Color.cyan;
                worldTextInstance.m_textField.fontSize = 24;
                worldTextInstance.m_textField.text = "WARDED AREA";
                worldTextInstance.m_timer = -2f;
            }
        }
        public static bool isServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
        public static bool isAdmin(long sender)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string SteamID = sender.ToString();
            if (
                ZNet.instance.m_adminList != null &&
                ZNet.instance.m_adminList.Contains(SteamID)
            )
                return true;
            else
            {
                return false;
            }
        }
        public static void Broadcast(string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static void RoutedBroadcast(long peer, string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static void ReloadZones(long sender, ZPackage pkg)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (peer != null)
            {
                string permissionnode = "HackShardGaming.WoV-Zones.Reload";
                string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
                if (PlayerPermission)
                {
                    ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);
                    Util.Broadcast("Reloading Zone");
                    Debug.Log("S2C ZoneHandler (SendPeerInfo)");
                    Game.instance.StartCoroutine(Util.SendAllUpdate());
                }
                else
                {
                    RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !ReloadZones (Required Permission: {permissionnode})");
                }
            }
        }
        public static IEnumerator SendAllUpdate()
        {
            foreach (var p in ZNet.instance.m_peers)
            {
                string SteamID = p.m_socket.GetHostName();
                ZRoutedRpc.instance.InvokeRoutedRPC(p.m_uid, "ZoneHandler", new object[] {
                        ZoneHandler.Serialize(SteamID)
                    });
            }
            yield return new WaitForSeconds(1);
        }

        public static void AddZone(long sender, ZPackage pkg)
        { 
            if (pkg != null && pkg.Size() > 0)
            { // Check that our Package is not null, and if it isn't check that it isn't empty.
                ZNetPeer peer = ZNet.instance.GetPeer(sender); // Get the Peer from the sender, to later check the SteamID against our Adminlist.
                if (peer != null)
                { // Confirm the peer exists
                    string permissionnode = "HackShardGaming.WoV-Zones.Add";
                    string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                    bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
                    if (PlayerPermission)
                        {
                        string msg = pkg.ReadString();
                        string[] results = msg.Split(' ');
                        string Name = results[0];
                        Debug.Log($"C-<S AddZone (RPC Call)");
                        string Type = results[1];
                        ZoneHandler.ZoneTypes zt = ZoneHandler.FindZoneType(results[1]);
                        if (zt.Name != Type)
                        {
                            msg = $"ERROR: The requested Zone Type {Type} does not exist!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        int Priority = Int32.Parse(results[2]);
                        if (Priority < 1 || Priority > 5)
                        {
                            msg = $"ERROR: The requested Priority {Priority} is out of bounds! (Priorities are ranged from 1-5)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string Shape = results[3];
                        if (Shape.ToLower() != "circle" && Shape.ToLower() != "square")
                        {
                            msg = $"ERROR: The requested Shape: {Shape} is incorrectly formated! (Shapes can either be circle or square only)";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        Single i = new float();
                        string X = results[4];
                        if (!Single.TryParse(X, out i))
                        {
                            msg = $"ERROR: The requested X {X} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string Y = results[5];
                        if (!Single.TryParse(Y, out i))
                        {
                            msg = $"ERROR: The requested Y {Y} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string R = results[6];
                        if (!Single.TryParse(R, out i))
                        {
                            msg = $"ERROR: The requested Radius {R} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string addline = Name + " " + Type + " " + Priority + " " + Shape + " " + X + " " + Y + " " + R;
                        File.AppendAllText(WorldofValheimZones.ZonePath.Value, addline + Environment.NewLine);
                    }
                    else
                    {
                        Util.RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !AddZone (Required Permission: {permissionnode})");
                        Debug.Log($"An unauthorized user {peerSteamID} attempted to use the AddZone RPC!");
                        string msg = pkg.ReadString();
                        Debug.Log($"Here is a log of the attempted AddZone {msg}");
                    }
                }
            }
        }
        public static Util.ConnectionData GetServer()
        {
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Assert(Util.Connections.Count == 1);
            return Util.Connections[0];
        }
        

    }
}
