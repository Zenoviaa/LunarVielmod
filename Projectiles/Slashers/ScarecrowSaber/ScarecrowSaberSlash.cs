using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.ScarecrowSaber
{
    internal class ScarecrowSaberPlayer : ModPlayer
    {
        public Vector2? DashVelocity { get; set; } = null;
        public float SlowdownTimer { get; set; }
        public bool DashRotation { get; set; }
        public float DashDirection { get; set; } = 1f;

        public float CooldownTimer { get; set; }
        public float FixRotationTimer { get; set; }
        public float FixRotationDuration { get; set; } = 15;
        public override void ResetEffects()
        {
            base.ResetEffects();
            //   DashRotation = false;
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            //Very simple dash
            if (DashVelocity != null)
            {
                Player.velocity = DashVelocity.Value;
                DashVelocity = null;
                FixRotationTimer = 0;
            }

            if (DashRotation)
            {
                Player.fullRotation += Player.velocity.Length() * 0.015f * DashDirection;
                Player.fullRotationOrigin = Player.Size / 2;
            }

            if (FixRotationTimer > 0)
            {
                FixRotationTimer--;
                float progress = FixRotationTimer / FixRotationDuration;
                Player.fullRotation = MathHelper.Lerp(0, Player.fullRotation, progress);
            }

            if (SlowdownTimer > 0)
            {
                Player.velocity *= 0.95f;
                SlowdownTimer--;
            }

            if(CooldownTimer > 0)
            {
                CooldownTimer--;
                if(CooldownTimer == 0)
                {
                    float num = 24;
                    for (int i = 0; i < num; i++)
                    {
                        float progress = (float)i / num;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 vel = rot.ToRotationVector2() * 3;
                        Dust.NewDustPerfect(Player.Center, DustID.InfernoFork, vel, Scale: 1);
                        Dust.NewDustPerfect(Player.Center, DustID.Torch, vel * 0.75f, Scale: 1);
                    }


                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                    soundStyle.PitchVariance = 0.1f;
                    SoundEngine.PlaySound(soundStyle, Player.position);
                }
            }
        }

    }

    internal class ScarecrowSaberSlash : ModProjectile
    {
        private bool _stoppedMoving;
        private bool _recoiled;
        private float _swingRot;
        private Vector2[] _oldSwingPos;
        private ref float Timer => ref Projectile.ai[0];
        private ref float SwingDirection => ref Projectile.ai[1];
        private ref float DeathTimer => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        public float holdOffset = 30;

        public override string Texture => "Stellamod/Items/Weapons/Melee/ScarecrowSaber";
        public override void SetDefaults()
        {
            base.SetDefaults();
            _oldSwingPos = new Vector2[32];
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 64;
            Projectile.width = 64;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            ScarecrowSaberPlayer scarecrowSaberPlayer = Owner.GetModPlayer<ScarecrowSaberPlayer>();
            scarecrowSaberPlayer.DashRotation = true;

            Timer++;
            if (Timer == 1)
            {
                //Thrust the player
                scarecrowSaberPlayer.DashVelocity = Projectile.velocity;
                int explosion = ModContent.ProjectileType<DaedusBombExplosion>();
                int damage = 0;
                int knockback = 2;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Bottom, Vector2.Zero, explosion, damage, knockback);

                //Dust Particles
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newVelocity = Owner.velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Owner.Bottom, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                    Dust.NewDust(Owner.Bottom, 0, 0, DustID.InfernoFork, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }

                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }

            //Invincible at the start of it
            if (Timer < 20)
            {
                Owner.immune = true;
                Owner.immuneTime = 6;
            }

            //I want it to keep swinging until you stop moving basically
            //Swing Direction/Velocity
            int dir = (int)SwingDirection;
            scarecrowSaberPlayer.DashDirection = dir;

            //LMAOOOO
            _swingRot += Owner.velocity.Length() * 0.015f * dir;
            if (_recoiled)
            {
                DeathTimer++;
            }
            if ((Timer > 8 && Owner.velocity.Length() < 5) || DeathTimer >= 25)
            {
                //Fix the player's orientation
                scarecrowSaberPlayer.DashRotation = false;
                scarecrowSaberPlayer.FixRotationTimer = 15;
                scarecrowSaberPlayer.CooldownTimer = 35;
                Projectile.Kill();
            }


            AI_OrientBlade();

            for (int i = _oldSwingPos.Length - 1; i > 0; i--)
            {
                _oldSwingPos[i] = _oldSwingPos[i - 1];
            }
            if (_oldSwingPos.Length > 0)
                _oldSwingPos[0] = Owner.Center + Projectile.rotation.ToRotationVector2() * holdOffset * 0.5f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        private void AI_OrientBlade()
        {
            //Position the blade
            Vector2 position = Owner.Center;
            position += _swingRot.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

            float rotation = Projectile.rotation;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Timer < 20)
            {
                float num = 24;
                for (int i = 0; i < num; i++)
                {
                    float progress = (float)i / num;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 6;
                    Dust.NewDustPerfect(target.Center, DustID.InfernoFork, vel, Scale: 1);
                    Dust.NewDustPerfect(target.Center, DustID.Torch, vel * 0.75f, Scale: 1);
                }

                //We need some cool sounds

                //Burn the target too
                target.AddBuff(BuffID.OnFire, 120);

                //If you hit at the start of the dash, you have a damage multiplier
                modifiers.FinalDamage *= 12;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_recoiled)
            {
                ScarecrowSaberPlayer scarecrowSaberPlayer = Owner.GetModPlayer<ScarecrowSaberPlayer>();
                scarecrowSaberPlayer.SlowdownTimer = 15;
                _recoiled = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float pr = Timer / 40f;
            pr = MathHelper.Clamp(pr, 0f, 1f);
            pr = 1f - pr;
            float baseWidth = Projectile.scale * Projectile.width * 1.2f * 0.5f * pr;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow * 0.1361f, Color.Transparent, completionRatio);
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            Projectile.oldPos = _oldSwingPos;
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StarTrail);

            Vector2 trailOffset = -Main.screenPosition;
            TrailDrawer.DrawPrims(_oldSwingPos, trailOffset, 155);
            Texture2D spinTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPos = Owner.Center - Main.screenPosition;
            Color drawColor = Color.LightGoldenrodYellow;

            float glowProgress = Timer / 40f;
            glowProgress = 1f - glowProgress;
            glowProgress = MathHelper.Clamp(glowProgress, 0f, 1f);
            drawColor *= glowProgress;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.35f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < _oldSwingPos.Length; i++)
            {
                drawPos = _oldSwingPos[i];
                float p = (float)i / (float)_oldSwingPos.Length;
                p = 1 - p;
                Color afterImageColor = drawColor * p;
                afterImageColor *= 0.5f;
                spriteBatch.Draw(spinTexture, drawPos - Main.screenPosition, null, afterImageColor, drawRotation, spinTexture.Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);

            //Draw really cool glow FX
            float glowProgress = Timer / 40f;
            glowProgress = 1f - glowProgress;
            glowProgress = MathHelper.Clamp(glowProgress, 0f, 1f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor) * glowProgress;
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                float rot = (float)i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
