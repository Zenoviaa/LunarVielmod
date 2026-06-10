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

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public partial class Gothivia :
    IDrawToRenderTarget
{
    private Asset<Texture2D> _wings3QTextureAsset;
    private Asset<Texture2D> _wings4QTextureAsset;
    private Asset<Texture2D> _bowTextureAsset;
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


    private const string Anim_Floating = "Floating";
    private const string Anim_Arrowhold = "ArrowHold";
    private const string Anim_Arrowshot = "Arrowshot";
    private const string Anim_Dichotamy = "Dichotamy";
    private const string Anim_Explode = "Explode";
    private const string Anim_Kickstart = "Kickstart";
    private const string Anim_Standalone = "Standalone";
    private const string Anim_Aurafarming = "Aurafarming";
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

    private void DrawTelegraphLine(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> bloomLineTextureAsset = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/BloomLine");
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(bloomLineTextureAsset, NPC.Center);
        lineDrawer.rotation = _aimingVelocity.ToRotation() - MathHelper.PiOver2;
        lineDrawer.color = Color.White * _telegraphLineAlpha * ExtraMath.Osc(0.4f, 1f, speed: 32) ;
        lineDrawer.color.A = 0;
        lineDrawer.TopCenterOrigin();
        lineDrawer.scale.Y *= 4;
        lineDrawer.scale.X *= 0.4f;
        spriteBatch.Draw(lineDrawer);
    }

    private void DrawBow(SpriteBatch spriteBatch)
    {
        FlameBowShader flamebowShader = ShaderContent.GetInstance<FlameBowShader>();
        flamebowShader.Time = Main.GlobalTimeWrappedHourly * -24;
        flamebowShader.FlameNoiseTexture = AssetManager.Noise.InvertedVoronoi;
        flamebowShader.InsideColor = Color.Yellow;
        flamebowShader.BloomColor = Color.Red;
        flamebowShader.DissipateThreshold = MathHelper.Lerp(1f, 0f, _bowDissipateAlpha);

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
    private void DrawWings(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> wings = GetWingsTextureAsset();
        Rectangle srcRec = wings.Value.GetFrame(_wingAnimationFrame.frame, _wingAnimationFrame.maxFrame);
        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(wings, NPC.Center);
        wingDrawer.sourceRect = srcRec;
        wingDrawer.scale *= 2;
        wingDrawer.CenterOrigin();
        spriteBatch.Draw(wingDrawer);
    }

    private void DrawSprite(SpriteBatch spriteBatch)
    {
        string texture = Texture + "_" + Animator.GetAnimation();
        Asset<Texture2D> textureAsset = ModContent.Request<Texture2D>(texture);
        SpritebatchDrawer npcDrawer = SpritebatchDrawer.FromNPC(NPC);
        npcDrawer.texture = textureAsset.Value;
        if (npcDrawer.spriteEffects == SpriteEffects.FlipHorizontally)
            npcDrawer.drawOrigin.X = npcDrawer.sourceRect!.Value.Width - npcDrawer.drawOrigin.X;
        spriteBatch.Draw(npcDrawer);
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
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutline);
    }
}
