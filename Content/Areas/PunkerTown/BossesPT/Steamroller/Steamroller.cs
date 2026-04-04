using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;

/*
 * 
 *
 *X appears directly on the ground right underneath you and after a moment, 
 *a sound cue comes in too before and Steamroller drills through the ground at a fast pace, 
 *shooting into the air and then waiting a moment before trying to drill back down on top of you, you have to dodge twice

Steamroller pops its head out like snagrets and tries to start drilling on top of you but gets stuck in the ground with its head and he starts drilling, 
creating a bunch of flying rocks that come out to hit you

Steamroller comes out and starts to shoot little bombs from the side with like cool spell circles 
and stuff while being up in the air arched over

Dune jump, where he comes out of the ground over you and leaps over basically, you just have to not move for this

You see rocks rumbling under the ground as he starts doing a dung defender type attack, and stops and pokes his head out and goes back in for a minute

Phase two, he splits in half and basically this one goes on the other side of you, or it tries to attack right after the other, 
since this is a slow timing boss this will work

Pops off its head as it comes out the ground and shoots itself at you, detaching itself as the rest of the body goes underground,
the head drills into the ground as well and you just have to dodge really, it goes back underground after this attack to reconnect

 */


public class RedX : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
    }
    
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle warningSound = AssetRegistry.Sounds.SteamPunking.MechSawRevUp;
            warningSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(warningSound, Projectile.position);
            float numDust = 8;
            for(float n = 0; n < numDust; n++)
            {
                float radians = (n / numDust) * MathHelper.TwoPi;
                Vector2 offset = radians.ToRotationVector2();
                offset *= 64;
                Vector2 pos = Projectile.Center + offset;
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                var dp = DustParticle.Spawn(pos, Vector2.Zero, spawnParams);
                dp.noTileCollide = true;
                dp.fast = true;
                dp.dampening = 0.1f;
                dp.gravity = 0;
            }
        }
    }


    private void DrawPixelatedX(SpriteBatch sb, Vector2 screenPos)
    {
        float easeIn = EasingFunction.InOutSine(Timer / 30f);
        float easeOut = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Yellow * 0.65f * easeIn * easeOut;
        drawer.color.A = 0;


        Vector2 scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, easeIn * easeOut);
        drawer.scale = scale;

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Yellow * 0.25f * easeIn * easeOut;
        glowDrawer.color.A = 0;
        glowDrawer.scale = scale * ExtraMath.Osc(0.6f, 1f, speed: 8);
        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.Draw(glowDrawer);

        Vector2 offset = Vector2.Lerp( Vector2.Zero, -Vector2.UnitX * 64, easeOut);
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.GradientPillar, Projectile.Center + offset);
        lineDrawer.color = Color.Yellow * 0.65f * easeIn * easeOut;
        lineDrawer.color.A = 0;
        lineDrawer.scale.X *= 0.04f;
        lineDrawer.scale.Y *= 4;

        Main.spriteBatch.Draw(lineDrawer);
        lineDrawer.worldPosition = Projectile.Center - offset;
        Main.spriteBatch.Draw(lineDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedX);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class Chain
{
    public Chain(Vector2 initialPosition, float pointLength, int numPoints)
    {
        this.segmentLength = pointLength;
        this.points = new Vector2[numPoints];
        for (int i = 0; i < this.points.Length; i++)
        {
            this.points[i] = initialPosition + Vector2.UnitX * i * 5;
        }
        this.pinned = new bool[numPoints];
    }
    public float segmentLength;
    public Vector2[] points;
    public bool[] pinned;
    public void Resolve()
    {

        for(int i = points.Length-1; i >= 1; i--)
        {
            ref Vector2 p2 = ref points[i - 1];
            ref Vector2 p1 = ref points[i];
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            if (distance > segmentLength)
            {
                float difference = segmentLength - distance;
                float percent = difference / distance / 2f;
                float offsetX = dx * percent;
                float offsetY = dy * percent;

                p1.X -= offsetX;
                p1.Y -= offsetY;

                if (!pinned[i - 1])
                {
                    p2.X += offsetX;
                    p2.Y += offsetY;
                }
      
            }
        }
    }
}

[Autoload(Side = ModSide.Client)]
public class FlyingSoilSystem : ModSystem
{
    private struct SoilBlock
    {
        public int topLeftVariant;
        public int topRightVariant;
        public int bottomLeftVariant;
        public int bottomRightVariant;
    }

