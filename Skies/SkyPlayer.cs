using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;

namespace Stellamod.Skies
{
    public class SkyPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.netMode == NetmodeID.Server)
                return;
            
            if (!SkyManager.Instance["Stellamod:CloudySky"].IsActive() 
                && Player.ZoneOverworldHeight || Player.ZoneSkyHeight || Player.ZoneUnderworldHeight)
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:CloudySky", targetCenter);
            }else if (SkyManager.Instance["Stellamod:CloudySky"].IsActive())
            {
                SkyManager.Instance.Deactivate("Stellamod:CloudySky");
            }

            if (!SkyManager.Instance["Stellamod:DesertSky"].IsActive() && Player.ZoneDesert)
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:DesertSky", targetCenter);
            } else if (SkyManager.Instance["Stellamod:DesertSky"].IsActive())
            {
                SkyManager.Instance.Deactivate("Stellamod:DesertSky");
            }
        }
    }

}
