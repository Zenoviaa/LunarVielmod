using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.InverseKinematics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;


public class Dreamwalker : ModNPC
{
    Pathfinder _pathfinder;
    #region Frame Data
    public static readonly Rectangle FrameLegPart3 = new Rectangle(44, 30, 18, 68);
    public static readonly Vector2 FrameLegPart3Origin = new Vector2(16, 0);

    public static readonly Rectangle FrameLegPart2 = new Rectangle(68, 22, 68, 16);
    public static readonly Vector2 FrameLegPart2Origin = new Vector2(68, 14);

    public static readonly Rectangle FrameLegPart1 = new Rectangle(146, 26, 42, 62);
    public static readonly Vector2 FrameLegPart1Origin = new Vector2(38, 60);

    public static readonly Rectangle FrameButt = new Rectangle(204, 52, 54, 44);
    public static readonly Vector2 FrameButtOrigin = new Vector2(54, FrameButt.Height * 0.5f);

    public static readonly Rectangle FrameHead = new Rectangle(362, 112, 46, 34);
    public static readonly Vector2 FrameHeadOrigin = new Vector2(0, FrameHead.Height * 0.5f);



    public static Vector2[] LegOrigins = new Vector2[]
    {
        FrameLegPart1Origin,
        FrameLegPart2Origin,
        FrameLegPart3Origin
    };

    public static Rectangle[] LegFrames = new Rectangle[]
    {
        FrameLegPart1,
        FrameLegPart2,
        FrameLegPart3
    };

    #endregion
    public class SpiderLeg
    {
        public readonly record struct StepData(Vector2 StepStart, Vector2 GroundPosition);

        public SpiderLeg()
        {
            segments = new FabrikSegment[2];
            segments[0].length = 64;
            segments[1].length = 64;
            legPositions = new Vector2[3];
            legAngles = new float[3];

            //hain.lengths[1] =

            armature = new ArmatureV2();
            armature.r1 = 62;
            armature.r2 = 62;
            rootMidAngle = Angle.Up;
            radiansOfFreedom = 3.8F;
            rootLength = 60;
            interpolatingLegAngles = new float[3];
            legLengths = new float[3];
            legLengths[0] = 60;
            legLengths[1] = 64;
            legLengths[2] = 64;
        }

        public ArmatureV2 armature;
        public FabrikSegment[] segments;
        public Vector2[] legPositions;
        public float[] legAngles;
        public float[] legLengths;
        public float[] interpolatingLegAngles;
        //   public ArmatureV2 armature;


        //The spider leg segment that points upwards needs to be constrained to a small range of motion (like 60 degrees or sum)
        //This leg will rotate based on where the ground point is
        //So if it's to the left ir rotates to the left and right it rotates to the right, checking the center point obv
        //going further the further it is

        //The armature then roots itself on the edge of that points and attempts to IK to the ground position
        //Then we just do some math to make sure the ground position is always reachable

        public Vector2 up;
        public Vector2 leftBoundaryOffset;
        public Vector2 rightBoundaryOffset;
        public Vector2 groundPosition;
        public Vector2 rootPosition;
        public Vector2 endRootEffector;
        public float rotation;
        public float rootMidAngle;
        public float radiansOfFreedom;
        public float rootLength;
        public float rootAngle;
        public bool overhead;

        public float stepTimer;
        public Vector2 stepStart;
        public void Update()
        {
            for (var i = 0; i < interpolatingLegAngles.Length; i++)
            {
                ref var legAngle = ref interpolatingLegAngles[i];
                legAngle = Utils.AngleLerp(legAngle, legAngles[i], 0.1f);
            }
        }

