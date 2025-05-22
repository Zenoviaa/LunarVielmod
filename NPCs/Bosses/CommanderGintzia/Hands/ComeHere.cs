using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal class ComeHere : BaseHand
    {
        private float DirectionToShootFrom;
        private float ChargeProgress = 0;
        private Vector2 AimFromPos;
        private int BlastDamage => 20;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(DirectionToShootFrom);
            writer.WriteVector2(AimFromPos);
            writer.Write(ChargeProgress);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DirectionToShootFrom = reader.ReadSingle();
            AimFromPos = reader.ReadVector2();
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

                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if (Timer < 90)
            {
                ChargeProgress = Timer / 60f;
                float offset = 512;
                Vector2 targetCenter = Target.Center;
                AimFromPos = new Vector2(Target.Center.X + DirectionToShootFrom * offset, targetCenter.Y);

                //Home to this point
                float maxSpeed = 24f;
                Vector2 targetVelocity = (AimFromPos - NPC.Center).SafeNormalize(Vector2.Zero);
                float distance = Vector2.Distance(NPC.Center, AimFromPos);
                if (distance < maxSpeed)
                {
                    targetVelocity *= distance;
                }
                else
                {
                    targetVelocity *= maxSpeed;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.03f);
                NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;


                float rotation = (Target.Center - NPC.Center).ToRotation();
                NPC.rotation = MathHelper.Lerp(NPC.rotation, rotation, 0.1f);
            }


            if (Timer == 180)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 velocity = Vector2.UnitX * -DirectionToShootFrom * 1500;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        ModContent.ProjectileType<ComeHereBlast>(), BlastDamage, 2, Main.myPlayer);
                }
            }

            if (Timer > 90)
            {
                NPC.velocity *= 0.92f;
            }

            if (Timer > 240)
            {
                ChargeProgress *= 0.92f;
            }

            if (Timer > 270)
            {
                ChargeProgress = 0f;
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
            spriteBatch.Draw(texture, drawPos, null, glowColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
