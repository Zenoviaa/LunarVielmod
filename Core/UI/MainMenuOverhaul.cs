using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.NPCs.Town;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
namespace Stellamod.Core.UI;

public class MainMenuFallingLeavesParticleSystem
{
    private struct LeafParticles
    {
        public LeafParticles(int maxParticleCount)
        {
            positions = new Vector2[maxParticleCount];
            velocities = new Vector2[maxParticleCount];
            timeLeft = new float[maxParticleCount];
        }

        public int length => positions.Length;
        public Vector2[] positions;
        public Vector2[] velocities;
        public float[] timeLeft;
    }

    private readonly TexturedQuad _quad;
    private readonly LeafParticles _leafParticles;
    private readonly LeafParticles _godrayParticles;
    private float _spawnTimer;
    private float _godraySpawnTimer;
    private readonly int[] _indexBuffer;
    private readonly Asset<Texture2D> _leavesTextureAsset;
    private int _drawSkip;
    public MainMenuFallingLeavesParticleSystem(int maxParticleCount)
    {
        _leavesTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/FallingLeavesNPetals");
        _quad = new TexturedQuad();

        _godrayParticles = new LeafParticles(25);
        _leafParticles = new LeafParticles(maxParticleCount);
        _indexBuffer = new int[400 * 6];
        int connectIndex = 0;
        for (int i = 0; i < _indexBuffer.Length; i += 6)
        {
            _indexBuffer[i] = connectIndex + 0;
            _indexBuffer[i + 1] = connectIndex + 2;
            _indexBuffer[i + 2] = connectIndex + 3;
            _indexBuffer[i + 3] = connectIndex + 0;
            _indexBuffer[i + 4] = connectIndex + 1;
            _indexBuffer[i + 5] = connectIndex + 3;
            connectIndex += 4;
        }
    }

