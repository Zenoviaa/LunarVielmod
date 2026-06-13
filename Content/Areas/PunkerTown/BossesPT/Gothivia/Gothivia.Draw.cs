using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public partial class Gothivia :
    IDrawToRenderTarget
{
    private Asset<Texture2D> _wings3QTextureAsset;
    private Asset<Texture2D> _wings4QTextureAsset;
    private Asset<Texture2D> _bowTextureAsset;
    private Asset<Texture2D> _fingerTextureAsset;
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
                SetupAnimator();
            return _animator;
        }
    }

    private TexturedQuad _wingQuad;
    private TexturedQuad WingQuad
    {
        get
        {
            _wingQuad ??= new TexturedQuad();
            return _wingQuad;
        }
    }

    private const string Anim_Dive = "Dive";
    private const string Anim_Floating = "Floating";
    private const string Anim_Arrowhold = "ArrowHold";
    private const string Anim_Arrowshot = "Arrowshot";
    private const string Anim_Dichotamy = "Dichotamy";
    private const string Anim_Explode = "Explode";
    private const string Anim_Kickstart = "Kickstart";
    private const string Anim_Standalone = "Standalone";
    private const string Anim_Aurafarming = "Aurafarming";

    private void DrawPixelatedTorchWings(GraphicsDevice gDevice)
    {
        Matrix CalculateMatrix(float zOffset, float tOffset)
        {
            Matrix perspectiveMatrix = Matrix.Identity;
            Vector3 yAxis = new Vector3(0, 1, 0);
            float zRotation = MathHelper.Lerp(MathHelper.ToRadians(-75), MathHelper.ToRadians(-60),
                ExtraMath.Osc(0f, 1f, speed: 4, offset: tOffset));

            Quaternion zQuaternion = Quaternion.CreateFromAxisAngle(yAxis, zRotation);
            Matrix flapMatrix = Matrix.CreateFromQuaternion(zQuaternion);


            Vector3 zAxis = new Vector3(0, 0, 1);
            float range = 9;
            float zRot = MathHelper.Lerp(MathHelper.ToRadians(range),
                MathHelper.ToRadians(0), ExtraMath.Osc(0f, 1f, speed: 4, offset: tOffset));
         
            Quaternion zWingQuat = Quaternion.CreateFromAxisAngle(zAxis, zRot);
            Matrix z = Matrix.CreateFromQuaternion(zWingQuat);


            Vector3 offset = Vector3.Zero;
            Matrix translationMatrix = Matrix.CreateTranslation(offset);
            Matrix zOffsetMatrix = Matrix.CreateRotationZ(zOffset);
            Matrix fullMatrix = z * flapMatrix  * perspectiveMatrix * translationMatrix * zOffsetMatrix;
            return fullMatrix;
        }
        Main.graphics.GraphicsDevice.Textures[0] = AssetManager.GlowMask.JumbledGlowCircle;
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        FlameWingShader flameWingShader = ShaderContent.GetInstance<FlameWingShader>();
        flameWingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        flameWingShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        flameWingShader.Distortion = 0.15f;
        flameWingShader.Time = Main.GlobalTimeWrappedHourly * 10;
        flameWingShader.BloomColor = Color.Red;
        flameWingShader.InsideColor = Color.White;

        Vector2 centerPoint = NPC.Center;
        centerPoint -= new Vector2(-64, 0);
        float scale = 0.6f;

        Matrix topWingMatrix = CalculateMatrix(zOffset: 0, tOffset: 0);
        WingQuad.CalculateRightCenterVertices(centerPoint, 1200 * scale, 200 * scale, topWingMatrix);
           
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 3));
        Color wingColor = Color.Lerp(Color.Lerp(Color.White, Color.Yellow, 0.5f), glowColor, ExtraMath.Osc(0f, 0.8f, speed: 1.5f));
        wingColor *=MathHelper.Lerp(0.4f, 1f, ExtraMath.Osc(0f, 1f, speed: 4));
        WingQuad.SetColor(wingColor);
        WingQuad.DrawWithShader(flameWingShader);

        WingQuad.FlipVerticesX(NPC.Center.X);
        WingQuad.DrawWithShader(flameWingShader);

        /*
        scale *= 0.9f;
        Matrix bottomWingMatrix = CalculateMatrix(zOffset: MathHelper.ToRadians(-35), tOffset: -1);
        WingQuad.CalculateRightCenterVertices(centerPoint, 1200 * scale, 200 * scale, bottomWingMatrix);
        WingQuad.SetColor(Color.Lerp(wingColor, Color.Red, 0.5f) * 0.75f);
        WingQuad.DrawWithShader(flameWingShader);

        WingQuad.FlipVerticesX(NPC.Center.X);
        WingQuad.DrawWithShader(flameWingShader);*/
    }



    private void SetupAnimator()
    {
        _animator = new Animator();
        Vector2 animationDrawOrigin = new Vector2(87, 120);
        var dichotamy = new SpriteAnimation(0, 14, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Dichotamy, dichotamy);

        var floating = new SpriteAnimation(0, 7, isLooping: true, drawOriginOverride: animationDrawOrigin, frameSpeed: 0.35f);
        _animator.AddAnimation(Anim_Floating, floating);

        var arrowhold = new SpriteAnimation(0, 8, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Arrowhold, arrowhold);

        var arrowshot = new SpriteAnimation(0, 4, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Arrowshot, arrowshot);

        var kickstart = new SpriteAnimation(0, 7, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Kickstart, kickstart);

        var explode = new SpriteAnimation(0, 9, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Explode, explode);

        var standalone = new SpriteAnimation(0, 4, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Standalone, standalone);

        var aura = new SpriteAnimation(0, 7, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Aurafarming, aura);

        var dive = new SpriteAnimation(0, 0, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Dive, dive);

        _animator.PlayAnimation(Anim_Floating);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        ModContent.GetInstance<GothiviaDomain>().drawGothivia = true;
        DrawTelegraphLine(spriteBatch);
        DrawWings(spriteBatch);
        DrawSprite(spriteBatch);
        DrawBow(spriteBatch);
        DrawFinger(spriteBatch);
        return false;
    }

    private Asset<Texture2D> GetWingsTextureAsset()
    {
        _wings3QTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Gwings3Q");
        _wings4QTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Gwings4Q");
        switch (_wingsPerspective)
        {
            default:
            case WingsPerspective.ThreeQ:
                return _wings3QTextureAsset;
            case WingsPerspective.FourQ:
                return _wings4QTextureAsset;
        }
    }

    private void DrawAuraTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(128, 128, ratio) * _figure8TrailAlpha;
        }
        Color GetTrailColor(float ratio)
        {
            return Color.Lerp(Color.Lerp(Color.White, Color.Red, ratio), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), ratio), ratio) * MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(ratio))* _figure8TrailAlpha;
        }
        Color GetTrailColor2(float ratio)
        {
            return Color.Lerp(Color.Lerp(Color.White, Color.Red, ratio), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), ratio), ratio) * MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(ratio)) * _figure8TrailAlpha * 0.5f;
        }
        BasicLaserShader auraShader = ShaderContent.GetInstance<BasicLaserShader>();
        auraShader.LaserTexture = AssetManager.LaserTextures.Aura;
        auraShader.InnerColor = Color.White;
        auraShader.OuterColor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, auraShader, NPC.Size * 0.5f);

        auraShader.InnerColor = Color.Yellow;
        auraShader.LaserTexture = AssetManager.LaserTextures.Bloom;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor2, GetTrailWidth, auraShader, NPC.Size * 0.5f);
    }
    private void DrawFaintFlamingTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(48, 48, ratio) ;
        }
        Color GetTrailColor(float ratio)
        {
            return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) ;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        flameTrailShader.LaserTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SmooothTrail").Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(NPC.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, NPC.Size * 0.5f);
    }

    private void DrawFlamingFigure8Trail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 96, ratio) * _figure8TrailAlpha;
        }
        Color GetTrailColor(float ratio)
        {
            return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _figure8TrailAlpha;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        flameTrailShader.LaserTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SmooothTrail").Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(NPC.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, NPC.Size * 0.5f);
    }

    private void DrawTelegraphLine(SpriteBatch spriteBatch)
    {
        if (_telegraphLineAlpha <= 0.05f)
            return;

        Asset<Texture2D> bloomLineTextureAsset = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/BloomLine");
        void DrawLineInner(Vector2 direction)
        {
            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(bloomLineTextureAsset, NPC.Center);
            lineDrawer.rotation = direction.ToRotation() - MathHelper.PiOver2;
            lineDrawer.color = Color.White * _telegraphLineAlpha * ExtraMath.Osc(0.4f, 1f, speed: 32);
            lineDrawer.color.A = 0;
            lineDrawer.TopCenterOrigin();
            lineDrawer.scale.Y *= 4;
            lineDrawer.scale.X *= 0.4f;

            spriteBatch.Draw(lineDrawer);
        }

        if(ShootRotations.Count > 0)
        {
            for(int i = 0; i < ShootRotations.Count; i++)
            {
                float angle = ShootRotations[i];
                Vector2 offset = angle.ToRotationVector2();
                DrawLineInner(offset);
            }
            return;
        }


        if (_numDirections == 0)
            DrawLineInner(_aimingVelocity);
        else
        {
            for(float f = 0; f < _numDirections; f++)
            {
                float rot = (float)f / (float)_numDirections;
                rot *= MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2();
                DrawLineInner(offset);
            }
        }
    }


    private bool ShouldRenderBow()
    {
        if (_bowDissipateAlpha < 0.05f)
            return false;
        if (_telegraphLineAlpha < 0.05f)
            return false;
        if (_renderFinger)
            return false;
        return true;
    }

    private bool ShouldRenderFinger()
    {
        if (_telegraphLineAlpha <= 0.05f)
            return false;
        if (!_renderFinger)
            return false;
        return true;
    }

    private void DrawBow(SpriteBatch spriteBatch)
    {
        if (!ShouldRenderBow())
            return;

        FlameBowShader flamebowShader = ShaderContent.GetInstance<FlameBowShader>();
        flamebowShader.Time = Main.GlobalTimeWrappedHourly * -24;
        flamebowShader.FlameNoiseTexture = AssetManager.Noise.InvertedVoronoi;
        flamebowShader.InsideColor = Color.Yellow;
        flamebowShader.BloomColor = Color.Red;
        flamebowShader.DissipateThreshold = MathHelper.Lerp(1f, 0f, _bowDissipateAlpha);
        flamebowShader.DistortionStrength = 1f;

        _bowTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Bow");
        Vector2 bowholdPosition = NPC.Center;
        bowholdPosition += _aimingVelocity * 100;
        bowholdPosition += Vector2.Lerp(_aimingVelocity * 700, Vector2.Zero, EasingFunction.OutExpo(_bowDissipateAlpha));
        spriteBatch.Restart(effect: flamebowShader.Effect);
        SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromTextureAsset(_bowTextureAsset, bowholdPosition);
        bowDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 18) * _telegraphLineAlpha;
        bowDrawer.color.A = 0;
        bowDrawer.rotation = _aimingVelocity.ToRotation();
        bowDrawer.sourceRect = _bowTextureAsset.Value.GetFrame(_bowFrame, 7);
        bowDrawer.CenterOrigin();
        spriteBatch.Draw(bowDrawer);


        bowDrawer.color = Color.DarkRed * ExtraMath.Osc(0.8f, 1f, speed: 12) * _telegraphLineAlpha * 0.35f;
        bowDrawer.color.A = 0;
        bowDrawer.scale *= 1.2f;
        bowDrawer.worldPosition -= _aimingVelocity * 32;
        spriteBatch.Draw(bowDrawer);
        spriteBatch.RestartDefaults();
    }

    private void DrawFinger(SpriteBatch spriteBatch)
    {
        if (!ShouldRenderFinger())
            return;

        _fingerTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_HollowFinger");
        FlameBowShader flamebowShader = ShaderContent.GetInstance<FlameBowShader>();
        flamebowShader.Time = Main.GlobalTimeWrappedHourly * -24;
        flamebowShader.FlameNoiseTexture = AssetManager.Noise.InvertedVoronoi;
        flamebowShader.InsideColor = Color.Yellow;
        flamebowShader.BloomColor = Color.Red;
        flamebowShader.DissipateThreshold = MathHelper.Lerp(1f, 0f, _bowDissipateAlpha);
        flamebowShader.DistortionStrength = 0.125f;

        Vector2 bowholdPosition = NPC.Center;
        bowholdPosition += _aimingVelocity * 154;
        bowholdPosition += Vector2.Lerp(_aimingVelocity * 700, Vector2.Zero, EasingFunction.OutExpo(_bowDissipateAlpha));

        SpritebatchParams flameBowShaderParams = SpritebatchParams.InWorldAndZoomed() with { effect = flamebowShader };
        using (SpritebatchStarter.Begin(spriteBatch, flameBowShaderParams))
        {
            SpritebatchDrawer fingerDrawer = SpritebatchDrawer.FromTextureAsset(_fingerTextureAsset, bowholdPosition);
            fingerDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 18) * _telegraphLineAlpha;
            fingerDrawer.color.A = 0;
            fingerDrawer.rotation = _aimingVelocity.ToRotation();
            if (MyTarget.Center.X < NPC.Center.X)
            {
                fingerDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
                fingerDrawer.rotation += MathHelper.Pi;
            }
          
            fingerDrawer.CenterOrigin();
            spriteBatch.Draw(fingerDrawer);
            spriteBatch.Draw(fingerDrawer);

            //Draw a drop shadow type effect for a little bit of extra bloom
            fingerDrawer.color = Color.DarkRed * ExtraMath.Osc(0.8f, 1f, speed: 12) * _telegraphLineAlpha * 0.35f;
            fingerDrawer.color.A = 0;
            fingerDrawer.scale *= 1.2f;
            fingerDrawer.worldPosition -= _aimingVelocity * 32;
            spriteBatch.Draw(fingerDrawer);

            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, bowholdPosition);
            glowDrawer.color = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 6)) * 0.6f;
            glowDrawer.color.A = 0;
            glowDrawer.rotation = fingerDrawer.rotation;
            glowDrawer.spriteEffects = fingerDrawer.spriteEffects;
            glowDrawer.scale *= 0.8f;
            spriteBatch.Draw(glowDrawer);
        }

        SpritebatchDrawer whiteGlowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, bowholdPosition);
        whiteGlowDrawer.color = Color.White;
        whiteGlowDrawer.color.A = 0;
        whiteGlowDrawer.rotation = _aimingVelocity.ToRotation();
        whiteGlowDrawer.scale *= 0.12f;
        whiteGlowDrawer.scale *= new Vector2(1.75f, 1f);
        whiteGlowDrawer.worldPosition += _aimingVelocity * 80;

        float dir = 1f;
        if (MyTarget.Center.X < NPC.Center.X)
        {
            dir = -1f;
        }
        whiteGlowDrawer.worldPosition += _aimingVelocity.RotatedBy(-MathHelper.PiOver2 * dir) * 20;
        spriteBatch.Draw(whiteGlowDrawer);
    }

    private void DrawWings(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> wings = GetWingsTextureAsset();
        Rectangle srcRec = wings.Value.GetFrame(_wingAnimationFrame.frame, _wingAnimationFrame.maxFrame);
        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(wings, NPC.Center);
        wingDrawer.sourceRect = srcRec;
        wingDrawer.scale *= 2;
        wingDrawer.CenterOrigin();
        wingDrawer.rotation = NPC.rotation;
        spriteBatch.Draw(wingDrawer);
    }
    private void DrawAura(SpriteBatch sb, Vector2 sp)
    {
        float fade = 1f;
        float inScale = EasingFunction.OutExpo(Timer / 30f);
        Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
        WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
        waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f + NPC.whoAmI;
        waveShader.Amplitude = 0.3f;
        waveShader.Frequency = 8;
        waveShader.XStrength = 6;
        waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        sb.Restart(effect: waveShader.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, NPC.Center);
        drawer.rotation = NPC.rotation;
        drawer.BottomCenterOrigin();
        drawer.color = Color.OrangeRed * fade * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: NPC.whoAmI);
        drawer.color.A = 0;
        drawer.scale *= 0.5f * inScale;
        drawer.scale.Y *= ExtraMath.Osc(1f, 1.1f, offset: NPC.whoAmI);
        sb.Draw(drawer);

        drawer.TopCenterOrigin();
        drawer.scale.Y *= 0.4f;
        drawer.spriteEffects |= SpriteEffects.FlipVertically;
        drawer.rotation = NPC.rotation;
        sb.Draw(drawer);

        sb.RestartDefaults();

        Asset<Texture2D> bloomLine = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bloomLine, NPC.Center + new Vector2(0f, 12));
        //      drawer2.BottomCenterOrigin();
        drawer2.scale *= new Vector2(0.55f, 0.55f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * inScale;
        drawer2.color = Color.Yellow * fade * 0.5f; ;
        drawer2.color.A = 0;
        drawer2.rotation = NPC.rotation;
        sb.Draw(drawer2);

        drawer2.scale *= 2;
        drawer2.color = Color.Red * fade * 0.5f; ;
        drawer2.color.A = 0;
        sb.Draw(drawer2);

        drawer2.scale *= 2;
        drawer2.color = Color.Red * fade * 0.15f; ;
        drawer2.color.A = 0;
        sb.Draw(drawer2);

        SpritebatchDrawer blastPillar = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, NPC.Center + new Vector2(0f, 12));
        blastPillar.BottomCenterOrigin();
        blastPillar.color = Color.Red * 0.5f * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: NPC.whoAmI) * fade;
        blastPillar.color.A = 0;
        blastPillar.scale *= 0.6f;
        blastPillar.rotation = NPC.rotation;
        sb.Draw(blastPillar);

        /*
        SpritebatchDrawer auraDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        auraDrawer.color = Color.Lerp(Color.OrangeRed, Color.Red, 
            ExtraMath.Osc(0f, 1f, speed: 6)) * 0.4f;
        auraDrawer.color.A = 0;
        auraDrawer.scale *= 0.8f;
        spriteBatch.Draw(auraDrawer);
        */
    }

    private void DrawSprite(SpriteBatch spriteBatch)
    {

        string texture = Texture + "_" + Animator.GetAnimation();
        Asset<Texture2D> textureAsset = ModContent.Request<Texture2D>(texture);
        SpritebatchDrawer npcDrawer = SpritebatchDrawer.FromNPC(NPC);
        npcDrawer.texture = textureAsset.Value;
        npcDrawer.worldPosition.Y += ExtraMath.Osc(-4f, 4f, speed: 2);
        if (npcDrawer.spriteEffects == SpriteEffects.FlipHorizontally)
            npcDrawer.drawOrigin.X = npcDrawer.sourceRect!.Value.Width - npcDrawer.drawOrigin.X;
        SpritebatchDrawer realDrawer = npcDrawer;
        if (_afterImageAlpha > 0.05f)
        {
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                npcDrawer.color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)NPC.oldPos.Length) * _afterImageAlpha * 0.5f;
                npcDrawer.worldPosition = pos;
                spriteBatch.Draw(npcDrawer);
            }
        }

        spriteBatch.Draw(realDrawer);
    }
    private void DrawOutline(SpriteBatch spriteBatch)
    {
        string texture = Texture + "_" + Animator.GetAnimation();
        Asset<Texture2D> textureAsset = ModContent.Request<Texture2D>(texture);
        SpritebatchDrawer npcDrawer = SpritebatchDrawer.FromNPC(NPC);
        npcDrawer.texture = textureAsset.Value;
        npcDrawer.color = _outliner.outlineColor;
        if (npcDrawer.spriteEffects == SpriteEffects.FlipHorizontally)
            npcDrawer.drawOrigin.X = npcDrawer.sourceRect!.Value.Width - npcDrawer.drawOrigin.X;
        spriteBatch.Draw(npcDrawer);

        Asset<Texture2D> wings = GetWingsTextureAsset();
        Rectangle srcRec = wings.Value.GetFrame(_wingAnimationFrame.frame, _wingAnimationFrame.maxFrame);
        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(wings, NPC.Center);
        wingDrawer.sourceRect = srcRec;
        wingDrawer.scale *= 2;
        wingDrawer.color = _outliner.outlineColor;
        wingDrawer.CenterOrigin();
        wingDrawer.rotation = NPC.rotation;
        spriteBatch.Draw(wingDrawer);
    }

    public void DrawToRenderTargets()
    {
        //PixelationManager.QueuePrimitivesDrawAction(DrawFaintFlamingTrail, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTorchWings, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawAura, DrawLayer.BehindNPCsWithOutline);
        OutlineRenderer.Queue(DrawOutline);
        if (_figure8TrailAlpha < 0.05f)
            return;

        PixelationManager.QueuePrimitivesDrawAction(DrawAuraTrail, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(DrawFlamingFigure8Trail, DrawLayer.BehindNPCsWithOutline);
    }
}
