using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.NPCs.Town;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class RippingGeyser : ModProjectile
    {
        private float _inFlash;
        private Vector2[] Points = new Vector2[64];
        private Vector2 InitialPosition;
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }

        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 450;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialPosition = reader.ReadVector2();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                InitialPosition = Projectile.Center;
            }

            if(Timer < 60)
            {
                Projectile.Center = Parent.Center;
            }

            if (Points == null)
                return;

            for (int i = 0; i < Points.Length; i++)
            {
                float completionRatio = (float)i / (float)Points.Length;
                float ease = EasingFunction.InOutSine(completionRatio);
                Vector2 start = InitialPosition;
                Vector2 end = Projectile.Center;
                Vector2 interpolatedPoint = Vector2.Lerp(start, end, ease);
                Points[i] = interpolatedPoint;
            }

            if (Timer >= 100)
            {
                ShakeModSystem.Shake = MathHelper.Lerp(6, 0, (Timer - 100) / 300f);
                for (int i = 0; i < 3; i++)
                {
                    Smear();
                }
            }
        }

        private void Smear()
        {
            float x = Main.rand.NextFloat(InitialPosition.X, Projectile.Center.X);
            Vector2 spawnPosition = new Vector2();
            spawnPosition.X = x;
            spawnPosition.Y = InitialPosition.Y;
            SmearDrawManager smearManager = ModContent.GetInstance<SmearDrawManager>();

            float length = MathHelper.SmoothStep(1000, 0, (Timer - 100) / 300f);
            float strength = 0.1f;
            smearManager.NewParticle(spawnPosition, Vector2.UnitY, length, 15, strength);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.Lerp(0f, 10, EasingFunction.QuadraticBump(completionRatio));
            w = MathHelper.Lerp(w, 5f, _inFlash);
            return w;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, _inFlash);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Points == null)
                return false;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            _inFlash = 0f;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);

            _inFlash = 1f;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);
            return false;
        }
    }
    public partial class E
    {
        private int RippingGeyserDaamge => 30;
        private void AI_RippingGeysterStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
                _forwardVector.Y = 0;
            }


            //Position yourself on the right or left of the target player
            float startupTime = 100;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 targetStartPosition = MyTarget.Center + _forwardVector * 700;
            targetStartPosition.Y -= 350;
            Vector2 targetVelocity = (targetStartPosition - NPC.Center);
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            NPC.direction = TargetDirection;

            TargetOutlineColor = Color.Yellow;
            if (Timer >= startupTime)
            {
                SwitchState(AIState.RippingGeyser_Dash);
            }
        }

        private void AI_RippingGeyserDash()
        {
            Timer++;
            if(Timer == 1)
            {
                TargetVector = NPC.velocity;
                _forwardVector = Vector2.UnitX * (MyTarget.Center.X > NPC.Center.X ? 1 : -1);
                NPC.direction = TargetDirection;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, 
                        ModContent.ProjectileType<RippingGeyser>(), RippingGeyserDaamge, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            //Dash forward, we need to create a projectile here
            float dashTime = 30;
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            float ease2 = EasingFunction.QuadraticBump(completionRatio);

            Vector2 dashSpeed = _forwardVector * 140;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, dashSpeed, ease);
            Vector2 lerp2 = Vector2.Lerp(TargetVector, interpolatedVelocity, ease2);
            NPC.velocity = lerp2;

            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if(Timer >= dashTime)
            {
                SwitchState(AIState.RippingGeyser_AuraFarm);
            }
        }

        private void AI_RippingGeyserAuraFarm()
        {
            Timer++;
            if(Timer == 1)
            {

            }

            TargetOutlineColor = Color.Transparent;
            float auraTime = 300;
            if(Timer >= auraTime)
            {
                SwitchState(AIState.RippingGeyser_End);
            }
        }



        private void AI_RippingGeyserEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            if(Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
