using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.EyeProjectiles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public class RekEye : ModNPC
{
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;
    private bool _contactDamage;
    private float _afterImageAlpha;
    private float _myRemainingLifeTime;
    private float _fireballAttackCount;
    private float _eyeFlash;
    private int _frame;
    private enum AIState
    {
        Spawn,
        Idle,
        LaserBomb,
        MiniFire,
        Crash,
        Return
    }
    private Asset<Texture2D> _eyeTextureAsset;
    private Asset<Texture2D> EyeTextureAsset
    {
        get
        {
            _eyeTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Eye");
            return _eyeTextureAsset;
        }
    }
    private NPC Parent => Main.npc[(int)NPC.ai[0]];
    private ref float Timer => ref NPC.ai[1];
    private AIState State
    {
        get
        {
            return (AIState)NPC.ai[2];
        }
        set
        {
            NPC.ai[2] = (float)value;
        }
    }
    private ref float AttackCycle => ref NPC.ai[3];
    private int _attackPhase;
    private float IdleTime => 120;
    private float Fireball_Prep_Time => 70;
    private float Fireball_End_Time => 30;

    private int Laser_Beam_Bomb_Damage => 40;
    private float Laser_Bomb_Charge_Time => 60;
    private float Laser_Bomb_Prep_Time => 60;
    private float Laser_Bomb_Shoot_Radians => 180;

    private int Crash_Damage => 60;
    private int Fireball_Damage => 40;

    private Outliner _outliner;

    private Color _eyeColor;
    private Vector2 _eyeOffset;
    private Vector2 _pointToMoveToward;
    private Player MyTarget => Main.player[NPC.target];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_attackPhase);
        writer.WriteVector2(_pointToMoveToward);
        writer.Write(_myRemainingLifeTime);
        writer.WriteVector2(_startPosition);
        writer.WriteVector2(_initialVelocity);
        writer.Write(_fireballAttackCount);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _attackPhase = reader.ReadInt32();
        _pointToMoveToward = reader.ReadVector2();
        _myRemainingLifeTime = reader.ReadSingle();
        _startPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _fireballAttackCount = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCSets.Heavy[Type] = true;
        Main.npcFrameCount[Type] = 32;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 80;
        NPC.height = 80;
        NPC.damage = 80;
        NPC.defense = 10;
        NPC.lifeMax = 5500;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 10f;
        NPC.aiStyle = -1;
    }
    
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.5f;
        if(NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0;
            if (_frame >= Main.npcFrameCount[Type])
                _frame = 0;
        }
        NPC.frame.Y = frameHeight * _frame;
    }
    private void CreateSpawnEffect()
    {

        for (int i = 0; i < 8; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
            var d = Dust.NewDustPerfect(NPC.Center, DustID.Torch, speed * 17, Scale: 1f);
            d.noGravity = true;

            Vector2 speeda = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
            var da = Dust.NewDustPerfect(NPC.Center, DustID.OrangeTorch, speeda * 11, Scale: 1f);
            da.noGravity = false;

            Vector2 speedab = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
            var dab = Dust.NewDustPerfect(NPC.Center, DustID.Torch, speeda * 30, Scale: 1f);
            dab.noGravity = false;
        }

        FXUtil.GlowCircleBoom(NPC.Center,
             innerColor: Color.White,
             glowColor: Color.Yellow,
             outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
    }
    public override void AI()
    {
        base.AI();
 
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
        }

        NPC.realLife = Parent.whoAmI;
        if(NPC.velocity.Length() > 5)
        {
            _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, 1f, 0.1f);
        }
        else
        {
            _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, 0f, 0.1f);
        }

        if (Main.rand.NextBool(8))
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = -Vector2.UnitY * 4;
            var d = Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1.2f, 2.1f));
            d.noGravity = true;
        }
        if (Main.rand.NextBool(8))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = NPC.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 25f),
                timeLeft = 45,
                innerColor = Color.Yellow.ToVector4(),
                outerColor = Color.Red.ToVector4()
            });
        }
        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.1f));
        _contactDamage = false;
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.LaserBomb:
                AI_LaserBomb();
                break;
            case AIState.MiniFire:
                AI_MiniFire();
                break;
            case AIState.Crash:
                AI_Crash();
                break;
            case AIState.Return:
                AI_Return();
                break;
        }
        _eyeColor = Color.Lerp(_eyeColor, Color.White, 0.04f);
        _eyeOffset = Vector2.Lerp(_eyeOffset, Vector2.Zero, 0.1f);
        _eyeFlash *= 0.96f;
        _myRemainingLifeTime--;
        if (_myRemainingLifeTime <= 0 || !Parent.active)
        {
            if (State != AIState.Return)
                SwitchState(AIState.Return);
        }
    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            Timer = 0;
            AttackCycle = 0;
            _fireballAttackCount = 0;
        }
    }

    private void AI_Return()
    {
        Timer++;
        if(Timer == 1)
        {
            _startPosition = NPC.Center;
            _pointToMoveToward = Parent.Center;
            _initialVelocity = NPC.velocity;
        }
        float time = 120;
        float ratio = Timer / time;
        float ease = EasingFunction.InExpo(ratio);
        Vector2 pointToMoveTowards = Vector2.Lerp(_startPosition, _pointToMoveToward, ease);
        Vector2 vel = pointToMoveTowards - NPC.Center;
        Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(Timer / 30f));
        NPC.velocity = easedVelocity;
        if(Timer >= time)
        {
            CreateSpawnEffect();
            CreateFirebreathChargeEffect(NPC.Center);
            if (MultiplayerHelper.IsHost)
            {
                ProjFirer projFirer = ProjFirer.From<PacmanBoom>(NPC);
                projFirer.New();
            }
            //I think setting active false won't kill the other npc?
            //I hope not
            NPC.active = false;
        }
    }
    private void AI_Spawn()
    {
        Timer++;
        if(Timer == 1)
        {
            CreateSpawnEffect();
            _myRemainingLifeTime = 1800;
        }

        if(Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void AI_Idle()
    {
        Timer++;
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }

        Vector2 pos = MyTarget.Center;
        pos.Y -= 90;
        Vector2 v = NPC.velocity;
        v.X *= 0.94f;
        v.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.01f) * 0.5f, 0.1f);
        v += (pos - NPC.Center).SafeNormalize(Vector2.Zero) * 1f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, v, 0.2f);
        NPC.rotation *= 0.94f;
    }
    private void AI_MiniFire()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        var sound = AssetRegistry.Sounds.Rek.SmallFlameBlast with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        NPC.TargetClosest();
                        _pointToMoveToward = MyTarget.Center + new Vector2(0, -128);
                        _startPosition = NPC.Center;
                        _initialVelocity = NPC.velocity;
                    }
                    LookAtPlayer();
                    _pointToMoveToward = _pointToMoveToward.RotatedBy(0.08f, MyTarget.Center);
                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time * 0.5f);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ease);
                    posToMoveTo = Vector2.Lerp(posToMoveTo, MyTarget.Center, ease * 0.5f);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
                    NPC.velocity = easedVelocity;
                    if(Timer >= Fireball_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    _outliner.warning = true;
                }
                break;
            case 1:
                {
                    LookAtPlayer();
                    _outliner.warning = true;
                    float dist = Vector2.Distance(NPC.Center, MyTarget.Center);
                    if (dist < 256)
                    {
                        NPC.velocity -= (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }
                    else
                    {
                        NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }

                    if (Main.rand.NextBool(2))
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(384, 384);
                        Vector2 vel = (NPC.Center - pos);
                        vel *= 0.05f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.OuterGlowColor = Color.Yellow;
                        fx.Scale *= 0.5f;
                    }

                    if(Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    LookAtPlayer();
                    if (Timer == 1 && _fireballAttackCount == 0)
                    {
                        NPC.TargetClosest();
                    }
                    if(Timer == 14)
                    {
                        CreateFirebreathChargeEffect(NPC.Center);
                    }

                    _outliner.attacking = true;
                    NPC.velocity *= 0.91f;

                    float dist = Vector2.Distance(NPC.Center, MyTarget.Center);
                    if(dist < 256)
                    {
                        NPC.velocity -= (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }
                    else
                    {
                        NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }

                    if (Timer >= 38)
                    {
                        CreateFireShoot(NPC.Center);
                        NPC.velocity = (MyTarget.Center - NPC.Center);
                        NPC.velocity *= -1;
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero);
                        NPC.velocity *= 8;
                        Timer = 0;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<BigVulcanFireball>(NPC);
                            firer.damage = Fireball_Damage;
                            firer.ai1 = 0.6f;
                            firer.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 18;
                            firer.New();
                        }

                        _fireballAttackCount++;
                        if(_fireballAttackCount >= 3)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.94f;
                    if(Timer >= Fireball_End_Time)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    public void CreateFirebreathChargeEffect(Vector2 position)
    {
        var sound = AssetRegistry.Sounds.RekFireballShoot with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(sound, position);

        for (float f = 0; f < 8; f++)
        {
            Vector2 pos = position + Main.rand.NextVector2CircularEdge(384, 384);
            Vector2 vel = (position - pos);
            vel *= 0.05f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Yellow;
            fx.Scale *= 0.5f;
        }

        if (Main.netMode != NetmodeID.Server)
        {
            PixelPrimitiveCircleFactory.CreateRekInwardBoom(position);
        }

    }
    public void CreateFireShoot(Vector2 position)
    {
        _eyeColor = Color.Red;
        var sound = AssetRegistry.Sounds.RekFireballShoot with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(sound, position);

        FXUtil.ShakeCamera(position, 1024, 8);
        for (float f = 0; f <8; f++)
        {
            Vector2 vel = _eyeOffset;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45);
            vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
            var dp = DustParticle.Spawn(position + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), vel);
            dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            dp.outerColor = Color.Red;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.Scale *= Main.rand.NextFloat(1f, 1.5f);
        }
        for (float f = 0; f < 16; f++)
        {
            Vector2 vel = _eyeOffset;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45);
            vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
            Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Torch, vel, Scale: 2f);
        }
        for (float f = 0; f < 16; f++)
        {
            Vector2 vel = _eyeOffset;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45);
            vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
            Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Lava, vel, Scale: 2f);
        }
    }
    private void LookAtPlayer()
    {
        Vector2 dir = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        dir *= 40;
        _eyeOffset = Vector2.Lerp(_eyeOffset, dir, 0.3f);
    }

    private void AI_LaserBomb()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _pointToMoveToward = MyTarget.Center + new Vector2(0, -128);
                        _startPosition = NPC.Center;
                        _initialVelocity = NPC.velocity;
                    }
                    LookAtPlayer();
                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ease);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
                    NPC.velocity = easedVelocity;
                    if (Timer >= Laser_Bomb_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    _outliner.warning = true;
                }
                break;
            case 1:
                {
                    LookAtPlayer();
                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.01f) * 0.5f, 0.1f);
                    NPC.velocity += ((MyTarget.Center + new Vector2(0, -128)) - NPC.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    NPC.rotation *= 0.94f;
                    if(Timer % 60 == 0)
                    {
                        CreateFirebreathChargeEffect(NPC.Center);
                    }
                    _eyeFlash = Timer / Laser_Bomb_Charge_Time;
                    _outliner.warning = true;
                    if (Timer >= Laser_Bomb_Charge_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _eyeColor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 36));

                    float dist = Vector2.Distance(NPC.Center, MyTarget.Center);
                    if (dist < 256)
                    {
                        NPC.velocity -= (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }
                    else
                    {
                        NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.7f;
                    }
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<LaserBeamBomb>(NPC);
                            firer.damage = Laser_Beam_Bomb_Damage;
                            firer.velocity = (MyTarget.Center - NPC.Center).RotatedBy(-0.5f);
                            firer.ai1 = MathHelper.ToRadians(135);
                            firer.ai2 = NPC.whoAmI;
                            firer.New();
                        }
                    }
                    if(Timer >= 100f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.94f;
                    if (Timer >= Fireball_End_Time)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_Crash()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        var sound = AssetRegistry.Sounds.Rek.SmallFlameBlast with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        NPC.TargetClosest();
                        _pointToMoveToward = MyTarget.Center + new Vector2(0, -196);
                        _startPosition = NPC.Center;
                        _initialVelocity = NPC.velocity;
                    }
                    LookAtPlayer();
                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ease);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
                    NPC.velocity = easedVelocity;
                    if (Timer >= Laser_Bomb_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 1:
                {
                    LookAtPlayer();
                    _outliner.warning = true;
                    NPC.velocity *= 0.94f;
                    if(Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _outliner.attacking = true;
                    //Give some initial velocity
                    if (Timer == 1)
                    {
                        NPC.velocity.Y = -6;
                    }

                    //Calculate Stomp Velocity
                    if (NPC.velocity.Y > 1)
                    {
                        NPC.velocity.Y *= MathHelper.Lerp(1.01f, 1.12f, EasingFunction.InExpo(Timer / 30f));
                        if (Timer % 5 == 0)
                        {
                            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero));
                            p.fadeToColor = Color.DarkRed;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < 0)
                            NPC.velocity.Y += 0.15f;
                        else if(NPC.velocity.Y < 15)
                            NPC.velocity.Y += 1.5f;
                    }

                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(64, 64);
                    Vector2 vel = -Vector2.UnitY * 4;
                    var d = Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1.2f, 2.1f));
                    d.noGravity = true;

                    Tile tile = Main.tile[NPC.position.ToTileCoordinates()];

                    if (tile.LiquidAmount > 0 || Timer >= 180)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            float radius = 150;
                            Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                            offset *= Main.rand.NextFloat(1f, radius);
                            offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                            velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                                Main.rand.NextFloat(0.3f, 0.7f));
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            float radius = 150;
                            Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                            offset *= Main.rand.NextFloat(1f, radius);
                            offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                            velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                                Main.rand.NextFloat(0.3f, 0.7f));
                        }

                        FXUtil.GlowCircleBoom(NPC.Bottom,
                           innerColor: Color.White,
                           glowColor: Color.Red,
                           outerGlowColor: Color.Black, duration: 25, baseSize: 0.34f);
                        for (float i = 0; i < 4; i++)
                        {
                            float progress = i / 4f;
                            float rot = progress * MathHelper.ToRadians(240);
                            Vector2 offset = rot.ToRotationVector2() * 24;
                            var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                                innerColor: Color.White,
                                glowColor: Color.Red,
                                outerGlowColor: Color.Black,
                                baseSize: 0.34f);
                            particle.Rotation = rot + MathHelper.ToRadians(45);
                        }

                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 50f);
                            var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                            particle.InnerColor = Color.White;
                            particle.GlowColor = Color.Red;
                            particle.OuterGlowColor = Color.Black;
                            particle.Duration = Main.rand.NextFloat(25, 50);
                            particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                            particle.VectorScale *= 0.5f;
                        }


                        ShakeScreenPosition.Shake = 16;
                        FXUtil.ShakeCamera(NPC.position, 1024, 129);
                        SoundStyle boom = new SoundStyle("Stellamod/Assets/Sounds/RocketExplosion");
                        boom.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom, NPC.position);

                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 3:
                {
                    if(Timer == 1)
                    {
       
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<MeteorBoom>(NPC);
                            firer.damage = Crash_Damage;
                            firer.position = NPC.Center;
                            firer.velocity = -NPC.velocity.SafeNormalize(Vector2.Zero) * 1544;
                            firer.New();

                            firer.position = NPC.Center;
                            firer.position.X -= 128;
                            firer.velocity = -NPC.velocity.SafeNormalize(Vector2.Zero) * 1544;
                            firer.velocity = firer.velocity.RotatedBy(-0.05f);
                            firer.New();

                            firer.position = NPC.Center;
                            firer.position.X += 128;
                            firer.velocity = -NPC.velocity.SafeNormalize(Vector2.Zero) * 1544;
                            firer.velocity = firer.velocity.RotatedBy(0.05f);
                            firer.New();

                            for(int i = 0; i < 7; i++)
                            {
                                ProjFirer lilFirer = ProjFirer.From<BigVulcanFireball>(NPC);
                                lilFirer.damage = Fireball_Damage;
                                lilFirer.ai1 = Main.rand.NextFloat(0.3f, 0.7f);
                                lilFirer.position = NPC.Center + Main.rand.NextVector2Circular(128, 64);
                                lilFirer.velocity = -Vector2.UnitY * 18;
                                lilFirer.velocity = lilFirer.velocity.RotatedByRandom(0.8f);
                                lilFirer.New();
                            }
                        }
                    }
                    FXUtil.ApplyContrast(MathHelper.Lerp(0.5f, 0f, Timer / 30f));
                    NPC.velocity *= 0.94f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    if(Timer >= Fireball_End_Time)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }


    private void ChooseAttack()
    {
        switch (_attackPhase)
        {
            case 0:
                SwitchState(AIState.LaserBomb);
                break;
            case 1:
                SwitchState(AIState.MiniFire);
                break;
            case 2:
                SwitchState(AIState.Crash);
                break;
        }

        _attackPhase++;
        _attackPhase %= 3;
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 16, ratio) * _afterImageAlpha;
        }
        Color GetTrailColor(float ratio)
        {
            return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.Orange, Color.Red, Color.DarkRed, Color.Black) * _afterImageAlpha * EasingFunction.OutSine(ratio);
            //    return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _afterImageAlpha;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;

        flameTrailShader.LaserTexture = AssetManager.LaserTextures.Aura.Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(NPC.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, NPC.Size * 0.5f);
        TrailDrawer.Draw(NPC.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, NPC.Size * 0.5f);
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        PixelationManager.QueueSpritebatchDrawAction(DrawEye, DrawLayer.OverPlayers);

        OutlineRenderer.Queue(DrawWhite);
        return false;
    }

    private void DrawEye(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.rotation = NPC.velocity.X * 0.05f;

        RedSunShader redSunShader = ShaderContent.GetInstance<RedSunShader>();
        redSunShader.Time = Main.GlobalTimeWrappedHourly * 9;
        redSunShader.InsideColor = Color.Yellow;
        redSunShader.BloomColor = Color.DarkRed;
        redSunShader.FlameNoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        Main.spriteBatch.Restart(SpriteSortMode.Immediate, effect: redSunShader.Effect, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
        SpritebatchDrawer redSunDrawer = SpritebatchDrawer.FromTextureAsset(AssetRegistry.NoiseTextures.WaterTrail.Asset, NPC.Center);
        redSunDrawer.scale *= NPC.scale * 0.6f;
        redSunDrawer.color = Color.White;
        redSunDrawer.color.A = 0;
        Main.spriteBatch.Draw(redSunDrawer);
        Main.spriteBatch.RestartDefaults();
        Main.spriteBatch.Draw(drawer);
        SpritebatchDrawer glow = SpritebatchDrawer.FromTextureAsset(AssetRegistry.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glow.scale *= NPC.scale * 0.6f;
        glow.color = Color.Red * ExtraMath.Osc(0.6f, 1f, speed: 3);
        glow.color.A = 0;
        Main.spriteBatch.Draw(glow);




        SpritebatchDrawer eyeDrawer = SpritebatchDrawer.FromTextureAsset(EyeTextureAsset, NPC.Center + _eyeOffset);
        eyeDrawer.color = _eyeColor;

        Main.spriteBatch.Draw(eyeDrawer);
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            var glowEyeDrawer = SpritebatchDrawer.FromTextureAsset(EyeTextureAsset, NPC.Center + _eyeOffset);
            glowEyeDrawer.worldPosition += (f + Main.GlobalTimeWrappedHourly * 4).ToRotationVector2() * 4;
            glowEyeDrawer.color = Color.Goldenrod * 0.4f;
            glowEyeDrawer.color.A = 0;
            Main.spriteBatch.Draw(glowEyeDrawer);
        }
        Vector2 scale = Vector2.Lerp(Vector2.One, Vector2.Zero, _eyeFlash);
        float rot = MathHelper.Lerp(MathHelper.ToRadians(55), 0, _eyeFlash);

        SpritebatchDrawer sparkleDrawer = SpritebatchDrawer.FromTextureAsset(AssetRegistry.GlowMasks.Star2.Value, NPC.Center);
        sparkleDrawer.color = Color.White * 1f * _eyeFlash;
        sparkleDrawer.color.A = 0;
        sparkleDrawer.rotation = rot;
        sparkleDrawer.scale = scale;
        Main.spriteBatch.Draw(sparkleDrawer);
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.color = _outliner.outlineColor;
        drawer.color.A = 0;
        drawer.rotation = NPC.velocity.X * 0.05f;
        Main.spriteBatch.Draw(drawer);
    }
}
