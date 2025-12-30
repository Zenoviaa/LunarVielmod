using Humanizer.Bytes;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.IgnitersNPowders;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Helpers;
using Terraria.Audio;
using Stellamod.Dusts;
using Stellamod.Common.Shaders;
namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class StarFlowerStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Blast Staff");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 50;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.shootSpeed = 35;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<StarFlowerSeed>();
            Item.shootSpeed = 15f;
            Item.mana = 60;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
    }

    public class StarFlowerSeed : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackTimer => ref Projectile.ai[1];  
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle riseSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
                riseSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(riseSound, Projectile.position);
            }
            if(Timer % 10 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 2f), newColor: Color.Yellow);
            }
            Projectile.velocity *= 0.94f;
            Projectile.rotation += Projectile.velocity.Length() * 0.1f + 0.1f;
            if(Projectile.velocity.Length() <= 1f)
            {
                AttackTimer++;
                if(AttackTimer == 1)
                {
                    SoundStyle riseSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower2");
                    riseSound.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(riseSound, Projectile.position);
                }
                if(AttackTimer >= 60)
                {
                    Projectile.Kill();
                }
            }
        }


        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            float lerp = AttackTimer / 60f;
            float interpolant = EasingFunction.InOutSine(lerp);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color color = Color.Yellow;
            color.A = 0;
            color *= lerp;
            for(float f = 0; f < 3; f++)
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<StarFlowerBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class StarFlowerBoom: ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 144;
            Projectile.height = 144;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                var p = FXUtil.GlowCircleBoom(Projectile.Center, Color.Orange, Color.Red, Color.Black);
                p.Scale *= 2;
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Orange);
                for(float f = 0; f < 32; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), vel, Scale: Main.rand.NextFloat(0.5f, 2f), newColor: Color.Yellow);
                }
                SoundStyle starFlowerBoomSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
                starFlowerBoomSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starFlowerBoomSound, Projectile.position);
            }

            if(Timer % 10 == 0)
            {
                Vector2 o = Main.rand.NextVector2Circular(64, 64);
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
      
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center +o,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(5, 10));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 0.5f;
                }

                for (float f = 0; f < 4; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), vel, Scale: Main.rand.NextFloat(0.5f, 2f), newColor: Color.Yellow);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;
            shader.BasePower = 0.5f;
            shader.InnerColor = Color.Lerp(Color.White, Color.Black, Timer / 30f);
            shader.GlowColor = Color.Lerp(Color.Yellow, Color.Black, Timer / 30f);
            shader.OuterGlowColor = Color.Lerp(Color.Blue, Color.Black, Timer / 30f);
            shader.Pixelation = 0.0015f;
            shader.Apply();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 scale = new Vector2(2f, 1f);
            spriteBatch.Restart(effect: shader.Effect, blendState: BlendState.Additive);
            for(float f = 0; f < 4f; f++)
            {
                float interpolant = f / 4f;
                float rot = interpolant * MathHelper.TwoPi;
                rot += MathHelper.PiOver4;
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                drawPos += rot.ToRotationVector2() * 32;
                spriteBatch.Draw(texture, drawPos, null, Color.White, rot, drawOrigin, scale, SpriteEffects.None, 0);
              //  spriteBatch.Draw(texture, drawPos, null, Color.White, rot, drawOrigin, scale * 0.5f, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
