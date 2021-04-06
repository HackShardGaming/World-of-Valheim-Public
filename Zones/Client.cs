
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
    //     peer.m_rpc.Register<ZPackage>("Client", new Action<ZRpc, ZPackage>(Client.RPC) );
    //
    //   Tell the server to call the RPC
    //     This is done in the SendPeerInfo Method
    //
    //      Debug.Log("S2C ClientState (SendPeerInfo)");
    //      Client._debug();
    //      rpc.Invoke("Client", new object[] {
    //          Client.Serialize()
    //      });
    //

    public static class Client
    {
        // This flag tells the client to enforce the zones or ignore it.
        public static bool EnforceZones = false;

        // PVP Details
        // Can we toggle PVP on the client
        public static bool PVPEnforced = false;
        // Are we enforcing PVP On (TRUE) or Off (FALSE) <- Replaced to zones

        // Current PVP mode for the client
        public static bool PVPMode = false;

        // Position Details
        // Can we hide/show our position
        public static bool PositionEnforce = false;
        // Are we enforcing Position On (TRUE) or Off (FALSE)

        // Show our position on the map
        public static bool ShowPosition = false;

        // Generic debug output
        // Do not change name to debug. Will break "debug()" function in class.

#if DEBUG
        public static void _debug()
        {
            Debug.Log("Loaded Client Data: ");
            Debug.Log("  EnforceZones: " + Client.EnforceZones);
            Debug.Log("  PVPEnforced: " + Client.PVPEnforced);
            Debug.Log("  PVPMode: " + Client.PVPMode);
            Debug.Log("  ShowPosition: " + Client.ShowPosition);
            Debug.Log("  PositionEnforce: " + Client.PositionEnforce);
        }
#endif

        // Compress the data into a zip (compressed) stream (serial)
        public static ZPackage Serialize()
        {
            var zip = new ZPackage();
            if (Client.PVPEnforced)// && Client.PVPisEnabled)
            {
                Client.PVPMode = true;
            }
            else
            {
                Client.PVPMode = false;
            }
            zip.Write(Client.EnforceZones);
            zip.Write(Client.PVPEnforced);
            zip.Write(Client.PVPMode);
            zip.Write(Client.PositionEnforce);
            zip.Write(Client.ShowPosition);
            return zip;
        }

        // Extract the data from the zipped data
        public static void Deserialize(ZPackage data)
        {
            Client.EnforceZones = data.ReadBool();
            Client.PVPEnforced = data.ReadBool();
            Client.PVPMode = data.ReadBool();
            Client.PositionEnforce = data.ReadBool();
            Client.ShowPosition = data.ReadBool();
        }

        // RPC function class. This is the class that you register to receive rpc data.
        public static void RPC(ZRpc rpc, ZPackage data)
        {
            Debug.Log("S2C Client (RPC Call)");
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
    }
}
