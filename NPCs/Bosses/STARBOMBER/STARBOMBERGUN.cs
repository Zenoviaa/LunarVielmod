
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.Particles;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.STARBOMBER
{

    public class STARBOMBERGUN : ModNPC
    {
        private const int TELEPORT_DISTANCE = 200;
        private float Size;
        private bool CheckSize;

        int chargetimer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shiffting Skull");
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
       
        
        public override void SetDefaults()
        {
            NPC.width = 84;
            NPC.height = 44;
            NPC.damage = 1;
            NPC.defense = 10;
            NPC.lifeMax = 20000;
            NPC.value = 1f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public float Shooter = 0;
        public float Shooting = 0;
        public float shootbreak = 0;
        public override void AI()
        {
            Shooter++;
            NPC.velocity *= 0.96f;
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];


            if (Shooter == 2)
            {
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARGUN"));
            }
            if (Shooter < 170)
            {

                for (int j = 0; j < 2; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 2f);
                    ParticleManager.NewParticle(NPC.Center, speed * 3, ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


                }







            }

            if (Shooter > 170)
            {

                Shooting++;
                shootbreak++;

            




            }

            if (Shooting == 6 && shootbreak < 60)
            {
                float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARSHOOT"));
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<BULLET>(), 47, 0f, 0, 0f, 0f);

                Shooting = 0;
            }

            if (Shooting == 6 && shootbreak > 120 && shootbreak < 180)
            {
                float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARSHOOT"));
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<BULLET>(), 47, 0f, 0, 0f, 0f);

                Shooting = 0;
            }

            

            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;


            if (Shooter > 530)
            {
              

                for (int j = 0; j < 50; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 3f);
                    ParticleManager.NewParticle(NPC.Center, speed * 4, ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


                }
                NPC.Kill();
            }


           
                
            
        }

        public override bool PreAI()
        {
            NPC npc = NPC;

            if (npc.HasValidTarget)
            {
                Player player = Main.player[NPC.target];

            

             
                // First, calculate a Vector pointing towards what you want to look at
                Vector2 vectorFromNpcToPlayer = player.Center - npc.Center;
                // Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
                float desiredRotation = vectorFromNpcToPlayer.ToRotation();
                // Now we can do 1 of 2 things. The simplest approach is to use the rotation value directly
                npc.rotation = desiredRotation;
                // A second approach is to use that rotation to turn the npc while obeying a max rotational speed. Experiment until you get a good value.
                npc.rotation = npc.rotation.AngleTowards(desiredRotation, 0.02f);
            }
            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 1;
            int d1 = DustID.BlueCrystalShard;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), .74f);
            }
            if (NPC.life <= 0)
            {

                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SomethingRed, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }

     
    }
}