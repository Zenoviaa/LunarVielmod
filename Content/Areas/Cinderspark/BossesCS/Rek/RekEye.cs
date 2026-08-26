using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.EyeProjectiles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
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

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public class RekEye : ModNPC
{
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;
    private bool _contactDamage;
    private float _afterImageAlpha;
    private float _myRemainingLifeTime;
    private float _fireballAttackCount;
    private int _frame;
    private enum AIState
    {
        Spawn,
        Idle,
        LaserBomb,
        MiniFire,
        Crash
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
    private float IdleTime => 60;
    private float Fireball_Prep_Time => 45;
    private float Fireball_End_Time => 30;

    private int Laser_Beam_Bomb_Damage => 40;
    private float Laser_Bomb_Charge_Time => 60;
    private float Laser_Bomb_Prep_Time => 60;
    private float Laser_Bomb_Shoot_Radians => 180;


    private Outliner _outliner;

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
        NPC.frameCounter += 0.25f;
        if(NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0;
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
        }
        _myRemainingLifeTime--;
        if (_myRemainingLifeTime <= 0 || !Parent.active)
        {
            CreateSpawnEffect();
            //I think setting active false won't kill the other npc?
            //I hope not
            NPC.active = false;
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

    private void AI_Spawn()
    {
        Timer++;
        if(Timer == 1)
        {
            CreateSpawnEffect();
            _myRemainingLifeTime = 1200;
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

        NPC.velocity.X *= 0.94f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.01f) * 0.5f, 0.1f);
        NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.15f;
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

                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time * 0.5f);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ratio);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
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
                    if(Timer == 1 && _fireballAttackCount == 0)
                    {
                        NPC.TargetClosest();
                    }

                    _outliner.attacking = true;
                    NPC.velocity *= 0.94f;
                    if(Timer >= 30)
                    {
                        NPC.velocity = (MyTarget.Center - NPC.Center);
                        NPC.velocity *= -1;
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero);
                        NPC.velocity *= 14;
                        Timer = 0;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<BigVulcanFireball>(NPC);
                            firer.ai1 = 0.6f;
                            firer.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
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
            case 2:
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
        for (float f = 0; f < 8; f++)
        {
            Vector2 pos = position + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (position - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Turquoise;
            fx.Scale *= 0.5f;
        }

        if (Main.netMode != NetmodeID.Server)
        {
            var screenShader = ModContent.GetInstance<ScreenShaderSystem>();
            screenShader.TintScreen(Color.Red, 0.1f, 15f);
            PixelPrimitiveCircleFactory.CreateRekInwardBoom(position);
        }

        for (float f = 0; f < 12; f++)
        {
            Vector2 pos = position + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (position - pos);
            vel *= 0.1f;

            DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
            spawnparams.innerColor = Color.Lerp(Color.White, Color.Red, Main.rand.NextFloat(0f, 1f));
            spawnparams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(pos, vel, spawnparams);
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }
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

                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time * 0.5f);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ratio);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
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
                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.01f) * 0.5f, 0.1f);
                    NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                    NPC.rotation *= 0.94f;
                    if(Timer % 15 == 0)
                    {
                        CreateFirebreathChargeEffect(NPC.Center);
                    }

                    _outliner.attacking = true;
                    if (Timer >= Laser_Bomb_Charge_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.01f) * 0.5f, 0.1f);
                    NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                    NPC.rotation *= 0.94f;
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<LaserBeamBomb>(NPC);
                            firer.damage = Laser_Beam_Bomb_Damage;
                            firer.velocity = (MyTarget.Center - NPC.Center).RotatedBy(-0.5f);
                            firer.ai1 = MathHelper.ToRadians(180);
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

                    float ratio = Timer / Fireball_Prep_Time;
                    float ease = EasingFunction.InOutQuad(ratio);
                    float ease2 = Timer / (Fireball_Prep_Time * 0.5f);
                    ease2 = EasingFunction.InOutSine(ease2);
                    Vector2 posToMoveTo = Vector2.Lerp(_startPosition, _pointToMoveToward, ratio);
                    Vector2 vel = posToMoveTo - NPC.Center;
                    Vector2 easedVelocity = Vector2.Lerp(_initialVelocity, vel, ease2);
                    if (Timer >= Laser_Bomb_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 1:
                {
                    Timer++;
                    _outliner.attacking = true;
                    //Give some initial velocity
                    if (Timer == 1)
                    {
                        NPC.velocity.Y = -9;
                    }

                    //Calculate Stomp Velocity
                    _contactDamage = true;
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
                        NPC.velocity.Y += 1.5f;
                    }

                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(64, 64);
                    Vector2 vel = -Vector2.UnitY * 4;
                    var d = Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1.2f, 2.1f));
                    d.noGravity = true;

                    if (Timer > 60)
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

            case 2:
                {
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
        redSunDrawer.scale *= NPC.scale;
        redSunDrawer.color = Color.White;
        redSunDrawer.color.A = 0;
        Main.spriteBatch.Draw(redSunDrawer);
        Main.spriteBatch.RestartDefaults();
        Main.spriteBatch.Draw(drawer);
        OutlineRenderer.Queue(DrawWhite);
        return false;
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