    private struct Soil
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float direction;
        public float timer;
        public int tileType;
        public bool active;
    }

    private int _soilIndex;
    private Soil[] _soils;
    public override void Load()
    {
        base.Load();
        _soils = new Soil[100];
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for(int i = 0; i < _soils.Length; i++)
        {
            ref Soil soil = ref _soils[i];
            if (!soil.active)
                continue;

            soil.timer++;
            soil.position += soil.velocity;
            soil.rotation += soil.direction * 0.05f;
            soil.velocity.Y += 0.25f;
            if(soil.timer > 90)
            {
                soil.active = false;
            }
        }
    }

    public void NewSoil(Vector2 worldPosition, Vector2 initialVelocity)
    {
        Point point = worldPosition.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        Tile tile = Main.tile[point];

        for(int i = 0; i < _soils.Length; i++)
        {
            _soilIndex++;
            _soilIndex %= _soils.Length;
            if (!_soils[_soilIndex].active)
            {
                break;
            }
        }

        Vector2 startPosition = point.ToWorldCoordinates();
        ref Soil soil = ref (_soils[_soilIndex]);
        soil.timer = 0;
        soil.active = true;
        soil.position = startPosition;
        soil.velocity = initialVelocity;
        soil.direction = Main.rand.NextBool(2) ? -1 : 1;
        soil.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        soil.tileType = tile.TileType;
    }
    private void DrawSoil(SpriteBatch sb , Vector2 screenPos)
    {
        for (int i = 0; i < _soils.Length; i++)
        {
            ref Soil soil = ref _soils[i];
            if (!soil.active)
                continue;
            Vector2 scale = Vector2.Lerp(Vector2.One, Vector2.Zero, soil.timer / 90f);
            Vector2 x = Vector2.UnitX * 8;
            x = x.RotatedBy(soil.rotation);
            x *= scale;
            Vector2 y = Vector2.UnitY * 8;
            y = y.RotatedBy(soil.rotation);
            y *= scale;
            Vector2 center = soil.position;

            Vector2 topLeft = center - x - y;
            Vector2 topRight = center + x - y;
            Vector2 bottomLeft = center - x + y;
            Vector2 bottomRight = center + x + y;
   
            Vector2 origin = Vector2.One * 8;
            Asset<Texture2D> texture = TextureAssets.Tile[soil.tileType];
            SpritebatchDrawer topLeftDrawer = SpritebatchDrawer.FromTextureAsset(texture, topLeft);
            topLeftDrawer.sourceRect = new Rectangle(0, 54, 16, 16);
            topLeftDrawer.rotation = soil.rotation;
            topLeftDrawer.drawOrigin = origin;
            topLeftDrawer.scale = scale;
            sb.Draw(topLeftDrawer);

            SpritebatchDrawer topRightDrawer = SpritebatchDrawer.FromTextureAsset(texture, topRight);
            topRightDrawer.sourceRect = new Rectangle(18, 54, 16, 16);
            topRightDrawer.rotation = soil.rotation;
            topRightDrawer.drawOrigin = origin;
            topRightDrawer.scale = scale;
            sb.Draw(topRightDrawer);

            SpritebatchDrawer bottomLeftDrawer = SpritebatchDrawer.FromTextureAsset(texture, bottomLeft);
            bottomLeftDrawer.sourceRect = new Rectangle(0, 72, 16, 16);
            bottomLeftDrawer.rotation = soil.rotation;
            bottomLeftDrawer.drawOrigin = origin;
            bottomLeftDrawer.scale = scale;
            sb.Draw(bottomLeftDrawer);

            SpritebatchDrawer bottomRightDrawer = SpritebatchDrawer.FromTextureAsset(texture, bottomRight);
            bottomRightDrawer.sourceRect = new Rectangle(18, 72, 16, 16);
            bottomRightDrawer.rotation = soil.rotation;
            bottomRightDrawer.drawOrigin = origin;
            bottomRightDrawer.scale = scale;
            sb.Draw(bottomRightDrawer);
        }
    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        PixelationManager.QueueSpritebatchDrawAction(DrawSoil);
    }
}

public class Bedrock : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.scale = Main.rand.NextFloat(0.7f, 1f);
        }
        if (Timer > 20)
            Projectile.tileCollide = true;
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation -= 0.05f;
        Projectile.rotation -= Projectile.velocity.Length() * 0.015f;
    }


    public override bool PreDraw(ref Color lightColor)
    {
        this.Outline(Color.Red, ref lightColor);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
        //        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundStyle deathSound = AssetRegistry.Sounds.Melee.HammerSmash2;
        deathSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(deathSound, Projectile.position);
        for(int i = 0; i < 6; i++)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
            if (Main.rand.NextBool(1))
            {
                spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
                Point point = Projectile.Center.ToTileCoordinates();
                while (!WorldGen.SolidTile(point))
                    point.Y++;

                int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
                Dust dust = Main.dust[d];
                dust.position += Main.rand.NextVector2Circular(32, 32);
                dust.velocity = spawnVelocity;
                dust.noLightEmittence = true;
            }
            spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
        for (float f = 0; f < 12; f++)
        {
            float lerp = f / 12f;
            float rot = lerp * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2();
            vel *= Main.rand.NextFloat(2, 5);
            Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
        }
    }
}

