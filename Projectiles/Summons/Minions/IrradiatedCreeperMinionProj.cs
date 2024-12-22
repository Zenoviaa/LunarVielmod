using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
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
        private enum AIState
        {
            Seeking,
            GoToSpot,
            Exploding
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;

        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[1];
        private ref float ExplodingProgress => ref Projectile.ai[2];
        private Vector2 TargetPosition;
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

        public override void SetDefaults()
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TargetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargetPosition = reader.ReadVector2();
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

            switch (State)
            {
                case AIState.Seeking:
                    AI_Seeking();
                    break;
                case AIState.GoToSpot:
                    AI_GoToSpot();
                    break;
                case AIState.Exploding:
                    AI_Exploding();
                    break;
            }
            Visuals();
        }

        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
            ExplodingProgress = 0f;
            Projectile.netUpdate = true;
        }
        private void AI_Seeking()
        {
            Timer++;
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                TargetPosition = targetCenter;
                SwitchState(AIState.GoToSpot);
            }
            else
            {
                SummonHelper.CalculateIdleValues(Owner, Projectile, Owner.Center, out Vector2 idleVector, out float idleDistance);
                SummonHelper.Idle(Projectile, idleDistance, idleVector);
            }
        }

        private void AI_GoToSpot()
        {
            Timer++;
            if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = Vector2.UnitY;
            else
            {
                if (Projectile.velocity.Length() < 5)
                    Projectile.velocity *= 1.02f;
                Projectile.extraUpdates = (int)MathHelper.Lerp(0f, 2f, Timer / 120f);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, TargetPosition, 6f);
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Exploding);
            }
        }

        private void AI_Exploding()
        {
            Timer++;
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                TargetPosition = targetCenter;
            }
            Vector2 vel = (TargetPosition - Projectile.Center) * 0.1f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 0.1f);
            Projectile.extraUpdates = 0;
            ExplodingProgress = Easing.InExpo(Timer / 30f);
            if (Timer >= 30)
            {
                //EXPLODE
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IrradiatedBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                }
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                FXUtil.GlowCircleBoom(Projectile.Center,
                  innerColor: Color.White,
                  glowColor: Color.Green,
                  outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.1f, 0.2f));

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Green,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
                int S1 = Main.rand.Next(0, 3);
                if (S1 == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb1"), Projectile.position);
                }
                if (S1 == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb2"), Projectile.position);
                }
                if (S1 == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb3"), Projectile.position);
                }
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048f, 16f);
                Projectile.velocity = -Vector2.UnitY * 8;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                    Projectile.netUpdate = true;
                }
                SwitchState(AIState.Seeking);
            }
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
