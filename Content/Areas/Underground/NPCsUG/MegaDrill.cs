using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class MegaDrill : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Teleport,
        Drill,
        Slam,
        Stuck,
    }
    Vector2 _drillDirection;
    Vector2 _crashPoint;
    Vector2 _crashOutpoint;
    bool _outpoint;
    bool _contactDamage;
    bool _solidLastFrame;
    Vector2 _teleportPosition;
    Outliner _outliner;
    const string ANIM_IDLE = "Idle";
    const string ANIM_DRILL = "Drill";
    const string ANIM_DRILLFAST = "DrillFast";
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float SlamTime => 75;

    float TooFar => 1024 * 1024f;
    float StuckTime => 120;
    float DrillSpeed => 14;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.Write(_solidLastFrame);
        writer.WriteVector2(_drillDirection);
        writer.WriteVector2(_crashPoint);
        writer.WriteVector2(_crashOutpoint);
        writer.Write(_outpoint);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _solidLastFrame = reader.ReadBoolean();
        _drillDirection = reader.ReadVector2();
        _crashPoint = reader.ReadVector2();
        _crashOutpoint = reader.ReadVector2();
        _outpoint = reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 34;
        NPC.width = NPC.height = 58;
        NPC.lifeMax = 50;
        NPC.defense = 7;
        NPC.HitSound = SoundID.DD2_CrystalCartImpact;
        NPC.DeathSound = SoundID.DD2_CrystalCartImpact;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.hide = true;
        NPC.npcSlots = 2;
    }
    public override void DrawBehind(int index)
    {
        base.DrawBehind(index);
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }


    public void Teleport(Vector2 posToTeleportTo)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = posToTeleportTo;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        if (_teleportPosition != Vector2.Zero)
        {
            NPC.velocity = Vector2.Zero;
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        NPC.TargetClosest();
        var distToPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if(distToPlayer > TooFar)
        {
            SwitchState(AIState.Teleport);
        }
        _contactDamage = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Drill:
                AI_Drill();
                break;
            case AIState.Stuck:
                AI_Stuck();
                break;
            case AIState.Slam:
                AI_Slam();
                break;
            case AIState.Teleport:
                AI_Teleport();
                break;
        }

            SoilFX();
        if (Main.rand.NextBool(4))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = NPC.Center,
                velocity = Main.rand.NextVector2Circular(2, 2) * 0.3f,
                color = Color.DarkGray,
                scale = Main.rand.NextFloat(0.85f, 1f),
                rotation = Main.rand.NextFloat() * 12f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

        if(Main.netMode != NetmodeID.Server)
        {
            var dist = Vector2.Distance(NPC.Center, Main.LocalPlayer.Center);
            var l = dist / 256f;
            l = MathHelper.Clamp(l, 0f, 1f);
            l = 1f - l;
            var maxShake = 1.5f;
            var shake = l * maxShake;
            if(ShakeScreenPosition.Shake < shake)
            {
                ShakeScreenPosition.Shake = shake;
            }
        }
        if (!_solidLastFrame)
        {
            if(Timer % 7 == 0)
            {
                MakeRocketFireFX();
            }
        }
        _outliner.Update();
        var tilePos = NPC.Center.ToTileCoordinates();
        var tile = Main.tile[tilePos];
        _solidLastFrame = WorldGen.SolidOrSlopedTile(tile);
        NPC.rotation = _drillDirection.ToRotation() + MathHelper.PiOver2;
    }
    
    void MakeRocketFireFX()
    {
        var rootPos = NPC.Center + -NPC.velocity.SafeNormalize(Vector2.Zero) * 64;
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with { innerColor = Color.Yellow.ToVector4(), outerColor = Color.OrangeRed.ToVector4(), 
            position = rootPos + Main.rand.NextVector2Circular(36, 36), velocity = -NPC.velocity });
        var colo = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(), Color.Gold, Color.DarkOrange);
        Particles.RocketFire.Spawn(new()
        {
            position = rootPos,
            velocity = -NPC.velocity.SafeNormalize(Vector2.Zero) * 8,
            color = colo,
            timeLeft = 24,
            scale = 0.6f
        });

    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }
    void SoilFX()
    {
        if (Main.rand.NextBool(9))
        {
            var worldPos = NPC.Center + _drillDirection * 48;
            var tilePos = (worldPos).ToTileCoordinates();
            var tile = Main.tile[tilePos];
            var touchingGround = WorldGen.SolidOrSlopedTile(tile);
            if(touchingGround)
                FXUtil.MakeSoilParticle(worldPos, -NPC.velocity.RotatedByRandom(1.5f).Resize(2)*Main.rand.NextFloat(3, 5f));
        }
    }
    void AI_Teleport()
    {
        Timer++;
        if(Timer < 90 && _crashPoint == default(Vector2))
        {
            Timer = 89;
        }

     
        NPC.velocity = Vector2.Lerp(NPC.velocity, _drillDirection * DrillSpeed, MathHelper.Lerp(0f, 0.01f, Timer / 60f));
        NPC.PlayAnimationLooped(ANIM_DRILLFAST);
        if (Timer == 90 && MultiplayerHelper.IsHost)
        {
            _drillDirection = Main.rand.NextBool(2) ? -Vector2.UnitY : Vector2.UnitY;
        }

        if (Timer == 90)
        {
            if (MultiplayerHelper.IsHost)
            {
                var pointToTeleportTo = NPC.PlayerImTargeting.Center;
                pointToTeleportTo.X += Main.rand.Next(-384, 384);
                Teleport(pointToTeleportTo - _drillDirection * 768);
                var centerPoint = pointToTeleportTo;
                _crashPoint = centerPoint;
                var dir = (int)MathF.Sign(_drillDirection.Y);
                _crashPoint = TileUtilities.FallToSolidTile(_crashPoint, dir);
                _crashPoint -= _drillDirection * 64;


                _crashOutpoint = centerPoint;
                _crashOutpoint = TileUtilities.FallToSolidTile(_crashOutpoint, -dir);
                _crashOutpoint -= _drillDirection * 80;

                _outpoint = false;
                NPC.netUpdate = true;
            }
        }

        if (Timer % 5 == 0)
            DrillFX();
        if (Timer >= 120)
        {
            SwitchState(AIState.Drill);
        }
    }

    void AI_Drill()
    {

        Timer++;
        NPC.TargetClosest();
        NPC.PlayAnimationLooped(ANIM_DRILL);

        var tilePos = NPC.Center.ToTileCoordinates();
        var tile = Main.tile[tilePos];
        var diff = (_crashPoint - NPC.Center);

        if (!_outpoint)
        {
            MakeTelegraphSmokeFX();
            NPC.velocity = Vector2.Lerp(NPC.velocity, _drillDirection * DrillSpeed, 0.2f);
            var diff2 = (_crashOutpoint - NPC.Center);
            if (MathF.Sign(diff2.Y) != MathF.Sign(_drillDirection.Y))
            {
                _outpoint = true;
                AttackCycle = 35;
            }
        } 
        else if (AttackCycle > 0)
        {
            AttackCycle--;
            NPC.velocity = Vector2.Lerp(NPC.velocity, _drillDirection * DrillSpeed * 0.03f, 0.5f);
        }
        else
        {
            _contactDamage = true;
            _outliner.attacking = true;
            if (NPC.velocity.Length() < DrillSpeed)
                NPC.velocity *= 1.1f;
                //NPC.velocity = Vector2.Lerp(NPC.velocity, _drillDirection * DrillSpeed, 0.03f);
            if (_outpoint)
            {
                if (MathF.Sign(diff.Y) != MathF.Sign(_drillDirection.Y))
                {
                    SwitchState(AIState.Slam);
                }
            }
   
        }


    }


    void AI_Stuck()
    {
        Timer++;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= StuckTime)
        {
            if(NPC.velocity.Length() < 15)
                NPC.velocity *= 1.01f;
        }
        else if (NPC.velocity.Length() > 0.1f)
        {
            NPC.velocity *= 0.1f;
        }
    }

    void DrillFX()
    {
        var pos = NPC.Center + _drillDirection * 48;
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(pos, pos - _drillDirection * 4, TriColorPalette.Fiery, new Vector2(5, 16));
        fac.particleCount /= 4;
        fac.scaleRange *= 0.5f;
        ParticleUtils.CreateSwirlingDustBurst2(fac);
    }

    void MakeTelegraphSmokeFX()
    {
        var dir = _crashPoint - _crashOutpoint;
        dir = dir.SafeNormalize(Vector2.Zero);
        var color = Color.Lerp(Color.OrangeRed, Color.Black, 0.75f);
        Particles.BackCloud.Spawn(new()
        {
            position = _crashOutpoint + Main.rand.NextVector2Circular(48, 48),
            velocity = dir * Main.rand.NextFloat(10, 20f),
            color = Color.DarkGray,
            timeLeft = Main.rand.Next(60, 120),
            scale = Main.rand.NextFloat(0.5f, 1f)
        });
    }

    void ImpactFX()
    {
        FXUtil.ShakeCamera(NPC.Center, 1024, 4);
        var pos = NPC.Center + _drillDirection * 48;
        var impactSound = AssetReferences.Assets.Sounds.Irradieagle_Wave.Asset with { PitchVariance = 0.5f, Pitch = -0.5f };
        SoundEngine.PlaySound(impactSound, NPC.position);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(pos, pos - _drillDirection * 4, TriColorPalette.Fiery, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustBurst(fac);
        for(var i= 0; i < 8; i++)
        {
            var impactVelocity = -_drillDirection * 6;
            impactVelocity = impactVelocity.RotatedByRandom(2.5f);
            impactVelocity *= Main.rand.NextFloat(0.5f, 4);
            var fx = FXUtil.GlowStretch(pos, impactVelocity);
            fx.OuterGlowColor = Color.DarkOrange;
            fx.InnerColor = Color.Gold;
            fx.GlowColor = Color.Yellow;
        }
    }

    void AI_Slam()
    {
        if(Timer < 15)
        {
            _contactDamage = true;
            _outliner.attacking = true;
            NPC.PlayAnimationLooped(ANIM_DRILL);
        }
   
        Timer++;
        if(Timer == 1)
        {
            ImpactFX();
            NPC.velocity *= 0.025f;
        }
        if (Timer % 5 == 0)
            DrillFX();
        var ratio = Timer / 24;
        NPC.VectorScale = Vector2.Lerp(new Vector2(1.2f, 0.9f), Vector2.One, EasingFunction.InOutSine(ratio));
        NPC.PlayAnimationLooped(ANIM_DRILL);
        NPC.velocity = Vector2.Lerp(NPC.velocity, _drillDirection * DrillSpeed * 0.04f, MathHelper.Lerp(0f, 0.05f, EasingFunction.InSine(Timer / SlamTime)));
        if (Timer >= SlamTime)
        {
            SwitchState(AIState.Teleport);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        var color = Color.Lerp(Color.OrangeRed, Color.Black, 0.75f);
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 32))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = color,
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.5f, 1f)
            });
        }

        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 16))
        {
            Particles.GlowDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat()),
                timeLeft = Main.rand.Next(45, 120),
                scale = Main.rand.NextFloat(0.15f, 0.25f)
            });
        }

        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(5, 0));
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -5));
        this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(0, 5));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for(var i =0; i < NPC.oldPos.Length; i++)
        {
            var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
            var ratio = (float)i / (float)NPC.oldPos.Length;
            var color = Color.Lerp(Color.DarkOrange, Color.Transparent, ratio) * 0.3f;
            NPC.DrawAnimator(spriteBatch, color, pos);
        }
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
