using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK
{
    internal class RekSnakeIdle : ModNPC
    {
        //Draw Code
        private string BaseTexturePath => "Stellamod/NPCs/Bosses/GothiviaTheSun/REK/";
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float SegmentStretch = 0.66f;
        private float ChargeTrailOpacity;
        private bool DrawChargeTrail;

        //Segments
        private RekSegment Head => Segments[0];
        private RekSegment[] Segments;
        private Vector2 HitboxFixer => new Vector2(NPC.width, NPC.height) / 2;

        //AI
        private bool _spawnRek;
        private enum ActionState
        {
            Dormant
        }


        private ActionState State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 90;
            NPC.lifeMax = 126000;

            NPC.damage = 900;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }


        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Segments == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(Segments.Length);
                for (int i = 0; i < Segments.Length; i++)
                {
                    writer.WriteVector2(Segments[i].Position);
                    writer.WriteVector2(Segments[i].Velocity);
                }
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool hasSegments = reader.ReadBoolean();
            if (hasSegments)
            {
                int length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    Vector2 pos = reader.ReadVector2();
                    Vector2 vel = reader.ReadVector2();
                    if (Segments != null && Segments.Length <= length)
                    {
                        Segments[i].Position = pos;
                        Segments[i].Velocity = vel;
                    }
                }
            }
        }

        public override void AI()
        {
            if (Segments == null)
            {
                //Initialize Segments
                int bodySegments = 4;
                int bodFrontExtraSegments = 7;
                int bodyExtraSegments = 6;
                int tailSegments = 8;
                List<RekSegment> segments = new List<RekSegment>();
                //Set the textures

                //Head
                RekSegment segment = new RekSegment(NPC);
                segment.TexturePath = Texture;
                segment.Position = segment.Position + new Vector2(1, 0);
                segment.Size = new Vector2(204, 204);
                segments.Add(segment);

                //Neck
                segment = new RekSegment(NPC);
                segment.TexturePath = $"{BaseTexturePath}RekNeck";
                segment.Position = segment.Position + new Vector2(1, 0);
                segment.Size = new Vector2(74, 90);
                segments.Add(segment);

                for (int i = 0; i < bodySegments; i++)
                {
                    segment = new RekSegment(NPC);
                    segment.Position = segment.Position + new Vector2(1, 0);
                    segment.TexturePath = $"{BaseTexturePath}RekBody{i + 1}";
                    switch (i)
                    {
                        case 0:
                            segment.Size = new Vector2(118, 106);
                            break;
                        case 1:
                            segment.Size = new Vector2(118, 106);
                            break;
                        case 2:
                            segment.Size = new Vector2(84, 98);
                            break;
                        case 3:
                            segment.Size = new Vector2(68, 58);
                            break;
                        case 4:
                            segment.Size = new Vector2(62, 50);
                            break;
                        case 5:
                            segment.Size = new Vector2(36, 32);
                            break;
                        case 6:
                            segment.Size = new Vector2(36, 46);
                            break;
                    }

                    segments.Add(segment);
                }

                for (int i = 0; i < bodFrontExtraSegments; i++)
                {
                    segment = new RekSegment(NPC);
                    segment.Position = segment.Position + new Vector2(1, 0);
                    if (i % 2 == 0)
                    {
                        segment.TexturePath = $"{BaseTexturePath}RekBody2";
                        segment.Size = new Vector2(118, 106);
                    }
                    else
                    {
                        segment.TexturePath = $"{BaseTexturePath}RekBody3";
                        segment.Size = new Vector2(84, 98);
                    }

                    segments.Add(segment);
                }

                //Front Tail Segments
                for (int i = 0; i < bodyExtraSegments; i++)
                {
                    segment = new RekSegment(NPC);
                    segment.Position = segment.Position + new Vector2(1, 0);
                    string texturePath = $"{BaseTexturePath}RekBody4";
                    segment.Size = new Vector2(68, 58);
                    if (i > bodyExtraSegments / 2)
                    {
                        texturePath = $"{BaseTexturePath}RekBody5";
                        segment.Size = new Vector2(62, 50);
                    }

                    segment.TexturePath = texturePath;
                    segments.Add(segment);
                }

                //Tail Segments
                for (int i = 0; i < tailSegments; i++)
                {

                    float p = i;
                    float progress = p / tailSegments;
                    progress = 1 - progress;
                    string texturePath = $"{BaseTexturePath}RekBody6";
                    segment.Size = new Vector2(36, 32);
                    if (i > 2)
                    {
                        texturePath = $"{BaseTexturePath}RekBody7";
                        segment.Size = new Vector2(36, 46);
                    }
                    segment = new RekSegment(NPC);
                    segment.Position = segment.Position + new Vector2(1, 0);
                    segment.TexturePath = texturePath;
                    segment.Scale = Math.Max(0.5f, progress);
                    segments.Add(segment);
                }

                segment = new RekSegment(NPC);
                segment.Position = segment.Position + new Vector2(1, 0);
                segment.TexturePath = $"{BaseTexturePath}RekTail";
                segment.Size = new Vector2(88, 18);
                segments.Add(segment);
                Segments = segments.ToArray();
                NPC.TargetClosest();
                return;
            }

            if (StellaMultiplayer.IsHost && _spawnRek)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<RekSnake>());
                NPC.active = false;
                _spawnRek = false;
            }

            switch (State)
            {
                case ActionState.Dormant:
                    AI_OrbitGoth();
                    break;
            }
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8, float accel = 16)
        {
            //chase target
            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X += accel;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X -= accel;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y += accel;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y -= accel;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AI_OrbitGoth()
        {
            NPC gothNPC = null;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (npc.type == ModContent.NPCType<GothiviaIdle>())
                {
                    gothNPC = npc;
                    break;
                }
            }

            if (gothNPC == null)
                return;

            Vector2 gothiviaPosition = gothNPC.Center;
            float gothiviaOrbitRadius = 384;

            //Orbit Around
            SegmentStretch = MathHelper.Lerp(SegmentStretch, 0.66f, 0.1f);
            ResetSegmentGlow();
            Vector2 direction = gothiviaPosition.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 360);
            Vector2 targetCenter = gothiviaPosition + direction * gothiviaOrbitRadius;
            AI_MoveToward(targetCenter, 96);

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int lifeToAggro = (int)(NPC.lifeMax * 0.99f);
            if (NPC.life <= lifeToAggro && State == ActionState.Dormant)
            {
                _spawnRek = true;
            }
        }

        #region Draw Code

        private void GlowWhite(float speed)
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                RekSegment segment = Segments[i];
                if (segment.GlowWhite)
                {
                    segment.GlowTimer += speed;
                    if (segment.GlowTimer >= 1f)
                        segment.GlowTimer = 1f;
                }
                else
                {
                    segment.GlowTimer -= speed;
                    if (segment.GlowTimer <= 0)
                        segment.GlowTimer = 0;
                }
            }
        }

        private void MakeLikeWorm()
        {
            if (Segments == null)
                return;
            //Segments
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void StartSegmentGlow(Color color)
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                StartSegmentGlow(i, color);
            }
        }

        private void StartSegmentGlow(int index, Color color)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhiteColor = color;
            segment.GlowWhite = true;
        }

        private void StopSegmentGlow()
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                StopSegmentGlow(i);
            }
        }

        private void StopSegmentGlow(int index)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
        }

        private void ResetSegmentGlow()
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                ResetSegmentGlow(i);
            }
        }

        private void ResetSegmentGlow(int index)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
            segment.GlowTimer = 0;
        }

        private void MoveSegmentsLikeWorm()
        {
            if (Segments == null)
                return;
            for (int i = 1; i < Segments.Length; i++)
            {
                MoveSegmentLikeWorm(i);
            }
        }

        private void MoveSegmentLikeWorm(int index)
        {
            if (Segments == null)
                return;
            int inFrontIndex = index - 1;
            if (inFrontIndex < 0)
                return;

            ref var segment = ref Segments[index];
            ref var frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this NPC
            // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX);
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            if (length == 0)
                length = 1;

            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;
            }

            float dist = (length - segment.Size.X * SegmentStretch * fixer) / length;

            float posX = dirX * dist;
            float posY = dirY * dist;

            //reset the velocity
            segment.Velocity = Vector2.Zero;


            // And set this NPCs position accordingly to that of this NPCs parent NPC.
            segment.Position.X += posX;
            segment.Position.Y += posY;
        }


        public float WidthFunctionCharge(float completionRatio)
        {
            return NPC.width * NPC.scale * (1f - completionRatio) * 2f;
        }

        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

            return Color.Lerp(Color.Orange, Color.Orange, (1f - completionRatio)) * ChargeTrailOpacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Segments == null)
                return false;
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            Vector2 size = new Vector2(90, 90);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:BasicTrail"];
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale * segment.Scale;
                spriteBatch.Draw(segment.Texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Segments == null)
                return;
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                if (!ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_Glow", out var asset))
                    continue;

                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;

                float osc = VectorHelper.Osc(0, 1);

                spriteBatch.Draw(asset.Value, drawPosition, null, drawColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                if (segment.GlowTimer > 0 && ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_White", out var whiteAsset))
                {
                    spriteBatch.Draw(whiteAsset.Value, drawPosition, null, segment.GlowWhiteColor * segment.GlowTimer, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
        #endregion
    }
}
