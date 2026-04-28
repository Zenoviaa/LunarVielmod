using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Snow.WeaponsSN;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;

//VERLIA TODO:
//Two bouncing moons attack
//NEW scrolling moon and glowing moon shader very cool
//NEW moon particle, make some cool like space-y dust

//Blue Magic Sword
//NEW magic sword glow mask
//Should be blue

//Spinning little moons
//Tiny little moons with the same new moon shader
//Cool explosion using the new radial shear shader that we made
//Sparkling little moon dusts would be super cool

//Blade dance
//Basically the same as legacy, not much has to change

//Clone summon
//She creates a really cool mirror and her clones pop out

//Phase 2 transition
//Desperation big moon
//Really cool blue moon
//Moon blade sprites and afterimage effects when it explodes
//When she starts this there's a cool circle that comes in and pulls you inward once she teleports to her center

//Sword Fall
//Blue magical swords spiral in and then glisten/bright/flash up, and then they come down with a bit of easing
//Camera will slightly pan upward so you can see them better like Bishinine's rain


//Running
//basic afterimage and trailing effects
//cool moon magic

//Dash Slash
//Very fast, uses the slash animations we already have just with a bit more

//Blade Dance V2
//Goes into the big sword, shouldn't look like the other one it'll be a lot more elegant, blue, magical, and moony

//Dying
//Verlia slows, down, falls down and looks into the mirror
//She sees a clone of herself before the mirror cracks and she dies
//and her sword drops on the ground (you can't pick it up it's just visual)

public class ScrollingMoonShader : CrystalShader<ScrollingMoonShader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _imageSizeParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _bendStrengthParam;
    private EffectParameter _tilingParam;
    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public float BendStrength
    {
        set
        {
            _bendStrengthParam ??= Effect.Parameters["bendStrength"];
            _bendStrengthParam.SetValue(value);
        }
    }
    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 ImageSize
    {
        set
        {
            _imageSizeParam ??= Effect.Parameters["imageSize"];
            _imageSizeParam.SetValue(value);
        }
    }

    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }

    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            ImageSize = value.Size();
        }
    }
}


public class VerliaBouncingMoonBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            for (float f = 0; f < 5f; f++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightSkyBlue,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            fx.Scale *= 1f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
       // waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 1.5f;
        waveDrawer.scale.Y *= 0.5f;
        waveDrawer.color = Color.SkyBlue;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
}
public class VerliaBouncingMoonShockwave : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
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

public class VerliaBouncingMoon : ModProjectile
{
    private float _flashAlpha;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    private Asset<Texture2D> _outlineMoonTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Vector2 _startPosition;
    private enum BounceState
    {
        Spawn,
        Bounce_1,
        Bounce_2,
        Bounce_Out
    }

    private ref float Timer => ref Projectile.ai[0];
    private BounceState State
    {
        get => (BounceState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float Direction => ref Projectile.ai[2];

    private Vector2 _squishScale;
    public Vector2 BounceUp1Distance => new Vector2(128, 75);
    public Vector2 BounceUp2Distance => new Vector2(256, 50);
    public Vector2 BounceOutDistance => new Vector2(384, 50);

    public float SpawnTime => 60;
    public float BounceUp1Time => 60;
    public float BounceUp2Time => 60;
    public float BounceOutTime => 120;
    public override void SendExtraAI(BinaryWriter writer)
    {

        base.SendExtraAI(writer);
        writer.WriteVector2(_startPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startPosition = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        base.AI();
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        if(Timer  % 24 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), Vector2.Zero );
            sp.outerColor = Color.LightBlue;
            sp.gravity = 0;
            sp.Scale *= 0.5f;
            sp.behindLayer = true;
        }


        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);


        switch (State)
        {
            case BounceState.Spawn:
                AI_Spawn();
                break;
            case BounceState.Bounce_1:
                AI_Bounce1();
                break;
            case BounceState.Bounce_2:
                AI_Bounce2();
                break;
            case BounceState.Bounce_Out:
                AI_BounceOut();
                break;
        }
    }