    public void Update()
    {
        //TODO: Maybe use a sparse set to remove the branching?
        //probably a non issue
        for(int i = 0; i < _leafParticles.length; i++)
        {
            ref float timeLeft = ref _leafParticles.timeLeft[i];
            if (timeLeft <= 0)
                continue;
            timeLeft--;

            ref Vector2 position = ref _leafParticles.positions[i];
            ref Vector2 velocity = ref _leafParticles.velocities[i];

            position += velocity;
            velocity.X *= 0.999f;
            velocity.Y += MathF.Sin(timeLeft * 0.1f) * 0.02f;
            velocity.Y += 0.002f;
        }
        for (int i = 0; i < _godrayParticles.length; i++)
        {
            ref float timeLeft = ref _godrayParticles.timeLeft[i];
            if (timeLeft <= 0)
                continue;
            timeLeft--;

            ref Vector2 position = ref _godrayParticles.positions[i];
            ref Vector2 velocity = ref _godrayParticles.velocities[i];

            position += velocity;
            velocity *= 0.999f;
        }

        _spawnTimer++;
        if(_spawnTimer > 8)
        {
            int spawnIndex = -1;
            for (int i = 0; i < _leafParticles.length; i++)
            {
                ref float timeLeft = ref _leafParticles.timeLeft[i];
                if (timeLeft <= 0)
                {
                    spawnIndex = i;
                    break;
                }
            }

            if(spawnIndex != -1)
            {
                Vector2 newParticlePosition = new Vector2(0);
                newParticlePosition.X = Main.screenWidth + 128;
                newParticlePosition.Y = Main.rand.Next(-200, Main.screenHeight / 2 + 200);

                Vector2 initialVelocity = new Vector2(0);
                initialVelocity.X -= Main.rand.NextFloat(2f, 4f);
                initialVelocity.Y = 0.1f;

                float lifeTime = Main.rand.NextFloat(480, 800);
                _leafParticles.positions[spawnIndex] = newParticlePosition;
                _leafParticles.velocities[spawnIndex] = initialVelocity;
                _leafParticles.timeLeft[spawnIndex] = lifeTime;
            }
            _spawnTimer = 0;
        }

        _godraySpawnTimer++;
        if(_godraySpawnTimer > 60)
        {
            int spawnIndex = -1;
            for (int i = 0; i < _godrayParticles.length; i++)
            {
                ref float timeLeft = ref _godrayParticles.timeLeft[i];
                if (timeLeft <= 0)
                {
                    spawnIndex = i;
                    break;
                }
            }

            if (spawnIndex != -1)
            {
                Vector2 newParticlePosition = new Vector2(0);
                newParticlePosition.X = Main.screenWidth + Main.rand.Next(-500 ,0);
                newParticlePosition.Y = Main.rand.Next(-200, 200);

                Vector2 initialVelocity = Main.rand.NextVector2Circular(1, 1);

                float lifeTime = 600;
                _godrayParticles.positions[spawnIndex] = newParticlePosition;
                _godrayParticles.velocities[spawnIndex] = initialVelocity;
                _godrayParticles.timeLeft[spawnIndex] = lifeTime;
            }
            _godraySpawnTimer = 0;
        }
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        Main.screenPosition = Vector2.Zero;
        int index = 0;
        int primCount = 0;
        //Batch together all of the quads
        VertexPositionColorTexture[] vertexBuffer = new VertexPositionColorTexture[4 * 400];

        for (int i = 0; i < _leafParticles.length; i++)
        {
            ref float timeLeft = ref _leafParticles.timeLeft[i];
            if (timeLeft <= 0)
                continue;
            ref Vector2 position = ref _leafParticles.positions[i];
            float radians = Main.GlobalTimeWrappedHourly * 2 + i * 2;
            float rotation = Main.GlobalTimeWrappedHourly * 1 + i;
            
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), radians);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);
            float scale = ExtraMath.Osc(0.25f, 0.45f, 0f, offset: i * 2);
            _quad.Transform(position, 48 * scale, 48 * scale, rotationMatrix, rotation);

            int frame = (int)ExtraMath.Osc(0, 8, speed: 0, offset: i);
            _quad.VerticalFrame(frame, 8);

            Color color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.Clamp(timeLeft / 60f));
            _quad.SetColor(color);
            _quad.Push(ref vertexBuffer, ref index);

            primCount += 2;
            if (index >= vertexBuffer.Length)
                break;
            i += _drawSkip;
        }

        SpriteDrawingShader shader = ShaderContent.GetInstance<SpriteDrawingShader>();
        shader.SpriteTexture = _leavesTextureAsset;
        shader.ApplyPasses();
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.DrawUserIndexedPrimitives(
            PrimitiveType.TriangleList, vertexBuffer, 0, vertexBuffer.Length, _indexBuffer, 0, primCount);


    }

    public void DrawFrontLeaves(GraphicsDevice graphicsDevice)
    {
        _drawSkip = 8;
        Draw(graphicsDevice);
        _drawSkip = 0;
    }
    public void DrawGodrays()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        for (int i = 0; i < _godrayParticles.length; i++)
        {
            ref float timeLeft = ref _godrayParticles.timeLeft[i];
            if (timeLeft <= 0)
                continue;
            ref Vector2 position = ref _godrayParticles.positions[i];

            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, position);
            //    drawer.worldPosition += new Vector2(Main.screenWidth, 0);
            drawer.worldPosition += new Vector2(80, -256);
            drawer.scale = new Vector2(0.25f * ExtraMath.Osc(0.5f, 1f, 0, offset: i), 2f) * 1.5f;
            drawer.scale.X *= 2f;
            drawer.rotation = MathHelper.ToRadians(55 + 180 - 25);
            drawer.BottomCenterOrigin();

            float outAlpha = EasingFunction.InOutSine(timeLeft / 180f);
            float inAlpha = EasingFunction.InOutSine((600 - timeLeft) / 180f);
            Color color = Color.Lerp(Color.Transparent, Color.White, inAlpha * outAlpha);
            color *= 0.098f;
            color.A = 0;
            drawer.color = color;

            //   drawer.color.A = 0;
            spriteBatch.Draw(drawer);
        
        }


    }
}

public class PerfectMagicBackgroundShader : CrystalShader<PerfectMagicBackgroundShader>
{
    
