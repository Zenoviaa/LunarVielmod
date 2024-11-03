using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class SuperStaffHold : ModProjectile
    {
        enum ActionState
        {
            Aim_And_Charge,
            Fire
        }

        const float Max_Charge_Time = 180;

        ActionState State
        {
            get
            {
                return (ActionState)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = (float)value;
            }
        }
        private ref float SwordRotation => ref Projectile.ai[1];
        float ChargeTimer;
        float FireTimer;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Aim_And_Charge:
                    AimAndCharge();
                    break;
                case ActionState.Fire:
                    Fire();
                    break;
            }
        }

        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 24;
            float maxParticleSpawnSpeed = 12;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 5;
                    var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, newColor: Color.White, Scale: 0.5f);
                    d.noGravity = true;
                }
            }
        }

        private void AimAndCharge()
        {
            //Aiming Code
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            //Charging Code
            if (ChargeTimer == Max_Charge_Time - 1)
            {
                //Complete Charge
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), speed, Scale: 0.5f, newColor: Color.LightCyan);
                    d.noGravity = true;
                }

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            ChargeTimer++;
            if (ChargeTimer == 1)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlastReady, Projectile.position);
            }

            ChargeVisuals(ChargeTimer, Max_Charge_Time);


            ChargeTimer = MathHelper.Clamp(ChargeTimer, 0, Max_Charge_Time);
            if (!player.channel)
            {
                State = ActionState.Fire;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            //Make velocity not move it
            return false;
        }

        private void Fire()
        {
            //Stay on player
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
        //    Projectile.spriteDirection = player.direction;
            Projectile.Center = playerCenter + Projectile.velocity * 1f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            FireTimer++;
            if (FireTimer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                Vector2 velocity = Projectile.velocity;
                //Funny Recoil
                float recoilStrength = 14;
                Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

                //Funny Screenshake
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);

                //Dust Burst Towards Mouse
                float chargeProgress = ChargeTimer / Max_Charge_Time;
                int count = (int)(48f * chargeProgress);
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15)) * 18;
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldCoin, newVelocity.X, newVelocity.Y);
                }

                float multiplier = chargeProgress * 3;
                int damage = (int)(multiplier * (float)Projectile.damage);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<SuperStaffConjureLightning>(), damage, Projectile.knockBack, player.whoAmI, ai1: chargeProgress);
            }

            if (FireTimer >= 60)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = SpriteEffects.None;
    
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float rotation = Projectile.rotation;
            Vector2 drawOrigin = new Vector2(0, texture.Height);
            float layerDepth = 0;
            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, 1f, spriteEffects, layerDepth);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            SpriteEffects spriteEffects = SpriteEffects.None;
  
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(glowTexture).Value;
            float rotation = Projectile.rotation;
            Vector2 drawOrigin = new Vector2(0, texture.Height);
            float layerDepth = 0;


            float chargeProgress = ChargeTimer / Max_Charge_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White.MultiplyRGB(lightColor), chargeProgress);

            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, 1f, spriteEffects, layerDepth);
        }
    }
}