    private void SwitchState(BounceState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    /// <summary>
    /// Controls movement for the arcing motion that the moons do
    /// </summary>
    /// <param name="time"></param>
    /// <param name="distance"></param>
    private void Bounce(float time, Vector2 distance)
    {
        if (Timer == 1)
        {
            _startPosition = Projectile.Center;
        }

        float ratio = Timer / time;
        float ease = EasingFunction.InExpo(ratio);
        Vector2 targetPosition = _startPosition + -Vector2.UnitY * distance.Y;
        Vector2 interpolatedPosition = Vector2.Lerp(_startPosition, targetPosition, ease);
        float ease2 = EasingFunction.QuadraticBump(ratio);
        interpolatedPosition.X += MathHelper.Lerp(0f, distance.X * Direction, ease2);

        Vector2 velocity = interpolatedPosition - Projectile.Center;
        Projectile.velocity = velocity;
        if(Timer >= time)
        {
            BounceEffect();
        }
    }

    private void BounceEffect()
    {
        _flashAlpha = 1f;
        _squishScale = new Vector2(0.8f, 1.4f);
        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitX * -Direction, Color.LightSkyBlue);
        donut.Scale *= 2;
        SoundStyle bounceSound = new SoundStyle("Stellamod/Assets/Sounds/VeriButterfly");
        bounceSound.PitchVariance = 0.5f;
        bounceSound.MaxInstances = 1;
        SoundEngine.PlaySound(bounceSound, Projectile.position);
    }

    private void AI_Spawn()
    {
        SwitchState(BounceState.Bounce_1);
    }

    private void AI_Bounce1()
    {
    
        Timer++;
        if(Timer == 1)
        {
            _flashAlpha = 1f;
            SoundStyle bounceSound = new SoundStyle("Stellamod/Assets/Sounds/Veripulse");
            bounceSound.PitchVariance = 0.5f;
            bounceSound.MaxInstances = 1;
            SoundEngine.PlaySound(bounceSound, Projectile.position);
        }
        Bounce(BounceUp1Time, BounceUp1Distance);
        if (Timer >= BounceUp1Time)
        {
            SwitchState(BounceState.Bounce_2);
        }
    }
    
    private void AI_Bounce2()
    {
        Timer++;
        Bounce(BounceUp2Time, BounceUp2Distance);
        if(Timer >= BounceUp2Time)
        {
            SwitchState(BounceState.Bounce_Out);
        }
    }

    private void AI_BounceOut()
    {
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.Y -= 15;
        }
        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y += 1f;
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
        if (player == null)
            return;

        if(Projectile.Bottom.Y > player.Top.Y)
        {
            Projectile.tileCollide = true;
        }
        
    }


    private void DrawAfterImage(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            moonSprite.color = Color.Lerp(Color.Blue, Color.DarkBlue, ratio);
            moonSprite.color *= MathHelper.Lerp(1f, 0f, ratio) * 0.4f;
            moonSprite.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            moonSprite.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio) * 0.75f;
            Main.spriteBatch.Draw(moonSprite);
        }

    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.7f;
        glowDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * Direction;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);
   

        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.LightSkyBlue, ExtraMath.Osc(0f, 0.3f, speed: 8));
        moonSprite.scale *= _squishScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


       

        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarEye, Projectile.Center);
        glowDrawer.color = Color.Blue * 0.16f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1f;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        glowDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.White;


        blackFireShader.PrimaryTexture = TrailRegistry.BeamTrail;
        blackFireShader.PrimaryTexture2 = TrailRegistry.DottedTrail;
        blackFireShader.BloomTexture = TrailRegistry.VortexTrail;
        blackFireShader.NoiseTexture = TrailRegistry.WhispyTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos,
            GetTrailColor, GetTrailWidth, blackFireShader, Projectile.Size * 0.5f);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(64, 0, ratio);
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.SkyBlue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawAfterImage, DrawLayer.BehindTiles);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails, DrawLayer.OverNPCs);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        _outlineMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");


        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.45f;
        shadowDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= _squishScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
