using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER
{

    public class STARBOMBERGUN : ModNPC
    {
        private const int TELEPORT_DISTANCE = 200;
        private float Size;
        private bool CheckSize;

        int chargetimer = 0;
        private float AlphaTimer=1f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shiffting Skull");
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
       
        
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 84;
            NPC.damage = 1;
            NPC.defense = 10;
            NPC.lifeMax = 20000;
            NPC.value = 1f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
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
                    ParticleManager.NewParticle(NPC.Center, speed * 3,
                        ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }

            if (Shooter > 170)
            {
                Shooting++;
                shootbreak++;
            }

            if (shootbreak < 60)
            {
                AlphaTimer -= 0.1f;
                if (AlphaTimer <= 0)
                    AlphaTimer = 0;
                if (Shooting >= 6)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);

                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARSHOOT"));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 2, speedYb - 2 * 2,
                            ModContent.ProjectileType<BULLET>(), 47, 0f, Owner: Main.myPlayer);
                    }

                    Shooting = 0;
                }
            }
            else
            {
                AlphaTimer += .02f;
                if(AlphaTimer >= 1)
                {
                    AlphaTimer = 1;
                }
            }

            //BREAK TIME IS OVER
            //Reset
            if(shootbreak > 120)
            {
                shootbreak = 0;
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


        public Color? GetLineAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - NPC.alpha / 50f);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Color lineDrawColor = (Color)GetLineAlpha(drawColor);
            lineDrawColor *= AlphaTimer;

            Vector2 drawOrigin = lineTexture.Size() / 2;

            float drawScale = NPC.scale;
            float rot = NPC.rotation;
            Main.spriteBatch.Draw(lineTexture, NPC.Center - Main.screenPosition, null, 
                lineDrawColor, rot, Vector2.Zero, drawScale, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}