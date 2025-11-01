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
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria.Audio;
using Stellamod.Dusts;
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
            Item.damage = 14;
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
            Item.mana = 15;
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
            Projectile.friendly = true;
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
            Projectile.velocity *= 0.94f;
            Projectile.rotation += 0.3f;
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
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Orange);
                for(float f = 0; f < 16f; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), vel, Scale: Main.rand.NextFloat(0.5f, 1f), newColor: Color.Yellow);
                }
                SoundStyle starFlowerBoomSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
                starFlowerBoomSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starFlowerBoomSound, Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;
            shader.InnerColor = Color.Yellow;
            shader.GlowColor = Color.Lerp(Color.Orange, Color.Blue, Timer / 60f);
            shader.OuterGlowColor = Color.Lerp(Color.Blue, Color.Black, Timer / 60f);
            shader.Apply();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 scale = Vector2.One;
            spriteBatch.Restart(effect: shader.Effect);
            for(float f = 0; f < 4f; f++)
            {
                float interpolant = f / 4f;
                float rot = interpolant * MathHelper.TwoPi;
                rot += MathHelper.PiOver4;
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                drawPos += rot.ToRotationVector2() * 60;
                spriteBatch.Draw(texture, drawPos, null, Color.White, rot, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos, null, Color.White, rot, drawOrigin, scale * 0.8f, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
