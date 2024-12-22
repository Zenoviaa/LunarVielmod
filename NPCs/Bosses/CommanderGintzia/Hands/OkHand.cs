using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.ScarecrowSaber;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal class OkHand : BaseHand
    {
        private int GrabbedPlayer = -1;
        private bool HasDoneGrab;
        private Vector2 ThrowVelocity;
        private float ChargeProgress;
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage /= 3;
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            GrabbedPlayer = reader.ReadInt32();
            ThrowVelocity = reader.ReadVector2();
            HasDoneGrab = reader.ReadBoolean();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(GrabbedPlayer);
            writer.WriteVector2(ThrowVelocity);
            writer.Write(HasDoneGrab);
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
                HasDoneGrab = false;
                GrabbedPlayer = -1;
                NPC.netUpdate = true;
                NPC.TargetClosest();


                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            ChargeProgress = MathHelper.Lerp(ChargeProgress, 1f, 0.1f);
            if (GrabbedPlayer == -1)
            {
                Vector2 directionToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 targetVelocity = directionToTarget;
                float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
                float maxSpeed = 5;
                if (distanceToTarget < maxSpeed)
                {
                    targetVelocity *= distanceToTarget;
                }
                else
                {
                    targetVelocity *= maxSpeed;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.5f);

                float targetRotation = directionToTarget.ToRotation();
                NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.WrapAngle(targetRotation), 0.1f);
            }
            else
            {
                if (Timer == 3)
                {
                    FXUtil.ShakeCamera(NPC.position, 1024, 8);
                    if (StellaMultiplayer.IsHost)
                    {
                        ThrowVelocity = new Vector2(Main.rand.NextBool(2) ? -1 : 1, 0);
                        NPC.netUpdate = true;
                    }
                }

                if (Timer < 90)
                {
                    Vector2 targetVelocity = ThrowVelocity;
                    targetVelocity *= MathHelper.Lerp(0f, 30, Timer / 60f);
                    ThrowVelocity = ThrowVelocity.RotatedBy(MathHelper.ToRadians(15));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                    NPC.rotation = NPC.velocity.ToRotation();
                    Player target = Main.player[GrabbedPlayer];
                    target.Center = NPC.Center;
                }

                if (Timer == 90)
                {
                    Player target = Main.player[GrabbedPlayer];
                    ScarecrowSaberPlayer scarecrowSaberPlayer = target.GetModPlayer<ScarecrowSaberPlayer>();
                    Vector2 throwMeVelocity = -Vector2.UnitY * 30;
                    scarecrowSaberPlayer.DashVelocity = throwMeVelocity.RotatedBy(ThrowVelocity.ToRotation());

                    for (int i = 0; i < 12; i++)
                    {
                        float progress = (float)i / 12f;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 vel = rot.ToRotationVector2() * 4;
                        Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                    }


                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SwordThrow");
                    soundStyle.PitchVariance = 0.1f;
                    SoundEngine.PlaySound(soundStyle, NPC.position);
                }

                if (Timer > 90)
                {
                    NPC.velocity *= 0.92f;
                }
            }

            if (Timer > 240)
            {
                SwitchState(AIState.Orbit);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(target, hurtInfo);

            if (GrabbedPlayer == -1 && !HasDoneGrab)
            {
                Timer = 2;
                GrabbedPlayer = target.whoAmI;
                HasDoneGrab = true;
                NPC.netUpdate = true;
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureRegistry.FourPointedStar.Value;
            Color glowColor = Color.White;
            glowColor *= ChargeProgress;
            glowColor.A = 0;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            float drawScale = MathHelper.Lerp(0f, 0.5f, ChargeProgress);
            spriteBatch.Draw(texture, drawPos, null, glowColor, NPC.rotation, texture.Size() / 2, drawScale, SpriteEffects.None, 0);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
