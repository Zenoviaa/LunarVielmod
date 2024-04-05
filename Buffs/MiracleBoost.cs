using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.MiracleSoul;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class MiraclePlayer : ModPlayer
    {
        private int _miracleSoulCooldown;
        public int miracleLevel;
        public int miracleTimeLeft;
        public bool hasMiracleSet;

        private const int Particle_Count = 2;
        private const int Miracle_Soul_Cooldown = 60;
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
            _miracleSoulCooldown--;
            float miracleLevelFloat = miracleLevel;
            float miracleWhipBoost = miracleLevelFloat * 0.1f;
            float miracleDamageBoost = miracleLevelFloat * 0.1f;
            Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += miracleWhipBoost;
            Player.GetDamage(DamageClass.Summon) += miracleDamageBoost;
            MiracleVisuals();
        }

        private void MiracleVisuals()
        {
            if (miracleLevel <= 0)
                return;

            float miracleLevelFloat = miracleLevel;
            float miracleVisualScaleFactor = miracleLevelFloat * 0.02f;
            float minScale = 0.2f + miracleVisualScaleFactor;
            float maxScale = 0.8f + miracleVisualScaleFactor;

            for (int m = 0; m < Particle_Count; m++)
            {
                Vector2 position = Player.RandomPositionWithinEntity();
                Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), Main.rand.NextFloat(minScale, maxScale));
                p.layer = Particle.Layer.BeforePlayersBehindNPCs;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Chance to cleave out soul
            if (hasMiracleSet && _miracleSoulCooldown <= 0 && Main.rand.NextBool(10))
            {
                _miracleSoulCooldown = Miracle_Soul_Cooldown;
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
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
