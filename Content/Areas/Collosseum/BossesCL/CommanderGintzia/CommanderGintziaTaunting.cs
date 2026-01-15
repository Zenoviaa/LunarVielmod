using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia
{
    public class CommanderGintziaTaunting : ModNPC
    {
        private Vector2 FollowCenter;
        private Vector2 _keyPosition;
        private Vector2 _keyVelocity;
        private float _keyRotation;
        private enum AIState
        {
            Spawn,
            FlyingAround,
            Despawn
        }

        private int _frame;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private float FadeProgress;
        private Player Target => Main.player[NPC.target];

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(FollowCenter);
            writer.WriteVector2(_keyPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            FollowCenter = reader.ReadVector2();
            _keyPosition = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 30;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);

            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            if (_frame >= 30)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            FollowCenter = ColosseumWaveManager.GongSpawnWorld + new Vector2(MathF.Sin(Timer * 0.01f) * 800, -168);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.FlyingAround:
                    AI_FlyingAround();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }
            Vector2 idlePosition = NPC.Center;
            idlePosition.Y += 48f;
            Vector2 vectorToIdlePosition = idlePosition - _keyPosition;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            float speed;
            float inertia;

            // Minion doesn't have a target: return to player and idle
            if (distanceToIdlePosition > 100f)
            {
                // Speed up the minion if it's away from the player
                speed = 20f;
                inertia = 80f;
            }
            else
            {
                // Slow down the minion if closer to the player
                speed = 3f;
                inertia = 100f;
            }

            if (distanceToIdlePosition > 20f)
            {
                // The immediate range around the player (when it passively floats about)
                // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                _keyVelocity = (_keyVelocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (_keyVelocity == Vector2.Zero)
            {
                // If there is a case where it's not moving at all, give it a little "poke"
                _keyVelocity.X = -0.28f;
                _keyVelocity.Y = -0.14f;
            }
            _keyPosition += _keyVelocity;
            _keyRotation = _keyVelocity.X * 0.05f;
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override bool CheckActive()
        {
            return !ColosseumWaveManager.IsActive();
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                int xSpawn = (int)NPC.position.X;
                int ySpawn = (int)NPC.position.Y;
                if (MultiplayerHelper.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<EvilCarpet>(),
                        ai2: NPC.whoAmI);
                }
                _keyPosition = NPC.Center;
                _keyRotation = 0;
            }

            FadeProgress = Timer / 90f;
            NPC.velocity.Y = MathHelper.Lerp(10, 0f, Timer / 90f);
            if (Timer >= 90f)
            {
                SwitchState(AIState.FlyingAround);
            }
        }

        private void AI_FlyingAround()
        {
            Timer++;
            if (Timer % 900 == 0)
            {
                BattleTaunt();
            }

            Vector2 targetCenter = FollowCenter + new Vector2(0, -212);
            Vector2 velToPlayer = targetCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            float maxSpeed = 6f;
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, targetCenter);
            if (distance < maxSpeed)
            {
                targetVelocity *= distance;
            }
            else
            {
                targetVelocity *= maxSpeed;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            float targetRotation = NPC.velocity.X * 0.025f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
       
            if (MultiplayerHelper.IsHost)
            {
                if (!ColosseumWaveManager.IsActive() || ColosseumWaveManager.goAwayGintzia)
                {
                    SwitchState(AIState.Despawn);
                }
            }


        }

        private void AI_Despawn()
        {
            Timer++;
            if (Timer == 1)
            {
                DeathTaunt();
            }
            FadeProgress = Timer / 90f;
            NPC.velocity.Y = MathHelper.Lerp(0f, -10f, Timer / 90f);
            NPC.velocity.X *= 0.98f;
            NPC.rotation *= 0.98f;
            FadeProgress = 1f - Timer / 90f;
            NPC.EncourageDespawn(60);
        }

        private void BattleTaunt()
        {
            string localString = "Taunt" + Main.rand.Next(1, 12);
            string taunt = LangText.Chat(this, localString);
            int combatText = CombatText.NewText(NPC.getRect(), Color.White, taunt, true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 250;
        }

        private void DeathTaunt()
        {
            string localString = "Death" + Main.rand.Next(1, 7);
            string taunt = LangText.Chat(this, localString);
            int combatText = CombatText.NewText(NPC.getRect(), Color.White, taunt, true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 250;
        }

        private void DrawGintzia(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;

            Color gintziaColor = Color.White.MultiplyRGB(drawColor);
            gintziaColor = gintziaColor.MultiplyRGB(Color.Gray);
            gintziaColor *= FadeProgress;

            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, NPC.frame, gintziaColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }
        private Color StripColors(float progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return Color.Lerp(Color.Transparent, Color.LightGray, EasingFunction.QuadraticBump(progressOnStrip)) * 0.5f;
        }

        private float StripWidth(float progressOnStrip)
        {
            float baseWidth = 80;
            return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            List<Vector2> gustpos = new List<Vector2>();
            Vector2 start = _keyPosition - Vector2.UnitX * 64;
            Vector2 end = _keyPosition + Vector2.UnitX * 64;
            float numPoints = 80f;
            for (float f = 0; f < numPoints; f++)
            {
                float lerpValue = f / numPoints;
                Vector2 gustPoint = Vector2.Lerp(end, start, lerpValue);
                gustpos.Add(gustPoint);
            }

            Vector2[] arr = gustpos.ToArray();
            float[] rot = new float[arr.Length];
            TrailDrawer.Draw(Main.spriteBatch, arr, rot, StripColors, StripWidth, shader);

            //Draw Key
            Texture2D keyTexture = TextureAssets.Item[ModContent.GetInstance<VoidKey>().Type].Value;
            Vector2 drawOrigin = keyTexture.Size() / 2f;
            Vector2 drawCenter = _keyPosition - screenPos;
            spriteBatch.Draw(keyTexture, drawCenter, null, drawColor, _keyRotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            //Draw glow
            Texture2D glowTexture = AssetManager.GlowMask.Shine.Value;
            drawOrigin = glowTexture.Size() / 2f;
            Color glowColor = Color.LightBlue;
            glowColor = Color.Lerp(glowColor, Color.Black, 0.8f);
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, _keyRotation, drawOrigin, NPC.scale * 0.15f, SpriteEffects.None, 0);


            DrawGintzia(spriteBatch, screenPos, drawColor);
            return false;
        }
    }
}
