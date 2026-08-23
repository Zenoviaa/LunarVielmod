using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.AccAB;


[Autoload(Side = ModSide.Client)]
public class MoonFlightRenderer : ModSystem
{
    private Player _renderPlayer;
    private Asset<Texture2D> _wingTextureAsset;
    private Asset<Texture2D> _wingOutlineTextureAsset;
    private Asset<Texture2D> _wingTextureAsset2;
    public RenderTargetProvider moonFlightRT = new RenderTargetProvider(() => RenderTargetParameters.DefaultScreenTarget with { Width = 256, Height = 256 });
    public RenderTargetProvider moonFlightSwapRT = new RenderTargetProvider(() => RenderTargetParameters.DefaultScreenTarget with { Width = 256, Height = 256 });
    public override void OnModLoad()
    {
        base.OnModLoad();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderWings;
    }

    private void RenderWings()
    {
        bool shouldDrawMoonflight = false;
        foreach (var player in Main.ActivePlayers)
        {           
            if (player.GetModPlayer<FastFlightPlayer>().hasMoonFlight)
            {
                shouldDrawMoonflight = true;
                break;
            }
        }
        if (!shouldDrawMoonflight)
            return;

        string texture = ModContent.GetInstance<Verlia>().Texture;
        _wingTextureAsset ??= ModContent.Request<Texture2D>(texture + "_Wing");
        _wingOutlineTextureAsset ??= ModContent.Request<Texture2D>(texture + "_WingOutline");
        _wingTextureAsset2 ??= ModContent.Request<Texture2D>(texture + "_WingSprite");

        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        gDevice.SetRenderTarget(moonFlightRT);
        gDevice.Clear(Color.Transparent);

        SpriteBatch spriteBatch = Main.spriteBatch;



        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
        foreach (var player in Main.ActivePlayers)
        {
            _renderPlayer = player;
            if (player.GetModPlayer<FastFlightPlayer>().hasMoonFlight)
            {
                DrawWings(spriteBatch, Vector2.Zero, Color.White);
            }
        }
        spriteBatch.End();

        gDevice.SetRenderTarget(moonFlightSwapRT);
        gDevice.Clear(Color.Transparent);

        PalettizerShader palettizerShader = PalettizerShader.Instance;
        palettizerShader.PaletteTexture = PaletteAssets.FromPaletteFile(PaletteAssets.MOONSPIRALTOWER).Value.ColorAtlas;//PaletteHelper.GetColorSpectrum("MoonspiralTower.pal");
        palettizerShader.Progress = 1f;
        palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
        palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
        palettizerShader.DitherAlpha = 0.125f;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, palettizerShader.Effect);
        spriteBatch.Draw(moonFlightRT, Vector2.Zero, Color.White);
        spriteBatch.End();
        gDevice.SetRenderTarget(null);
    }

    private float HeightOffset
    {
        get
        {
            return 0;
        }
    }

    private float WingOffset
    {
        get
        {
            return -12;
        }
    }

    private float WingVelocity
    {
        get
        {
            return 15;
        }
    }
    private Vector2 LeftWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.3f, EasingFunction.Clamp(_renderPlayer.velocity.X / -WingVelocity));
            return leftWingScale;
        }
    }
    private float LeftWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(12), EasingFunction.Clamp(_renderPlayer.velocity.X / -WingVelocity));
            return rot;
        }
    }
    private Vector2 RightWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.3f, EasingFunction.Clamp(_renderPlayer.velocity.X / WingVelocity));
            return leftWingScale;
        }
    }
    private float RightWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(-12), EasingFunction.Clamp(_renderPlayer.velocity.X / -WingVelocity));
            return rot;
        }
    }

    private Vector2 GetDrawPosition()
    {
        return Main.screenPosition + moonFlightRT.Size * 0.5f;
    }

    private void DrawWings_Inner(SpriteBatch spriteBatch)
    {

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, _renderPlayer.Center);
        glowDrawer.color = Color.DeepSkyBlue * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 0.66f;
        glowDrawer.scale *= 0.6f;
        glowDrawer.rotation = MathHelper.ToRadians(degrees);

        glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);

        glowDrawer.rotation = MathHelper.ToRadians(-degrees);
        glowDrawer.drawOrigin.X = glowDrawer.texture.Size().X - glowDrawer.drawOrigin.X;
        spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, GetDrawPosition());
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        // wingDrawer.drawOrigin.X = WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.DarkBlue;

        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.scale = LeftWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);
    }

    private void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        VerlianWingsShader wingShader = VerlianWingsShader.Instance;
        wingShader.BloomColorStart = Color.White;
        wingShader.BloomColorEnd = Color.Lerp(Color.Lerp(Color.Blue, Color.Black, 0.5f), Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 2));
        wingShader.PerlinNoiseTexture = AssetManager.Noise.Whirly.Value;
        wingShader.ScrollingTexture = TrailRegistry.WaterTrail.Value;
        wingShader.DistortionStrength = 0.15f;
        wingShader.MaskSize = _wingTextureAsset.Size();
        wingShader.Frequency = 1f;
        wingShader.Tiling = Vector2.One * 2.5f;
        wingShader.ScrollOffset = new Vector2(-Main.GlobalTimeWrappedHourly * 0.4f, 0.0f);
        DrawWings_Inner(spriteBatch);

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, wingShader.Effect);

        SpritebatchDrawer wingDrawer;

        //Draw main wings
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, GetDrawPosition());
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.5f) * 0.5f;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        wingDrawer.scale = RightWingScale;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.scale = LeftWingScale;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);

        //Draw stars in wings
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, wingShader.Effect);

        wingShader.Tiling = Vector2.One * 16f;
        wingShader.ScrollingTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2").Value;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, GetDrawPosition());
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.35f) * 0.6f;
        wingDrawer.color.A = 0;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);



        wingShader.BloomColorEnd = Color.White;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingOutlineTextureAsset, GetDrawPosition());

        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.White;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
    }
}
public class FastFlightPlayer : ModPlayer
{
    private float _frameCounter;
    private int _frame;
    private float _frameSpeed;
    private float _wingTimer;
    public bool hasFastFlight;
    public bool hasMoonFlight;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasFastFlight = false;
        hasMoonFlight = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasFastFlight)
            return;
        _wingTimer++;
        if (_wingTimer % 7 == 0)
        {
            //  Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.White, Scale: 0.5f);
        }

        if (IsFlying())
        {
            _frameSpeed = 4;
            _frameCounter++;
            if (_frameCounter >= _frameSpeed)
            {
                _frameCounter = 0;
                _frame++;
                if (_frame >= 8)
                {
                    _frame = 0;
                }
            }
        }
        else
        {
            if (_frame > 0)
            {
                _frameCounter--;
                if (_frameCounter <= 0)
                {
                    _frameCounter = _frameSpeed;
                    _frame--;
                }
            }
        }

        if (hasMoonFlight)
        {
            Player.wingTimeMax = Player.wingTimeMax + 7;
            Player.GetDamage(DamageClass.Generic) += 0.05f;
            Player.moveSpeed += 0.2f;
        }
    }

    private bool IsFlying()
    {
        return Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;
        if (!hasFastFlight)
            return;
        if (Player.dead)
            return;
        if (Player.GetModPlayer<MovePlayer>().eaten)
            return;

        if (!hasMoonFlight)
        {
            DrawDefaultFastFlight(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
        else
        {
            DrawMoonFlight(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
    }


    private void DrawMoonFlight(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        float alpha = EasingFunction.InOutSine(_wingTimer / 60f);

        Texture2D moonFlightTexture = ModContent.GetInstance<MoonFlightRenderer>().moonFlightSwapRT;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Color glowColor = Color.White;
        glowColor *= alpha;
        Vector2 drawOrigin = moonFlightTexture.Size() / 2f;
        Vector2 drawScale = Vector2.One * 0.75f;
        Vector2 drawPosition = Player.Center - Main.screenPosition;

        drawPosition.Y += 3;
        drawPosition.Y += Player.gfxOffY;

        //This draws it in persepctive
        drawScale *= 0.8f;
        drawPosition.X -= Player.direction * 12;
        drawPosition.Y += ExtraMath.Osc(0f, -6);

        //  spriteBatch.Draw(wingsTexture, drawPosition, frame, glowColor * 0.7f, 0, drawOrigin, drawScale, SpriteEffects.None, 0);

        Color wingColor = glowColor;
        //   wingColor *=
        var drawData = new DrawData(
           moonFlightTexture,
            drawPosition,
            null,
            wingColor,
            0,
            drawOrigin,
            drawScale,
            SpriteEffects.None,
            0
        );
        drawData.shader = drawInfo.cWings;
        drawInfo.DrawDataCache.Add(drawData);
    }

    private void DrawDefaultFastFlight(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        float alpha = EasingFunction.InOutSine(_wingTimer / 60f);

        Asset<Texture2D> wingsTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/FastFlightProj");
        Asset<Texture2D> glowMaskWingsTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/FastFlightProj_GlowMask");
        Rectangle frame = wingsTextureAsset.Value.GetFrame(_frame, 8);
        SpriteBatch spriteBatch = Main.spriteBatch;
        Color glowColor = Color.White;
        glowColor *= alpha;
        glowColor.A = 0;
        Vector2 drawOrigin = frame.Size() / 2f;
        Vector2 drawScale = Vector2.One * 0.75f;
        Vector2 drawPosition = Player.Center - Main.screenPosition;

        drawPosition.Y -= 12;
        drawPosition.Y += Player.gfxOffY;

        //This draws it in persepctive
        drawScale.X *= 0.65f;
        drawPosition.X -= Player.direction * 12;
        drawPosition.Y += ExtraMath.Osc(0f, -6);
        Texture2D zuiTexyt = AssetManager.GlowMask.SimpleGlowCircle.Value;
        spriteBatch.Draw(glowMaskWingsTextureAsset.Value, drawPosition, frame, glowColor * 0.5f, 0, frame.Size() / 2f, drawScale * 1.2f, SpriteEffects.None, 0);
        //  spriteBatch.Draw(wingsTexture, drawPosition, frame, glowColor * 0.7f, 0, drawOrigin, drawScale, SpriteEffects.None, 0);

        Color wingColor = glowColor * 0.7f;
        wingColor.A = 0;
        //   wingColor *=
        var drawData = new DrawData(
            wingsTextureAsset.Value,
            drawPosition,
            frame,
            wingColor,
            0,
            frame.Size() * 0.5f,
            drawScale,
           SpriteEffects.None,
            0
        );
        drawData.shader = drawInfo.cWings;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

[AutoloadEquip(EquipType.Wings)]
public class FastFlight : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(14, 9f, 3);
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
        Item.accessory = true;
    }

    public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
        ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
    {
        ascentWhenFalling = 0.85f; // Falling glide speed
        ascentWhenRising = 0.15f; // Rising speed
        maxCanAscendMultiplier = 2;
        maxAscentMultiplier = 3f;
        constantAscend = 0.135f;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<FastFlightPlayer>().hasFastFlight = true;
    }
}

public class MoonFlight : ModItem
{


    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
        Item.accessory = true;
    }


    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<FastFlightPlayer>().hasMoonFlight = true;


    }
}
