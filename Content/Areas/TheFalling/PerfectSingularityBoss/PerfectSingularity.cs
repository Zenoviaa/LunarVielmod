using Microsoft.Xna.Framework.Input;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss;

public class PerfectEyesShader : CrystalShader<PerfectEyesShader>
{
    public Texture2D Texture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[0] = value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }
    }
    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
        }
    }
    public float DistortionStrength
    {
        set
        {
            Effect.Parameters["distortionStrength"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}
public class PerfectRingShader : CrystalShader<PerfectRingShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}

public class PerfectSingularityShader : CrystalShader<PerfectSingularityShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Texture2D Eyes
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
        }
    }
}

public partial class PerfectSingularity : ScarletBoss
{
    private enum PerfectSingularityPhase
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Chain_Whip,
        Chain_Jail,
        Chain_Arrow,
        Astral_Chains,
        Astral_Blast,

        Astral_Dash,
        Meteor_Rain,
        Solare_Flare,
        Alien_Laser_Beam
    }

    private Vector2 _intensityShake;
    private float _intensityTimeLeft;
    private float _intensity;
    private bool _contactDamage;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private PerfectSingularityPhase Phase
    {
        get => (PerfectSingularityPhase)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    //CHAIN WHIP ATTACK 
    private int ChainWhip_Damage => 90;
    private float ChainWhip_Count => 5;
    private float ChainWhip_TimeBetweenAttacks => 30;
    private float ChainWhip_StartupTime => 90;
    private float ChainWhip_FireTime => 55;

    //CHAIN JAIL ATTACK
    private int ChainJail_Damage => 90;
    private float ChainJail_Count => 15;
    private float ChainJail_TimeBetweenAttacks => 30;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.MustAlwaysDraw[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 90;
        NPC.height = 90;
        NPC.damage = 150;
        NPC.defense = 66;
        NPC.lifeMax = 300000;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ItRains");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override bool? CanBeHitByItem(Player player, Item item) => false;
    public override bool CanBeHitByNPC(NPC attacker) => false;
    public override bool? CanBeHitByProjectile(Projectile projectile) => false;
    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (Phase != PerfectSingularityPhase.Despawn)
                {
                    SwitchState(PerfectSingularityPhase.Despawn);
                }
            }
        }

        var g = Keyboard.GetState();
        if (g.IsKeyDown(Keys.L))
        {
            SwitchState(PerfectSingularityPhase.Chain_Whip);
        }
        if(_intensityTimeLeft > 0)
        {
            if (Main.rand.NextBool(4))
            {
                _intensityShake = Main.rand.NextVector2Circular(8, 8);
                ShakeScreenPosition.Shake = 3;
            }
            _intensityTimeLeft--;
        }
        _contactDamage = false;
        _outliner.SetDefaults();
        switch (Phase)
        {
            case PerfectSingularityPhase.Spawn:
                AI_Spawn();
                break;
            case PerfectSingularityPhase.Despawn:
                AI_Despawn();
                break;
            case PerfectSingularityPhase.Idle:
                AI_Idle();
                break;
            case PerfectSingularityPhase.Death:
                AI_Death();
                break;
            case PerfectSingularityPhase.Chain_Whip:
                AI_ChainWhip();
                break;
            case PerfectSingularityPhase.Chain_Jail:
                AI_ChainJail();
                break;
            case PerfectSingularityPhase.Chain_Arrow:
                AI_ChainArrow();
                break;
            case PerfectSingularityPhase.Astral_Chains:
                AI_AstralChains();
                break;
            case PerfectSingularityPhase.Astral_Blast:
                AI_AstralBlast();
                break;
            case PerfectSingularityPhase.Astral_Dash:
                AI_AstralDash();
                break;
            case PerfectSingularityPhase.Meteor_Rain:
                AI_MeteorRain();
                break;
            case PerfectSingularityPhase.Solare_Flare:
                AI_SolarFlare();
                break;
            case PerfectSingularityPhase.Alien_Laser_Beam:
                AI_AlienLaser();
                break;
        }
        _outliner.Update();
        NPC.rotation = NPC.velocity.X * 0.05f;
    }

    private void ChooseAttack()
    {

    }

    private void AI_AlienLaser()
    {

    }
    private void AI_SolarFlare()
    {

    }
    private void AI_MeteorRain()
    {

    }
    private void AI_AstralDash()
    {

    }
    private void AI_AstralBlast()
    {

    }
    private void AI_AstralChains()
    {

    }
    private void AI_ChainArrow()
    {

    }
    private void AI_ChainJail()
    {
        //Chain Jail - Chains quickly scroll through and whip around, kinda like that one attack on sister splinter in how they spawn in
        //For this attack, we should randomly select points and the chain hitscanes will raycast down, and then inverse so they go upward
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        PlayIntensifySound();
                        NPC.TargetClosest();
                    }

                    //Reusing some of the same functions for visuals here
                    NPC.velocity *= 0.9f;
                    EmitIntensityParticles(rate: 4);
                    Intensify(ChainWhip_StartupTime, intensity: 0.1f);
                    _outliner.warning = true;
                    if(Timer >= ChainWhip_StartupTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {

                }
                break;
        }
    }
    private void AI_ChainWhip()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        Intensify(ChainWhip_StartupTime, intensity: 0.1f);
                       
                        PlayIntensifySound();
                        NPC.TargetClosest();
                    }

                    //Pulls a chain out of the ground and then whips it at you, there is a very simple attack
                    //Basically he aims at you and a chain juts out of him
                    //Then it impacts the ground and slides towards you, so you just walk away or roll through it
                    //He does this 5 times
                    NPC.velocity *= 0.9f;
                    EmitIntensityParticles(rate: 4);
             
                    _outliner.warning = true;
                    if(Timer >= ChainWhip_StartupTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        //Rest of the effects will be on the chain itself.
                        Vector2 velToTarget = (MyTarget.Center - NPC.Center);
                        velToTarget = velToTarget.SafeNormalize(Vector2.Zero);
                        Recoil(-velToTarget * 16);
                        PlayChainwhipSound();
                        if (MultiplayerHelper.IsHost)
                        {
                     
                            Vector2 spawnPoint = NPC.Center;
                            spawnPoint += velToTarget * Main.rand.NextFloat(350, 380);

                            //ai1 dictates the style of chain, we're reusing this projectile a lot btw.
                            Projectile.NewProjectile(SourceFromThis, spawnPoint, velToTarget, 
                                ModContent.ProjectileType<ChainHitscan>(), ChainWhip_Damage, 1, Main.myPlayer, ai1: 0);
                        }
                    }
                    _outliner.attacking = true;
                    if(Timer >= ChainWhip_FireTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    //A bit of delay between the attacks to make sure that it's fair
                    _outliner.attacking = true;
                    if (Timer >= ChainWhip_TimeBetweenAttacks)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter >= ChainWhip_Count)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle--;
                        }
                    }
                }
                break;
            case 3:
                {
                    SwitchState(PerfectSingularityPhase.Idle);
                }
                break;
        }

    }
    private void AI_Spawn()
    {
        SwitchState(PerfectSingularityPhase.Idle);
    }

    private void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.8f;
        NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 0.5f;
    }
    private void AI_Despawn()
    {

    }
    private void AI_Death()
    {

    }
    private void SwitchState(PerfectSingularityPhase phase)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            Phase = phase;
            AttackCounter = 0;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
        Main.NewText(phase);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
