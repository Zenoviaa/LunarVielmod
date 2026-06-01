using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Effects.RoyalMagic;


public class PerfectWingShader : CrystalShader<PerfectWingShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
    
    public Texture2D StarTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
        }
    }

    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
        }
    }

    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
        }
    }

    public Vector2 PrimaryTextureSize
    {
        set
        {
            Effect.Parameters["primaryTextureSize"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float Distortion
    {
        set
        {
            Effect.Parameters["distortion"].SetValue(value);
        }
    }
}
public class StarBombBoomShader : CrystalShader<StarBombBoomShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

}

public class RoyalShockwaveCircleShader : CrystalShader<RoyalShockwaveCircleShader>
{
    private EffectParameter _timeParam;
    private EffectParameter _frequencyParam;
    private EffectParameter _amplitudeParam;
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }

    public float Amplitude
    {
        set
        {
            _amplitudeParam ??= Effect.Parameters["amplitude"];
            _amplitudeParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Amplitude = 0.5f;
        Frequency = 4.0f;
    }
}


public class CometTrailShader : CrystalShader<CometTrailShader>
{
    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
        }
    }

    public Asset<Texture2D> LaserTexture
    {
        set
        {
            Effect.Parameters["laserTexture"].SetValue(value.Value);
        }
    }


    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        TransformMatrix = TrailDrawer.WorldViewPoint2;
        BloomColor = Color.BlueViolet;
        BlendState = BlendState.AlphaBlend;
        LaserTexture = AssetManager.LaserTextures.CometTrail;
        Time = Main.GlobalTimeWrappedHourly * 24;
    }
}
public class StarMixShader : CrystalShader<StarMixShader>
{
    public Texture2D MixTexture
    {
        set
        {
            Effect.Parameters["mixTexture"].SetValue(value);
        }
    }
}

