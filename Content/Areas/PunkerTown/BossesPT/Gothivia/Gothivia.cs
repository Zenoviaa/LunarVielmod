using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public class FireVortexSmokeShader : CrystalShader<FireVortexSmokeShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        }
    }
    public Color GradientTopColor
    {
        set
        {
            Effect.Parameters["gradientTopColor"].SetValue(value.ToVector4());
        }
    }

    public Color GradientBottomColor
    {
        set
        {
            Effect.Parameters["gradientBottomColor"].SetValue(value.ToVector4());
        }
    }
    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
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
public class FireVortexShader : CrystalShader<FireVortexShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        }
    }
    public Color GradientTopColor
    {
        set
        {
            Effect.Parameters["gradientTopColor"].SetValue(value.ToVector4());
        }
    }

    public Color GradientBottomColor
    {
        set
        {
            Effect.Parameters["gradientBottomColor"].SetValue(value.ToVector4());
        }
    }
    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
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

public partial class Gothivia : ScarletBoss
{
    private enum WingsPerspective : byte
    {
        ThreeQ,
        FourQ
    }
    private enum AIState
    {
        Spawn,
        Death,
        Despawn,

        Idle,

        //This is where she summons the discs
        Dichotamy,

        //This is where she does the blowtorches
        Archery,

        //Bounce Kick
        Kick,

        //This is the one 
        BoostBounce,

        Suns,

        //The infinity sign
        SunCharge,

        //Fire Tornado
        FireTornado,

        ReallyStartGoth,
        StartGoth,
        BoostBounce1,
        BoostBounce2,
        BoostBounce3,
        SunExplosionCharge1,
        SunExplosionCharge2,
        Suns1,
        Suns2,
        BonfireLeft,
        BonfireRight,
        TheZoomer,
        ExplodeOut,
        StandCuss,
        Desperation,
        Invisible,
    }

