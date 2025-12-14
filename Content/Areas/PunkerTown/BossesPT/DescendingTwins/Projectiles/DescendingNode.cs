using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public class DescendingNode : ModNPC,
        IDrawOutlines
    {
        private enum AIState
        {
            Idle,
            Death
        }
        private ref float Timer => ref NPC.ai[0];
        private ref float StartRotation => ref NPC.ai[1];
        private AIState State
        {
            get => (AIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }

        private ref float ShotAt => ref NPC.ai[3];
        private int BeamDamage => 25;
        private Color _outlineColor;
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 96;
            NPC.height = 96;
            NPC.damage = 100;
            NPC.defense = 19;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;

            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
            }
            _outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 16));
        }

        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle mineDeploy = AssetRegistry.Sounds.SteamPunking.DescendingMineDeploy;
                mineDeploy.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mineDeploy, NPC.position);
            }

            float inTime = 60f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 initialVelocity = StartRotation.ToRotationVector2() * MathHelper.Lerp(75f, 0f, ease);
            Vector2 hoverVelocity = new Vector2(0, MathF.Sin(Timer * 0.06f));
            NPC.velocity = initialVelocity + hoverVelocity;
            if(Timer > 5)
            {
                NPC.dontTakeDamage = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 targetNormal = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    Vector2 fireVelocity = targetNormal * 15f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity, 
                        ModContent.ProjectileType<DescendingNodeBeam>(), BeamDamage, 1, Main.myPlayer);
                }
            }


            //Make a cool little explosion
            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Lerp(Color.Green, Color.DarkBlue, 0.5f),
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
                particle.Scale *= 0.5f;
            }

            NPC.active = false;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(projectile, hit, damageDone);

            //This will be called on the server I'm pretty sure
            //Since the server owns the projectile, meaning our method will work :)
            if (projectile.type == ModContent.ProjectileType<DescendingNodeTriggeringBeam>())
            {
                projectile.Kill();
                SwitchState(AIState.Death);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawSprite(spriteBatch, screenPos, drawColor);

            drawColor *= ExtraMath.Osc(0f, 0.5f, speed: 10f);
            drawColor.A = 0;
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, 1, spriteEffects, 0f);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            DrawSprite(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
        }
    }
}