public class Verlia : ScarletBoss,
    IDrawOutlines
{
    private enum AIState
    {
        Spawn,
        Idle,

        //Phase 1
        Two_Bouncing_Moons,
        Blue_Magic_Sword,
        Spining_Little_Moons,
        Blade_Dance,
        Clone_Summon,

        //Phase 2
        Phase_2_Transition,
        Desperation_Big_Moon,
        Sword_Fall,
        Running,
        Dash_Slash,
        Blade_Dance_V2,

        Death
    }


    private Color _outlineColor;
    private bool _warning;
    private bool _attacking;

    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            _patternManager ??= new PatternManager<AIState>(new Tuple<AIState, float>(AIState.Two_Bouncing_Moons, 1.0f));
            return _patternManager;
        }
    }


    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
                SetupAnimator();
            return _animatorBackingField;
        }
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];

    private int Twin_Bouncing_Moon_Damage => 30;

    #region Defaults

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 1;
        NPC.defense = 15;
        NPC.lifeMax = 6750;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Verlia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman."))
            });
    }

    #endregion

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;

            case AIState.Idle:
                AI_Idle();
                break;

            case AIState.Two_Bouncing_Moons:
                AI_TwoBouncingMoons();
                break;
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            AttackCycle = 0;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
        }
    }

    private void AI_TwoBouncingMoons()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    Animator.PlayAnimation(Anim_HoldUp);
                    if(Timer == 10)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: 1);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: -1);
                        }
                    }

                    if(Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                    if(Timer >= 180)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
            case 1:
                Animator.PlayAnimation(Anim_Pulse);
                if (Animator.IsFinished())
                {
                    Timer = 0;
                    AttackCycle++;
                }
                break;
            case 2:
                Animator.PlayAnimation(Anim_Explode);
                if (Animator.IsFinished())
                {
                    Timer = 0;
                    AttackCycle++;
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
    private void AI_Spawn()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }

    private void AI_Idle()
    {
        Timer++;
        if(Timer >= 60)
        {
            ChooseAttack();
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    #region Drawing


    private const string Anim_Summon = "Summon";
    private const string Anim_IdleSummon = "IdleSummon";
    private const string Anim_UnSummon = "UnSummon";
    private const string Anim_HoldUp = "HoldUp";
    private const string Anim_Sword = "Sword";
    private const string Anim_SwordSlash = "SwordSlash";
    private const string Anim_Down = "Down";
    private const string Anim_Pulse = "Pulse";
    private const string Anim_Explode = "Explode";
    private const string Anim_TeleportIn = "TeleportIn";
    private const string Anim_Run = "Run";
    private const string Anim_ReadyDash = "ReadyDash";
    private const string Anim_DashFrame = "DashFrame";
    private const string Anim_Spin = "Spin";
    private void SetupAnimator()
    {
        _animatorBackingField = new Animator();
        Vector2 drawOrigin = new Vector2(100, 114);
        var idle = new SpriteAnimation(0, 1, isLooping: true, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_Summon, idle);

        var swordHold = new SpriteAnimation(0, 4, isLooping: true, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_IdleSummon, swordHold);

        var handOut = new SpriteAnimation(0, 3, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_UnSummon, handOut);

        var lookOver = new SpriteAnimation(0, 7, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_HoldUp, lookOver);

        var morph = new SpriteAnimation(0, 10, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_Sword, morph);

        var swimming = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_SwordSlash, swimming);

        var battle = new SpriteAnimation(0, 9, isLooping: true, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_Down, battle);

        var forwardSlash = new SpriteAnimation(0, 21, isLooping: false, drawOrigin, frameSpeed: 0.22f);
        _animatorBackingField.AddAnimation(Anim_Pulse, forwardSlash);

        var backSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_Explode, backSlash);

        var foundYou = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_TeleportIn, foundYou);

        var holding = new SpriteAnimation(0, 8, isLooping: true, drawOrigin);
        _animatorBackingField.AddAnimation(Anim_Run, holding);

        var bigSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin, frameSpeed: 0.5f);
        _animatorBackingField.AddAnimation(Anim_ReadyDash, bigSlash);

        var running = new SpriteAnimation(0, 0, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        _animatorBackingField.AddAnimation(Anim_DashFrame, running);

        var running2 = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        _animatorBackingField.AddAnimation(Anim_Spin, running);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(184);
        NPC.frame.Height = 184;
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawSprite(spriteBatch, screenPos, drawColor);
        return false;
    }
    #endregion
    public override void OnKill()
    {
        base.OnKill();
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
  //      throw new System.NotImplementedException();
    }
}