    private WingsPerspective _wingsPerspective;
    private bool _contactDamage;
    private float _telegraphLineOffTimer;
    private float _telegraphLineAlpha;
    private float _bowDissipateAlpha;
    private int _bowFrame;
    private Vector2 _aimingVelocity;
    private Outliner _outliner;
    private AnimationFramer _wingAnimationFrame;
    private AnimationFramer _bowAnimationFrame;
    private ref float Timer => ref NPC.ai[0];

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Empress of the Green sun and nature. Everything empowering and living falls under her reign.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Gothivia, One of the Green Sun", "2"))
            });
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 1;

        NPCID.Sets.MustAlwaysDraw[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 60;
        NPC.height = 60;
        NPC.damage = 100;
        NPC.defense = 150;
        NPC.lifeMax = 300000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 99);
        NPC.boss = true;
        NPC.npcSlots = 10f;
        NPC.scale = 1f;

        NPC.aiStyle = -1;
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gothivia");
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }
    private float Ground => 16000;
    private void EnablePlatformArena()
    {
        DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
        fallSystem.noWings = true;
        fallSystem.inSpace = true;
        fallSystem.hoveringPlatform = true;
        fallSystem.hoverPlatformY = Ground;
        //     fallSystem.noProjTileCollide = true;

    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    private void CreateFlameNSmokeParticles()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        Main.windSpeedTarget = 0.5f;
        if (Main.rand.NextBool(8))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(Main.screenHeight, Main.screenHeight + 300);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            var ufp = UnderworldFlameParticle.Spawn(pos, -Vector2.UnitY * 10 + Vector2.UnitX * 5, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            ufp.ySlow = false;
        }
        if (Main.rand.NextBool(3))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            UnderworldSmokeParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.5f, 0.8f));
        }
    }

    public override BossLevel GetBossLevel()
    {
        return BossLevel.Superboss;
    }

    public override void AI()
    {
        base.AI();
        EnablePlatformArena();
        CreateFlameNSmokeParticles();
        _outliner.SetDefaults();

        //Animate the wings
        //The perspective only decides which wing texture to use
        //We'll set that in the ai states, check the original code

        _wingsPerspective = WingsPerspective.ThreeQ;
        _wingAnimationFrame.maxFrame = 60;
        _wingAnimationFrame.frameSpeed = 2;
        _wingAnimationFrame.UpdateTick();
        _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
        switch (State)
        {
            case AIState.Spawn:
                SwitchState(AIState.Idle);
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Dichotamy:
                AI_Dichotamy();
                break;
            case AIState.Archery:
                AI_Archery();
                break;
        }
        if(_telegraphLineOffTimer > 0)
        {
            _telegraphLineOffTimer--;
            _telegraphLineAlpha *= 0.4f;
        }
        _outliner.Update();
    }

    private void AI_Idle()
    {
        _wingsPerspective = WingsPerspective.FourQ;
        NPC.velocity *= 0.96f;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            Vector2 targetCenter = MyTarget.Center;
            Vector2 targetHoverCenter = targetCenter + new Vector2(312, 0);
            NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

            float hoverSpeed = 5;
            float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
            NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
        }

        if (Timer < 50)
        {
            NPC.velocity.Y -= 0.08f;
        }

        if (Timer >= 60)
        {
            SwitchState(AIState.Dichotamy);
            NPC.velocity.Y *= 0;
        }
    }

    private void AI_Dichotamy()
    {
        NPC.velocity *= 0.96f;
        Timer++;
        Player player = Main.player[NPC.target];
        float ai1 = NPC.whoAmI;
        if (Timer == 1)
        {
            FXUtil.ApplyVignette(2f, timer: 150);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSummon") { PitchVariance = 0.3f }, NPC.Center);
            PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 80, 460);
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);
            }
        }

        if (Timer == 80)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DUAL2") { PitchVariance = 0.5f }, NPC.Center);
            ShakeScreenPosition.Shake = 5;
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Vector2.UnitY * 512;
                    offset = offset.RotatedBy(i / 2f * MathHelper.TwoPi);
                    Vector2 spawnPoint = NPC.Center + offset;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, -offset, ModContent.ProjectileType<BouncingRazorSuns>(), 1, 1, Main.myPlayer, ai2: i);
                }
            }
        }


        if (Timer >= 150)
        {
            SwitchState(AIState.Archery);
        }
    }

    private float _circleDegrees;
    private float _circleDistance;
    private float _circleSpeed;
    private float _movementSpeed;
    private float _accelTimer;
    private void FaceTarget()
    {
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    private void AI_Archery()
    {
        void BowShot()
        {
            //Setting the attack cycle to 1 in this case does the bow shot
            AttackCycle = 2;
            PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 80, 460);
            _telegraphLineAlpha = 0;
            _telegraphLineOffTimer = 45;
            if (!MultiplayerHelper.IsHost)
                return;


           // Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 2400;
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, _aimingVelocity.SafeNormalize(Vector2.Zero) * 2400,
                ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer);
        }
        
        _outliner.attacking = true;

        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }


        FaceTarget();
        Vector2 velocity = NPC.Center.DirectionTo(MyTarget.Center) * 10;
        float ai1 = NPC.whoAmI;
        if (Timer == 3)
        {
            _circleDistance = 270;
        }

        if (Timer == 80)
        {
            _movementSpeed = 12;
            _circleSpeed = 3;
        }

        if (Timer == 170)
        {
            _movementSpeed = 25;

        }

        if (Timer == 210)
        {
            _movementSpeed = 16;
        }


        if (Timer == 240)
        {
            _movementSpeed = 12;
            _circleSpeed = 2;
        }


        void Circle()
        {
            float movementSpeed = 17;
            Vector2 offset = -Vector2.UnitY * 200;
            offset = offset.RotatedBy(MathHelper.ToRadians(_circleDegrees));
            Vector2 targetPos = MyTarget.Center + offset;
            Vector2 targetVelocity = (targetPos - NPC.Center);
            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetPos, movementSpeed);

        }


        switch (AttackCycle)
        {
            case 0:
                _accelTimer++;
                _circleDegrees += _circleSpeed;
                Circle();

                     
                Vector2 targetAimingVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                _aimingVelocity = Vector2.Lerp(_aimingVelocity, targetAimingVelocity, 1f);
                _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.3f);
                if(Timer % 8 == 0 && _bowFrame < 3)
                {
                    _bowFrame++;
                }
                if (_bowDissipateAlpha < 1)
                    _bowDissipateAlpha += 0.045f;
                if (_bowFrame > 3)
                    _bowFrame = 0;
                Animator.PlayAnimation(Anim_Arrowhold);
                break;
            case 1:
//                Circle();

                _accelTimer = 0;
                if (_bowDissipateAlpha < 1)
                    _bowDissipateAlpha += 0.045f;
                _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.3f);
                _bowFrame = 3;
                NPC.velocity *= 0.94f;
                break;
            case 2:
                if (Timer % 8 == 0 && _bowFrame < 6)
                {
                    _bowFrame++;
                }
                _bowDissipateAlpha -= 0.05f;
                NPC.velocity *= 0.4f;
                Animator.PlayAnimation(Anim_Arrowshot);
                if (Animator.IsFinished())
                    AttackCycle = 0;
                break;
        }
        NPC.velocity *= 0.96f;

        void PrepareBowShot(int time)
        {
            if(Timer == time - 15)
            {
                AttackCycle = 1;
            }
            if(Timer == time)
            {
                BowShot();
            }
        }
        PrepareBowShot(60);
        PrepareBowShot(154);
        PrepareBowShot(248);

        if (Timer >= 282)
        {
            Timer = 0;
            AttackCounter++;
            if (AttackCounter >= 3)
            {
                //For now, we gotta make the discs first
                SwitchState(AIState.Idle);
            }
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
}
