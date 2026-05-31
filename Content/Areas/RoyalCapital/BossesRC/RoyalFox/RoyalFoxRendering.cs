using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox
{
    /// <summary>
    /// Manages a single segment of Fenix
    /// </summary>
    public class FoxSegment
    {
        //Represents a body part
        public FoxSegment(Texture2D texture, FoxSegment parent, Vector2 origin)
        {
            this.texture = texture;
            this.parent = parent;
            this.origin = origin;
            this.scale = Vector2.One;

            drawColor = Color.White;
            frameWidth = texture.Width;
            frameHeight = texture.Height / Num_Perspectives;
            perspectiveRotation = FoxDegrees._0_CC;

            children = new List<FoxSegment>(Default_Capacity);
            initialForwardVectors = new List<Vector3>(Default_Capacity);
            forwardVectors = new List<Vector3>(Default_Capacity);
            alpha = 1f;
            //Add to the parent
            parent?.children.Add(this);
            parent?.initialForwardVectors.Add(Vector3.UnitX);
            parent?.forwardVectors.Add(Vector3.UnitX);
        }

        public const int Num_Perspectives = 8;
        public const int Default_Capacity = 3;

        public Texture2D texture;
        public Color drawColor;
        public float alpha;
        public Rectangle? frame;
        public FoxSegment parent;

        //Keep track of all of our children and our relationships to them
        public List<FoxSegment> children;
        public List<Vector3> initialForwardVectors;
        public List<Vector3> forwardVectors;
        public float angleOffset;
        public Vector3 position;
        public Vector3 initialPosition;
        public Vector2 worldPosition;

        public Vector2 origin;
        public Vector2 scale;



        public bool flipX;


        public Vector4 eulerAngles;
        public float fullRotation;
        public bool useFreeAngle;
        public float angle;
        public float fakeAngle;
        public int frameHeight;
        public int frameWidth;
        public FoxDegrees perspectiveRotation;
        public bool noDarken;

        private void SetPerspective()
        {

            //We need to have different lengths depending on the perpsective
            //We'll store it in an array
            int directionIndex = (int)perspectiveRotation;

            //So now do the same hting but for the frame
            //The assets have 8 frames be default so
            int y = directionIndex * frameHeight;
            frame = new Rectangle(0, y, frameWidth, frameHeight);

            float xRotation = GetFullEulerAngles().X;
            Vector3 axis = new Vector3(1, 0, 0);
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, xRotation);
            Vector3 currentVector = new Vector3(0, 1, 0);
            currentVector = Vector3.Transform(currentVector, quaternion);

            for (int i = 0; i < 8; i++)
            {
                FoxDegrees degrees = (FoxDegrees)i;
                if(SetIfInPerspective(degrees, currentVector))
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
        private bool SetIfInPerspective(FoxDegrees degrees, Vector3 currentVector)
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
            else if (position.Z >= 12 && !noDarken)
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

        public Vector4 GetFullEulerAngles()
        {
            Vector4 angles = eulerAngles;
            FoxSegment next = parent;
            while (next != null)
            {
                angles += next.eulerAngles;
                next = next.parent;
            }
            return angles;
        }

        public void ResetTransformations()
        {
            //Set to the initial positions of the rig
            //Default to facing right if there's no parent
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                Vector3 initialForwardVector = child.initialPosition - initialPosition;
                initialForwardVectors[i] = initialForwardVector;
                forwardVectors[i] = initialForwardVector;
            }
        }

        public void ApplyEulerAngles()
        {
            Vector4 eulerAngles = GetFullEulerAngles();

            //We have 2 z transformations here, which may be a bit confusing
            //This is because the first z transformation happens in 2D space,
            //The second Z tranformation is in 3d space and actually gives us the full range of rotations
            ApplyZTransformations(eulerAngles.Z);
            ApplyXTransformations(eulerAngles.X);
            ApplyYTransformations(eulerAngles.Y);
            ApplyZTransformations(eulerAngles.W);
        }
        public void ApplyZTransformations(float zRotation)
        {
            //Let's just split it, I think we're getting race conditioned?
            //So first we apply the z rotation to the forward vector so it rotates around the joints properly
            Vector3 zAxis = new Vector3(0, 0, 1);
            Quaternion rotation = Quaternion.CreateFromAxisAngle(zAxis, zRotation);
            for(int i = 0; i < children.Count; i++)
            {
                forwardVectors[i] = Vector3.Transform(forwardVectors[i], rotation);
            }
        }
        public void ApplyYTransformations(float yRotation)
        {
            Vector3 yAxis = new Vector3(0, 1, 0);
            Quaternion yQuaternion = Quaternion.CreateFromAxisAngle(yAxis, yRotation);
            for (int i = 0; i < children.Count; i++)
            {
                forwardVectors[i] = Vector3.Transform(forwardVectors[i], yQuaternion);
            }
        }

        public void ApplyXTransformations(float xRotation)
        {
            Vector3 xAxis = new Vector3(1, 0, 0);
            Quaternion xQuaternion = Quaternion.CreateFromAxisAngle(xAxis, xRotation);
            for (int i = 0; i < children.Count; i++)
            {
                forwardVectors[i] = Vector3.Transform(forwardVectors[i], xQuaternion);
            }
        }

        public Vector3 GetForwardVector(FoxSegment child)
        {
            int indexOfChild = children.IndexOf(child);
            if (indexOfChild == -1)
                return new Vector3(1, 0, 0);
            return forwardVectors[indexOfChild];
        }
        public void SetWorldTransformations()
        {
            if(parent != null)
            {
                Vector3 forwardVector = parent.GetForwardVector(this);
                position = parent.position + forwardVector;
                if (children.Count > 0)
                {
                    Vector3 rotationVector = forwardVectors[0];
                    angle = MathF.Atan2(rotationVector.Y, rotationVector.X);
                    
                }
                else
                {
                    angle = MathF.Atan2(forwardVector.Y, forwardVector.X);
                }

                if(angleOffset != 0 && !useFreeAngle)
                {
                    //225
                    //180
                    //135
                    float direction = 1;
                    switch (perspectiveRotation)
                    {
                        case FoxDegrees._225_CC:
                        case FoxDegrees._180_CC:
                        case FoxDegrees._135_CC:
                            direction = -1;
                            break;
                    }

                    Vector4 eulers = GetFullEulerAngles();
                    angle += angleOffset * direction;
                    if (perspectiveRotation == FoxDegrees._270_CC || perspectiveRotation == FoxDegrees._90_CC)
                    {
                        angle = eulers.W;
                    }
                }
            } 
            else
            {
                position = Vector3.Zero;
                if (children.Count > 0)
                {
                    Vector3 rotationVector = forwardVectors[0];
                    angle = MathF.Atan2(rotationVector.Y, rotationVector.X);

                }
            }

   
            Vector2 rootPosition = GetRoot().worldPosition;
            worldPosition = rootPosition + new Vector2(position.X, position.Y);
        }

        public void Update()
        {
      
            SetWorldTransformations();
            SetPerspective();
            SetDrawColor();
        }




        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D textureToDraw = texture;
         
            Vector2 drawPosition = worldPosition - screenPos;


            Color finalColor = drawColor.MultiplyRGBA(lightColor) * alpha;

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

            float drawAngle = angle + fakeAngle;

            //Just calculate the angle based on the direction
            spriteBatch.Draw(textureToDraw, drawPosition, frame, finalColor, drawAngle, drawOrigin, scale, spriteEffects, 0);
            //DrawWireframe(spriteBatch, drawPosition);
        }

        private void DrawWireframe(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Vector3 forwardVector = Vector3.Zero;
            if (children.Count > 0)
                forwardVector = GetForwardVector(children[0]);
            Vector2 start = drawPosition;
            Vector2 end = start + new Vector2(forwardVector.X, forwardVector.Y);

    
            Primitives2D.DrawCircle(spriteBatch, start, 4, 8, Color.Red);
            Primitives2D.DrawLine(spriteBatch, start, end, Color.Wheat);
            Primitives2D.DrawCircle(spriteBatch, end, 4, 8, Color.Red);
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
}


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
        FoxSegment butt = new FoxSegment(body[0], null, new Vector2(33, 27));


        FoxSegment mid = new FoxSegment(body[1], butt, new Vector2(22, 20));
        mid.initialPosition = new Vector3(25, 0, -0.01f);

        FoxSegment body3 = new FoxSegment(body[2], mid, new Vector2(36, 26));
        body3.initialPosition = new Vector3(62, 0, -0.02f);

        FoxSegment neck = new FoxSegment(body[3], body3, new Vector2(25, 20));
        neck.initialPosition = new Vector3(108, 0, 0.01f);

        FoxSegment headSegment = new FoxSegment(head, neck, new Vector2(48, 42));
        headSegment.initialPosition = new Vector3(140, -13, -0.05f);
        headSegment.noDarken = true;

        bodyParts = new FoxSegment[5];
        bodyParts[0] = butt;
        bodyParts[1] = mid;
        bodyParts[2] = body3;
        bodyParts[3] = neck;
        bodyParts[4] = headSegment;



        //Create back leg
        backFrontLeg = CreateBackLeg(backLeg, butt, false);
        backBehindLeg = CreateBackLeg(backLeg, butt, isBehind: true);

        MakeBehind(backBehindLeg);
        MakeFront(backFrontLeg);

        //Create front leg
        frontFrontLeg = CreateFrontLeg(frontLeg, body3, false);
        frontBehindLeg = CreateFrontLeg(frontLeg, body3, true);

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

        for (int i = 0; i < segments.Length; i++)
        {
            FoxSegment segment = segments[i];
            segment.drawColor = color;
        }
    }


    public FoxSegment[] CreateBackLeg(Texture2D[] backLeg, FoxSegment butt, bool isBehind)
    {
        FoxSegment[] segments = new FoxSegment[3];
        FoxSegment backLegThighFront = new FoxSegment(backLeg[0], butt, new Vector2(19, 25));

        float depth = LegDepth;
        backLegThighFront.initialPosition = new Vector3(0, -7, -depth);

        //backLegThighFront.attachmentPoint = 0f;
        //backLegThighFront.localHeight = 16;

        FoxSegment backLegLegFront = new FoxSegment(backLeg[1], backLegThighFront, new Vector2(19, 45));
        backLegLegFront.initialPosition = backLegThighFront.initialPosition + new Vector3(5, 23, 0.1f);
        //backLegLegFront.drawAngleOffset = MathHelper.ToRadians(-90);
        //backLegLegFront.attachmentPoint = 0.5f;
        //backLegLegFront.localLength = 25;

        FoxSegment backFootFront = new FoxSegment(backLeg[2], backLegLegFront, new Vector2(7, 6));
        backFootFront.initialPosition = backLegLegFront.initialPosition + new Vector3(0, 32, -0.1f);

        segments[0] = backLegThighFront;
        segments[1] = backLegLegFront;
        segments[2] = backFootFront;
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].angleOffset = -MathHelper.PiOver2;
        }
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

    public FoxSegment[] CreateFrontLeg(Texture2D[] frontLeg, FoxSegment neck, bool isBehind)
    {
        FoxSegment[] segments = new FoxSegment[3];

        float depth = LegDepth;

        FoxSegment frontLegThighFront = new FoxSegment(frontLeg[0], neck, new Vector2(8, 21));
        frontLegThighFront.initialPosition = new Vector3(90, 3, -depth);

        FoxSegment frontLegLegFront = new FoxSegment(frontLeg[1], frontLegThighFront, new Vector2(12, 40));
        frontLegLegFront.initialPosition = frontLegThighFront.initialPosition + new Vector3(5, 13, 0.01f);

        FoxSegment frontLegFootFront = new FoxSegment(frontLeg[2], frontLegLegFront, new Vector2(7, 6));
        frontLegFootFront.initialPosition = frontLegLegFront.initialPosition + new Vector3(0, 32, 0f);



        segments[0] = frontLegThighFront;
        segments[1] = frontLegLegFront;
        segments[2] = frontLegFootFront;
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].angleOffset = -MathHelper.PiOver2;
        }
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
            segment.ResetTransformations();
        }
        for (int i = 0; i < segmentsByNumberOfParents.Length; i++)
        {
            var segment = segmentsByNumberOfParents[i];
            segment.ApplyEulerAngles();
        }
        for (int i = 0; i < segmentsByNumberOfParents.Length; i++)
        {
            var segment = segmentsByNumberOfParents[i];
            segment.Update();
        }
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
public class FoxTail
{

    public FoxTail(int segmentCount)
    {
        positions = new Vector2[segmentCount];
    }

    public Vector2 rootPosition;
    public Vector2 endPosition;
    public Vector2[] positions;

    public void Update()
    {

        //So I'm thinking we just lerp between the root and end position and add some sining motions?
        //That'd be the easiest way to do it I think

    }

    public void Draw()
    {

    }
}

public class RoyalFoxTails
{

}
