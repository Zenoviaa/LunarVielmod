using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class StarShower : BaseTome
    {
        private int Star;
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<StarShowerStar>();
            Item.shootSpeed = 10;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.mana = 7;
            Item.damage = 14;
        }

        public override Color GetTomeHintColor()
        {
            return Main.DiscoColor;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
    
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if (Star == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3"), player.position);
            }
            if (Star == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya2"), player.position);
            }
            if (Star == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1"), player.position);

            }
            Star += 1;
            Star = Star % 3;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: Star);
            return false;
        }
    }
    public class StarShowerStar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        private float RandScale;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Death");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 5;
            Projectile.scale = 0.9f;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                RandScale = Main.rand.NextFloat(0.8f, 1f);
            }
            if (Timer % 15 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.Zero, GetVariantColor(), Main.rand.NextFloat(0.3f, 0.8f));
                dp.outerColor = GetVariantColor();
            }
            NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 200);
            if(closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 2);
            }
            if(Timer >= 10)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Timer * 0.25f) * 0.05f);
            Projectile.rotation += Projectile.velocity.Length() * 0.005f;
            Projectile.rotation += 0.05f;
        }

        public override void OnKill(int timeLeft)
        {
            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.Lerp(GetVariantColor(), Color.Black, 0.5f), Main.rand.NextFloat(0.5f, 1f));
            sp.initialColor = Color.White;
        }

        private Color GetVariantColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Yellow;
                case 1:
                    return Color.Pink;
                case 2:
                    return Color.LightCyan;
            }
        }
        public float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(32, 0, completionRatio);
        }

        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(GetVariantColor(), Color.Transparent, completionRatio) * 0.7f;
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserShader.Instance;
            shader.LaserTexture = TrailRegistry.StarTrail;
            shader.OuterColor = GetVariantColor();
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, offset: Projectile.Size / 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, GetVariantColor(), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), RandScale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }


        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.75f * Main.essScale);
        }
    }


}