public class RoyalMagicStarsShader : CrystalShader<RoyalMagicStarsShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public Vector2 ScreenOffset
    {
        set
        {
            Effect.Parameters["screenOffset"].SetValue(value);
        }
    }
}
public class RoyalMagicBallShader : CrystalShader<RoyalMagicBallShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }
    public Texture2D StarTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Effect.Parameters["primaryTextureSize"].SetValue(value.Size());
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

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }

    public float Distortion
    {
        set
        {
            Effect.Parameters["distortion"].SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}

public class RoyalMagicBeamShader : CrystalShader<RoyalMagicBeamShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }

    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Vector2 Tiling
    {
        set
        {
            Effect.Parameters["tiling"].SetValue(value);
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }

    public float Distortion
    {
        set
        {
            Effect.Parameters["distortion"].SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}
[Autoload(Side = ModSide.Client)]
public class RoyalMagicRenderer : ModSystem
{
    public delegate void PrimitiveDrawAction(GraphicsDevice gDevice);
    public delegate void SpritebatchDrawAction(SpriteBatch sb);
    public struct Particles
    {
        public Particles(int maxParticles)
        {
            position = new Vector2[maxParticles];
            velocity = new Vector2[maxParticles];
            timeleft = new float[maxParticles];
            Length = maxParticles;
        }
        public Vector2[] position;
        public Vector2[] velocity;
        public float[] timeleft;
        public readonly int Length;
    }

    private Vector2 _oldMouseWorld;
    private readonly Particles _smearParticles = new Particles(384);
    private ManagedRenderTarget _directionRT;
    private ManagedRenderTarget _swirlRT;
    private ManagedRenderTarget _maskRT;

    private ManagedRenderTarget _outlineRT;
    private Queue<PrimitiveDrawAction> _primitiveDrawActions;// = new Queue<PrimitiveDrawAction>();

    private Asset<Texture2D> _royalSmokeMaskTextureAsset;
    public override void Load()
    {
        base.Load();
        _primitiveDrawActions = new Queue<PrimitiveDrawAction>();
        _royalSmokeMaskTextureAsset = ModContent.Request<Texture2D>("Stellamod/Effects/RoyalMagic/RoyalSmokeMask");
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderSwirls;
    }
    public override void Unload()
    {
        base.Unload();
        _royalSmokeMaskTextureAsset = null;
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderSwirls;
    }
    public override void OnModLoad()
    {
        base.OnModLoad();
        //Temporary, delete later cause these render targest aren't always needed
        PrepareRenderTargets();
    }

    public static void Queue(PrimitiveDrawAction drawAction)
    {
        RoyalMagicRenderer magicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
        magicRenderer._primitiveDrawActions.Enqueue(drawAction);
    }

    private void PrepareRenderTargets()
    {
        _maskRT = ManagedRenderTarget.New();
        _directionRT = ManagedRenderTarget.New();
        _swirlRT = ManagedRenderTarget.New();
        _outlineRT = ManagedRenderTarget.New();
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        SimulateParticles();
    }

    private void FenixSmoke()
    {

        Vector2 vel = (_oldMouseWorld - Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5;
        SpawnParticle(Main.MouseWorld, vel, 180);
        _oldMouseWorld = Main.MouseWorld;

        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicStarParticle.Spawn(Main.MouseWorld, vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
        }
        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicSwordParticle.Spawn(Main.MouseWorld + Main.rand.NextVector2Circular(32, 32), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            sp.behindLayer = Main.rand.NextBool(2);
        }
        if (Main.rand.NextBool(2))
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Main.MouseWorld + Main.rand.NextVector2Circular(32, 32), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.Scale *= 0.5f;
            sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
            sp.behindLayer = true;
        }
    }

    public void SpawnParticle(Vector2 position, Vector2 velocity, float timeLeft)
    {
        int freeIndex = -1;
        for (int i = 0; i < _smearParticles.Length; i++)
        {
            ref float t = ref _smearParticles.timeleft[i];
            if (t <= 0)
            {
                freeIndex = i;
                break;
            }
            //velocity *= 0.98f;
        }

        if (freeIndex == -1)
            return;

        _smearParticles.timeleft[freeIndex] = timeLeft;
        _smearParticles.position[freeIndex] = position;
        _smearParticles.velocity[freeIndex] = velocity;
    }
    private void SimulateParticles()
    {
        for (int i = 0; i < _smearParticles.Length; i++)
        {
            ref float timeLeft = ref _smearParticles.timeleft[i];
            if (timeLeft <= 0)
                continue;
            timeLeft--;

            ref Vector2 position = ref _smearParticles.position[i];
            ref Vector2 velocity = ref _smearParticles.velocity[i];
            position += velocity;
            velocity *= 0.96f;
        }
    }

    private void DrawMaskParticles(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_royalSmokeMaskTextureAsset, Vector2.Zero);
        for (int i = 0; i < _smearParticles.Length; i++)
        {
            ref float timeLeft = ref _smearParticles.timeleft[i];
            if (timeLeft <= 0)
                continue;
            ref Vector2 position = ref _smearParticles.position[i];
            maskDrawer.worldPosition = position;
            maskDrawer.VerticalFrame(i % 4, 4);
            maskDrawer.CenterOrigin();
            maskDrawer.rotation = Main.GlobalTimeWrappedHourly + i * 3;
            float progress = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(timeLeft / 180f));
            Color brighterColor = Color.Lerp(Color.Lerp(Color.Black, Color.White, 0.5f), Color.White, MathHelper.Lerp(0f, 1f, (i % 8f) / 8f));

            maskDrawer.color = Color.Lerp(Color.Transparent, brighterColor, progress);

            float offset = MathHelper.Lerp(0.8f, 1f, MathHelper.Lerp(0f, 1f, (i % 8f) / 8f));
            maskDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * offset, progress);
            spriteBatch.Draw(maskDrawer);
        }


    }
    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    private void RenderSwirls()
    {
        // return;
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        gDevice.SetRenderTarget(_maskRT);
        gDevice.Clear(Color.Transparent);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
        DrawMaskParticles(spriteBatch);
        spriteBatch.End();

        while (_primitiveDrawActions.Count > 0)
        {
            _primitiveDrawActions.Dequeue()(gDevice);
        }

        gDevice.SetRenderTarget(_directionRT);
        gDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Brightest, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, Vector2.Zero);
        for (int i = 0; i < _smearParticles.Length; i++)
        {
            ref float timeLeft = ref _smearParticles.timeleft[i];
            if (timeLeft <= 0)
                continue;
            ref Vector2 position = ref _smearParticles.position[i];
            sbDrawer.worldPosition = position;
            float progress = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(timeLeft / 180f));


            ref Vector2 velocity = ref _smearParticles.velocity[i];
            float angle = MathF.Atan2(-velocity.Y, -velocity.X);

            //Normalize the angle between 0-1
            float normalAngle = angle / MathHelper.Pi * 0.5f + 0.5f;
            Color color = new Color(normalAngle, 0, 0, progress);

            sbDrawer.color = color;
            sbDrawer.scale = new Vector2(1);
            spriteBatch.Draw(sbDrawer);
        }

        spriteBatch.End();


        gDevice.SetRenderTarget(_swirlRT);
        gDevice.Clear(Color.Transparent);

        //Prepare to draw this effect
        RoyalSwirlsShader swirlsShader = ShaderContent.GetInstance<RoyalSwirlsShader>();
        swirlsShader.Time = Main.GlobalTimeWrappedHourly;
        swirlsShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        swirlsShader.ScreenOffset = GetScreenOffset(scale: 1);
        Color lightColor = new Color(34, 41, 59);
        lightColor = Color.Lerp(lightColor, Color.White, 0.25f);

        Color darkColor = new Color(8, 7, 34);
        swirlsShader.LightColor = lightColor;
        swirlsShader.DarkColor = darkColor;
        swirlsShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        swirlsShader.DirectionTexture = _directionRT;
        swirlsShader.StarTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/FogEmpty").Value;

        //So, For this effect we want the swirls to be swiling around and scrolling around probably
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, swirlsShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.Swirl, Vector2.Zero);
        drawer.dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        drawer.drawOrigin = Vector2.Zero;
        drawer.color = Color.White;
        spriteBatch.Draw(drawer);
        spriteBatch.End();



        Color outlineColor = new Color(150, 150, 235) * 0.5f;
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        gDevice.SetRenderTarget(_outlineRT);
        gDevice.Clear(Color.Transparent);

        RoyalOutlineShader mixerShader2 = ShaderContent.GetInstance<RoyalOutlineShader>();
        mixerShader2.TexelSize = texelSize;
        mixerShader2.OutlineColor = outlineColor;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, mixerShader2.Effect);
        spriteBatch.Draw(_swirlRT, Vector2.Zero, Color.White);
        spriteBatch.End();

        gDevice.SetRenderTarget(_directionRT);
        gDevice.Clear(Color.Transparent);



        RoyalMixShader mixerShader = ShaderContent.GetInstance<RoyalMixShader>();
        mixerShader.MixTexture = _outlineRT;

        mixerShader.TexelSize = texelSize;
        mixerShader.OutlineColor = outlineColor;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, mixerShader.Effect);
        spriteBatch.Draw(_maskRT, Vector2.Zero, Color.White);
        spriteBatch.End();



        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.BehindNPCsWithOutline);
        //    throw new NotImplementedException();
    }


    private void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        //    Main.NewText("G");

        sb.Draw(_directionRT, Vector2.Zero, Color.White);
        // sb.Draw(_swirlRT, Vector2.Zero, Color.White);
    }
}

