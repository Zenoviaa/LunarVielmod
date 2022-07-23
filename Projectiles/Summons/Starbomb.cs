using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using IL.Terraria.Graphics.Effects;
using Terraria.Graphics.Effects; // Don't forget this one!
using Stellamod.UI.Systems;
using Terraria.Audio;
using ParticleLibrary;

namespace Stellamod.Projectiles.Summons
{
    public class Starbomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starbomb");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
       
        private int rippleCount = 3;
        private int rippleSize = 5;
        private int rippleSpeed = 15;
        private float distortStrength = 100f;

        public override void SetDefaults()
        {
            Projectile.originalDamage = (int)90f;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 2f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            

        }
        float alphaCounter;
        public override void AI()
        {



            if (Projectile.timeLeft >= 240)
            {
                Projectile.aiStyle = ProjectileID.Bubble;
            }




            // ai[0] = state
            // 0 = unexploded
            // 1 = exploded
            if (Projectile.timeLeft <= 240)
            {
                Projectile.velocity *= 0.95f;
            }


            if (Projectile.timeLeft == 180)
            {
                ShakeModSystem.Shake = 6;
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starexplosion"));
                for (int j = 0; j < 25; j++)
                {

                    Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, speed2 * 5 + Projectile.velocity, ModContent.ProjectileType<Starout>(), (int)(90), 0f, 0, 0f, 0f);
                }
            }
            if (Projectile.timeLeft <= 180)
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = 1; // Set state to exploded
                    Projectile.alpha = 255; // Make the Projectile invisible.
                    Projectile.friendly = false; // Stop the bomb from hurting enemies.

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
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {


            Vector2 center = Projectile.Center + new Vector2(0f, Projectile.height * -0.1f);

            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(Projectile.width * 0.6f, Projectile.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)





            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 frameOrigin = frame.Size() / 2;
            Vector2 offset = new Vector2(Projectile.width - frameOrigin.X);
 

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + frameOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Color color1 = Color.White * (float)(((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Projectiles/Summon/MoonjellySummon/Starbomb_Glow").Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color1, Projectile.rotation, frameOrigin, Projectile.scale, effects, (int)0f);
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Projectiles/Summon/MoonjellySummon/Starbomb_Glow").Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color1, Projectile.rotation, frameOrigin, Projectile.scale, effects, (int)0f);


                Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, frameOrigin, Projectile.scale, effects, (int)0f);
                Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, frameOrigin, Projectile.scale, effects, (int)0f);

            }
            return false;
        }
        
    }
}