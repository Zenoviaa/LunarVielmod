using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using ReLogic.Utilities;

namespace Stellamod.NPCs.Bosses.Sylia
{
    internal class SyliaDeath : ModNPC
    {        
        //Animation Stuffs
        private int _frameCounter;
        private int _frameTick;
        private int _wingFrameCounter;
        private int _wingFrameTick;

        ref float Timer => ref NPC.ai[0];
        public SlotId slotId;
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(24, 48);
            NPC.damage = 1;
            NPC.defense = 1;
            NPC.lifeMax = 1;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 40);
            NPC.scale = 1f;
            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;
        }

        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<VoidParticle>(pos, vel, Color.White, 1f);
                    if (i % 2 == 0)
                    {
                        Dust d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
                        d.noGravity = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw the Wings
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 origin = new Vector2(48, 48);
            Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/SyliaWings").Value;
            int wingFrameSpeed = 2;
            int wingFrameCount = 10;
            spriteBatch.Draw(syliaWingsTexture, drawPosition,
                syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            Texture2D syliaIdleTexture = ModContent.Request<Texture2D>(Texture).Value;
            int syliaIdleSpeed = 2;
            int syliaIdleFrameCount = 30;
            spriteBatch.Draw(syliaIdleTexture, drawPosition,
                syliaIdleTexture.AnimationFrame(ref _frameCounter, ref _frameTick, syliaIdleSpeed, syliaIdleFrameCount, true),
                drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RisingSummon");
                soundStyle.Pitch = -0.75f;
                slotId = SoundEngine.PlaySound(soundStyle);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 9f);
            }
            ChargeVisuals(Timer, 120);

            if(Timer == 180)
            {
                //EXPLODE
                //DEATH
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomSigil>(), 0, 0, Main.myPlayer);
                }

                //Stop the sound
                SoundEngine.TryGetActiveSound(slotId, out ActiveSound? result);
                result?.Stop();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaTransition") { PitchVariance = 0.15f, Pitch = -0.75f }, NPC.position);
                NPC.active = false;
            }
        }
    }
}
