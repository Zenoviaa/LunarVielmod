using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Skies
{
    public class SkyPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.netMode == NetmodeID.Server)
                return;

            var inUnderworld = Player.ZoneUnderworldHeight;
            inUnderworld |= Player.GetModPlayer<MyPlayer>().ZoneCinder;
            if (!SkyManager.Instance["CrystalMoon:CloudySky"].IsActive()
                && Player.ZoneOverworldHeight || Player.ZoneSkyHeight || inUnderworld)
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("CrystalMoon:CloudySky", targetCenter);
            }
            else if (SkyManager.Instance["CrystalMoon:CloudySky"].IsActive())
            {
                SkyManager.Instance.Deactivate("CrystalMoon:CloudySky");
            }

            if (!SkyManager.Instance["CrystalMoon:DesertSky"].IsActive() && Player.ZoneDesert)
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("CrystalMoon:DesertSky", targetCenter);
            }
            else if (SkyManager.Instance["CrystalMoon:DesertSky"].IsActive())
            {
                SkyManager.Instance.Deactivate("CrystalMoon:DesertSky");
            }
        }
    }

}
