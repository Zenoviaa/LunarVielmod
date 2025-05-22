using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.Audio;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal class HandShake : BaseHand
    {
        private float ChargeProgress;
        private float StartOffset;
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

        protected override void AI_Orbit()
        {
            base.AI_Orbit();
            ChargeProgress = MathHelper.Lerp(ChargeProgress, 0f, 0.1f);
        }
        protected override void AI_Attack()
        {
            base.AI_Attack();
            if (Timer == 1)
            {
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

            float rotation = (Target.Center - NPC.Center).ToRotation();
            NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.WrapAngle(rotation), 0.1f);

            ChargeProgress = Timer / 60f;
            if (Timer < 90)
            {
                //Home to this point
                NPC.velocity = (targetPos - NPC.Center) * 0.1f;
                NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            }

            if (Timer > 90 && Timer < 110)
            {
                NPC.velocity *= 0.92f;
            }

            if(Timer > 111)
            {
                NPC.velocity *= 0.98f;
            }

            if (Timer > 110 && Timer % 60 == 0)
            {
                Vector2 vel = (targetCenter - NPC.Center).SafeNormalize(Vector2.Zero);
                vel *= 14;
                NPC.velocity = vel;
                NPC.velocity *= 0.98f;

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

            if (Timer > 300)
            {
                SwitchState(AIState.Orbit);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureRegistry.FourPointedStar.Value;
            Color glowColor = Color.White;
            glowColor *= ChargeProgress;
            glowColor.A = 0;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, null, glowColor, NPC.rotation, texture.Size() / 2, 0.33f, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
