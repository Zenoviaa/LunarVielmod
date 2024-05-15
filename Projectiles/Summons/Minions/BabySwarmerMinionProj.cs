using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class BabySwarmerMinionProj : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        Player Owner => Main.player[Projectile.owner];
        private float Speed
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 4;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 34;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0.5f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
            Projectile.spriteDirection = Projectile.direction;
            if (!SummonHelper.CheckMinionActive<BabySwarmerMinionBuff>(Owner, Projectile))
                return;
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);
            Timer--;
            if (!foundTarget)
            {
                //Idle
                SummonHelper.CalculateIdleValues(Owner, Projectile,
                    out Vector2 vectorToIdlePosition,
                    out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                Speed--;
                Speed = MathHelper.Clamp(Speed, 0, 15);
            }
            else if (Timer <= 0)
            {
                //chase target
                float maxSpeed = 15;
                float acceleration = 1;

                //Accelerate
                Speed += acceleration;
                Speed = MathHelper.Clamp(Speed, 0, maxSpeed);

                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                Vector2 targetVelocity = directionToTarget * Speed;

                if (Projectile.velocity.X < targetVelocity.X)
                {
                    Projectile.velocity.X++;
                    if (Projectile.velocity.X >= targetVelocity.X)
                    {
                        Projectile.velocity.X = targetVelocity.X;
                    }
                }
                else if (Projectile.velocity.X > targetVelocity.X)
                {
                    Projectile.velocity.X--;
                    if (Projectile.velocity.X <= targetVelocity.X)
                    {
                        Projectile.velocity.X = targetVelocity.X;
                    }
                }

                if (Projectile.velocity.Y < targetVelocity.Y)
                {
                    Projectile.velocity.Y++;
                    if (Projectile.velocity.Y >= targetVelocity.Y)
                    {
                        Projectile.velocity.Y = targetVelocity.Y;
                    }
                }
                else if (Projectile.velocity.Y > targetVelocity.Y)
                {
                    Projectile.velocity.Y--;
                    if (Projectile.velocity.Y <= targetVelocity.Y)
                    {
                        Projectile.velocity.Y = targetVelocity.Y;
                    }
                }
            }

            if (Timer > 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver4 / 4);
            }

            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed * 0.3f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            TrailDrawer.WidthFunc = WidthFunction;
            TrailDrawer.ColorFunc = ColorFunction;
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);

            Vector2 frameSize = Projectile.Frame().Size();

            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            TrailDrawer.DrawPrims(Projectile.oldPos, frameSize * 0.5f - Main.screenPosition, 155);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Timer = 16;
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 3;
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
    }
}