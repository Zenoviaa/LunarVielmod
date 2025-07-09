using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.DamageRework
{


    public class MoonPlayer : ModPlayer
    {
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Disbale damage variation on everything
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.DamageVariationScale *= 0;
            if (ModContent.GetInstance<StellamodClientConfig>().RedDamageNumbersToggle)
            {
                modifiers.HideCombatText();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (ModContent.GetInstance<StellamodClientConfig>().RedDamageNumbersToggle)
            {
                if (hit.Crit)
                {
                    CombatText.NewText(target.getRect(), Color.Lerp(Color.DarkRed, Color.White, 0.1f), hit.Damage, dramatic: hit.Crit);
                }
                else
                {
                    CombatText.NewText(target.getRect(), Color.Lerp(Color.Red, Color.White, 0.25f), hit.Damage, dramatic: hit.Crit);
                }
       
            }
        }
    }
}