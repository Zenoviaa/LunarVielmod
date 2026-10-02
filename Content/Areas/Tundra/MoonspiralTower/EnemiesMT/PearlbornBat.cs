using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornClone : ModProjectile,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Prepare,
        Dash,
        Out
    }

    AIState _state;
    Vector2 _dashVelocity;
    Vector2 _initialVelocity;
    float _side;
    float _alpha;
    ref float Timer => ref Projectile.ai[0];
    NPC Parent
    {
        get => Main.npc[(int)Projectile.ai[1]];
    }
    Player Target
    {
        get => Main.player[(int)Projectile.ai[2]];
    }

    float DashTime => 70;
    float PrepTime => 60;
    float OutTime => 30;
    float AnimationSpeed => 5;
    Vector2 SideOffset => Vector2.UnitY.RotatedBy(_side) * 192;
    public override string Texture => ModContent.GetInstance<PearlbornBat>().Texture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_dashVelocity);
        writer.Write((byte)_state);
        writer.Write(_side);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
        _dashVelocity = reader.ReadVector2();
        _state = (AIState)reader.ReadByte();
        _side = reader.ReadSingle();
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _initialVelocity = Projectile.velocity;
        _side = Main.rand.NextFloat(6.28f);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        switch (_state)
        {
            case AIState.Prepare:
                _alpha = MathHelper.Lerp(_alpha, 1f, 0.1f);
                AI_Prepare();
                break;
            case AIState.Dash:
                _alpha = 1f;
                AI_Dash();
                break;
            case AIState.Out:
                _alpha = MathHelper.Lerp(_alpha, 0f, 0.1f);
                AI_Out();
                break;
        }

        Projectile.frameCounter++;
        if (Projectile.frameCounter >= AnimationSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
        }

        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.05f, 0.1f);
    }

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            _state = state;
            Projectile.netUpdate = true;
        }
    }

    void DisperseEffect()
    {
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
        for (var i = 0; i < 8; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = (pos - Projectile.Center);
            vel = vel.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10, 20);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }
    }

    void AI_Prepare()
    {
        Timer++;
        if (Timer == 1)
        {
            DisperseEffect();
        }
        var targetPos = Target.Center + SideOffset;
        var targetVelocity = targetPos - Projectile.Center;
        var ratio = Timer / PrepTime;
        var easing = EasingFunction.Anticipation2(ratio);
        var newVelocity = Vector2.Lerp(_initialVelocity, targetVelocity, easing);
        Projectile.hostile = false;
        Projectile.velocity = newVelocity;
        if (Timer >= PrepTime)
        {
            SwitchState(AIState.Dash);
        }
    }

    void AI_Dash()
    {
        Timer++;
        if (Timer == 1)
        {
            _dashVelocity = (Target.Center - Projectile.Center);
            _dashVelocity = _dashVelocity.SafeNormalize(Vector2.Zero);
            _dashVelocity *= 15;
        }
        Projectile.hostile = true;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, _dashVelocity, 0.03f);
        if (Timer >= DashTime)
        {
            SwitchState(AIState.Out);
        }
    }

    void AI_Out()
    {
        Projectile.hostile = false;
        Projectile.velocity *= 0.8f;
        Timer++;
        if (Timer >= OutTime)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= 0.6f;
        drawer.color *= ExtraMath.Osc(0.9f, 1f, speed: 16, offset: Projectile.identity);
        drawer.color *= _alpha;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        var color = !Projectile.hostile ? Color.Yellow : Color.Red;
        var drawer = Projectile.Drawer;
        drawer.color = color;
        drawer.color *= _alpha;
        spriteBatch.Draw(drawer);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        DisperseEffect();
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutline);
    }
}

