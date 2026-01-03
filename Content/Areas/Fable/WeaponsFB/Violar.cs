using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class Violar : ModItem
    {
        public override void SetDefaults()
        {
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
            Item.DamageType = DamageClass.Ranged;
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
            Projectile.damage = 0;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
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
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }

            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32f);
        }



        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(70, 74);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = ExplodingTimer;
            Color drawColor = Color.Lerp(Color.Transparent, Color.Orange, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, Projectile.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            {
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
                    int S2 = Main.rand.Next(0, 3);
                    if (S2 == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong"), Projectile.position);
                    }
                    if (S2 == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong2"), Projectile.position);
                    }
                    if (S2 == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong3"), Projectile.position);
                    }


                    var entitySource = Projectile.GetSource_FromThis();
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music1").Type, Projectile.damage, 0, Projectile.owner);
                        Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music2").Type, Projectile.damage, 0, Projectile.owner);
                        Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music1").Type, Projectile.damage, 0, Projectile.owner);
                        Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music2").Type, Projectile.damage, 0, Projectile.owner);
                    }


                    Projectile.Kill();
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), Projectile.position);
                    Timer = 0;
                }
                if (Timer >= 100)
                {
                    Projectile.scale += 0.002f;
                    ExplodingTimer += 0.005f;
                }
            }
        }
    }
}