    private EffectParameter _timeParam;
    public float Time
    {
        set
        {
            _timeParam = Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public Texture2D OutlineTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
}

[Autoload(Side = ModSide.Client)]
public class MainMenuOverhaul : ModSystem
{
    private MainMenuFallingLeavesParticleSystem _leavesParticleSystem;
    private RenderTarget2D _pixelTarget;
    private RenderTarget2D _pixelTarget2;
    private RenderTarget2D _fullTarget;
    private Asset<Texture2D> _ereshkigalTextureAsset;
    private Asset<Texture2D> _cloudsTextureAsset;
    private Asset<Texture2D> _cloudsOutlineTextureAsset;
    private Point _oldScreenSize;
    private bool _initTargets;
    public override void Load()
    {
        base.Load();
        _cloudsOutlineTextureAsset = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/Clouds6_Outline");
        _cloudsTextureAsset = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/Clouds6");
        _ereshkigalTextureAsset = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}Ereshkigal");
        MethodInfo baseMethod = typeof(Interface).GetMethod("AddMenuButtons", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        MonoModHooks.Add(baseMethod, DetourMenuButtons);
        IL_Main.DrawMenu += LeftAlignButtons;
        On_OverlayManager.Draw += BlackOutBackground;
        On_Main.UpdateMenu += UpdateParticleSystem;
        On_AWorldListItem.GetIcon += InitializeIconElement;

        //   AWorldListItem
    }

    private Asset<Texture2D> InitializeIconElement(On_AWorldListItem.orig_GetIcon orig, AWorldListItem self)
    {
        if(self.Data.WorldSizeX > 8400)
            return ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Menu/LunarTree", AssetRequestMode.ImmediateLoad);
        return orig(self);
    }

    private Asset<Texture2D> InitializeIconElement(On_AWorldListItem.orig_GetSeedIcon orig, AWorldListItem self, string seed)
    {
        return ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Menu/LunarTree", AssetRequestMode.ImmediateLoad);
        orig(self, seed);
    }

    private UIElement InitializeIconElement(On_AWorldListItem.orig_GetIconElement orig, AWorldListItem self)
    {
    
        UIImage element = new UIImage(ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Menu/LunarTree", AssetRequestMode.ImmediateLoad))
        {
            Top = new StyleDimension(-10f, 0f),
            Left = new StyleDimension(-6f, 0f),
            IgnoresMouseInteraction = true
        };
        return element;
    }

    public override void Unload()
    {
        base.Unload();
        _cloudsTextureAsset = null;
        _ereshkigalTextureAsset = null;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();   
        _leavesParticleSystem = new MainMenuFallingLeavesParticleSystem(400);
    }

    private void UpdateParticleSystem(On_Main.orig_UpdateMenu orig)
    {
        orig();
        _leavesParticleSystem?.Update();
    }

    public bool IsMenuActive => MenuLoader.CurrentMenu == ModContent.GetInstance<Stellamenu>();

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();

    }

    private void ResizeRTs()
    {
        _pixelTarget2?.Dispose();
        _pixelTarget2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X / 2, Main.ScreenSize.Y / 2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        _pixelTarget?.Dispose();
        _pixelTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X / 2, Main.ScreenSize.Y / 2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        _fullTarget?.Dispose();
        _fullTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        _initTargets = true;
    }
    private void BlackOutBackground(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (IsMenuActive && Main.gameMenu && _oldScreenSize != Main.ScreenSize)
        {
            Main.QueueMainThreadAction(ResizeRTs);
            _oldScreenSize = Main.ScreenSize;
        }
        // throw new NotImplementedException();
        orig(self, spriteBatch, layer, beginSpriteBatch);
        if (Main.gameMenu && layer == RenderLayers.Landscape && IsMenuActive && _initTargets)
        {
            //Pixelation Effect :)
        
            spriteBatch.GraphicsDevice.SetRenderTarget(_fullTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);


            var perfectMagicShader = ShaderContent.GetInstance<PerfectMagicBackgroundShader>();
            perfectMagicShader.Time = Main.GlobalTimeWrappedHourly;
            perfectMagicShader.OutlineTexture = _cloudsOutlineTextureAsset.Value;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                perfectMagicShader.Effect);

            SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(_cloudsTextureAsset, Main.screenPosition);
            sbDrawer.drawOrigin = Vector2.Zero;
            sbDrawer.dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            sbDrawer.color = Color.Lerp(Color.White, Color.Black, 0.9f);
            spriteBatch.Draw(sbDrawer);
            spriteBatch.End();

            _leavesParticleSystem?.Draw(spriteBatch.GraphicsDevice);


            spriteBatch.GraphicsDevice.SetRenderTarget(_pixelTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(_fullTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();


            spriteBatch.GraphicsDevice.SetRenderTarget(_fullTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _leavesParticleSystem?.DrawFrontLeaves(spriteBatch.GraphicsDevice);

            spriteBatch.GraphicsDevice.SetRenderTarget(_pixelTarget2);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(_fullTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();




            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Black);



            var starsTexture = TextureRegistry.StarNoise2;
            var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
            MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

            eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
            eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
            eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            eff.UseImage2(noiseTexture);
            Vector2 _parallax = new Vector2(Main.GlobalTimeWrappedHourly, Main.GlobalTimeWrappedHourly * -196);
            eff.Shader.Parameters["uImageOffset"].SetValue(-_parallax * 0.0005f);
            eff.UseOpacity(1f);
            eff.Apply();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);
            spriteBatch.Draw(starsTexture.Value,
               new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                null, Color.White * 0.3f);


            /*
            spriteBatch.Draw(starsTexture.Value, 
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                new Rectangle((int)-_parallax.X, (int)-_parallax.Y, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);
            */
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(_pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            int width = spriteBatch.GraphicsDevice.Viewport.Bounds.Width;
            int height = spriteBatch.GraphicsDevice.Viewport.Bounds.Height;
            SpritebatchDrawer ereshDrawer = SpritebatchDrawer.FromTextureAsset(_ereshkigalTextureAsset, Main.screenPosition);
            ereshDrawer.scale = Vector2.One * 0.5f;
            //ereshDrawer.drawOrigin = Vector2.Zero;

            ereshDrawer.worldPosition.X += (float)width * 0.9f - _ereshkigalTextureAsset.Width() * 0.5f;
            ereshDrawer.worldPosition += _ereshkigalTextureAsset.Size() * 0.5f * 0.5f;
            float radians = 0.03f;
            ereshDrawer.rotation = MathHelper.Lerp(-radians, radians, ExtraMath.Osc(0f, 1f, speed: 0.35f));
            spriteBatch.Draw(ereshDrawer);

            spriteBatch.Draw(_pixelTarget2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            _leavesParticleSystem?.DrawGodrays();

            spriteBatch.End();
        }
    }
    private void DetourMenuButtons(Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, ref int offY, ref int spacing, ref int buttonIndex, ref int numButtons)
    {
        offY += 100;
    }

    /// <summary>
    /// Reorients the butons on the main menu to be aligned to the left side of the screen
    /// </summary>
    /// <param name="il"></param>
    private void LeftAlignButtons(ILContext il)
    {
        try
        {
            ILCursor c = new ILCursor(il);


            // Terraria sets the button offset to be half the screen width
            //We just want a slight offset from the left side of the screen
            //The 250 is the variable declaration right before the X position lines
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(250));
            c.Index += 2;
            c.Emit(OpCodes.Pop);
            c.EmitDelegate<Func<int>>(() =>
            {
                if (IsMenuActive)
                {
                    return 150;
                }

                //We didn't pop the division operation, so just return the screen width, it's alr dividing in 2
                return Main.screenWidth;
            });


            /*
            
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(220));
            c.Emit(OpCodes.Pop);
            c.EmitDelegate<Func<int>>(() =>
            {
                if (IsMenuActive)
                {
                    return 350;
                }

                //We didn't pop the division operation, so just return the screen width, it's alr dividing in 2
                return 220;
            });

            */
            //Need to set the X Origin point of the button texts to be 0
            c.GotoNext(MoveType.After, i => i.MatchLdcR4(215));

            //This one is the Y
            c.GotoPrev(MoveType.After, i => i.MatchLdcR4(0.5f));

            //This one is the X
            c.GotoPrev(MoveType.After, i => i.MatchLdcR4(0.5f));
            c.Emit(OpCodes.Pop);
            c.EmitDelegate<Func<float>>(() =>
            {
                if (IsMenuActive)
                {
                    return 0f;
                }
                return 0.5f;
            });

           
            //Since the text is now left aligned, the hitbox for clicking on them is in the wrong spot
            //Thankfully we can just move it over by 50% to the right
            c.GotoNext(MoveType.After, i => i.MatchLdcI4(-2));
            c.GotoNext(MoveType.After, i => i.MatchLdcR4(0.5f));
            for (int i = 0; i < 2; i++)
            {
                c.GotoNext(MoveType.After, i => i.MatchLdcR4(0.5f));
                c.Emit(OpCodes.Pop);
                c.EmitDelegate<Func<float>>(() =>
                {
                    if (IsMenuActive)
                    {
                        return 1f;
                    }
                    return 0.5f;
                });
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<Stellamod>(), il);
        }
    }
}
