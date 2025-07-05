using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Stellamod.Core.Map.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.Map
{
    internal class MapPlayer : ModPlayer
    {
        public Vector2? spotToTeleportTo = null;
        public float spotToTeleportTimer;

        public bool mapPieceSpringHillsInner;
        public bool mapPieceWitchTown;
        public bool mapPieceWarriorsDoor;
        public bool teleportSpotSpringHillsInner;
        public bool teleportSpotSpringHillsOuter;
        public bool teleportSpotWitchTown;
        public bool teleportSpotWarriorsDoor;
        public void ResetMap()
        {
            mapPieceSpringHillsInner = false;
            mapPieceWitchTown = false;
            mapPieceWarriorsDoor = false;
            teleportSpotSpringHillsInner = false;
            teleportSpotSpringHillsOuter = false;
            teleportSpotWitchTown = false;
            teleportSpotWarriorsDoor = false;
        }

        public override void PreUpdate()
        {
            base.PreUpdate();

        }
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (spotToTeleportTo.HasValue)
            {
                Player.velocity *= 0;
            }
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (spotToTeleportTo.HasValue)
            {
                Vector2 teleportSpot = spotToTeleportTo.Value;
                spotToTeleportTimer++;
                if (spotToTeleportTimer == 1)
                {
                    FXUtil.SnapFocusCamera(Player.position, 300);
                }
                Main.playerInventory = false;
                MapUISystem uiSystem = ModContent.GetInstance<MapUISystem>();
                uiSystem.CloseThis();

                float duration = 120;
                float progress = spotToTeleportTimer / duration;

                if (spotToTeleportTimer < 100)
                {
                    if (spotToTeleportTimer % 4 == 0)
                    {

                    }
                }

                if (spotToTeleportTimer == 100)
                {
                    Vector2 teleportPosition = (Vector2)spotToTeleportTo;
                    Player.Teleport(teleportPosition);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                }

                if (spotToTeleportTimer == duration)
                {
                    spotToTeleportTo = null;
                    spotToTeleportTimer = 0;
                }
            }
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["mapPieceSpringHillsInner"] = mapPieceSpringHillsInner;
            tag["mapPieceWitchTown"] = mapPieceWitchTown;
            tag["mapPieceWarriorsDoor"] = mapPieceWarriorsDoor;
            tag["teleportSpotSpringHillsInner"] = teleportSpotSpringHillsInner;
            tag["teleportSpotSpringHillsOuter"] = teleportSpotSpringHillsOuter;
            tag["teleportSpotWitchTown"] = teleportSpotWitchTown;
            tag["teleportSpotWarriorsDoor"] = teleportSpotWarriorsDoor;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            mapPieceSpringHillsInner = tag.GetBool("mapPieceSpringHillsInner");
            mapPieceWitchTown = tag.GetBool("mapPieceWitchTown");
            mapPieceWarriorsDoor = tag.GetBool("mapPieceWarriorsDoor");
            teleportSpotSpringHillsInner = tag.GetBool("teleportSpotSpringHillsInner");
            teleportSpotSpringHillsOuter = tag.GetBool("teleportSpotSpringHillsOuter");
            teleportSpotWitchTown = tag.GetBool("teleportSpotWitchTown");
            teleportSpotWarriorsDoor = tag.GetBool("teleportSpotWarriorsDoor");
        }
    }
}
