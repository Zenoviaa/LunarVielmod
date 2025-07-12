using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Core.DashSystem;

namespace Stellamod
{


    public class Stellamod : Mod
    {
        public static Mod Instance { get; private set; }
        public override void Load()
        {
            base.Load();
            Instance = this;
            ShaderLoader.LoadShaders(this);
            ReTexture.Load();
        }

        public override void Unload()
        {
            base.Unload();
            ReTexture.Unload();
        
        }
        internal enum MessageType : byte
        {
            DashPlayerSync,
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            base.HandlePacket(reader, whoAmI);
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                // This message syncs ExampleLifeIncreasePlayer.exampleLifeFruits and ExampleLifeIncreasePlayer.exampleManaCrystals
                case MessageType.DashPlayerSync:
                    byte playernumber = reader.ReadByte();
                    DashPlayer dashPlayer = Main.player[playernumber].GetModPlayer<DashPlayer>();
                    dashPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        dashPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                default:
                    Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}