public class SteamrollerBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class SteamrollerBomb : ModProjectile
{
    private Asset<Texture2D> _glowTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _glowTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class Steamroller : ScarletBoss,
    IDrawOutlines
{
    public class SteamrollerSegment
    {
        public const string Anim_SpinSlow = "spinslow";
        public const string Anim_SpinFast = "spinfast";
        public const string Anim_CannonComeOut = "cannoncomeout";
        public const string Anim_CannonShoot = "cannonshoot";
        public const string Anim_CannonIdle = "cannonidle";
        private int _index;
        public SteamrollerSegment(int index)
        {
            _index = index;
            Animator.extraUpdates = _index * 4;
            glowColor = Color.Black;
        }
 
        public Animator _animator;
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = new Animator();
                    var idle = new SpriteAnimation(0, 3, isLooping: true);
                    _animator.AddAnimation(Anim_SpinSlow, idle);

                    var running = new SpriteAnimation(4, 12, isLooping: true, frameSpeed: 0.35f);
                    _animator.AddAnimation(Anim_SpinFast, running);

                    var cannotComeOut = new SpriteAnimation(12, 18, isLooping: false);
                    _animator.AddAnimation(Anim_CannonComeOut, cannotComeOut);

                    var cannotShoot = new SpriteAnimation(18, 28, isLooping: false);
                    _animator.AddAnimation(Anim_CannonShoot, cannotShoot);

                    var cannonIde = new SpriteAnimation(28, 29, isLooping: true);
                    _animator.AddAnimation(Anim_CannonIdle, cannonIde);
                }

                return _animator;
            }
        }
        public enum SteamrollerAnimationState
        {
            Spin_Slow,
            Spin_Fast,
            Cannon_ComeOut,
            Cannon_Shoot,
            Cannon_Idle
        }

        public SteamrollerAnimationState animationState;
        public Asset<Texture2D> steamrollerSegmentTextureAsset;
        public Asset<Texture2D> steamrollerGlowSegmentTextureAsset;
        public Color glowColor;
        public bool paused;
        public void Update()
        {
            switch (animationState)
            {
                case SteamrollerAnimationState.Spin_Slow:
                    AI_SpinSlow();
                    break;
                case SteamrollerAnimationState.Spin_Fast:
                    AI_SpinFast();
                    break;
                case SteamrollerAnimationState.Cannon_ComeOut:
                    AI_CannonComeOut();
                    break;
                case SteamrollerAnimationState.Cannon_Shoot:
                    AI_CannonShoot();
                    break;
                case SteamrollerAnimationState.Cannon_Idle:
                    AI_CannonIdle();
                    break;
            }
            if (paused)
                return;
            Animator.Update();
        }
  
        private void AI_SpinSlow()
        {
            Animator.PlayAnimation(Anim_SpinSlow);
        }
        private void AI_SpinFast()
        {
            Animator.PlayAnimation(Anim_SpinFast);
        }
        private void AI_CannonComeOut()
        {
            Animator.PlayAnimation(Anim_CannonComeOut);
        }
        private void AI_CannonShoot()
        {
            Animator.PlayAnimation(Anim_CannonShoot);
        }
        private void AI_CannonIdle()
        {
            Animator.PlayAnimation(Anim_CannonIdle);
        }

        public void Draw(SpriteBatch sb, Vector2 segmentPosition, Vector2 nextSegmentPosition, Color drawColor)
        {
            steamrollerGlowSegmentTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "SteamrollerBody_Glow");
            steamrollerSegmentTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "SteamrollerBody");
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(steamrollerSegmentTextureAsset, segmentPosition);
            float rotation = (segmentPosition - nextSegmentPosition).ToRotation();
            rotation += MathHelper.PiOver2;

            int frameHeight = 148;
            drawer.sourceRect = new Rectangle(0, Animator.GetFrameY(frameHeight), steamrollerSegmentTextureAsset.Width(), frameHeight);
            drawer.drawOrigin = new Vector2(drawer.sourceRect.Value.Width, drawer.sourceRect.Value.Height) * 0.5f;
            drawer.rotation = rotation;
            drawer.color = drawColor;
            sb.Draw(drawer);

            if (glowColor == Color.Black)
                return;

            drawer.color = glowColor;
            drawer.color.A = 0;
            drawer.texture = steamrollerGlowSegmentTextureAsset.Value;
            sb.Draw(drawer);
        }
    }
    private const string Anim_SpinSlow = "spinslow";
    private const string Anim_SpinFast = "spinfast";
    private enum AIState
    {
        SpawnDrill,
        IdleDrill,
        Driller,

        X_Drill_Start,
        X_Drill_Rise,
        X_Drill_Fall,

        Snagret_PopStart,
        Snagret_PopRise,
        Snagret_PopFallNStuckk,

        DuneJump_Start,
        DuneJump_Rise,
        DuneJump_Fall,

        DungDefenderRock_Start,
        DungDefenderRock_Blast,
        DungDefenderRock_End,

        Phase_Transition,

        Cannon_Start,
        Cannon_Fire,
        Cannon_End,

        HeadPop_Start,
        HeadPop_Drill,
        HeadPop_Fall,

        MeteorJump_Start,
        MeteorJump_Fall,
        MeteorJump_Repair
    }

    private bool _quickDrill;
    private bool _driller2;
    private bool _xDrillSnagretVariant;
    private bool _xDrillDungVariant;
    private bool _xDrillFallVariant;
    private bool _renderDashTrail;
    private float _dashTrailTimer;
    private float _dashTrailAlpha;
    private float _jumpSpeed;
    private bool _crashed;
    private float _currentSpeed;
    private bool _contactDamage;
    private Color _targetOutlineColor;
    private Color _outlineColor;
    private Vector2 _squishScale;
    private Vector2 _targetPosition;
    private Vector2 _startVelocity;
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if(_animator == null)
            {
                _animator = new Animator();
                var idle = new SpriteAnimation(0, 3, isLooping: true);
                _animator.AddAnimation(Anim_SpinSlow, idle);

                var running = new SpriteAnimation(4, 11, isLooping: true, frameSpeed: 0.35f);
                _animator.AddAnimation(Anim_SpinFast, running);
            }
 
            return _animator;
        }
    }

    private SteamrollerSegment[] _steamrollerSegments;
    private SteamrollerSegment[] SteamRollerSegments
    {
        get
        {
            if(_steamrollerSegments == null)
            {
                _steamrollerSegments = new SteamrollerSegment[16];
                for (int i = 0; i < _steamrollerSegments.Length; i++)
                {
                    _steamrollerSegments[i] = new SteamrollerSegment(i);

                }

            }
   

            return _steamrollerSegments;
        }
    }
    public Chain _chain;
    public Chain Chain
    {
        get
        {
            if(_chain == null)
            {
  
                _chain = new Chain(NPC.Center, 80, 16);
            }
            return _chain;
        }
    }


    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if(_patternManager == null)
            {
                _patternManager = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.X_Drill_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.DuneJump_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.DungDefenderRock_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.Cannon_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.HeadPop_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.MeteorJump_Start, 1.0f));
            }
            return _patternManager;
        }
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    //Damage Values
    private int BedrockDamage => 32;
    private float IdleTime => 120;
    private float XDrillWarningTime => 60;
    private float DrillTime = 160;
    private float DungDefenderWarningTime => 90;
    public Vector2 GetSegmentPosition(int verletIndex)
    {
        if (verletIndex < 0)
            return NPC.Center;

        return Chain.points[verletIndex];
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 12;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        _squishScale = Vector2.One;
        NPC.width = 128;
        NPC.height = 128;
        NPC.damage = 100;
        NPC.defense = 28;
        NPC.lifeMax = 18000;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;
        NPC.behindTiles = true;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SitriAndTheMechs");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    public override void AI()
    {
        base.AI();
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamRollerSegments[i].paused = false;
        }

        _xDrillDungVariant = false;
        _xDrillFallVariant = false;
        _renderDashTrail = false;
        _contactDamage = false;
        _targetOutlineColor = Color.Transparent;
        switch (State)
        {
            case AIState.SpawnDrill:
                AI_SpawnDrill();
                break;
            case AIState.IdleDrill:
                AI_IdleDrill();
                break;
            case AIState.Driller:
                AI_Driller();
                break;

            case AIState.X_Drill_Start:
                AI_XDrillStart();
                break;
            case AIState.X_Drill_Rise:
                AI_XDrillRise();
                break;
            case AIState.X_Drill_Fall:
                AI_XDrillFall();
                break;

            case AIState.DuneJump_Start:
                _xDrillFallVariant = true;
                AI_XDrillStart();
                break;
            case AIState.DuneJump_Rise:
                _xDrillFallVariant = true;
                AI_XDrillRise();
                break;
            case AIState.DuneJump_Fall:
                _xDrillFallVariant = true;
                AI_XDrillFall();
                break;

            case AIState.DungDefenderRock_Start:
                AI_DungDefenderRockStart();
                break;
            case AIState.DungDefenderRock_Blast:
                AI_DungDefenderRockBlast();
                break;
            case AIState.DungDefenderRock_End:
                _xDrillDungVariant = true;
                AI_XDrillFall();
                break;

            case AIState.Snagret_PopStart:
                _xDrillSnagretVariant = true;
                AI_XDrillStart();
                break;
            case AIState.Snagret_PopRise:
                _xDrillSnagretVariant = true;
                AI_XDrillRise();
                break;
            case AIState.Snagret_PopFallNStuckk:
                _xDrillSnagretVariant = true;
                AI_XDrillFall();
                break;

            case AIState.Cannon_Start:
                AI_CannonStart();
                break;
            case AIState.Cannon_Fire:
                AI_CannonFire();
                break;
            case AIState.Cannon_End:
                AI_CannonEnd();
                break;

            case AIState.HeadPop_Start:
                AI_HeadPopStart();
                break;
            case AIState.HeadPop_Fall:
                AI_HeadPopFall();
                break;
            case AIState.HeadPop_Drill:
                AI_HeadPopDrill();
                break;

            case AIState.MeteorJump_Start:
                AI_MeteorJumpStart();
                break;
            case AIState.MeteorJump_Fall:
                AI_MeteorJumpFall();
                break;
            case AIState.MeteorJump_Repair:
                AI_MeteorJumpEnd();
                break;
        }
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamrollerSegment segment = SteamRollerSegments[i];
            segment.Update();
        }
        _dashTrailTimer += _renderDashTrail ? 1 : -1;
        _dashTrailTimer = MathHelper.Clamp(_dashTrailTimer, 0, 60);
        _dashTrailAlpha = EasingFunction.InOutSine(_dashTrailTimer / 60f);
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.3f);
    }

    public override void PostAI()
    {
        base.PostAI();

        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 32; i++)
        {
            Chain.Resolve();

        }
    
    }
    private void AI_MeteorJumpStart()
    {

    }

    private void AI_MeteorJumpFall()
    {

    }


    private void AI_MeteorJumpEnd()
    {

    }

    private void AI_HeadPopStart()
    {

    }

    private void AI_HeadPopFall()
    {

    }

    private void AI_HeadPopDrill()
    {

    }

    private void AI_CannonStart()
    {

    }

    private void AI_CannonFire()
    {

    }

    private void AI_CannonEnd()
    {

    }
    private void DungDefenderRocks()
    {
        Vector2 bottom = _targetPosition - Vector2.UnitY * 64;
        Point point = bottom.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        bottom = point.ToWorldCoordinates();
        if (Main.rand.NextBool(4))
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 25);

            vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(1, 2);

            var sp = SmokeParticle.Spawn(bottom, vel);
            sp.initialColor = Color.Brown * 0.5f;
            sp.fadeToColor = Color.Transparent;

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitX * (Main.rand.NextBool(2) ? -5 : 5);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -50);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

        }
        if (Main.rand.NextBool(3))
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            Dust.NewDustPerfect(bottom + Main.rand.NextVector2Circular(64, 64), DustID.Dirt, spawnVelocity, Scale: 2);
        }
    }
    private void AI_DungDefenderRockStart()
    {
        Timer++;
        if (Timer == 1)
        {
   
            NPC.TargetClosest();

        }
        if(Timer < DungDefenderWarningTime - 30)
        {
            _startVelocity = NPC.velocity;
            _targetPosition = MyTarget.Bottom;
        }



        //Ease in to the start position for the attack
        float ratio = Timer / DungDefenderWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 400);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        DungDefenderRocks();
        _targetOutlineColor = Color.Yellow;

        if (Timer >= DungDefenderWarningTime)
        {
            if (AttackCycle >= 4)
            {
                SwitchState(AIState.IdleDrill);
            }
            else
            {
                SwitchState(AIState.DungDefenderRock_Blast);
            }

        }

    }
    private void AI_DungDefenderRockBlast()
    {
        Animator.PlayAnimation(Anim_SpinFast);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = NPC.Center;
            }
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeModSystem.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.DungDefenderRock_End);
        }
    }

    private void AI_DungDefenderRockEnd()
    {

    }
    private void MakeSteamParticlesRandomlyAtSegments()
    {
        for(int i = 0; i < Chain.points.Length; i++)
        {
            Vector2 point = Chain.points[i];
            if (Main.rand.NextBool(150))
            {
                var zap = LegacyParticle.NewParticle<ZapParticle>(point + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(1, 1), Color.White, 1f);
                zap.innerColor = Color.Goldenrod;
                zap.outerColor = Color.Lerp(zap.innerColor, Color.Black, 0.5f);
                zap.fadeToColor = Color.Lerp(zap.outerColor, Color.Black, 0.5f);
            }
            if (Main.rand.NextBool(150))
            {
                Vector2 spawnPosition = point;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
            SwitchState(AIState.DungDefenderRock_Start);
        }
    }

    private void AI_SpawnDrill()
    {
        ShowNamePlate();
        SwitchState(AIState.IdleDrill);
    }

    private void AI_IdleDrill()
    {
        AttackCycle = 0;
        _quickDrill = false;
        _driller2 = false;
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            var segment = SteamRollerSegments[i];
            segment.glowColor = Color.Black;
        }
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if(Timer == 1)
        {
            _currentSpeed = NPC.velocity.Length();
            NPC.TargetClosest();
        }

        Vector2 undergroundPosition = MyTarget.Center + new Vector2(0, 1500);
        undergroundPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 5) * 100;
        Vector2 vel = (undergroundPosition - NPC.Center).SafeNormalize(Vector2.Zero);

        float ratio = Timer / 90f;
        float ease = EasingFunction.InOutSine(ratio);
        float speed = MathHelper.Lerp(_currentSpeed, 30, ease);

        float distToTarget = Vector2.Distance(undergroundPosition, NPC.Center);
        if (speed < distToTarget)
            speed = distToTarget;
        NPC.velocity = vel * speed;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        if(Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }

    private void AI_Driller()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.SteamPunking.SteamrollerDig;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, NPC.position);
        }
        ShakeModSystem.Shake = 4;
        float ratio = Timer / 30f;
        float ease = EasingFunction.QuadraticBump(ratio);
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.9f), ease );

        Vector2 bottom = NPC.Top - Vector2.UnitY * 64;
        Point point = bottom.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        bottom = point.ToWorldCoordinates();
        if (Main.rand.NextBool(4))
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 25);
            var spawnParams = DustParticleSpawnParams.Default;
    
            spawnParams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(bottom, vel, spawnParams);
            dp.fast = true;

            vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(1, 2);

            var sp = SmokeParticle.Spawn(bottom, vel);
            sp.initialColor = Color.Brown * 0.5f;
            sp.fadeToColor = Color.Transparent;

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitX * (Main.rand.NextBool(2) ? -5 : 5);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -50);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
        }

        if (Main.rand.NextBool(3))
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
            Dust dust = Main.dust[d];
            dust.velocity = spawnVelocity;
            dust.noLightEmittence = true;   
        }

        if (Main.rand.NextBool(6))
        {
            Vector2 spawnPosition = bottom;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
        }
        for(int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamRollerSegments[i].paused = true;
        }

        MakeSteamParticlesRandomlyAtSegments();

        if (_driller2)
        {
            if (MultiplayerHelper.IsHost)
            {
                if (Timer % 10 == 0)
                {
                    Vector2 spawnPosition = bottom;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);

                    Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-15, -25);
                    spawnVelocity.X = Main.rand.NextFloat(-15, 15);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity, 
                        ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                }
            }
        }


        float dt = DrillTime;
        if (_quickDrill)
            dt *= 0.2f;
        if (Timer < dt)
        {
            NPC.velocity.Y *= 0.01f;
            NPC.velocity.X *= 0.01f;
            NPC.velocity.Y += 0.15f;
            if (Timer < 60)
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }
            else
            {
                Animator.PlayAnimation(Anim_SpinFast);
            }

        }
        else
        {
            NPC.velocity.Y += 0.5f;
            NPC.velocity.Y *= 1.1f;
            if (_quickDrill)
            {
                AttackCycle++;
                SwitchState(AIState.DungDefenderRock_Start);
            } 
            else if(Timer > dt + 60)
            {
                SwitchState(AIState.IdleDrill);

            }
        }


    }

    private void AI_XDrillStart()
    {
        Timer++;
        if(Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
            if (_xDrillSnagretVariant)
            {
                _targetPosition.X += MyTarget.direction * 128;
            }
        }

        //X Appears on the ground
        if(Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero, 
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            if (_xDrillSnagretVariant)
            {
                SwitchState(AIState.Snagret_PopRise);
            }
            else if (_xDrillFallVariant)
            {
                SwitchState(AIState.DuneJump_Rise);
            }
            else
            {
                SwitchState(AIState.X_Drill_Rise);
            }

        }
    }

    private void GroundImpact()
    {

        int[] gores = AutoGoreLoader.FindGores("GrayRock");
        foreach (int g in gores)
        {
            Gore.NewGore(NPC.GetSource_FromThis(),
                NPC.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }

        var p = Particle<ThickSmokeParticle>.Spawn(NPC.Bottom, Vector2.Zero, Color.DarkGray);

        var sear = LegacyParticle.NewParticle<SearParticle>(NPC.Center, Vector2.Zero);
        sear.innerColor = Color.Gray;
        sear.outerColor = Color.Blue;
        sear.fadeToColor = Color.Black;
        FXUtil.ShakeCamera(NPC.Center, 1024, 8);
        ShakeModSystem.Shake = 2;


        for (float f = 0; f < 4f; f++)
        {
            Vector2 pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(80, 80);
            var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
            zap.innerColor = Color.Gray;
            zap.outerColor = Color.Red;
            zap.fadeToColor = Color.Black;
            zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
            zap.Rotation = Main.rand.NextFloat(0f, 3f);
        }

        SoundStyle smashSound;
        smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
        foreach (int g in gores)
        {
            Gore.NewGore(NPC.GetSource_FromThis(),
                NPC.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }
        FXUtil.ShakeCamera(NPC.Center, 1024, 64);

        var p3 = FXUtil.GlowCircleBoom(NPC.Center,
           innerColor: Color.Gray,
           glowColor: Color.Red,
           outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
        p3.Scale *= 4;

        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, NPC.position);


        var part = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
        part.fadeToColor = Color.Black;
        part.outerColor = Color.Gray;
        part.noStretch = true;
        part.shrink = true;

        var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
        part2.fadeToColor = Color.Black;
        part2.outerColor = Color.Gray;
        part2.noStretch = true;
        part2.color *= 0.5f;
        for (float f = 0; f < 5; f++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
            vel.Y -= 10;
            var d = Dust.NewDustPerfect(NPC.Center,
                ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

        }

        for(float f = 0; f < 16; f++)
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 50);
            var spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 2f;
            spawnParams.outerColor = Color.Red;
            DustParticle.Spawn(NPC.Center, vel, spawnParams);
        }

    }

    private void AI_XDrillRise()
    {
        Animator.PlayAnimation(Anim_SpinFast);
        Timer++;
        if(Timer == 1)
        {
            _crashed = false;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = NPC.Center;
            }
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeModSystem.Shake = 3;
        if(Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            if (_xDrillDungVariant)
            {
                SwitchState(AIState.X_Drill_Fall);
            }
            else if (_xDrillSnagretVariant)
            {
                SwitchState(AIState.Snagret_PopFallNStuckk);
            }
            else if (_xDrillFallVariant)
            {
                SwitchState(AIState.DuneJump_Fall);
            }
            else
            {
                SwitchState(AIState.X_Drill_Fall);
            }
        }
    }

    private void AI_XDrillFall()
    {
        Timer++;
        if (Timer == 1)
        {
            if (_xDrillFallVariant)
            {
                float dir = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
                _jumpSpeed = dir;
                _jumpSpeed *= 21;
                if (MultiplayerHelper.IsHost)
                {
                    for(int i = 0; i < 6; i++)
                    {
                        Vector2 spawnPosition = NPC.Center;
                        spawnPosition.X += Main.rand.NextFloat(-64, 64);
                        spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                        Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-12, -17);
                        spawnVelocity.X = dir * Main.rand.NextFloat(2f, 15f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity,
                            ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                    }

                }
            }
            _currentSpeed = NPC.velocity.X;
            _crashed = false;

            GroundImpact();

            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            SoundEngine.PlaySound(steaming, NPC.position);

            SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
            mechMove.PitchVariance = 0.3f;
        //    SoundEngine.PlaySound(mechMove, NPC.position);


        }

        _targetOutlineColor = Color.Red;
        _contactDamage = true;
        _renderDashTrail = true;
        Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f);


        if(Timer < 70)
            RetargetCameraModifier.ReTargetPosition = targetPos;

        for(int i = 0; i < _steamrollerSegments.Length; i++)
        {
            var segment = _steamrollerSegments[i];
            segment.glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.InOutSine(Timer / 60f)) * ExtraMath.Osc(0f, 1f, speed: 10, offset: i);
        }

        if (_xDrillDungVariant)
        {
            if (NPC.velocity.Y < 0)
                NPC.velocity.Y *= 0.9f;
            if (NPC.velocity.Y < 25)
                NPC.velocity.Y += 0.5f;

            if (NPC.velocity.Y > 12)
            {
                Animator.PlayAnimation(Anim_SpinFast);
                NPC.velocity.Y *= 1.1f;

            }
            else
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }

            if (NPC.velocity.Y > 50)
                NPC.velocity.Y = 50;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        }
        else if (_xDrillSnagretVariant)
        {
            if (NPC.velocity.Y < -0.5f)
            {
                NPC.velocity.Y *= 0.9f;
               // NPC.velocity.X = MathF.Sin(Timer * 0.04f) * 8;
            }
            else
            {
                Vector2 targetVel = (MyTarget.Center - NPC.Center);
                float rot = targetVel.ToRotation();
                rot += MathHelper.PiOver2;
                NPC.velocity += targetVel.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0f, 4f, EasingFunction.OutExpo((Timer - 90) / 90f));
                NPC.rotation = Utils.AngleLerp(NPC.rotation, rot, 0.1f);
            }
        }
        else
        {
            if (NPC.velocity.Y < 0)
                NPC.velocity.Y *= 0.97f;
            if (NPC.velocity.Y < 25)
                NPC.velocity.Y += 0.5f;

            if (NPC.velocity.Y > 12)
            {
                Animator.PlayAnimation(Anim_SpinFast);
                NPC.velocity.Y *= 1.1f;

            }
            else
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }

            if (NPC.velocity.Y > 50)
                NPC.velocity.Y = 50;
            float xDirectionToTarget = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
            float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            float xSpeed = xDirectionToTarget * dist * 0.25f;

            if (_xDrillDungVariant)
            {
                NPC.velocity.X *= 0.9f;
            }
            else if (_xDrillFallVariant)
                NPC.velocity.X = MathHelper.Lerp(_currentSpeed, _jumpSpeed, EasingFunction.InOutSine(Timer / 25f));
            else if (Timer < 45)
                NPC.velocity.X = MathHelper.Lerp(_currentSpeed, xSpeed, EasingFunction.InOutSine(Timer / 60f));

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        }


        Vector2 bottom = NPC.Bottom + Vector2.UnitY * 64;
        Point tilePoint = bottom.ToTileCoordinates();
        if (WorldGen.InWorld(tilePoint.X, tilePoint.Y) && Timer > 20)
        {
            Tile tile = Main.tile[tilePoint];
            if (WorldGen.SolidTile(tile) && !_crashed)
            {
                _crashed = true;
                SoundStyle smash2 = AssetRegistry.Sounds.Melee.HammerSmash2;
                smash2.PitchVariance = 0.3f;
                SoundEngine.PlaySound(smash2, NPC.position);

                FXUtil.ShakeCamera(NPC.Center, 1024, 24);
                if (_xDrillSnagretVariant)
                {
                    _driller2 = true;
                }
                if (_xDrillDungVariant)
                {
                    _quickDrill = true;
                }
           
                SwitchState(AIState.Driller);

            }           
        }

        MakeSteamParticlesRandomlyAtSegments();
    }

    private Vector2 GetDrawOrigin()
    {
        if (_animator == null)
            return NPC.frame.Size() / 2f;
        Vector2? drawOrigin = _animator.GetDrawOrigin();
        if (drawOrigin.HasValue)
            return drawOrigin.Value;
        return NPC.frame.Size() / 2f;
    }

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _dashTrailAlpha;
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 64, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.Yellow;
        laserShader.OuterColor = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - screenPos;

        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;



        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_steamrollerSegments == null)
            return false;

        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);

        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Restart(effect: whiteShader.Effect);
        float outlineScale = 2;
        Vector2 left = Vector2.UnitX * -outlineScale;
        Vector2 right = Vector2.UnitX * outlineScale;
        Vector2 up = Vector2.UnitY * -outlineScale;
        Vector2 down = Vector2.UnitY * outlineScale;
        Draw(spriteBatch, screenPos + left, _outlineColor);
        Draw(spriteBatch, screenPos + right, _outlineColor);
        Draw(spriteBatch, screenPos + up, _outlineColor);
        Draw(spriteBatch, screenPos + down, _outlineColor);
        spriteBatch.RestartDefaults();

        Draw(spriteBatch, screenPos, drawColor);
        for (int i = 1; i < _steamrollerSegments.Length-1; i++)
        {
            SteamrollerSegment segment = _steamrollerSegments[i];
            Vector2 pos = Chain.points[i];
            Vector2 nextPos = Chain.points[i + 1];
            Color lightingColor = Lighting.GetColor(pos.ToTileCoordinates());
            segment.Draw(spriteBatch, pos, nextPos, drawColor);
        }
 
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        //   throw new NotImplementedException();


    }
}
