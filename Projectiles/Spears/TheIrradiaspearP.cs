using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Spears
{
    internal class TheIrradiaspearP : ModProjectile
    {
        private enum ActionState
        {
            Charge,
            Out,
            HitStun
        }

        //Values
        private Vector2 HoldOffset => new Vector2(56, 12);
        private float RotationOffset => MathHelper.ToRadians(120);

        private float ChargeTime => 25 / Owner.GetAttackSpeed(DamageClass.Melee);
        private float SwingTime => 25f / Owner.GetAttackSpeed(DamageClass.Melee);

        private float ThrustDistance => 128;
        private float MaxChargeDistanceMult => 3.5f;
        private bool MaxCharge;
        private Player Owner => Main.player[Projectile.owner];


        //AI
        private float FlashTimer;
        private ref float Timer => ref Projectile.ai[0];
        private ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float HitStunTimer => ref Projectile.ai[2];

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            //Don't do damage whil charging
            return State != ActionState.Charge;
        }

        public override void AI()
        {
            FlashTimer -=0.02f;
            if (FlashTimer <= 0)
                FlashTimer = 0;
            switch (State)
            {
                case ActionState.Charge:
                    AI_Charge();
                    break;
                case ActionState.Out:
                    AI_Out();
                    break;
                case ActionState.HitStun:
                    AI_HitStun();
                    break;
            }
        }

        private void AI_Charge()
        {
            Timer++;
            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 directionToMouseWorld = Owner.Center.DirectionTo(mouseWorld);
            Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.ChangeDir(Projectile.direction);
                Projectile.velocity = directionToMouseWorld * ThrustDistance;
                Projectile.netUpdate = true;
            }

            float progress = Timer / ChargeTime;
            float easedProgress = Easing.OutCubic(progress);
            float rotation = Projectile.velocity.ToRotation();


            float holdRotation = rotation;
            Vector2 holdOffset = HoldOffset;
            if (Owner.direction == -1)
            {
                holdOffset.Y *= -1;
            }
           
            Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
            Vector2 swingEnd = playerCenter + Projectile.velocity + holdOffset.RotatedBy(rotation);
            Vector2 swingCenter = Vector2.Lerp(swingEnd, swingStart, easedProgress);

            Projectile.Center = swingCenter;
            Projectile.rotation = rotation + RotationOffset * Owner.direction;
            if (Owner.direction == -1)
            {
                Projectile.rotation -= MathHelper.Pi;
            }

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Timer == ChargeTime)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Teleport");
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                FlashTimer = 1;
                //Idk some visual or sound here
                for(int i = 0; i < 8; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, 
                        newColor: ColorFunctions.AcidFlame);
                }
            }

            if(Timer >= ChargeTime && Timer % 8 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), velocity, 
                    newColor: Color.DarkGray);
            }

            if (!Owner.channel)
            {
                if (Timer >= ChargeTime)
                {
                    MaxCharge = true;
                    Timer = 0;
                    State = ActionState.Out;
                }
                else  if (Timer > ChargeTime / 2)
                {
                    Timer = 0;
                    State = ActionState.Out;
                } else
                {
                    Projectile.Kill();
                }
                Projectile.netUpdate = true;
            }
        }

        private void AI_Out()
        {
            FlashTimer -= 0.1f;
            Timer++;
            if(Timer == 1 && MaxCharge)
            {
                //Throw Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Egg_Shot");
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                //Rocket Boost
                Vector2 velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                for(int i = 0; i < 16; i++)
                {
                    Vector2 dustVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                    dustVelocity = dustVelocity * Main.rand.NextFloat(2, 15f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                        newColor: ColorFunctions.AcidFlame);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), dustVelocity,
                        newColor: Color.DarkGray);
                }
       
            }
            float progress = Timer / SwingTime;
            float easedProgress = Easing.SpikeOutCirc(progress);
             
            //
            Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            //Lerp between two points ig

            float distanceMult = MaxCharge ? MaxChargeDistanceMult : 1f;
            float rotation = Projectile.velocity.ToRotation();

            float holdRotation = rotation;
            Vector2 holdOffset = HoldOffset;
            if (Owner.direction == -1)
            {
                holdOffset.Y *= -1;
            }

            Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
            Vector2 swingEnd = playerCenter + (Projectile.velocity * distanceMult) + holdOffset.RotatedBy(rotation);
            Vector2 swingCenter = Vector2.Lerp(swingStart, swingEnd, easedProgress);

            Projectile.Center = swingCenter;
            Projectile.rotation = rotation + RotationOffset * Owner.direction;
            if (Owner.direction == -1)
            {
                Projectile.rotation -= MathHelper.Pi;
            }

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Timer >= SwingTime)
            {
                Projectile.Kill();
            }
        }

        private void AI_HitStun()
        {
            HitStunTimer--;

            float progress = Timer / SwingTime;
            float easedProgress = Easing.SpikeOutCirc(progress);

            //
            Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            //Lerp between two points ig

            float distanceMult = MaxCharge ? MaxChargeDistanceMult : 1f;
            float rotation = Projectile.velocity.ToRotation();

            float holdRotation = rotation;
            Vector2 holdOffset = HoldOffset;
            if (Owner.direction == -1)
            {
                holdOffset.Y *= -1;
            }

            Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
            Vector2 swingEnd = playerCenter + (Projectile.velocity * distanceMult) + holdOffset.RotatedBy(rotation);
            Vector2 swingCenter = Vector2.Lerp(swingStart, swingEnd, easedProgress);

            Projectile.Center = swingCenter;
            Projectile.rotation = rotation + RotationOffset * Owner.direction;
            if (Owner.direction == -1)
            {
                Projectile.rotation -= MathHelper.Pi;
            }

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);


            if (HitStunTimer <= 0)
            {
                State = ActionState.Out;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (MaxCharge)
            {
                //Big impact sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Wave");
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                HitStunTimer = 15;
                State = ActionState.HitStun;
                Projectile.netUpdate = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.position, 1024, 24);
                float num = 5;
                for(int i = 0; i < num; i++)
                {
                    float progress = (float)i / num;
                    float rot = MathHelper.TwoPi * progress;
                    Vector2 velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                    velocity = velocity.RotatedBy(rot);
                    velocity = velocity * 8;
                    float explosionDelay = progress * 30;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity,
                        ModContent.ProjectileType<TheIrradiaspearSparkProj>(), 
                        Projectile.damage, Projectile.knockBack, Projectile.owner,
                        ai1: explosionDelay);
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<TheIrradiaspearExplosionBigProj>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (MaxCharge)
            {
                modifiers.FlatBonusDamage += 50;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            if (MaxCharge)
            {
                return Color.Lerp(ColorFunctions.AcidFlame, Color.Transparent, completionRatio);
            }
            else
            {
                return Color.Transparent;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.DNATrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D whiteTexture = ModContent.Request<Texture2D>($"{Texture}_White").Value;

            //Draw White
            float whiteProgress = 0f;
            switch (State)
            {
                default:
                case ActionState.Charge:
                    whiteProgress = Timer / ChargeTime;
                    break;
                case ActionState.Out:
                    whiteProgress = Timer / SwingTime;
                    whiteProgress = 1f - whiteProgress;
                    break;
            }

            whiteProgress = MathHelper.Clamp(whiteProgress, 0f, 1f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            if (Timer >= ChargeTime)
            {
                drawPos += Main.rand.NextVector2Circular(2, 2);
            }

            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;
            Rectangle? drawRectangle = null;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            float drawScale = Projectile.scale;
            SpriteEffects spriteEffects = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //Glow stuff
            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
            float rotationOffset = MathF.Sin(Timer * 0.05f) * 4;//VectorHelper.Osc(1f, 2f, 5);
            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.1f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset * (1f - whiteProgress)).RotatedBy(radians) * time;
                spriteBatch.Draw(texture, rotatedPos, drawRectangle, drawColor * whiteProgress * 0.2f, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }

            //Draw Main sprite
            Main.EntitySpriteDraw(texture, drawPos, drawRectangle, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);

            //Draw White Flash
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


            var shader = ShaderRegistry.MiscSilPixelShader;
            float progress = 1f + MathF.Sin(Timer * 0.1f);

            //The color to lerp to
            shader.UseColor(Color.White);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            shader.UseSaturation(progress * FlashTimer);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            spriteBatch.Draw(texture, drawPos, drawRectangle, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            spriteBatch.End();
            spriteBatch.Begin();


            //Draw white overlay
            spriteBatch.Draw(whiteTexture, drawPos, drawRectangle, drawColor * whiteProgress, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.DarkSeaGreen.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 150, Color.LightGoldenrodYellow, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}


