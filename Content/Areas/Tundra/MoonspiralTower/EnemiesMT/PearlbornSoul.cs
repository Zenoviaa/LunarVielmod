using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornSoul : ModNPC,
    IDrawToRenderTarget
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
    float ZigZagDashTime => 45;
    float ZigZagRange => 192;
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
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.TrailCacheLength[Type] = 96;
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
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PearlescentScrap>(), minimumDropped: 2, maximumDropped: 4));
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
        NPC.rotation = NPC.velocity.ToRotation();
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
        if (!NPC.HasValidTarget || Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (Timer == 1 || _circleOriginPoint == default(Vector2))
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
    

        if (NPC.HasValidTarget && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= ChaseDistance)
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
        var targetDirection = Vector2.Lerp(directionToTarget, perpDirection, MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f);
        var targetVelocity = targetDirection * ChaseSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);
        if (Timer >= ChaseTime)
        {
            SwitchState(AIState.ZigZagPrepare);
        }
    }

    void AI_ZigZagPrepare()
    {
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.StormDragon_CloudBolt.Asset with
            {
                PitchVariance = 0.4f,
                Pitch = 0.5f,
                Volume = 0.25f
            };

            SoundEngine.PlaySound(sound, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                _side = Main.rand.NextBool(2) ? -1 : 1;
                _startDashPoint = NPC.Center;
                _endDashPoint = Main.player[NPC.target].Center;
                _endDashPoint += new Vector2(0, _side * 256);
                NPC.netUpdate = true;
            }
        }

        if (Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
        }

        //this can be snappy.
        var startPos = _startDashPoint;
        var endPos = _endDashPoint;
        var ratio = Timer / ZigZagPrepareTime;
        var ease = EasingFunction.InOutSine(ratio);
        var posToMoveTo = Vector2.Lerp(startPos, endPos, ease);
        var targetVelocity = posToMoveTo - NPC.Center;
        NPC.velocity = targetVelocity;

        if (Timer >= ZigZagPrepareTime * 1.2f)
        {
            SwitchState(AIState.ZigZag);
        }
    }

    void AI_ZigZag()
    {
        Timer++;
        if (Timer == 1)
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
        if(Timer == 7)
        {
            var sound = AssetReferences.Assets.Sounds.StormDragon_FlyingIn.Asset with { 
                PitchVariance = 0.4f,
                Pitch = 0.5f,
                Volume = 0.25f };
            SoundEngine.PlaySound(sound, NPC.position);
        }

        var ratio = Timer / ZigZagDashTime;
        var ease = ratio;
        var pos = Vector2.Lerp(_startDashPoint, _endDashPoint, ease);
        pos.X += MathF.Sin(ratio * 12f) * ZigZagRange;
        var targetVelocity = pos - NPC.Center;
        NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InSine(Timer / 40f));
        if (Timer >= ZigZagDashTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
            for (var i = 0; i < 16; i++)
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
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }

    public void DrawToRenderTargets()
    {

        if(State == AIState.ZigZag)
        {
            PearlbornSoulRenderer.PrepareForRenderingRed(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });
        } 
        else if(State == AIState.ZigZagPrepare)
        {
            PearlbornSoulRenderer.PrepareForRenderingYellow(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });
        } 
        else
        {
            PearlbornSoulRenderer.PrepareForRendering(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });

        }
        PearlbornSoulRenderer.PrepareForExclusionRenderer(new()
        {
            TrailCache = NPC.oldPos,
            TrailOffset = NPC.Size * 0.5f
        });
    }
}



[Autoload(Side = ModSide.Client)]
public class PearlbornSoulRenderer : ModSystem
{
    public record struct SoulDrawData(Vector2[] TrailCache, Vector2 TrailOffset);
    static Queue<SoulDrawData> _maskDrawQueue = new();
    static Queue<SoulDrawData> _soulDrawQueue = new();
    static Queue<SoulDrawData> _yellowOutlineSoulDrawQueue = new();
    static Queue<SoulDrawData> _redOutlineSoulDrawQueue = new();
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
        if (_redOutlineSoulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrailOutline, DrawLayer.OverNPCs);
        }
        if (_yellowOutlineSoulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrailOutlineYellow, DrawLayer.OverNPCs);
        }
        if (_soulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrail, DrawLayer.OverNPCs);
        }
        if(_maskDrawQueue.Count > 0)
        {
            MoonEffect.PrepareForExclusionRendering(DrawExclusionMask);
        }
    }


    float GetTrailWidth(float progress)
    {
        return MathHelper.SmoothStep(18, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        return DrawUtilities.InterpolateColorArray(progress, Color.White, Color.SkyBlue);
    }


    void DrawSoulTrailInner(
        SpriteBatch sb,
        ref Queue<SoulDrawData> drawQueue,
        List<VertexPositionColorTexture> batch,
        List<Vector3> points)
    {
        while (drawQueue.Count > 0)
        {
            var drawData = drawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.TrailCache, GetTrailColor, GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);

            var drawPoint = new Vector3(drawData.TrailCache[1], (drawData.TrailCache[1] - drawData.TrailCache[0]).ToRotation());
            points.Add(drawPoint);
        }

        var indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);
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
        sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), indices, pass.Shader);
    }

    void DrawSoulTrailOutlineYellow(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);

        using (RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _yellowOutlineSoulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach (var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }

            sb.End();
        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using (sb.Ctx(oldParameters with { effect = outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Yellow);
        }
        sb.Begin(oldParameters);
    }

    void DrawSoulTrailOutline(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);

        using (RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _redOutlineSoulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach (var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }

            sb.End();
        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using (sb.Ctx(oldParameters with { effect = outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Red);
        }
        sb.Begin(oldParameters);
    }

    void DrawSoulTrail(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);
        using (RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _soulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach (var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }
            sb.End();

        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using (sb.Ctx(oldParameters with { effect = outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Lerp(Color.SkyBlue, Color.Black, 0.5f));
        }

        sb.Begin(oldParameters);
    }

    void DrawExclusionMask(SpriteBatch sb)
    {
        sb.EndOut(out var oldParameters);
        var batch = new List<VertexPositionColorTexture>();
        var points = new List<Vector3>();
        DrawSoulTrailInner(sb, ref _maskDrawQueue, batch, points);
        sb.Begin(oldParameters);
    }

    public static void PrepareForRenderingRed(in SoulDrawData drawData)
    {
        _redOutlineSoulDrawQueue.Enqueue(drawData);
    }

    public static void PrepareForRenderingYellow(in SoulDrawData drawData)
    {
        _yellowOutlineSoulDrawQueue.Enqueue(drawData);
    }

    public static void PrepareForExclusionRenderer( in SoulDrawData drawData)
    {
        _maskDrawQueue.Enqueue(drawData);
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
        var points = DrawUtilities.PruneFarPoints(Projectile.oldPos);
        return ProjectileHelper.OldPosColliding(points, projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        Time--;
        Projectile.Center = Parent.Center;
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

