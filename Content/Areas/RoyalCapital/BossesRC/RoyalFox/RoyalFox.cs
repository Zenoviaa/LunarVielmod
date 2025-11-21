using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox
{

    /// <summary>
    /// Represents the degrees rotation of this frame, going counter clockwise
    /// </summary>
    public enum FoxDegrees : byte
    {
        _0_CC,
        _45_CC,
        _90_CC,
        _135_CC,
        _180_CC,
        _225_CC,
        _270_CC,
        _315_CC
    }

    /// <summary>
    /// Manages a single segment of Fenix
    /// </summary>
    public class FoxSegment
    {
        //Represents a body part
        public FoxSegment(Texture2D texture, FoxSegment parent, float length, float angle, Vector2 origin)
        {
            this.texture = texture;
            this.parent = parent;
            this.length = length;
            this.angle = angle;
            this.origin = origin;
            this.scale = Vector2.One;

            drawColor = Color.White;
            frameWidth = texture.Width;
            frameHeight = texture.Height / Num_Perspectives;
            perspectiveRotation = FoxDegrees._0_CC;
            attachmentPoint = 1f;
        }
        public const int Num_Perspectives = 8;


        public Texture2D texture;
        public Color drawColor;
        public Rectangle? frame;
        public FoxSegment parent;

        public Vector3 position;
        public Vector3 initialPosition;
        public Vector2 worldPosition;

        public Vector2 origin;
        public Vector2 scale;

        public float length;
        public float angle;
        public float drawAngleOffset;
        public float attachmentPoint;
        public bool flipX;
        public float localLength;
        public float localHeight;
        public float drawHeight;
        public float localAngle;

        public float xAxisRotation;
        public int frameHeight;
        public int frameWidth;
        public FoxDegrees perspectiveRotation;

        private void SetPerspective()
        {

            //We need to have different lengths depending on the perpsective
            //We'll store it in an array
            int directionIndex = (int)perspectiveRotation;

            //So now do the same hting but for the frame
            //The assets have 8 frames be default so
            int y = directionIndex * frameHeight;
            frame = new Rectangle(0, y, frameWidth, frameHeight);

            float xRotation = GetFullXRotation();
            Vector3 axis = new Vector3(1, 0, 0);
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, xRotation);
            Vector3 currentVector = new Vector3(0, 1, 0);
            currentVector = Vector3.Transform(currentVector, quaternion);

            for (int i = 0; i < 8; i++)
            {
                FoxDegrees degrees = (FoxDegrees)i;
                if(SetIfInPerspective2(degrees, currentVector))
                {
                    break;
                }
            }
        }


        public int GetChildIndex()
        {
            int index = 0;
            FoxSegment next = parent;
            while (next != null)
            {
                index++;
                next = next.parent;
            }
            return index;
        }
        private bool SetIfInPerspective2(FoxDegrees degrees, Vector3 currentVector)
        {
            Vector3 forwardVector = new Vector3(0, 1, 0);
            currentVector.Y = MathF.Round(currentVector.Y);
            currentVector.Z = MathF.Round(currentVector.Z);
         
            switch (degrees)
            {
                default:
                case FoxDegrees._0_CC:
                    forwardVector = new Vector3(0, 1, 0);
                    break;
                case FoxDegrees._45_CC:
                    forwardVector = new Vector3(0, 1, 1);
                    break;
                case FoxDegrees._90_CC:
                    forwardVector = new Vector3(0, 0, 1);
                    break;
                case FoxDegrees._135_CC:
                    forwardVector = new Vector3(0, -1, 1);
                    break;
                case FoxDegrees._180_CC:
                    forwardVector = new Vector3(0, -1, 0);
                    break;
                case FoxDegrees._225_CC:
                    forwardVector = new Vector3(0, -1, -1);
                    break;
                case FoxDegrees._270_CC:
                    forwardVector = new Vector3(0, 0, -1);
                    break;
                case FoxDegrees._315_CC:
                    forwardVector = new Vector3(0, 1, -1);
                    break;
            }

            if (currentVector == forwardVector)
            {
                perspectiveRotation = degrees;
                return true;
            }
            return false;

        }

        private void SetDrawColor()
        {
            if (position.Z <= 0)
            {
                drawColor = Color.White;
            }
            else if (position.Z >= 12)
            {
                drawColor = Color.Lerp(Color.White, Color.Black, 0.75f);
            }
        }
        public FoxSegment GetRoot()
        {
            FoxSegment root = this;
            while (root.parent != null)
                root = root.parent;
            return root;
        }

        public float GetFullXRotation()
        {
            float xRot = xAxisRotation;
            FoxSegment next = parent;
            while(next != null)
            {
                xRot += next.xAxisRotation;
                next = next.parent;
            }
            return xRot;
        }
        public void Update()
        {
            //Calculate frame based on the direction
    
            Vector3 combinedPosition = initialPosition;
          
            float xRotation = GetFullXRotation();
            //Butt = 0, 0, 0,
            //Thigh = 0, 2, -1
            //Leg = Thigh + 0, 10, 0
            //Foot = Leg + 0, 5, 0

            Vector3 axis = new Vector3(1, 0, 0);
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, xRotation);

            //So let's break this down
            //First we're going to rotate the position of the leg locally, along the X axis
            //We have an initial position to work with, which is just the leg extended downward
            //We can just rotate around 0,0,0 since the rig is based there, think of this like vertices in a model

            Vector3 rotatedPosition = Vector3.Transform(combinedPosition, quaternion);
      
            //Now that we have the position relative to the rotation that we want
            //We can apply other transformations
            position = rotatedPosition;

            Vector2 rootPosition = GetRoot().worldPosition;
            worldPosition = rootPosition + new Vector2(position.X, position.Y);
            
            
            SetPerspective();
            SetDrawColor();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D textureToDraw = texture;
         
            Vector2 drawPosition = worldPosition - screenPos;

            float upAngle = angle - MathHelper.PiOver2;
            Vector2 upVector = upAngle.ToRotationVector2();
            upVector *= drawHeight;
            drawPosition += upVector;

            Color finalColor = drawColor.MultiplyRGB(lightColor);

            Vector2 drawOrigin = origin;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (flipX)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                if (frame != null)
                {
                    drawOrigin.X = frame.Value.Width - origin.X;
                }
                else
                {
                    drawOrigin.X = texture.Width - origin.X;
                }
            }

            float drawAngle = angle + drawAngleOffset;
            spriteBatch.Draw(textureToDraw, drawPosition, frame, finalColor, drawAngle, drawOrigin, scale, spriteEffects, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float offset = 2;
            Draw(spriteBatch, screenPos + Vector2.UnitX * offset, lightColor);
            Draw(spriteBatch, screenPos - Vector2.UnitX * offset, lightColor);
            Draw(spriteBatch, screenPos + Vector2.UnitY * offset, lightColor);
            Draw(spriteBatch, screenPos - Vector2.UnitY * offset, lightColor);
        }
    }
    public class FoxSegmentComparer : IComparer<FoxSegment>
    {
        public int Compare(FoxSegment x, FoxSegment y)
        {
            return y.position.Z.CompareTo(x.position.Z);
        }
    }
    public class FoxChildComparer : IComparer<FoxSegment>
    {
        public int Compare(FoxSegment x, FoxSegment y)
        {
            return y.GetChildIndex().CompareTo(x.GetChildIndex());
        }
    }
    public class RoyalFoxRig
    {
        public FoxSegment[] segmentsByZLayer;
        public FoxSegment[] segmentsByNumberOfParents;

        public FoxSegmentComparer zComparer;
        public FoxChildComparer parentsComparer;

        public readonly FoxSegment rootSegment;

        public readonly FoxSegment backLegFrontThighSegment;
        public readonly FoxSegment backLegBehindThighSegment;
        public readonly FoxSegment frontLegFrontThighSegment;
        public readonly FoxSegment frontLegBehindThighSegment;

        public readonly FoxSegment[] backFrontLeg;
        public readonly FoxSegment[] backBehindLeg;
        public readonly FoxSegment[] frontFrontLeg;
        public readonly FoxSegment[] frontBehindLeg;
        public readonly FoxSegment[] bodyParts;
        public readonly FoxSegment headPart;
        public float LegDepth => 17;
        public RoyalFoxRig
            (Texture2D[] backLeg,
            Texture2D[] frontLeg,
            Texture2D[] body,
            Texture2D head)
        {
            zComparer = new FoxSegmentComparer();
            parentsComparer = new FoxChildComparer();
            //This needs to be built from back t o front
            //So we should start with the butt
            //Only segment with no parent
            FoxSegment butt = new FoxSegment(body[0], null, 25, 0, new Vector2(33, 27));
 

            FoxSegment mid = new FoxSegment(body[1], butt, 40, 0, new Vector2(22, 20));
            mid.initialPosition = new Vector3(25, 4, -0.01f);

            FoxSegment body3 = new FoxSegment(body[2], mid, 40, 0, new Vector2(36, 26));
            body3.initialPosition = new Vector3(62, 0, -0.02f);

            FoxSegment neck = new FoxSegment(body[3], body3, 40, 0, new Vector2(25, 20));
            neck.initialPosition = new Vector3(108, -4, 0.01f);

            FoxSegment headSegment = new FoxSegment(head, neck, 40, 0, new Vector2(48, 42));
            headSegment.initialPosition = new Vector3(140, -13, -0.05f);

            bodyParts = new FoxSegment[5];
            bodyParts[0] = butt;
            bodyParts[1] = mid;
            bodyParts[2] = body3;
            bodyParts[3] = neck;
            bodyParts[4] = headSegment;

            //Create back leg
            backFrontLeg = CreateBackLeg(backLeg, butt, false);
            backBehindLeg = CreateBackLeg(backLeg, butt, isBehind: true);
            backBehindLeg[0].attachmentPoint += 0.75f;

            MakeBehind(backBehindLeg);
            MakeFront(backFrontLeg);

            //Create front leg
            frontFrontLeg = CreateFrontLeg(frontLeg, body3, false);
            frontBehindLeg = CreateFrontLeg(frontLeg, body3, true);
            frontBehindLeg[0].attachmentPoint -= 0.25f;

            MakeBehind(frontBehindLeg);
            MakeFront(frontFrontLeg);

            MakeMiddle(bodyParts);

            //Create the segments list
            List<FoxSegment> segmentsList = new List<FoxSegment>();
            segmentsList.Add(butt);
            segmentsList.Add(mid);
            segmentsList.Add(body3);
            segmentsList.Add(neck);
            segmentsList.Add(headSegment);
            segmentsList.AddRange(backFrontLeg);
            segmentsList.AddRange(backBehindLeg);
            segmentsList.AddRange(frontFrontLeg);
            segmentsList.AddRange(frontBehindLeg);

            rootSegment = butt;
            
            backLegFrontThighSegment = backFrontLeg[0];
            backLegBehindThighSegment = backBehindLeg[0];
            frontLegFrontThighSegment = frontFrontLeg[0];
            frontLegBehindThighSegment = frontBehindLeg[0];
            headPart = headSegment;
            headPart.localHeight = 10;
            headPart.localLength = 0;
            segmentsByZLayer = segmentsList.ToArray();


            segmentsByNumberOfParents = segmentsList.ToArray();
            Array.Sort(segmentsByNumberOfParents, parentsComparer);
        }

        public void MakeBehind(FoxSegment[] segments)
        {
            Color darkenColor = Color.Lerp(Color.White, Color.Black, 0.6f);
            SetColor(segments, darkenColor);
        }

        public void MakeMiddle(FoxSegment[] segments)
        {
            Color darkenColor = Color.White;
            SetColor(segments, darkenColor);
        }

        public void MakeFront(FoxSegment[] segments)
        {
            Color darkenColor = Color.White;
            SetColor(segments, darkenColor);
        }

        public void SetColor(FoxSegment[] segments, Color color)
        {

            for(int i = 0; i < segments.Length; i++)
            {
                FoxSegment segment = segments[i];
                segment.drawColor = color;
            }
        }


        public FoxSegment[] CreateBackLeg(Texture2D[] backLeg, FoxSegment butt, bool isBehind)
        {
            FoxSegment[] segments = new FoxSegment[3];
            FoxSegment backLegThighFront = new FoxSegment(backLeg[0], butt, 20, 0, new Vector2(19, 25));

            float depth = LegDepth;
            backLegThighFront.initialPosition = new Vector3(0, -7, -depth);

            //backLegThighFront.attachmentPoint = 0f;
            //backLegThighFront.localHeight = 16;

            FoxSegment backLegLegFront = new FoxSegment(backLeg[1], backLegThighFront, 35, 0, new Vector2(19, 45));
            backLegLegFront.initialPosition = backLegThighFront.initialPosition + new Vector3(5, 23, 0.1f);
            //backLegLegFront.drawAngleOffset = MathHelper.ToRadians(-90);
            //backLegLegFront.attachmentPoint = 0.5f;
            //backLegLegFront.localLength = 25;

            FoxSegment backFootFront = new FoxSegment(backLeg[2], backLegLegFront, 20, 0, new Vector2(7, 6));
            backFootFront.initialPosition = backLegLegFront.initialPosition + new Vector3(0, 32, -0.1f);

            segments[0] = backLegThighFront;
            segments[1] = backLegLegFront;
            segments[2] = backFootFront;
            if (isBehind)
            {
                for(int i = 0; i < segments.Length; i++)
                {
                    segments[i].initialPosition.X += 8;
                    segments[i].initialPosition.Z *= -1;
                }
            }
            return segments;
        }

        public FoxSegment[] CreateFrontLeg(Texture2D[] frontLeg, FoxSegment neck, bool isBehind)
        {
            FoxSegment[] segments = new FoxSegment[3];

            float depth = LegDepth;

            FoxSegment frontLegThighFront = new FoxSegment(frontLeg[0], neck, 5, 0, new Vector2(8, 21));
            frontLegThighFront.initialPosition = new Vector3(90, 3, -depth);

            FoxSegment frontLegLegFront = new FoxSegment(frontLeg[1], frontLegThighFront, 40, 0, new Vector2(12, 40));
            frontLegLegFront.initialPosition = frontLegThighFront.initialPosition + new Vector3(5, 13, 0.1f);

            FoxSegment frontLegFootFront = new FoxSegment(frontLeg[2], frontLegLegFront, 20, 0, new Vector2(7, 6));
            frontLegFootFront.initialPosition = frontLegLegFront.initialPosition + new Vector3(0, 32, 0f);

            segments[0] = frontLegThighFront;
            segments[1] = frontLegLegFront;
            segments[2] = frontLegFootFront;

            if (isBehind)
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    segments[i].initialPosition.X += 8;
                    segments[i].initialPosition.Z *= -1;
                }
            }
            return segments;
        }

        public void Update()
        {
            for (int i = 0; i < segmentsByNumberOfParents.Length; i++)
            {
                var segment = segmentsByNumberOfParents[i];
                segment.Update();
            }

            //Sort by the sorting order
            Array.Sort(segmentsByZLayer, zComparer);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            for (int i = 0; i < segmentsByZLayer.Length; i++)
            {
                var segment = segmentsByZLayer[i];
                segment.Draw(spriteBatch, screenPos, lightColor);
            }
        }
    }
    public partial class RoyalFox : ScarletBoss
    {
        private RoyalFoxRig _rigBackingField;
        private RoyalFoxRig Rig
        {
            get
            {
                _rigBackingField ??= CreateRig();
                return _rigBackingField;
            }
        }

        private ref float Timer => ref NPC.ai[0];
        private enum AIState
        {
            Spawn,
            Despawn,
            Idle
        }

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public Texture2D GetSubTexture(string fileName)
        {
            string path = Texture + $"_{fileName}";
            return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
        }

        public RoyalFoxRig CreateRig()
        {
            Texture2D[] backLegTextures = new Texture2D[3];
            backLegTextures[0] = GetSubTexture("BackThigh");
            backLegTextures[1] = GetSubTexture("BackLeg");
            backLegTextures[2] = GetSubTexture("Foot");

            Texture2D[] frontLegTextures = new Texture2D[3];
            frontLegTextures[0] = GetSubTexture("FrontThigh");
            frontLegTextures[1] = GetSubTexture("FrontLeg");
            frontLegTextures[2] = GetSubTexture("Foot");

            Texture2D head = GetSubTexture("Head");

            Texture2D[] bodyTextures = new Texture2D[4];
            bodyTextures[0] = GetSubTexture("Body3");
            bodyTextures[1] = GetSubTexture("Body2");
            bodyTextures[2] = GetSubTexture("Body1");
            bodyTextures[3] = GetSubTexture("Neck");

            var rig = new RoyalFoxRig(backLegTextures, frontLegTextures, bodyTextures, head);
            return rig;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 200;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 24000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AlcaricFox");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void AI()
        {
            base.AI();
            NPC.velocity.X *= 0;
            UpdateRig();
        }


        private void UpdateRig()
        {

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                Rig.rootSegment.xAxisRotation -= MathHelper.PiOver4;
                Main.mouseLeftRelease = false;
            }
            if (Main.mouseRightRelease && Main.mouseRight)
            {
                Rig.rootSegment.xAxisRotation += MathHelper.PiOver4;
                Main.mouseRightRelease = false;
            }

            if (Main.mouseMiddle)
            {
                Rig.rootSegment.xAxisRotation = 0;
            }
            Rig.rootSegment.worldPosition = NPC.Center;

            //Calling update twice sine it has to calculate the new x axis position
            //Yeah this is technically inefficient but it's too inexpensive to matter, quick and dirty solution :p
            Rig.Update();
            Rig.Update();


        }

        public override void OnKill()
        {
            base.OnKill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rig.Draw(spriteBatch, screenPos, drawColor);
            return false;
        }
    }
}