public class DashBlurShader : CrystalShader<DashBlurShader>
{
    public float BlurStrength
    {
        set
        {
            Effect.Parameters["blurStrength"].SetValue(value);
        }
    }
}

public class RoyalMixShader : CrystalShader<RoyalMixShader>
{
    public Vector2 TexelSize
    {
        set
        {
            Effect.Parameters["texelSize"].SetValue(value);
        }
    }
    public Texture2D MixTexture
    {
        set
        {
            Effect.Parameters["mixTexture"].SetValue(value);
        }
    }

    public Color OutlineColor
    {
        set
        {
            Effect.Parameters["outlineColor"].SetValue(value.ToVector4());
        }
    }

}
public class RoyalOutlineShader : CrystalShader<RoyalOutlineShader>
{
    public Vector2 TexelSize
    {
        set
        {
            Effect.Parameters["texelSize"].SetValue(value);
        }
    }

    public Color OutlineColor
    {
        set
        {
            Effect.Parameters["outlineColor"].SetValue(value.ToVector4());
        }
    }

    public float Levels
    {
        set
        {
            Effect.Parameters["levels"].SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Levels = 16.0f;
    }
}
public class RoyalSwirlsShader : CrystalShader<RoyalSwirlsShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
        }
    }

    public Texture2D DirectionTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[2] = value;
        }
    }
    public Texture2D StarTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[3] = value;
            Effect.Parameters["primaryTextureSize"].SetValue(value.Size());
        }
    }

    public Vector2 ScreenOffset
    {
        set
        {
            Effect.Parameters["screenOffset"].SetValue(value);
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

    public Color LightColor
    {
        set
        {
            Effect.Parameters["lightColor"].SetValue(value.ToVector3());
        }
    }

    public Color DarkColor
    {
        set
        {
            Effect.Parameters["darkColor"].SetValue(value.ToVector3());
        }
    }
}