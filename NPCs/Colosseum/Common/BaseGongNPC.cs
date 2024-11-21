using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Lights;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public abstract class BaseGongNPC : ModNPC
    {
        private enum AIState
        {
            Idle,
            Shake,
            Away
        }
        private ref float Timer => ref NPC.ai[0];

        private float Alpha;
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 20;
            NPC.height = 80;
            NPC.lifeMax = 1000;
            NPC.defense = 9999;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2;

            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                float length = NPC.oldPos.Length;
                float f = i;
                float progress = f / length;
                Color startColor = Color.White;
                Color endColor = Color.Transparent;
                Color color = Color.Lerp(startColor, endColor, progress);

                Vector2 drawPos = oldPos + NPC.Size / 2;
                spriteBatch.Draw(texture, drawPos, null, drawColor * Alpha, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly * 0.2f;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f, speed: 0.5f);
                spriteBatch.Draw(texture, drawPosition + offset, null, drawColor * Alpha, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPosition, null, drawColor * Alpha, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (State == AIState.Idle)
            {
                SwitchState(AIState.Shake);
            }
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Shake:
                    AI_Shake();
                    break;
                case AIState.Away:
                    AI_Away();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            Alpha = MathHelper.Lerp(Alpha, 1f, 0.1f);
            Timer++;
            Vector2 targetVelocity = new Vector2(0f, MathF.Sin(Timer * 0.002f) * 0.1f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            if (Timer % 16 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), Scale: 0.2f);
            }
        }

        private void AI_Shake()
        {
            Alpha = MathHelper.Lerp(Alpha, 1f, 0.1f);
            Timer++;
            if (Timer == 1)
            {
                //BONG Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Gong");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                FXUtil.ShakeCamera(NPC.position, distance: 2048, strength: 8);
                StartColosseum();
            }
            if (Timer < 60)
            {
                SpecialEffectsPlayer specialEffectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                specialEffectsPlayer.blurStrength = 0.66f;
            }

            Vector2 targetVelocity = Vector2.Zero;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);

            float shakeProgress = Timer / 90f;
            shakeProgress = 1f - shakeProgress;
            shakeProgress = MathHelper.Clamp(shakeProgress, 0f, 1f);

            float rotProgress = MathF.Sin(Timer * 0.66f) * shakeProgress;
            NPC.rotation = MathHelper.Lerp(MathHelper.ToRadians(-15), MathHelper.ToRadians(15), rotProgress);
            if (Timer > 150)
            {
                SwitchState(AIState.Away);
            }
        }

        private void AI_Away()
        {
            Alpha = MathHelper.Lerp(Alpha, 0f, 0.1f);
            Timer++;
            Vector2 targetVelocity = -Vector2.UnitY * 8;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            if (Timer > 90)
            {
                NPC.Kill();
            }
        }

        protected virtual void StartColosseum()
        {

        }
    }
}
