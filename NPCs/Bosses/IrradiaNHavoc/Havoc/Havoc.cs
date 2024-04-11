using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc
{
    internal class HavocSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Vector2 Size => Texture.Size();
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
    }
    internal class Havoc : ModNPC
    {

        private enum ActionState
        {
            Idle,
            Charge,
            Laser,
            Laser_Big,
            Circle
        }

        //AI
        ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }

        float Timer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        float OrbitTimer;
        Vector2 ArenaCenter;

        //Draw Code
        //Segment Positions;
        HavocSegment[] Segments;
        float SegmentStretch = 1.8f;
        float StartSegmentStretch;


        //Attacks
        //Charge
        Vector2 ChargeDirection;

        //Textures
        public const string BaseTexturePath = "Stellamod/NPCs/Bosses/IrradiaNHavoc/Havoc/";
        private HavocSegment Head => Segments[0];
        private HavocSegment BodyFront => Segments[1];
        private HavocSegment BodyMiddle => Segments[2];
        private HavocSegment BodyBack => Segments[3];
        private HavocSegment BodyTail => Segments[4];
        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 90;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.boss = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;

            //Initialize Segments
            Segments = new HavocSegment[5];
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i] = new HavocSegment();
                Segments[i].Position = NPC.position;
                Segments[i].Rotation = 0;
                Segments[i].Velocity = Vector2.Zero;
            }

            //Set the textures
            Segments[0].TexturePath = Texture;
            Segments[1].TexturePath = $"{BaseTexturePath}HavocSegmentFront";
            Segments[2].TexturePath = $"{BaseTexturePath}HavocSegmentMiddle";
            Segments[3].TexturePath = $"{BaseTexturePath}HavocSegmentBack";
            Segments[4].TexturePath = $"{BaseTexturePath}HavocTail";
        }

        public override void AI()
        {
 
            if(ArenaCenter == default(Vector2))
            {
                ArenaCenter = NPC.position;
            }

            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Charge:
                    AI_Charge();
                    break;
                case ActionState.Laser:
                    AI_Laser();
                    break;
                case ActionState.Laser_Big:
                    AI_LaserBig();
                    break;
                case ActionState.Circle:
                    AI_Circle();
                    break;
            }

            //This controls how far apart the segments are
            //Set to 1 if you want them to be touching each other, any number bigger than 1 spaces them out,
            //Smaller than 1 makes them overlap


            //Set head position and rotation
            //If using the worm like movement, don't forget to set the head position and rotation before calling that MoveSegments function

        }

        private void AI_Idle()
        {
            NPC.TargetClosest();

            //Orbit Around
            float orbitDistance = 500;
            float speed = 20;
            OrbitTimer += 0.01f;
            Vector2 targetCenter = MovementHelper.OrbitAround(ArenaCenter, Vector2.UnitY, orbitDistance, OrbitTimer);
            Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetCenter, speed);
            NPC.velocity = targetVelocity;
            NPC.rotation = targetVelocity.ToRotation();
            SegmentStretch = MathHelper.Lerp(SegmentStretch, 1.8f, 0.03f);

            Timer++;
            if(Timer >= 500)
            {
                State = ActionState.Charge;
                Timer = 0;
            }

            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void AI_Charge()
        {
            Player target = Main.player[NPC.target];
            Timer++;

            float orbitDistance = MathHelper.Lerp(500, 300, Timer / 150);
            if (Timer < 100)
            {
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                    StartSegmentStretch = SegmentStretch;
                }

                //Ease in
                float progress = Timer / 100;
                float easedProgress = Easing.InOutCubic(progress);
                SegmentStretch = MathHelper.Lerp(StartSegmentStretch, 1f, easedProgress);

                ChargeDirection = NPC.Center.DirectionTo(target.Center);

               
                float speed = 20;
                OrbitTimer += 0.02f;
                Vector2 targetCenter = MovementHelper.OrbitAround(ArenaCenter, Vector2.UnitY, orbitDistance, OrbitTimer);
                Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetCenter, speed);
                NPC.velocity = targetVelocity;
                NPC.velocity *= 0.8f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, ChargeDirection.ToRotation(), 0.08f);
            } 
            else if (Timer < 150)
            {
                ChargeDirection = NPC.Center.DirectionTo(target.Center);
                float speed = 20;
                OrbitTimer += 0.02f;
                Vector2 targetCenter = MovementHelper.OrbitAround(ArenaCenter, Vector2.UnitY, orbitDistance, OrbitTimer);
                Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetCenter, speed);
                NPC.velocity = targetVelocity;
                NPC.velocity *= 0.3f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, ChargeDirection.ToRotation(), 0.08f);
            } 
            else if(Timer == 151)
            {
                NPC.velocity = ChargeDirection * 40;
            } 
            else if (Timer < 230)
            {
                float timer = Timer - 151;
                float maxTimer = 230 - 151;

                float progress = timer / maxTimer;
                float easedProgress = Easing.InOutCubic(progress);
                SegmentStretch = MathHelper.Lerp(1f, 3f, easedProgress);
            }
            else if (Timer == 230)
            {
                State = ActionState.Idle;
                Timer = 0;
            }

            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void AI_Laser()
        {

        }

        private void AI_LaserBig()
        {

        }

        private void AI_Circle()
        {

        }

        public override void PostAI()
        {
            //Move segments according to velocity
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].Position += Segments[i].Velocity;
            }
        }

        private void MoveSegmentsInLine(Vector2 direction)
        {
            for(int i = 1; i < Segments.Length; i++)
            {
                ref HavocSegment segment = ref Segments[i];
                ref HavocSegment frontSegment = ref Segments[i - 1];
                Vector2 offset = direction * segment.Size.X * SegmentStretch;
                Vector2 targetPosition = frontSegment.Position + offset;
                segment.Position = Vector2.Lerp(segment.Position, targetPosition, 0.5f);
            }
        }

        private void MoveSegmentsLikeWorm()
        {
            for (int i = 1; i < Segments.Length; i++)
            {
                MoveSegmentLikeWorm(i);
            }
        }

        private void MoveSegmentLikeWorm(int index)
        {
            int inFrontIndex = index - 1;
            if (inFrontIndex < 0)
                return;

            ref HavocSegment segment = ref Segments[index];
            ref HavocSegment frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this NPC
            // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX) * 0.33f;
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;

                //Unbreak that rotation
                segment.Rotation *= 3;
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

        public float WidthFunction(float completionRatio)
        {
            return NPC.width * NPC.scale / 4.2f;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * NPC.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        internal PrimitiveTrail BeamDrawer;
        Vector2 HitboxFixer = new Vector2(90, 90) / 2;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Chain
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailRegistry.LaserShader.UseColor(Color.LightGoldenrodYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);
            for (int i = 1; i < Segments.Length; i++)
            {
                HavocSegment segment = Segments[i - 1];
                HavocSegment nextSegment = Segments[i];
                List<Vector2> points = new();
                for (int j = 0; j <= 8; j++)
                {
                    points.Add(Vector2.Lerp(segment.Position, nextSegment.Position, j / 8f));
                }
                BeamDrawer.Draw(points, -Main.screenPosition + HitboxFixer, 32);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                HavocSegment segment = Segments[i];
                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;
                spriteBatch.Draw(segment.Texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                HavocSegment segment = Segments[i];
                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;

                float osc = VectorHelper.Osc(0, 1);
                spriteBatch.Draw(segment.GlowTexture, drawPosition, null, drawColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
