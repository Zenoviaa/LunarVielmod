using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class BurningAngelProj : ModProjectile
    {

        private int Spawned;
        private bool ParticleNo;

        public override void SetDefaults()
        {
            
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 40;
            Projectile.width = 40;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 100;
         
        }




        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation  -= 0.5f;


            Projectile.velocity *= 0.97f;



            if (Timer == 1)
            {
                ShakeModSystem.Shake = 1;
            }
            if (Timer < 30)
            {
                if (Main.mouseLeft)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;

                }

             


                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemTime = 10;
                player.itemAnimation = 10;
                player.itemRotation = rotation * player.direction;


            }

            if (Timer == 99)
            {
                ShakeModSystem.Shake = 4;
                float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                Projectile.Kill();
            }


            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);



            //Projectile.netUpdate = true;

        }

      


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeModSystem.Shake = 5;
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
            Projectile.Kill();




        }


    }
}

