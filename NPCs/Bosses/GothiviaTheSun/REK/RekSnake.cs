using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK
{
    internal class RekSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Vector2 Size => Texture.Size();
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale = 1f;
    }

    internal class RekSnake : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        private string BaseTexturePath => "Stellamod/NPCs/Bosses/GothiviaTheSun/REK/";
        private float SegmentStretch = 0.66f;

        //Segments
        RekSegment Head => Segments[0];
        RekSegment[] Segments;
        Vector2 HitboxFixer => new Vector2(NPC.width, NPC.height) / 2;

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
            NPC.damage = 100;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;

            //Initialize Segments
            int bodySegments = 6;
            int bodyExtraSegments = 8;
            int tailSegments = 8;
            Segments = new RekSegment[2 + bodySegments + bodyExtraSegments + tailSegments + 1];
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i] = new RekSegment();
                Segments[i].Position = NPC.position;
                Segments[i].Rotation = 0;
                Segments[i].Velocity = Vector2.Zero;
            }

            //Set the textures
            Segments[0].TexturePath = Texture;
            Segments[1].TexturePath = $"{BaseTexturePath}RekNeck";
            for(int i = 0; i < bodySegments; i++)
            {
                Segments[2 + i].TexturePath = $"{BaseTexturePath}RekBody{i+1}";
            }

            for(int i = 0; i < bodyExtraSegments; i++)
            {
                string texturePath = $"{BaseTexturePath}RekBody4";
                if (i > bodyExtraSegments / 2)
                {
                    texturePath = $"{BaseTexturePath}RekBody5";
                }

                Segments[2 + bodySegments + i].TexturePath = texturePath;
            }

       
            for(int i = 0; i < tailSegments; i++)
            {
                float p = i;
                float progress = p / (float)tailSegments;
                progress = 1 - progress;
                string texturePath = $"{BaseTexturePath}RekBody6";
                if(i > 2)
                {
                    texturePath =  $"{BaseTexturePath}RekBody7";
                }

                var segment = Segments[2 + bodySegments + bodyExtraSegments + i];
                segment.TexturePath = texturePath;
                segment.Scale = Math.Max(0.33f, progress);
            }

            Segments[Segments.Length-1].TexturePath = $"{BaseTexturePath}RekTail";
        }

        public override void AI()
        {
            Timer++;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            Vector2 directionToTarget = NPC.Center.DirectionTo(target.Center);
            NPC.velocity = directionToTarget * 8;
            NPC.rotation = NPC.velocity.ToRotation();

            //Segments
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
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


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
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
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
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
            }
        }
    }
}
