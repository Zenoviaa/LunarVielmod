using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class ArchariliteEnergyShot : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 12;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<Cinderscrap>());
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            Vector2 velocity = shootParams.velocity * shootParams.speed * 32;
            velocity *= 3;
            velocity *= shootParams.chargeStrength;

            float bowDamage = shootParams.damage * shootParams.chargeStrength * 3;
            Vector2 position = shootParams.position;
            int damage = (int)bowDamage;
            float knockback = shootParams.knockBack;
            Projectile.NewProjectile(source, position, velocity,
                ModContent.ProjectileType<ArchariliteArrow>(), damage, knockback, player.whoAmI);
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(5);
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2"));
            }

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y,
                    ModContent.ProjectileType<ArchariliteArrowSmall>(), damage, knockback, player.whoAmI);
            }
        }
    }




    public class ArchariliteArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire, 180);
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;

            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.velocity *= 1.02f;
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
            }
            for (int i = 0; i < 32; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 10;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
            return false;
        }
    }

    public class ArchariliteArrowSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;
            Projectile.timeLeft = 290;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.extraUpdates += 1;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 256);
            if (nearest != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, 0, Color.OrangeRed, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 10;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
            return false;
        }
    }
}
