using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.GameContent;
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
        fireShader.Time = Main.GlobalTimeWrappedHourly * 0.3f;
        fireShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        fireShader.GradientTopColor = new Color(224, 187, 122);
        fireShader.GradientBottomColor = new Color(59, 19, 13);
        fireShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, fireShader.Effect);

        Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth , Main.screenHeight);
        spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, Color.White);

        spriteBatch.End();


        //Draw the smokee
        FireVortexSmokeShader smokeShader = ShaderContent.GetInstance<FireVortexSmokeShader>();
        smokeShader.GradientTopColor = new Color(125, 125, 125) ;
        smokeShader.GradientBottomColor = new Color(22, 22, 22);
        smokeShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        smokeShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        smokeShader.Time = 1.5f + Main.GlobalTimeWrappedHourly * 0.1f;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, smokeShader.Effect);
        targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        Color c = Color.Lerp(Color.White, Color.Black, 0.5f)  *0.4f;
        spriteBatch.Draw(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds7").Value, targetRect, c);

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

    private bool _contactDamage;
    private Outliner _outliner;
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

    }

    public override void SetDefaults()
    {
        NPC.damage = 100;
        NPC.defense = 150;
        NPC.lifeMax = 255000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 99);
        NPC.boss = true;
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.takenDamageMultiplier = 0.75f;
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
    public override void AI()
    {
        base.AI();
        EnablePlatformArena();

        NPCID.Sets.MustAlwaysDraw[Type] = true;
        if(Main.netMode != NetmodeID.Server)
        {
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
        }
        _outliner.SetDefaults();
        switch (State)
        {
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

    }
    private void AI_Dichotamy()
    {

    }
    private void AI_Archery()
    {

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
