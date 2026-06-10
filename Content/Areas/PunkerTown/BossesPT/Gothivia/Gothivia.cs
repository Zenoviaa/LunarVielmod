using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Core.AssetReferences.Projectiles;

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

[Autoload(Side = ModSide.Client)]
public class GothiviaDomain : ModSystem
{
    private ManagedRenderTarget _domainSwapRT;
    private ManagedRenderTarget _domainRT;
    public bool drawGothivia;
    public override void OnModLoad()
    {
        _domainSwapRT = ManagedRenderTarget.New();
        _domainRT = ManagedRenderTarget.New();
        On_Main.DrawNPCs += DrawBlack;

    }
    public override void Load()
    {

        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += DrawClouds;
    }

    private bool ShouldRender() => drawGothivia;
    private void DrawClouds()
    {
        if (!ShouldRender())
            return;
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (config.FocusMode)
        {
            return;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_domainRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));

        FireVortexShader fireShader = ShaderContent.GetInstance<FireVortexShader>();
        fireShader.Time = Main.GlobalTimeWrappedHourly * 0.1f;
        fireShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        fireShader.GradientTopColor = new Color(224, 187, 122);
        fireShader.GradientBottomColor = new Color(59, 19, 13);
        fireShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, fireShader.Effect);

        Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth , Main.screenHeight);
        spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, Color.Lerp(Color.White, Color.Black, 0.3f));

        spriteBatch.End();


        //Draw the smokee
        FireVortexSmokeShader smokeShader = ShaderContent.GetInstance<FireVortexSmokeShader>();
        smokeShader.GradientTopColor = new Color(125, 125, 125) ;
        smokeShader.GradientBottomColor = new Color(22, 22, 22);
        smokeShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        smokeShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        smokeShader.Time = 1.5f + Main.GlobalTimeWrappedHourly * 0.1f;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, smokeShader.Effect);
        targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        Color c = Color.Lerp(Color.White, Color.Black, 0.5f);
         spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, c);

        spriteBatch.End();



        _domainSwapRT ??= ManagedRenderTarget.New();
        graphicsDevice.SetRenderTarget(_domainSwapRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));


        PalettizerShader palettizerShader = PalettizerShader.Instance;
        palettizerShader.PaletteTexture = PaletteHelper.GetColorSpectrum("Hell.pal");
        palettizerShader.Progress = 1f;
        palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
        palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
        palettizerShader.DitherAlpha = 0.125f;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, palettizerShader.Effect);
        spriteBatch.Draw(_domainRT, Vector2.Zero, Color.White);
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_domainRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));

        spriteBatch.Begin();
        spriteBatch.Draw(_domainSwapRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
        spriteBatch.End();



    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= DrawClouds;
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Main.DrawNPCs -= DrawBlack;
    }

    private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (ShouldRender())
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.Clear(Color.Transparent);
            Color drawColor2 = Color.Lerp(Color.White, Color.Black, 0f);
            drawColor2 *= 1f;
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.FocusMode)
            {
                spriteBatch.Draw(_domainRT, new Rectangle(0, 0, Main.screenWidth * 2 , Main.screenHeight * 2), drawColor2);
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.25f);
            }


            //  spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                SpritebatchDrawer blackDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
                blackDrawer.dstRect = new Rectangle(0, (int)(drawPosition.Y-Main.screenPosition.Y)+48, Main.screenWidth, Main.screenHeight);
                blackDrawer.drawOrigin = Vector2.Zero;
                blackDrawer.color = Color.White * 0.15f;
            //    spriteBatch.Draw(blackDrawer);
                var bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine");
             
                //drawPosition -= Main.screenPosition;
                drawPosition.Y += 48;
                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bloomLine, drawPosition);
                drawer.rotation += MathHelper.PiOver2;
                drawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 12);
                drawer.color.A = 0;
                drawer.scale.Y *= 8;
                spriteBatch.Draw(drawer);

 
            }


            drawGothivia = false;
        }

        orig(self, behindTiles);
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
    private Outliner _outliner;
    private AnimationFramer _wingAnimationFrame;
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
        if(Timer == 1)
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
                for(int i = 0; i < 2; i++)
                {
                    Vector2 offset = Vector2.UnitY * 512;
                    offset = offset.RotatedBy((float)i / 2f * MathHelper.TwoPi);
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
    private void BowShot()
    {
        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 80, 460);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { PitchVariance = 0.5f }, NPC.Center);
        if (MultiplayerHelper.IsHost)
            return;


        Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 24;
        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, 
            ModContent.ProjectileType<GothinTorch>(), 600, 1, Main.myPlayer, 0, 0);
    }
    private void AI_Archery()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }

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


        if (Timer > 50)
        {

            _circleDegrees += _circleSpeed;
            float circleRadians = MathHelper.ToRadians(_circleDegrees);
            Vector2 offsetFromPlayer = new Vector2(_circleDistance, 0).RotatedBy(circleRadians);
            Vector2 circlePosition = MyTarget.Center + offsetFromPlayer;

            //This is just how quickly the NPC will move to the circle position
            //This number should be higher than the circle speed

            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, _movementSpeed);

        }

        if (Timer < 80 && Timer > 134)
        {

            _circleDegrees += _circleSpeed;
            float circleRadians = MathHelper.ToRadians(_circleDegrees);
            Vector2 offsetFromPlayer = new Vector2(_circleDistance, 0).RotatedBy(circleRadians);
            Vector2 circlePosition = MyTarget.Center + offsetFromPlayer;

            //This is just how quickly the NPC will move to the circle position
            //This number should be higher than the circle speed

            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, _movementSpeed);

        }

        if (Timer < 164 && Timer > 224)
        {

            _circleDegrees += _circleSpeed;
            float circleRadians = MathHelper.ToRadians(_circleDegrees);
            Vector2 offsetFromPlayer = new Vector2(_circleDistance, 0).RotatedBy(circleRadians);
            Vector2 circlePosition = MyTarget.Center + offsetFromPlayer;

            //This is just how quickly the NPC will move to the circle position
            //This number should be higher than the circle speed

            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, _movementSpeed);
        }

        NPC.velocity *= 0.96f;
        if (Timer == 60)
        {
            BowShot();
        }

        if (Timer == 154)
        {
            BowShot();
        }

        if (Timer == 248)
        {
            BowShot();
        }


        if (Timer >= 282)
        {
            Timer = 0;
            AttackCounter++;
            if(AttackCounter >= 3)
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
