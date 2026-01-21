using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Players
{
    public static class RecoilExt
    {
        public static void AddRecoil(this Player player, Vector2 velocity)
        {
            ref Vector2 currentVelocity = ref player.GetModPlayer<RecoilPlayer>().recoilVelocity;
            currentVelocity += velocity;
    
        }
    }

    public class RecoilPlayer : ModPlayer
    {
        public Vector2 recoilVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            Player.velocity += recoilVelocity;
            recoilVelocity = Vector2.Lerp(recoilVelocity, Vector2.Zero, 0.8f);
        }
        
        public override void CopyClientState(ModPlayer targetCopy)
        {
            base.CopyClientState(targetCopy);
            RecoilPlayer clone = targetCopy as RecoilPlayer;
            clone.recoilVelocity = recoilVelocity;
            clone.Player.velocity = Player.velocity;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            base.SyncPlayer(toWho, fromWho, newPlayer);
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.RecoilPlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.WriteVector2(recoilVelocity);
            packet.WriteVector2(Player.velocity);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            base.SendClientChanges(clientPlayer);
            RecoilPlayer clone = clientPlayer as RecoilPlayer;
            if (recoilVelocity != clone.recoilVelocity)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
        public void ReceivePlayerSync(BinaryReader reader)
        {
            recoilVelocity = reader.ReadVector2();
            Player.velocity = reader.ReadVector2();
        }
    }
}
