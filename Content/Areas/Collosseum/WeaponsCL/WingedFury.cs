using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class WingedFuryFeather : ScarletProjectile
    {
        private enum AIState
        {
            Falling,
            Chasing
        }
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];

            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                State = state;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            switch (State)
            {
                case AIState.Falling:
                    if (Timer == 1)
                    {
                        float numDust = 4;
                        for (float n = 0; n < numDust; n++)
                        {
                            Vector2 vel = -Vector2.UnitY * 5;
                            vel = vel.RotatedByRandom(0.5f);
                            LegacyParticle.NewParticle<EmberParticle>(Projectile.Center, vel);
                        }
                    }

                    if (Projectile.velocity.Y < 0)
                    {
                        Projectile.velocity.Y += 0.5f;
                    }
                    else
                    {
                        Projectile.velocity *= 0.95f;
                        if (Projectile.velocity.Length() <= 0.5f)
                        {
                            SwitchState(AIState.Chasing);
                        }
                    }
                    DrawHelper.AnimateTopToBottom(Projectile, 4);
                    break;
                case AIState.Chasing:
                    if (Timer == 2)
                    {
                        var d = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                        d.shrink = true;
                        d.Scale *= 0.5f;

                        SoundStyle pingSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack");
                        pingSound.PitchVariance = 0.3f;
                        pingSound.Pitch = -0.5f;
                        SoundEngine.PlaySound(pingSound, Projectile.position);
                        Projectile.velocity = Vector2.UnitY;
                    }

                    Projectile.extraUpdates = 1;
                    NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 2000);
                    if (closest != null)
                    {
                        Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center);
         
                        Projectile.scale *= 0.99f;
                    }
                    if (Projectile.velocity.Length() <= 15)
                        Projectile.velocity *= 1.05f;
                    if (Timer % 10 == 0)
                    {
                        var p = LegacyParticle.NewParticle<EmberParticle>(Projectile.Center, -Projectile.velocity + Main.rand.NextVector2Circular(1, 1));
                    }
                    Projectile.frame = 0;
                    break;
            }
            //Basically these fly up, slowly fall down, and then home onto nearby enemies at random intervals

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;
                Vector2 oldDrawCenter = OldCenterPos[i] - Main.screenPosition;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);

                float drawScale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                spriteBatch.Draw(texture, oldDrawCenter, frame, drawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            Vector2 drawCenter = Projectile.Center - Main.screenPosition;

            spriteBatch.Draw(texture, drawCenter, frame, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float num = 2f;
            for (float f = 0; f < num * 3; f++)
            {
                float progress = f / num * 3;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.Goldenrod;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(12, 25);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.25f;
            }

        }
    }
    public class WingedFury : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 11;
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            FunctionRepeatHelper.Repeat(() =>
                base.ShootBow(player, source, shootParams), repeats: 2, rate: 5);

        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            for (float f = 0; f < 6; f++)
            {
                Vector2 position = shootParams.position;
                Vector2 velocity = shootParams.velocity * shootParams.chargeStrength * 32;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(0.2f, 0.4f);
                velocity.Y -= 15;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WingedFuryFeather>(), (int)bowDamage, 0, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }
}
