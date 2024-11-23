using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal class ScissorHand : BaseHand
    {
        private float DirectionToShootFrom;
        private float ChargeProgress = 0;
        private int FingerBlastDamage => 20;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(DirectionToShootFrom);
            writer.Write(ChargeProgress);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DirectionToShootFrom = reader.ReadSingle();
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
                    DirectionToShootFrom = Main.rand.NextBool(2) ? -1 : 1;
                    NPC.netUpdate = true;
                }
            }

            float offset = 168;
            Vector2 targetCenter = Target.Center;
            Vector2 targetPos = new Vector2(Target.Center.X + DirectionToShootFrom * offset, targetCenter.Y);

            float rotation = (Target.Center - NPC.Center).ToRotation();
            NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.WrapAngle(rotation), 0.1f);

            ChargeProgress = Timer / 60f;
            if (Timer < 90)
            {
                //Home to this point
                float maxSpeed = 24f;
                Vector2 targetVelocity = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
                float distance = Vector2.Distance(NPC.Center, targetPos);
                if (distance < maxSpeed)
                {
                    targetVelocity *= distance;
                }
                else
                {
                    targetVelocity *= maxSpeed;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);
                NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            }

            if (Timer >= 90 && Timer % 60 == 0)
            {
                ChargeProgress = 0;
                NPC.rotation -= DirectionToShootFrom * MathHelper.ToRadians(75);
                NPC.velocity += DirectionToShootFrom * Vector2.UnitX * 1;
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 fireVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    fireVelocity *= 20;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<ScissorBlast>(), FingerBlastDamage, 1, Main.myPlayer);
                }

                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

            }

            if(Timer > 90)
            {
                NPC.velocity *= 0.98f;
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
            spriteBatch.Draw(texture, drawPos, null, glowColor, NPC.rotation, texture.Size() / 2, 0.4f, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