public class PearlbornBat : ModNPC
{
    enum AIState
    {
        Idle,
        Chase,
        Summon
    }
    int _frame;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    Vector2 _randomPointToTrack;
    float ChaseTime => 120;
    float AttackDistance => 128;
    float ChaseDistance => 384;
    int CloneCount => 7;
    int TimeBetweenClones => 45;
    int Damage_Clone => 25;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 7;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 3;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_randomPointToTrack);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _randomPointToTrack = reader.ReadVector2();
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Summon:
                AI_Summon();
                break;
        }
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        Timer++;
        if (Timer == 1 || !NPC.HasValidTarget)
        {
            NPC.TargetClosest();
        }

        if (Timer % 80 == 0 || _randomPointToTrack == default(Vector2))
        {
            if (MultiplayerHelper.IsHost)
            {
                _randomPointToTrack = NPC.Center + Main.rand.NextVector2Circular(128, 128);
            }
        }

        var pointToMoveTo = _randomPointToTrack;
        var targetVelocity = pointToMoveTo - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        var maxSpeed = 3;
        var speed = MathF.Min(Vector2.Distance(pointToMoveTo, NPC.Center), maxSpeed);
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.15f);
        var distance = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center);
        if (distance <= ChaseDistance)
        {
            SwitchState(AIState.Chase);
        }
    }


    void AI_Chase()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (NPC.HasValidTarget)
        {

            var myTarget = Main.player[NPC.target];
            var targetPos = myTarget.Center;
            targetPos += new Vector2(0, -90).RotatedBy(Timer * 0.03f);
            var targetVelocity = (targetPos - NPC.Center);
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.03f);
            NPC.SpriteFaceTarget();
            if (Timer >= ChaseTime && Vector2.Distance(NPC.Center, targetPos) < AttackDistance)
            {
                SwitchState(AIState.Summon);
            }
        }
        else
        {
            SwitchState(AIState.Idle);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);

        for (var i = 0; i < NPC.oldPos.Length; i++)
        {
            var posToDrawAt = NPC.oldPos[i] + NPC.Size * 0.5f;
            var afDraw = drawer;
            var progress = i / (float)NPC.oldPos.Length;
            afDraw.worldPosition = posToDrawAt;
            afDraw.rotation = NPC.oldRot[i];
            afDraw.color = Color.Lerp(Color.SkyBlue, Color.Transparent, progress) * 0.15f;
            spriteBatch.Draw(afDraw);
        }
        spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    void AI_Summon()
    {
        Timer++;
        if (Timer % TimeBetweenClones == 0)
        {
            AttackCycle++;
            if (MultiplayerHelper.IsHost)
            {
                var cloneFirer = ProjFirer.From<PearlbornClone>(NPC);
                cloneFirer.velocity = Main.rand.NextVector2CircularEdge(12, 12);
                cloneFirer.damage = Damage_Clone;
                cloneFirer.New();
            }
            if (AttackCycle >= CloneCount)
            {
                SwitchState(AIState.Idle);
            }
        }
        NPC.velocity *= 0.96f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.05f) * 0.5f, 0.2f);
        NPC.SpriteFaceTarget();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            int leftWingGore = Mod.Find<ModGore>($"{Name}_Gore_LeftWing").Type;
            int rightWingGore = Mod.Find<ModGore>($"{Name}_Gore_RightWing").Type;
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int fureGore = Mod.Find<ModGore>($"{Name}_Gore_Fur").Type;

            // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, leftWingGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(34, 0), NPC.velocity, rightWingGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(17, 0), NPC.velocity, headGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(17, 17), NPC.velocity, fureGore);
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
        //   return base.CanHitPlayer(target, ref cooldownSlot);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.2f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
    }
}


[Autoload(Side = ModSide.Client)]
public class PearlbornSoulRenderer : ModSystem
{
    public record struct SoulDrawData(Vector2[] TrailCache, Vector2 TrailOffset);
    static readonly Queue<SoulDrawData> _soulDrawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += PreparePixelatedRendering;
    }

    private void PreparePixelatedRendering(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (_soulDrawQueue.Count <= 0)
            return;
        PixelationManager.QueuePrimitivesDrawAction(DrawPrimitives, DrawLayer.OverPlayers);
    }

    float GetTrailWidth(float progress)
    {
        return MathHelper.SmoothStep(32, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        return DrawUtilities.InterpolateColorArray(progress, Color.White, Color.SkyBlue, Color.DarkBlue, Color.Purple);
    }

    void DrawPrimitives(GraphicsDevice gDevice)
    {
        var batch = new List<VertexPositionColorTexture>();
        while (_soulDrawQueue.Count > 0)
        {
            var drawData = _soulDrawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.TrailCache, GetTrailColor, GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);
        }

        var indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 2);
        var pass = AssetReferences.Effects.Generic.SoulTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.LaserTextures.SpectralSoulSuck.Asset.Value
        };
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value
        };
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Apply();

        //Now we just need to make and draw the shader
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), indices, pass.Shader);
    }

    public static void PrepareForRendering(in SoulDrawData drawData)
    {
        _soulDrawQueue.Enqueue(drawData);
    }
}

