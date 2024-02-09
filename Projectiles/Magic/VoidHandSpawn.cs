using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    internal class VoidHandSpawn : ModProjectile
    {
        bool Moved;
        Vector2 OldVelotcity;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            base.Projectile.penetrate = 4;
            base.Projectile.width = 20;
            base.Projectile.height = 20;
            base.Projectile.timeLeft = 700;
            base.Projectile.alpha = 255;
            base.Projectile.friendly = true;
            base.Projectile.hostile = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
        }
        public override bool PreAI()
        {

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.spriteDirection = Projectile.direction;
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 3;
                    }


                }
            }
            if (Projectile.ai[0] == 40)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
                var EntitySource = Projectile.GetSource_FromThis();
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, OldVelotcity.X, OldVelotcity.Y, 
                    ModContent.ProjectileType<VoidHand>(), 40, 1, Projectile.owner, 0, 0);
                Projectile.timeLeft = 2;
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand3"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand2"), Projectile.position);
                }
            }
            if (Projectile.ai[0] == 20)
            {

                SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned, Projectile.position);
            }
            return true;
        }
        public override void AI()
        {



            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand"), Projectile.position);
                OldVelotcity = Projectile.velocity;

                Moved = true;
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            Projectile.velocity *= .86f;
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 12;
            }

        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 60; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
 
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);

        }
    }
}