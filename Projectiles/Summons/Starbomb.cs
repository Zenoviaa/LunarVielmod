using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class Starbomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Starbomb");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
       
        private int rippleCount = 3;
        private int rippleSize = 5;
        private int rippleSpeed = 15;
        private float distortStrength = 100f;

        public override void SetDefaults()
        {
          
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            // These below are needed for a minion weapon
            Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 2f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            // ai[0] = state
            // 0 = unexploded
            // 1 = exploded
            Projectile.velocity *= 0.95f;
            if (Projectile.timeLeft == 180)
            {
                ShakeModSystem.Shake = 6;
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starexplosion"));
                for (int j = 0; j < 40; j++)
                {

                    Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, speed2 * 9 + Projectile.velocity, ModContent.ProjectileType<Starout>(), 320 + Projectile.damage, 0f, 0, 0f, 0f);
                }
            }
            if (Projectile.timeLeft <= 180)
            {
               
                if (Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = 1; // Set state to exploded
                    Projectile.alpha = 255; // Make the Projectile invisible.
                  

                    if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                    {
                        Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", Projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(Projectile.Center);
                    }
                }

                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                {
                    float progress = (180f - Projectile.timeLeft) / 60f;
                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
                }
            }

            if (Projectile.timeLeft == 0)
            {
                Projectile.Kill();
            }
            Vector3 RGB = new(2.55f, 0.45f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreDraw(ref Color lightColor)
        {
           /*
            Vector2 center = Projectile.Center + new Vector2(0f, Projectile.height * -0.1f);

            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(Projectile.width * 0.6f, Projectile.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);*/
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)





            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 frameOrigin = frame.Size() / 2;
            Vector2 offset = new Vector2(Projectile.width - frameOrigin.X);
            Vector2 drawPos = Projectile.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            if (Projectile.timeLeft >= 180)
            {
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(209, 220, 34, 70), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(209, 0, 180, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
                      
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
        }    
    }
}