        public void Step(StepData stepData)
        {
            stepStart = stepData.StepStart;
            stepTimer = 0;
            groundPosition = stepData.GroundPosition;
        }
        public void Solve(Vector2 rootPosition, Vector2 targetPosition)
        {
            //200 is about the length of the full length I believe
            //magic number but yeah
            this.rootPosition = rootPosition;
            if (stepTimer < 16)
            {
                stepTimer++;
            }

            targetPosition = Vector2.Lerp(stepStart, targetPosition, stepTimer / 16F) + Vector2.Lerp(Vector2.Zero, up * 64, EasingFunction.QuadraticBump(stepTimer / 16F));

            var rm = up.ToRotation();

            var diffBetweenRoot = (targetPosition - rootPosition);
            var signedRootDistance = diffBetweenRoot.Length() * Vector2.Dot(diffBetweenRoot.SafeNormalize(Vector2.Zero), Vector2.UnitX.RotatedBy(rotation));
            rootAngle = rm + MathHelper.Clamp(signedRootDistance / 200F, -1, 1) * radiansOfFreedom * 0.5F;
            endRootEffector = rootPosition + rootAngle.ToRotationVector2() * rootLength;

            //so here's how this works
            //first naively stretch the entire limb towards the point
            var straightShot = (targetPosition - endRootEffector).SafeNormalize(Vector2.Zero);
            legAngles[2] = legAngles[1] = straightShot.ToRotation();

            legPositions[1] = endRootEffector;
            legPositions[2] = endRootEffector + legAngles[1].ToRotationVector2() * 64;

            //calcualteh ow much we overshot the target by
            //we're going to combine that with a dot product to figure out how much to rotate the base limb backwards to get to our desired position
            var endEffector = legPositions[2] + legAngles[2].ToRotationVector2() * 64;
            var diffBetweenEndAndActualEnd = (targetPosition - endEffector);
            var angleDirection = legAngles[2].ToRotationVector2();
            var signedDistance = diffBetweenEndAndActualEnd.Length() * Vector2.Dot(diffBetweenEndAndActualEnd.SafeNormalize(Vector2.Zero), angleDirection);
            var pctToCorrect = MathHelper.Clamp(signedDistance / 64F, -1, 0);

            //Now we take this and rotate the base limb up to 90 degrees
            var a = MathHelper.PiOver2 * pctToCorrect;

            //Limps going left gotta flip
            if (overhead)
            {
                a *= -1;
            }
            legAngles[1] += a;
            legPositions[2] = endRootEffector + legAngles[1].ToRotationVector2() * 64;

            //then we correct the 2nd angle to be towards the target
            legAngles[2] = (targetPosition - legPositions[2]).ToRotation();

            legPositions[0] = rootPosition;
            legAngles[0] = rootAngle;
        }
    }


    public SpiderLeg[] Legs
    {
        get
        {
            if (field == null)
            {
                field = new SpiderLeg[8];
                for (var i = 0; i < 8; i++)
                {
                    field[i] = new SpiderLeg();
                }

                field[0].leftBoundaryOffset = new Vector2(-230, 0);
                field[0].rightBoundaryOffset = new Vector2(-15, 0);

                field[1].leftBoundaryOffset = new Vector2(-215, 0);
                field[1].rightBoundaryOffset = new Vector2(0, 0);

                field[2].leftBoundaryOffset = new Vector2(25, 0);
                field[2].rightBoundaryOffset = new Vector2(210, 0);

                field[3].leftBoundaryOffset = new Vector2(25, 0);
                field[3].rightBoundaryOffset = new Vector2(235, 0);

                field[4].leftBoundaryOffset = new Vector2(-225, 0);
                field[4].rightBoundaryOffset = new Vector2(-4, 0);

                field[5].leftBoundaryOffset = new Vector2(-205, 0);
                field[5].rightBoundaryOffset = new Vector2(-8, 0);

                field[6].leftBoundaryOffset = new Vector2(12, 0);
                field[6].rightBoundaryOffset = new Vector2(205, 0);

                field[7].leftBoundaryOffset = new Vector2(12, 0);
                field[7].rightBoundaryOffset = new Vector2(225, 0);
            }
            return field;
        }
    }


    enum AIState : byte
    {
        Idle,
        Walk,
        Shock
    }