public class PearlbornDash : ModProjectile
{
    NPC Parent => Main.npc[(int)Projectile.ai[0]];
    ref float Time => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(48);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.7f;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(Projectile.oldPos, projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        Time--;
        if (Time <= 0 || !Parent.active || Parent.type != ModContent.NPCType<PearlbornSoul>())
        {
            Projectile.Kill();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
public class PearlbornSoul : ModNPC
{
    enum AIState : byte
    {
        Idle,
        Chase,
        ZigZagPrepare,
        ZigZag
    }

    float _side;
    Vector2 _startDashPoint;
    Vector2 _endDashPoint;
    Vector2 _circleOriginPoint;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float ChaseDistance => 384;
    float AttackDistance => 64;
    float CircleOffset => 80;
    float ChaseSpeed => 6;
    float ChaseTime => 120;
    float ZigZagPrepareTime => 30;
    float ZigZagDashTime => 30;
    float ZigZagRange => 256;
    int Damage_PearlbornDash => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startDashPoint);
        writer.WriteVector2(_endDashPoint);
        writer.WriteVector2(_circleOriginPoint);
        writer.Write(_side);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startDashPoint = reader.ReadVector2();
        _endDashPoint = reader.ReadVector2();
        _circleOriginPoint = reader.ReadVector2();
        _side = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.HitSound = SoundID.NPCHit30;
        NPC.DeathSound = SoundID.NPCDeath38;
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.ZigZagPrepare:
                AI_ZigZagPrepare();
                break;
            case AIState.ZigZag:
                AI_ZigZag();
                break;
        }
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        Timer++;
        if(!NPC.HasValidTarget || Timer == 1)
        {
            NPC.TargetClosest();
        }

        if(Timer == 1 || _circleOriginPoint == default(Vector2))
        {
            _circleOriginPoint = NPC.Center;
        }


        var posToTrack = _circleOriginPoint + new Vector2(0, CircleOffset).RotatedBy(Timer * 0.05f);
        if (NPC.HasValidTarget)
        {
            var directionToPlayer = Main.player[NPC.target].Center - NPC.Center;
            directionToPlayer = directionToPlayer.SafeNormalize(Vector2.Zero);
            _circleOriginPoint += directionToPlayer * 1.5f;
        }

        var targetVelocity = posToTrack - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        var moveSpeed = 3;
        var speed = MathF.Min(moveSpeed, Vector2.Distance(posToTrack, NPC.Center));
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        NPC.rotation = NPC.velocity.X * 0.05f;


        if(NPC.HasValidTarget && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= ChaseDistance)
        {
            SwitchState(AIState.Chase);
        }
    }

    void AI_Chase()
    {
        Timer++;
        if (!NPC.HasValidTarget)
            SwitchState(AIState.Idle);

        //Just move towards the player while moving up and down
        
        //honestly no clue what this gonna do lol
        //I'll see what it looks like first
        var directionToTarget = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
        var perpDirection = directionToTarget.RotatedBy(MathHelper.PiOver2);
        var targetDirection = Vector2.Lerp(directionToTarget, perpDirection, MathF.Sin(Timer * 0.5f) * 0.5f + 0.5f);
        var targetVelocity = targetDirection * ChaseSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);
        if(Timer >= ChaseTime)
        {
            SwitchState(AIState.ZigZagPrepare);
        }
    }

    void AI_ZigZagPrepare()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _side = Main.rand.NextBool(2) ? -1 : 1;
                _startDashPoint = NPC.Center;
                _endDashPoint = Main.player[NPC.target].Center;
                _endDashPoint += new Vector2(0, _side * 128);
                NPC.netUpdate = true;
            }
        }

        if(Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
        }

        //this can be snappy.
        var startPos = _startDashPoint;
        var endPos = _endDashPoint;
        var ratio = Timer / ZigZagPrepareTime;
        var ease = EasingFunction.InOutBounce(ratio);
        var posToMoveTo = Vector2.Lerp(startPos, endPos, ease);
        var targetVelocity = posToMoveTo - NPC.Center;
        NPC.velocity = targetVelocity;

        if(Timer >= ZigZagPrepareTime)
        {
            SwitchState(AIState.ZigZag);
        }
    }

    void AI_ZigZag()
    {
        Timer++;
        if(Timer == 1)
        {
            _startDashPoint = NPC.Center;
            _endDashPoint = _startDashPoint + new Vector2(0, 384 * -_side);
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<PearlbornDash>(NPC);
                firer.ai0 = NPC.whoAmI;
                firer.ai1 = ZigZagDashTime;
                firer.damage = Damage_PearlbornDash;
                firer.New();
            }
        }

        var ratio = Timer / ZigZagDashTime;
        var ease = ratio;
        var pos = Vector2.Lerp(_startDashPoint, _endDashPoint, ease);
        pos.X += MathF.Sin(ratio * 12f) * ZigZagRange;
        var targetVelocity = pos - NPC.Center;
        NPC.velocity = targetVelocity;
        if(Timer >= ZigZagDashTime)
        {
            SwitchState(AIState.Idle);
        }
    }
    
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
            for(var i = 0; i < 16; i++)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(24, 24);
                var vel = (pos - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(9, 15f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.6f, 1f)),
                    timeLeft = 120
                });
            }
        }
    }
    public override void OnKill()
    {
        base.OnKill();
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        PearlbornSoulRenderer.PrepareForRendering(new()
        {
            TrailCache = NPC.oldPos,
            TrailOffset = NPC.Size * 0.5f
        });
        return false;
    }
}