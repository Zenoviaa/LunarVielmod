using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal class Fist : BaseHand
    {
        private float ChargeProgress;
        private float StartOffset;
        private float ColorProgress;
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(StartOffset);
            writer.Write(ChargeProgress);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            StartOffset = reader.ReadSingle();
            ChargeProgress = reader.ReadSingle();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && Timer > 90;
        }
        protected override void AI_Orbit()
        {
            base.AI_Orbit();
            ChargeProgress = MathHelper.Lerp(ChargeProgress, 0f, 0.1f);
        }
        protected override void AI_Attack()
        {
            base.AI_Attack();
            ColorProgress = MathHelper.Lerp(ColorProgress, 1f, 0.5f);
            if (Timer == 1)
            {
                ChargeProgress = 0;
                NPC.TargetClosest();
                if (StellaMultiplayer.IsHost)
                {
                    StartOffset = Main.rand.NextFloat(-10f, 10f);
                    NPC.netUpdate = true;
                }

                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }


            StartOffset += 0.1f;
            Vector2 targetCenter = Target.Center;
            Vector2 offsetVel = Vector2.UnitX * 152;
            Vector2 targetPos = targetCenter + offsetVel.RotatedBy(StartOffset);


            ChargeProgress = Timer / 150f;
            if (Timer < 120)
            {

                //Home to this point
                NPC.velocity = (targetPos - NPC.Center) * 0.1f;
                NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            }

            if (Timer > 120 && Timer < 150)
            {
                NPC.velocity *= 0.92f;
            }

            if (Timer < 150)
            {
                float rotation = (Target.Center - NPC.Center).ToRotation();
                NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.WrapAngle(rotation), 0.2f);

            }

            if (Timer == 150)
            {
                Vector2 vel = (targetCenter - NPC.Center).SafeNormalize(Vector2.Zero);
                vel *= 12;
                NPC.velocity = vel;


                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SwordSlice");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if (Timer > 180)
            {
                NPC.velocity *= 0.92f;
            }
            if (Timer > 210)
            {
                SwitchState(AIState.Orbit);
            }
        }



        public float WidthFunction(float completionRatio)
        {
            float baseWidth = 48f;
            return MathHelper.SmoothStep(baseWidth, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            ColorProgress *= 0.9999f;
            Color startColor = Color.White;
            Color trailColor = Color.Lerp(startColor, Color.Transparent, completionRatio);
            trailColor *= ColorProgress;
            return trailColor;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.RestartDefaults();
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition + NPC.Size / 2;
            TrailDrawer.DrawPrims(NPC.oldPos, trailOffset, 155);


            Texture2D texture = TextureRegistry.FourPointedStar.Value;
            Color glowColor = Color.White;
            glowColor *= ChargeProgress;
            glowColor.A = 0;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            float drawRotation = 0;
            float drawScale = 0.4f;
            spriteBatch.Draw(texture, drawPos, null, glowColor, drawRotation, texture.Size() / 2, drawScale, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
