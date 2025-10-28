using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Gustbeak.Projectiles;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands
{
    public class OpenPalm : BaseHand
    {
        private float DirectionToShootFrom;
        private float ChargeProgress = 0;
        private float StartOffset = 0;
        private float ColorProgress;
        private int FingerBlastDamage => 20;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(DirectionToShootFrom);
            writer.Write(StartOffset);
            writer.Write(ChargeProgress);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DirectionToShootFrom = reader.ReadSingle();
            StartOffset = reader.ReadSingle();
            ChargeProgress = reader.ReadSingle();
        }
        protected override void AI_Orbit()
        {
            base.AI_Orbit();
            ChargeProgress = MathHelper.Lerp(ChargeProgress, 0f, 0.1f);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        protected override void AI_Attack()
        {
            base.AI_Attack();
            ColorProgress = MathHelper.Lerp(ColorProgress, 1f, 0.5f);
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (StellaMultiplayer.IsHost)
                {
                    DirectionToShootFrom = Main.rand.NextBool(2) ? -1 : 1;
                    StartOffset = Main.rand.NextFloat(-10f, 10f);
                    NPC.netUpdate = true;
                }
            }

            Vector2 targetCenter = Target.Center;
            Vector2 offsetVel = -Vector2.UnitY * 152;
            Vector2 targetPos = targetCenter + offsetVel;

            float rotation = (Target.Center - NPC.Center).ToRotation();
            NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.WrapAngle(rotation), 0.1f);

            ChargeProgress = Timer / 60f;
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
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 91)
            {
                DrawWindTrail = false;
                TargetOutlineColor = Color.Red;
                NPC.velocity *= 0.95f;
            }
            else
            {
                DrawWindTrail = true;
                TargetOutlineColor = Color.Yellow;
            }
            if (Timer >= 90 && Timer % 90 == 0)
            {
                ChargeProgress = 0;
                NPC.rotation -= DirectionToShootFrom * MathHelper.ToRadians(75);
                NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 fireVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    fireVelocity *= 7;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<WindBlast>(), FingerBlastDamage, 1, Main.myPlayer);
                }

                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                for (int i = 0; i < 12; i++)
                {
                    float progress = i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

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
            glowColor *= 0.5f;
            glowColor.A = 0;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, null, glowColor, NPC.rotation, texture.Size() / 2, 0.5f, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
