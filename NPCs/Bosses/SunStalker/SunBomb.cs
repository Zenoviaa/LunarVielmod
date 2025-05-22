using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;



namespace Stellamod.NPCs.Bosses.SunStalker
{
    internal class SunBomb : ModProjectile
    {
        public bool Dead;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SunBomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.width = 20;
            Projectile.height = 35;
            Projectile.hostile = true;
        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 1024f, 54f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode"), Projectile.position);
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
            }
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
            }
            var EntitySource = Projectile.GetSource_Death();

            int Gore1 = ModContent.Find<ModGore>("Stellamod/Rock1").Type;
            int Gore2 = ModContent.Find<ModGore>("Stellamod/Rock2").Type;
            Gore.NewGore(EntitySource, Projectile.position, Projectile.velocity, Gore1);
            Gore.NewGore(EntitySource, Projectile.position, Projectile.velocity, Gore2);
        }
        public override void AI()
        {

            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 160)
            {
                {
                    if (Main.rand.NextBool(2))
                    {
                        var entitySource = Projectile.GetSource_FromThis();
                        NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SunStalkerLighting>());
                    }
                }
                if (Projectile.ai[0] >= 50)
                {
                    {
                        if (Main.rand.NextBool(6))
                        {
                            var entitySource = Projectile.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SunStalkerRayLight>());
                        }
                    }
                }
            }
            else
            {
                if (!Dead)
                {
                    Projectile.timeLeft = 1;
                    Dead = true;
                }

            }



        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.75f * Main.essScale);

        }
    }
}

