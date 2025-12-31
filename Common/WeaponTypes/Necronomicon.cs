using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public class NecronomiconHold : ModProjectile
    {
        private Vector2 InitialVelocity;
        private Vector2 SpawnPos;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        private Necronomicon _necronomiconBackingField;
        private Necronomicon Necronomicon
        {
            get
            {
                _necronomiconBackingField ??= Owner.HeldItem.GetGlobalItem<Necronomicon>();
                return _necronomiconBackingField;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();

            Timer++;
            if(Timer == 1)
            {
                SoundStyle summonSound = new SoundStyle("Stellamod/Assets/Sounds/CombusterReady");
                summonSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(summonSound, Projectile.position);
                InitialVelocity = Projectile.velocity;
                int playerDir = InitialVelocity.X > 0 ? 1 : -1;
                SpawnPos = Owner.Bottom + Vector2.UnitX * playerDir * 64;
            }
            if(Timer < 60 && Timer % 5 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(SpawnPos, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(4f, 15f), Color.White, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                dp.outerColor = Necronomicon.hintColor;
                dp.dampening = 0.1f;
                dp.gravity = 0.02f;
            }
            if (Timer == 60)
            {
                SoundStyle summounSound = new SoundStyle("Stellamod/Assets/Sounds/GSummon");
                summounSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(summounSound, Projectile.position);

                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, -Vector2.UnitY * 5,
                        Owner.HeldItem.shoot, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                for (float n = 0; n < 7f; n++)
                {
                    DustParticle dp = Particle<DustParticle>.Spawn(SpawnPos, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(4f, 15f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
                    dp.outerColor = Necronomicon.hintColor;
                    dp.dampening = 0.1f;
                    dp.gravity = 0.02f;
                }
                for (float n = 0; n < 7f; n++)
                {
                    SmokeParticle dp = Particle<SmokeParticle>.SpawnInAlphaLayer(SpawnPos, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(1f, 4f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
                    dp.initialColor = Color.Lerp(Necronomicon.hintColor, Color.Black, 0.5f);
                }
            }
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            UpdatePlayerVisuals(player, rrp);
            AI_MoveTowardsCursor();
            
            //   Owner.heldProj = Projectile.whoAmI;
        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            int playerDir = InitialVelocity.X > 0 ? 1 : -1;
            player.ChangeDir(playerDir);
            //player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        private void AI_MoveTowardsCursor()
        {
            //Welp first thing we'll do is the channeling and cursor movement
            //Calculate where the heck we need to go
            Vector2 targetPosition = Owner.MountedCenter + Owner.direction * Vector2.UnitX * 32;

            Vector2 diffToPosition = targetPosition - Projectile.Center;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, diffToPosition, 0.2f);
        }

        private Color GetTrailColor(float ratio)
        {
            Color hintColor = Necronomicon.hintColor;
            return Color.Lerp(hintColor, Color.Transparent, ratio) * 0.3f;
        }

        private float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(32, 0, ratio);
        }

        private void DrawPixelatedAura(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> auraTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow");
            Vector2 drawOrigin = auraTextureAsset.Size() / 2f;
            Color drawColor = Necronomicon.hintColor;

            float drawRotation = Projectile.rotation;
            float drawScale = Projectile.scale;
            Vector2 scale = Vector2.One * drawScale;
            scale *= 1.3f;
            scale *= ExtraMath.Osc(0.8f, 1f);
            drawColor.A = 0;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(auraTextureAsset.Value, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale * EasingFunction.QuadraticBump(Timer / 120f), SpriteEffects.None, 0);
                spriteBatch.Draw(auraTextureAsset.Value, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale * 0.5f * EasingFunction.QuadraticBump(Timer / 120f), SpriteEffects.None, 0);
            }


            Asset<Texture2D> magicCircleTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/PentagramP2");

            Color manaCircleColor = drawColor;
            float manaCapacity = (float)Owner.statMana / (float)Owner.statManaMax2;
            manaCircleColor *= manaCapacity;
            spriteBatch.Draw(magicCircleTextureAsset.Value, Owner.Center - Main.screenPosition, null, manaCircleColor, Main.GlobalTimeWrappedHourly * 0.4f, 
                magicCircleTextureAsset.Size() / 2f, scale * 0.4f * EasingFunction.QuadraticBump(Timer / 120f), SpriteEffects.None, 0);

            //Draw the gradient pillar
            float alpha = 1f;
            if (Timer > 60f)
            {

                alpha = MathHelper.SmoothStep(1f, 0f,( Timer - 60f) / 60f);
            }
            manaCircleColor *= alpha;
            int playerDir = InitialVelocity.X > 0 ? 1 : -1;
            Vector2 spawnPos = Owner.Bottom + Vector2.UnitX * playerDir * 64;
            Asset<Texture2D> gradientPillar = AssetManager.GlowMask.GradientPillar;
            spriteBatch.Draw(gradientPillar.Value, SpawnPos - Main.screenPosition, null, manaCircleColor, 0,
                    new Vector2(gradientPillar.Width() / 2f, gradientPillar.Height()), scale * 0.4f * EasingFunction.InOutSine(Timer / 60f), SpriteEffects.None, 0);

            //Draw teh flash
            Vector2 flashScale = new Vector2(0.6f, 2.5f);
            Asset<Texture2D> muzzleFlash = AssetManager.GlowMask.MuzzleFlash;
            spriteBatch.Draw(muzzleFlash.Value, SpawnPos - Main.screenPosition, null, manaCircleColor, MathHelper.PiOver2,
                    muzzleFlash.Size() / 2f, scale * 0.4f * EasingFunction.InOutSine(Timer / 60f) * flashScale, SpriteEffects.None, 0);

            manaCircleColor = Color.White;
            manaCircleColor.A = 0;
            manaCircleColor *= alpha;
            spriteBatch.Draw(muzzleFlash.Value, SpawnPos - Main.screenPosition, null, manaCircleColor, MathHelper.PiOver2,
                muzzleFlash.Size() / 2f, scale * 0.4f * EasingFunction.InOutSine(Timer / 60f) * flashScale, SpriteEffects.None, 0);
        }

        private void DrawPixelatedTomeTrail(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, offset: Projectile.Size / 2);
        }

        private void DrawTomeSprite(ref Color lightColor)
        {
            Texture2D closeYourTomeTyrant = TextureAssets.Item[Owner.HeldItem.type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-2, 2, MathUtil.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourTomeTyrant.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = InitialVelocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float layerDepth = 0;
            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale * EasingFunction.QuadraticBump(Timer / 120f), drawEffects, layerDepth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedAura, DrawLayer.OverNPCs);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTomeTrail, DrawLayer.OverNPCsAdditive);
            DrawTomeSprite(ref lightColor);
            return false;
        }
    }
    public class Necronomicon : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isNecronomicon;
        public int necronomiconStaminaCost;
        public Color hintColor;
        public override bool CanShoot(Item item, Player player)
        {
            if (isNecronomicon)
            {
            
                DashPlayer comboPlayer = player.GetModPlayer<DashPlayer>();
  
                if (comboPlayer.DashCount >= necronomiconStaminaCost && player.ownedProjectileCounts[ModContent.ProjectileType<NecronomiconHold>()] == 0)
                    return true;
                return false;
            }
            return base.CanShoot(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           
            
            if (isNecronomicon)
            {
                player.GetModPlayer<DashPlayer>().Consume(necronomiconStaminaCost);
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<NecronomiconHold>(), damage, knockback, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
