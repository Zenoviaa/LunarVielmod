using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class Violar : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToArtifact();
            Item.width = 40;
            Item.height = 10;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.autoReuse = true;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/violar");

            // Weapon Properties
            Item.damage = 8;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.crit = 25;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<Violarproj>();
            Item.shootSpeed = 4f;
            Item.value = 5000;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }

    public class Violarproj : ModProjectile
    {
        public float ExplodingTimer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Violarproj");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 152;
            Projectile.light = 0.78f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool PreAI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            //Lerping
            float progress = ExplodingTimer;
            Color drawColor = Color.Lerp(Color.Transparent, Color.Orange, progress);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, Projectile.Frame(), drawColor, Projectile.rotation, Projectile.Frame().Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(projectileTexture, Projectile.Center - Main.screenPosition, Projectile.Frame(), lightColor, Projectile.rotation, Projectile.Frame().Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;  
            Timer++;
            if (Timer == 150)
            {
                int S1 = Main.rand.Next(0, 3);
                if (S1 == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong"), Projectile.position);
                }
                if (S1 == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong2"), Projectile.position);
                }
                if (S1 == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong3"), Projectile.position);
                }

                var entitySource = Projectile.GetSource_FromThis();
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MusicDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 6f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Orange,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.01f, 0.15f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                Projectile.Kill();
                SoundStyle explosionSound = AssetManager.GetSound("MorrowExp");
                explosionSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);
                Timer = 0;
            }
            if (Timer >= 100)
            {
                Projectile.scale += 0.01f;
                ExplodingTimer += 0.005f;
            }
            
        }
    }
}












