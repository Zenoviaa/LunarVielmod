using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class OFlame : ModProjectile
    {
        private Vector2 MovementVelocity;
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float VelTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("OFlame");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(MovementVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MovementVelocity = reader.ReadVector2();
        }


        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void AI()
        {
            Timer++;
            VelTimer++;
            if (Projectile.position.X <= Owner.position.X)
            {
                Projectile.velocity.X += 0.1f;
            }
            else
            {
                Projectile.velocity.X -= 0.1f;
            }

            if (Timer <= 40)
            {
                Projectile.velocity *= 0.97f;
            }
            else
            {
                Projectile.velocity.X += MovementVelocity.X;
                Projectile.velocity.Y += MovementVelocity.Y;
            }

            if (VelTimer >= 40 && Main.myPlayer == Projectile.owner)
            {
                VelTimer = 0;
                MovementVelocity.X = Main.rand.NextFloat(-0.15f, 0.15f);
                MovementVelocity.Y = Main.rand.NextFloat(-0.15f, 0.15f);
                Projectile.netUpdate = true;

            }

            Visuals();
        }


        private void Visuals()
        {
            //Animation
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

            //Dusts
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed, Scale: 2f);
                d.noGravity = true;
            }
        }
    }
}