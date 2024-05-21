using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{

    /*
     * This minion shows a few mandatory things that make it behave properly. 
     * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
     * If the player targets a certain NPC with right-click, it will fly through tiles to it
     * If it isn't attacking, it will float near the player with minimal movement
     */
    public class IrradiatedCreeperMinionProj : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Creeper");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            
            // Only determines the damage type
            Projectile.minion = true;
           
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
          
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<IrradiatedCreeperMinionBuff>(player, Projectile))
                return;

            SummonHelper.SearchForTargets(player, Projectile, 
                out bool foundTarget, 
                out float distanceFromTarget, 
                out Vector2 targetCenter);
            Timer--;
            if (foundTarget && Timer <= 0)
            {
                float distance = 15f;
                float speed = 13;

                Vector2 initialSpeed = Projectile.Center.DirectionTo(targetCenter);
                if(distanceFromTarget < speed)
                {
                    speed = distanceFromTarget;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 randVelocity = Main.rand.NextVector2CircularEdge(2, 2);
                        Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, randVelocity);
                    }
                }
                initialSpeed*= speed;

                Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
                offset.Normalize();
                offset *= (float)(Math.Cos(Timer* (Math.PI / 180)) * (distance / 3));
                Projectile.velocity = initialSpeed + offset;

            }
            else if (Timer > 0)
            {
                Projectile.velocity *= 0.99f;
            }
            else
            {
                SummonHelper.CalculateIdleValues(player, Projectile, 
                     out Vector2 vectorToIdlePosition,
                     out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity += Main.rand.NextVector2CircularEdge(8, 8);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
            Timer = 30;
            for (int i = 0; i < 8; i++)
            {
                Vector2 randVelocity = Main.rand.NextVector2CircularEdge(2, 2);
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, randVelocity);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<JungleBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            target.AddBuff(ModContent.BuffType<AcidFlame>(), 60);
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.AcidFlame, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return true;
        }
    }
}
