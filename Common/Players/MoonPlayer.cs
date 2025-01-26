using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Players
{


    public class MoonPlayer : ModPlayer
    {
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Disbale damage variation on everything
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.DamageVariationScale *= 0;
            if (ModContent.GetInstance<LunarVeilClientConfig>().RedDamageNumbersToggle)
            {
                modifiers.HideCombatText();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (ModContent.GetInstance<LunarVeilClientConfig>().RedDamageNumbersToggle)
            {
                CombatText.NewText(target.getRect(), Color.Lerp(Color.Red, Color.White, 0.25f), hit.Damage, dramatic: hit.Crit);
            }
        }

        public override void PostUpdate()
        {

            if (Player.ZoneSnow)
            {

                //Update Rain
                Main.raining = true;

                //That way, if it is already raining, it won't be overriden
                //And if it is not raining, it'll just be permanent until you leave the biome
                if (Main.rainTime <= 2)
                    Main.rainTime = 2;


                Main.maxRaining = 0.8f;

                Main.maxRain = 140;

            }

        }
    }
}