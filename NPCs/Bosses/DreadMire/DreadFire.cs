using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    public class DreadFire : ModProjectile
    {
        public float ShakeVel;
        public float ShakeVelY;
        public int Time = 1;
        public float Set = 0.99f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Fire");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.2f;
        }

        public override bool PreAI()
        {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;

            }
            return true;
        }

        float alphaCounter = 0.8f;
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.37f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.47f * (5 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), Projectile.position);
            }
            
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RuneDust>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
            }

            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2048f, 64f);
            var entitySource = Projectile.GetSource_FromThis();
            if(Main.myPlayer == Projectile.owner)
            {
                NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<DreadFireBomb>());
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(Set);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Time = reader.ReadInt32();
            Set = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Time = Main.rand.Next(30, 120);
                    Set = Main.rand.NextFloat(0.97f, 0.99f);
                    Projectile.netUpdate = true;
                }

                Projectile.timeLeft += Time;
            }

            Projectile.velocity *= Set;
            if (Projectile.timeLeft < 50)
            {
                if (Projectile.ai[0] % 5 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity.Y = Main.rand.NextFloat(-ShakeVelY, ShakeVelY);
                    Projectile.velocity.X = Main.rand.NextFloat(-ShakeVel, ShakeVel);
                    Projectile.netUpdate = true;
                }

                ShakeVelY += 0.09f;
                ShakeVel += 0.09f;
                Projectile.velocity *= 0.96f;
                alphaCounter += 0.12f;
                Main.bloodMoon = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 256f, 16f);

                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Firework_Red);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;
                Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                vector2_1.Normalize();
                Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = vector2_2;
                vector2_2.Normalize();
                Vector2 vector2_3 = vector2_2 * 34f;
                dust.position = Projectile.Center - vector2_3;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            }
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 
                    ModContent.DustType<RuneDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 
                    ModContent.DustType<RuneDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

        }

    }
}