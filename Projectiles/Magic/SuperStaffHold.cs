using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
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

        private float Max_Charge_Time => 180;

        ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float HeldRotation => ref Projectile.ai[1];
        private float _width;
        private Vector2[] _lightningZaps;

        public LightningTrail[] LightningTrailPath;
        float ChargeTimer;
        float FireTimer;
        float LightningTimer;
        public override void SetDefaults()
        {
            _lightningZaps = new Vector2[4];
            LightningTrailPath = new LightningTrail[4];
            for (int i = 0; i < 4; i++)
            {
                LightningTrailPath[i] = new LightningTrail();
            }

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
            LightningTimer++;
            if (LightningTimer % 3 == 0)
            {


                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    _lightningZaps[i] = Projectile.Center + Main.rand.NextVector2Circular(75, 75);
                }

                for (int i = 0; i < LightningTrailPath.Length; i++)
                {
                    LightningTrailPath[i].RandomPositions(_lightningZaps);
                }
            }
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
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightGoldenrodYellow, Scale: 0.35f);
                    d.noGravity = true;
                }
            }
        }
        private bool ShouldConsumeMana()
        {
            return ChargeTimer % 8 == 0;
        }

        private void AimAndCharge()
        {
            //Aiming Code
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                HeldRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
            }

            Projectile.velocity = HeldRotation.ToRotationVector2();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = playerCenter + Projectile.velocity * 1f;

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

            }

            if(ChargeTimer % 9 == 0)
            {
                SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

            // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
            // player.channel indicates whether the player is still holding down the mouse button to use the item.
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;


            ChargeVisuals(ChargeTimer, Max_Charge_Time);
            ChargeTimer = MathHelper.Clamp(ChargeTimer, 0, Max_Charge_Time);
            if (!player.channel || !stillInUse)
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
                float chargeProgress = ChargeTimer / Max_Charge_Time;
                Vector2 velocity = Projectile.velocity;
                //Funny Recoil
                float recoilStrength = 14;
                Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0, 14, chargeProgress);
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

                //Funny Screenshake
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, MathHelper.Lerp(0, 32, chargeProgress));

                //Dust Burst Towards Mouse
            
                int count = (int)(48f * chargeProgress);
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0, 18f);
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldCoin, newVelocity.X, newVelocity.Y);
                }

                float multiplier = chargeProgress * 3;
                int damage = Projectile.damage + (int)(multiplier * (float)Projectile.damage);

                Vector2 shootVelocity = Projectile.velocity;
                shootVelocity *= MathHelper.Lerp(8f, 1f, chargeProgress);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity,
                    ModContent.ProjectileType<SuperStaffConjureLightning>(), damage, Projectile.knockBack, player.whoAmI, ai1: chargeProgress);
            }

            if (FireTimer >= 30)
            {
                Projectile.Kill();
            }
        }

        private float WidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 6 * _width;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
            return trailColor;
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


            var prevBelndState = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            _width = 1;
            for (int i = 0; i < 4; i++)
            {
                LightningTrailPath[i].Draw(spriteBatch, _lightningZaps, Projectile.oldRot, ColorFunction, WidthFunction, Projectile.Size / 2);
                _width -= 0.1f;
            }

            Main.graphics.GraphicsDevice.BlendState = prevBelndState;
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
