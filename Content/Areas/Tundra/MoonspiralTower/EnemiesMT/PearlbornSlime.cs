using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornSlime : ModNPC
{
    enum AIState : byte
    {
        Idle,
        Jump,
        JumpTo,
        CircularSpike,
        SniperSpike
    }

    Vector2 _squishScale;
    Vector2 _jumpStartPosition;
    Vector2 _jumpEndPosition;
    int _frame;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float IdleTime => 90;
    float ChaseDistance => 384;
    float JumpHeight => -256;
    float CircularSquishTime => 60;
    float SniperSpikeSquishTime => 120;
    int DamagePearlbornSpike => 30;

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PearlescentScrap>(), minimumDropped: 2, maximumDropped: 4));
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_jumpEndPosition);
        writer.WriteVector2(_jumpStartPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _jumpEndPosition = reader.ReadVector2();
        _jumpStartPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 32;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public override void AI()
    {
        base.AI();

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Jump:
                AI_Jump();
                break;
            case AIState.JumpTo:
                AI_JumpTo();
                break;
            case AIState.CircularSpike:
                AI_CircularSpike();
                break;
            case AIState.SniperSpike:
                AI_SniperSpike();
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
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1 || !NPC.HasValidTarget)
            NPC.TargetClosest();

        var sqrDist = Vector2.DistanceSquared(NPC.Center, Main.player[NPC.target].Center);
        if (sqrDist <= ChaseDistance)
        {
            SwitchState(AIState.JumpTo);
        }
        NPC.velocity.X *= 0.96f;
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Jump);
        }
    }


    public GlowDonutParticle MakeDonut(Vector2 position, Vector2 velocity)
    {
        var p = LegacyParticle.NewParticle<GlowDonutParticle>(position, velocity);
        p.innerColor = Color.White;
        p.outerColor = Color.DarkBlue;
        p.fadeToColor = Color.Black;
        return p;
    }

    void AI_Jump()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1)
        {
            NPC.velocity.Y = -15;
            if (MultiplayerHelper.IsHost)
            {
                NPC.velocity.X = (Main.rand.NextBool(2) ? -1 : 1) * 7;
            }
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            MakeDonut(NPC.Bottom, Vector2.UnitY);
        }
        NPC.velocity.X *= 0.96f;
        if (NPC.velocity.Y < -5)
            NPC.velocity.Y *= 0.96f;
        if (NPC.velocity.Y >= -0.5f)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_JumpTo()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1)
        {
            var target = Main.player[NPC.target];
            _jumpStartPosition = NPC.Center;
            _jumpEndPosition = target.Center;
            _jumpEndPosition = TileUtilities.FallToSolidTile(_jumpEndPosition);
            _jumpEndPosition.Y -= 8;
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            MakeDonut(NPC.Bottom, Vector2.UnitY);
        }


        var jumpTicks = Vector2.Distance(_jumpStartPosition, _jumpEndPosition) / 16f;
        var ratio = Timer / jumpTicks;
        var posToMoveTo = Vector2.Lerp(_jumpStartPosition, _jumpEndPosition, ratio);
        var up = MathHelper.Lerp(0, JumpHeight, EasingFunction.OutExpo(ratio));
        var down = MathHelper.Lerp(JumpHeight, 0, EasingFunction.InExpo(ratio));
        var y = MathHelper.Lerp(up, down, ratio);
        posToMoveTo.Y += y;
        NPC.velocity = Vector2.Zero;
        NPC.noTileCollide = true;
        NPC.Center = posToMoveTo;
        if (Timer >= jumpTicks)
        {
            //decide attack;
            if (MultiplayerHelper.IsHost)
            {
                if (Main.rand.NextBool(2))
                {
                    SwitchState(AIState.CircularSpike);
                }
                else
                {
                    SwitchState(AIState.SniperSpike);
                }
            }
        }
    }

    void AI_CircularSpike()
    {
        Timer++;
        if (Timer == 1)
        {
            for (var i = 0; i < 8; i++)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(24, 24);
                var vel = (pos - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(4, 9);
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

        float ratio = Timer / CircularSquishTime;
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 1f), EasingFunction.QuadraticBump(ratio));
        NPC.velocity *= 0.96f;
        NPC.noTileCollide = false;
        if (Timer == 30)
        {
            if (MultiplayerHelper.IsHost)
            {
                var spikeFirer = ProjFirer.From<PearlbornSpear>(NPC);
                spikeFirer.damage = DamagePearlbornSpike;
                spikeFirer.knockback = 1;
                spikeFirer.ai1 = NPC.whoAmI;
                for (var i = 0; i < 8; i++)
                {
                    var progress = i / 8f;
                    var rotatedSpikeFirer = spikeFirer;
                    var rot = progress * MathHelper.TwoPi;
                    var rotationOffset = rot.ToRotationVector2();
                    rotatedSpikeFirer.velocity = rotationOffset * 128;
                    rotatedSpikeFirer.New();
                }
            }
        }

        if (Timer >= CircularSquishTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_SniperSpike()
    {
        Timer++;
        if (Timer % 4 == 0)
        {
            var pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(128, 128);
            var vel = (NPC.Center - pos) * 0.05f;
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

        float ratio = Timer / SniperSpikeSquishTime;
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 1f), EasingFunction.QuadraticBump(ratio));
        NPC.velocity *= 0.96f;
        NPC.noTileCollide = false;
        if (Timer == 60)
        {
            if (MultiplayerHelper.IsHost)
            {
                var spikeFirer = ProjFirer.From<PearlbornSpear>(NPC);
                spikeFirer.damage = DamagePearlbornSpike;
                spikeFirer.knockback = 1;
                spikeFirer.ai1 = NPC.whoAmI;
                var target = Main.player[NPC.target];
                var vel = (target.Center - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= 384;
                spikeFirer.velocity = vel;
                spikeFirer.New();
            }
        }

        if (Timer >= SniperSpikeSquishTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.scale = _squishScale;
        spriteBatch.Draw(drawer);
        return false;

    }
}
public class PearlbornSpear : ModProjectile
{
    Vector2[] TrailPoints
    {
        get
        {
            if (field == null)
            {
                field = new Vector2[32];
                var start = Projectile.Center;
                var end = Projectile.Center + Projectile.velocity;
                for (var i = 0; i < field.Length; i++)
                {
                    ref var pos = ref field[i];

                    pos = Vector2.Lerp(start, end, i / (float)field.Length);
                }
            }
            return field;
        }
    }
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var start = Projectile.Center;
        var end = Projectile.Center + Projectile.velocity;
        var collisionPoint = 0F;
        var lineWidth = 6;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, start, lineWidth, ref collisionPoint))
            return true;
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.ignoreWater = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.Center = Parent.Center;
    }

    float GetTrailWidth(float progress)
    {
        return MathHelper.SmoothStep(32, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        return Color.Lerp(Color.White, Color.DarkGray, Timer / 60f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PearlbornSlimeRenderer.PrepareForRendering(new()
        {
            GetTrailWidth = GetTrailWidth,
            GetTrailColor = GetTrailColor,
            TrailOffset = Vector2.Zero,
            Points = TrailPoints
        });
        return false;
    }
}
[Autoload(Side = ModSide.Client)]
public class PearlbornSlimeRenderer : ModSystem
{
    public record struct SlimeDrawData(Vector2[] Points, Func<float, Color> GetTrailColor, Func<float, float> GetTrailWidth, Vector2 TrailOffset);
    static readonly Queue<SlimeDrawData> _drawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += DrawPixelated;
    }

    private void DrawPixelated(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (_drawQueue.Count <= 0)
            return;
        PixelationManager.QueuePrimitivesDrawAction(RenderSlime, DrawLayer.OverPlayers);
    }

    void RenderSlime(GraphicsDevice gDevice)
    {
        //May need to render this to a render target first so we can outline it, will see how it looks first.
        var batch = new List<VertexPositionColorTexture>();
        while (_drawQueue.Count > 0)
        {
            var drawData = _drawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.Points, drawData.GetTrailColor, drawData.GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);
        }

        var indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 2);
        var pass = AssetReferences.Effects.Generic.SoulTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSlime_Spike.Asset.Value
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

    public static void PrepareForRendering(SlimeDrawData drawData)
    {
        _drawQueue.Enqueue(drawData);
    }
}

