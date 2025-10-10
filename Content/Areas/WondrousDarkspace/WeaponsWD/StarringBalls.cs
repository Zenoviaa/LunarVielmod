using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class StarringBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 16;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SparkBallsP>();
            Item.shootSpeed = 8f;
            Item.mana = 6;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankOrb>();
        }
    }

    public class SparkBallsP : ScarletProjectile
    {
        private ITrailer _trailer;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("MeatBall");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            TrailCacheLength = 15;
            Projectile.damage = 12;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            _trailer ??= TrailPresets.StarringBalls;
            _trailer.DrawTrail(ref lightColor, OldCenterPos);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2), MathUtil.Osc(0.8f, 1f, speed: 3), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.LightPink;
            glowColor.A = 0;
            glowColor *= Timer / 30f;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation,
                    new Vector2(32, 32), 1f, SpriteEffects.None, 0f);
            }
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }
            if (Main.rand.NextBool(25))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Pink, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Projectile.velocity *= 1.02f;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if (nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 6);
            }

            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.0f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<StarringBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
        }
    }

    public class StarringBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Pink, Main.rand.NextFloat(0.5f, 1.45f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.01f, 0.15f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
            }

        }
    }
}