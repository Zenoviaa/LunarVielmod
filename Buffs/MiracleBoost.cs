using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Summons.MiracleSoul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class MiraclePlayer : ModPlayer
    {
        public int miracleLevel;
        public int miracleTimeLeft;
        public bool hasMiracleSet;

        public override void ResetEffects()
        {
            if (miracleTimeLeft <= 0)
            {
                miracleTimeLeft = 0;
                miracleLevel = 0;
            }
      
            hasMiracleSet = false;
        }

        public override void PostUpdateEquips()
        {
            miracleTimeLeft--;
            float miracleWhipBoost = (float)miracleLevel * 0.1f;
            float miracleDamageBoost = (float)miracleLevel * 0.05f;
            Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += miracleWhipBoost;
            Player.GetDamage(DamageClass.Summon) += miracleDamageBoost;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Chance to cleave out soul
            if (hasMiracleSet && Main.rand.NextBool(15))
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, -8f));
                Projectile.NewProjectile(Player.GetSource_FromThis(),
                    target.Center, velocity, ModContent.ProjectileType<MiracleSoulCollectibleProj>(), 0, 0, Player.whoAmI);

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcaneExplode"), target.position);
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff<MiracleBoost>())
            {
                miracleTimeLeft = 0;
                miracleLevel = 0;
                Player.ClearBuff(ModContent.BuffType<MiracleBoost>());
            }
        }
    }

    public class MiracleBoost : ModBuff
    {
        //This buff doesn't do anything, it just shows you how much time you got left.
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