    float _walkDirection;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    Vector2[] _actualLegPositions = new Vector2[3];
    float IdleTime => 150;
    float WalkTime => 240;
    int WanderTileWidth => 75;
    int WanderTileHeight => 75;
    float WanderWalkSpeed => 3;
    public override string Texture => AssetReferences.Assets.Textures.Darkspace.NPCs.Dreamspider.KEY;
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
        //return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _walkDirection = reader.ReadSingle();
        _pathfinder.NetReceive(reader);
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_walkDirection);
        _pathfinder.NetSend(writer);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddToDarkspace();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 32;
        NPC.width = 16;
        NPC.height = 16;
        NPC.lifeMax = 100;
        NPC.defense = 12;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;
        var particles = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Moonly, new Vector2(5, 35));
        ParticleUtils.CreateSwirlingDustCircle(particles);

        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Body", NPC.position, NPC.velocity + new Vector2(0, 1));
    }

    bool IsNearTile()
    {
        Point center = NPC.Center.ToTileCoordinates();
        var rect = new Rectangle(
            center.X - 1,
            center.Y - 1,
            3,
            3);

        rect = TileUtilities.Clamp(rect);
        for (var i = rect.Left; i < rect.Right; i++)
        {
            for (var j = rect.Top; j < rect.Bottom; j++)
            {
                var tile = Main.tile[i, j];
                if (tile.HasTile && MoonUtils.QuickSolid(i, j))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns a vector that shows which way gravity is, defaulting to down if there's no nearby tiles
    /// </summary>
    /// <returns></returns>
    Vector2 GetDownVector()
    {

        Vector2 down = Vector2.Zero;
        foreach (var direction in Vector2.OctagonalOffsets)
        {
            var dist = 196;
            var hitPoint = CollisionHelper.SolidTileRayCast(NPC.Center, direction, dist);
            var dq = Vector2.DistanceSquared(NPC.Center, hitPoint);
            down += direction * MathHelper.Lerp(1f, 0f, dq / (dist * dist));
        }
        down /= 8F;
        return down;
    }
    List<Vector2> _pointBuffer = new();
    Vector2 GetNextPointToWanderTo(int tileSearchWidth, int tileSearchHeight)
    {
        Point center = NPC.Center.ToTileCoordinates();
        var rect = new Rectangle(
            center.X - tileSearchWidth / 2,
            center.Y - tileSearchHeight / 2,
            tileSearchWidth,
            tileSearchHeight);

        rect = TileUtilities.Clamp(rect);

        var list = _pointBuffer;
        list.Clear();
        for (var x = rect.Left; x < rect.Right; x++)
        {
            for (var y = rect.Top; y < rect.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;

                //Check if it is adjacent to a tile
                var adjacentToTile = false;
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var otherPoint = new Point(x + i, y + j);
                        var otherTile = Main.tile[otherPoint];
                        if (otherTile.HasTile && MoonUtils.QuickSolid(otherPoint.X, otherPoint.Y))
                        {
                            adjacentToTile = true;
                            break;
                        }
                    }
                }
                if (!adjacentToTile)
                    continue;
                list.Add(new Point(x, y).ToWorldCoordinates());
            }

        }
        if (list.Count <= 0)
            return NPC.Center;
        return list[MoonUtils.Rand.Next(list.Count)];

    }

    /// <summary>
    /// Tries to find an air tile that is touching a ground tile, so that we can walk over to it
    /// </summary>
    /// <param name="tileSearchWidth"></param>
    /// <param name="tileSearchHeight"></param>
    /// <param name="maxAttempts"></param>
    /// <returns></returns>
    Vector2 GetNextPointToWanderTo(int tileSearchWidth, int tileSearchHeight, int maxAttempts)
    {
        Point center = NPC.Center.ToTileCoordinates();
        var rect = new Rectangle(
            center.X - tileSearchWidth / 2,
            center.Y - tileSearchHeight / 2,
            tileSearchWidth,
            tileSearchHeight);

        rect = TileUtilities.Clamp(rect);

        for (var a = 0; a < maxAttempts; a++)
        {
            var randomPoint = MoonUtils.Rand.NextPointInRectangle(rect);
            if (Vector2.DistanceSquared(randomPoint.ToWorldCoordinates(), NPC.Center) < 256 * 256)
                continue;

            var tile = Main.tile[randomPoint];
            if (tile.HasTile)
                continue;

            //Check if it is adjacent to a tile
            var adjacentToTile = false;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var otherPoint = randomPoint + new Point(i, j);
                    var otherTile = Main.tile[otherPoint];
                    if (otherTile.HasTile)
                    {
                        adjacentToTile = true;
                    }
                }
            }

            if (!adjacentToTile)
                continue;

            return randomPoint.ToWorldCoordinates();
        }
        return NPC.Center;
    }
    void MakeIK()
    {
        //So this algorithm is simple
        //We take the orotp oint of the leg which we have to calculate
        //Let's do a single leg first
        //It's easy to upscale

        //so we keep track of the end effector point of each leg
        //if the end effector is invalid get a new point
        //this new point should be calculated based off the leg index, but it should go in the direction the spider is moving
        // NPC.rotation = 0;
        var down = GetDownVector();
        var facingLeft = Vector2.UnitX.RotatedBy(NPC.rotation);
        var facingRight = -Vector2.UnitX.RotatedBy(NPC.rotation);
        for (var i = 0; i < Legs.Length; i++)
        {
            Vector2 rootPosition = NPC.Center;
            var leg = Legs[i];
            leg.up = -Vector2.UnitY.RotatedBy(NPC.rotation);
            leg.rotation = NPC.rotation;
            var left = NPC.Center + leg.leftBoundaryOffset.RotatedBy(NPC.rotation);

            var right = NPC.Center + leg.rightBoundaryOffset.RotatedBy(NPC.rotation);
            var middle = (left + right) * 0.5f;
            var endEffectorPosition = leg.groundPosition;


            var leftDP = Vector2.Dot((left - endEffectorPosition).SafeNormalize(Vector2.Zero), facingLeft);
            var rightDP = Vector2.Dot((right - endEffectorPosition).SafeNormalize(Vector2.Zero), facingRight);
            if (leftDP > 0)
            {
                leg.Step(new(leg.groundPosition, CollisionHelper.SolidTileRayCast(right + new Vector2(-10, 0).RotatedBy(NPC.rotation), down, 80)));
            }
            else if (rightDP > 0)
            {
                leg.Step(new(leg.groundPosition, CollisionHelper.SolidTileRayCast(left + new Vector2(10, 0).RotatedBy(NPC.rotation), down, 80)));
            }

            if (i == 0 || i == 1 || i == 4 || i == 5)
            {
                leg.overhead = true;
            }
            else
            {
                leg.overhead = false;
            }


            leg.Solve(rootPosition, leg.groundPosition);
            leg.Update();
        }
    }



    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Walk:
                AI_Walk();
                break;
            case AIState.Shock:
                AI_Shock();
                break;
        }

        //Fall down if not near a tile
        //Spiders have unique gravity lol

        var down = GetDownVector();
        var right = down.RotatedBy(-MathHelper.PiOver2);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, right.ToRotation(), 0.1f);
        MakeIK();
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        NPC.netUpdate = true;
    }
    void AI_Shock()
    {

    }


    void AI_Walk()
    {
        Timer++;
        if (Timer == 1)
        {
            var pointToMoveTo = GetNextPointToWanderTo(WanderTileWidth, WanderTileHeight);
            NPC.TargetClosest();
         //   pointToMoveTo = NPC.PlayerImTargeting.Center;
            _pathfinder.NewPath(pointToMoveTo, NPC.Bottom, 75, searchJobStyle: SearchJobStyle.Crawler);

        }
        MoonUtils.AI_SpiderPathfinding(_pathfinder, NPC, WanderWalkSpeed);
        NPC.TargetClosest();
        NPC.FaceDirectionsToVelocity();

        if (Timer >= WalkTime || _pathfinder.path.Count <= 0)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_Idle()
    {
        Timer++;
        if (IsNearTile())
            NPC.velocity *= 0.96F;
        if (Timer >= IdleTime)
        {
            if (MultiplayerHelper.IsHost)
            {
                _walkDirection = MoonUtils.Rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
                SwitchState(AIState.Walk);
            }
        }
    }


    void DrawLeg(SpriteBatch sb, Vector2 screenPos, Color drawColor, int index, bool isBehind = false)
    {
        SpiderLeg leg = Legs[index];

        var rootPosition = leg.rootPosition;
        //valuate positions
        for (var i = 0; i < _actualLegPositions.Length; i++)
        {
            if (i == 0)
            {
                _actualLegPositions[i] = rootPosition;
            }
            else
            {
                _actualLegPositions[i] = _actualLegPositions[i - 1] + leg.interpolatingLegAngles[i - 1].ToRotationVector2() * leg.legLengths[i - 1];
            }

        }

        //draw foot
        for (var i = 0; i < leg.legPositions.Length; i++)
        {

            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(TextureAssets.Npc[Type], _actualLegPositions[i]);
            drawer.drawOrigin = LegOrigins[i];
            drawer.sourceRect = LegFrames[i];
            drawer.rotation = leg.interpolatingLegAngles[i];
            if (isBehind)
                drawer.color = drawer.color.MultiplyRGB(new Color(125, 125, 125));
            switch (i)
            {
                case 0:
                    drawer.rotation += MathHelper.ToRadians(120);
                    break;
                case 1:
                    drawer.rotation += MathHelper.ToRadians(170);
                    break;
                case 2:
                    drawer.rotation += MathHelper.ToRadians(270);
                    break;
            }

            sb.Draw(drawer);
        }
    }

    void DrawFrontLegs(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawLeg(spriteBatch, screenPos, drawColor, 0);
        DrawLeg(spriteBatch, screenPos, drawColor, 1);
        DrawLeg(spriteBatch, screenPos, drawColor, 2);
        DrawLeg(spriteBatch, screenPos, drawColor, 3);
    }

    void DrawBehindLegs(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawLeg(spriteBatch, screenPos, drawColor, 4, true);
        DrawLeg(spriteBatch, screenPos, drawColor, 5, true);
        DrawLeg(spriteBatch, screenPos, drawColor, 6, true);
        DrawLeg(spriteBatch, screenPos, drawColor, 7, true);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBehindLegs(spriteBatch, screenPos, drawColor);
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(TextureAssets.Npc[Type], NPC.Center);
        drawer.sourceRect = FrameButt;
        drawer.drawOrigin = FrameButtOrigin;
        drawer.rotation = NPC.rotation;
        if (NPC.spriteDirection == -1)
            drawer.FlipHorizontally();
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromTextureGetLightingColor(TextureAssets.Npc[Type], NPC.Center);
        drawer.rotation = NPC.rotation;
        drawer.sourceRect = FrameHead;
        drawer.sourceRect = new Rectangle(268, 62, FrameHead.Width, FrameHead.Height);
        drawer.drawOrigin = FrameHeadOrigin;
        if (NPC.spriteDirection == -1)
            drawer.FlipHorizontally();
        spriteBatch.Draw(drawer);
        DrawFrontLegs(spriteBatch, screenPos, drawColor);
        return false;
        /*
        _pathfinder.DebugDrawPath(spriteBatch, Color.Red);
        var leg = Legs[0];
        var left = NPC.Center + leg.leftBoundaryOffset.RotatedBy(NPC.rotation);
        var upPoint = NPC.Center + leg.up * 64;
        drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.WhiteCircle.Asset);
        drawer.worldPosition = left;
        drawer.scale *= 0.05f;
        spriteBatch.Draw(drawer);*/
        /*
 var down = GetDownVector();
 drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.WhiteCircle.Asset);
 drawer.worldPosition = NPC.Center + down * 24;
 drawer.color = Color.White;
 drawer.scale *= 0.07F;
 spriteBatch.Draw(drawer);

 Vector2[] offsets = new Vector2[4]
 {
     new Vector2(-120, 0),
     new Vector2(-90, 0),
     new Vector2(90, 0),
     new Vector2(120, 0)
 };

 for (var i = 0; i < 4; i++)
 {
     drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.WhiteCircle.Asset);
     drawer.worldPosition = Legs[i].legPositions[2] + Legs[i].legAngles[2].ToRotationVector2() * 64;
     drawer.scale *= 0.05f;
     spriteBatch.Draw(drawer);
 }*/
    }
}
