using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

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

    private Color _outlineColor;
    private ref float Timer => ref Projectile.ai[0];
    private BounceState State
    {
        get => (BounceState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float Direction => ref Projectile.ai[2];

    private Vector2 _squishScale;
    private Vector2 _scale;
    private Vector2 _targetScale;
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
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        base.AI();
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        if (Timer % 24 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), Vector2.Zero);
            sp.outerColor = Color.LightBlue;
            sp.gravity = 0;
            sp.Scale *= 0.5f;
            sp.behindLayer = true;
        }


        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);

        OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
        _targetScale = Vector2.One;
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
                _targetScale *= 1.25f;
                break;
            case BounceState.Bounce_Out:
                AI_BounceOut();
                _targetScale *= 1.5f;
                break;
        }
        if (Projectile.hostile)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        _scale = Vector2.Lerp(_scale, _targetScale, 0.1f);
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

        float bounceD = 90;
        if (State == BounceState.Bounce_2)
        {
            bounceD = 0;
        }
        
        targetPosition.X += Direction * bounceD;
        Vector2 interpolatedPosition = Vector2.Lerp(_startPosition, targetPosition, ease);
        float ease2 = EasingFunction.QuadraticBump(ratio);
        interpolatedPosition.X += MathHelper.Lerp(0f, distance.X * Direction, ease2);

        Vector2 velocity = interpolatedPosition - Projectile.Center;
        Projectile.velocity = velocity;
        if (Timer >= time)
        {
            BounceEffect();
        }
    }

    private void BounceEffect()
    {
        ShakeScreenPosition.Shake = 2;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        _flashAlpha = 1f;
        _squishScale = new Vector2(0.8f, 1.4f);
        for(int i = 0; i < 3; i++)
        {
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitX * -Direction, Color.LightSkyBlue);
            donut.Scale *= 2;

        }

        SoundStyle bounceSound = AssetRegistry.Sounds.Verlia.MoonBounceOnce;
        if (State == BounceState.Bounce_2)
        {
            bounceSound = AssetRegistry.Sounds.Verlia.MoonBounceTwo;
        }
        SoundEngine.PlaySound(bounceSound, Projectile.position);
    }

    private void AI_Spawn()
    {
        SwitchState(BounceState.Bounce_1);
    }

    private void AI_Bounce1()
    {
        Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
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
        Projectile.hostile = false;
        Timer++;
        Bounce(BounceUp2Time, BounceUp2Distance);
        if (Timer >= BounceUp2Time)
        {
            SwitchState(BounceState.Bounce_Out);
        }
    }

    private void AI_BounceOut()
    {
        Projectile.hostile = true;
        if (Timer < 80)
        {
            Vector2 pos = Projectile.Center;
            pos.Y += 384;
        //    CameraTargetSystem.AddTarget(pos);
        }

        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.X *= -1;
        }

        Player telegraphPlayer = Main.LocalPlayer;
        Point tile = telegraphPlayer.Center.ToTileCoordinates();
        tile.Y -= 8;
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 worldPosition = tile.ToWorldCoordinates();

        if (Timer % 4 == 0)
        {
            Vector2 pos = worldPosition;
            pos.X += Main.rand.NextFloat(-384f, 384f);
            Vector2 vel = -Vector2.UnitY;
            vel *= Main.rand.NextFloat(2f, 15f);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.3f;
        }


        if (Timer == 1)
        {
            Projectile.velocity.Y -= 15;
        }
        Projectile.velocity.X *= 0.98f;

        if (Projectile.velocity.Y > 0)
        {
            if (Timer % 8 == 0)
            {
                var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Bottom, -Projectile.velocity);
                p2.Scale *= 1.5f;
            }
            ShakeScreenPosition.Shake = 2;
            Projectile.velocity.Y *= 1.05f;
        }
        else
        {
            Projectile.velocity.Y += 0.75f;
        }
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
        if (player == null)
            return;

        if (Projectile.Bottom.Y > player.Top.Y)
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
            float ratio = i / (float)Projectile.oldPos.Length;
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
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _squishScale * _targetScale;
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
        moonSprite.color = Color.Lerp(Color.White, Color.Black, 0.18f);
        moonSprite.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();



        Player player = Main.LocalPlayer;
        Point tile = player.Center.ToTileCoordinates();
        tile.Y -= 8;
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 worldPosition = tile.ToWorldCoordinates();

        if (State == BounceState.Bounce_Out)
        {
            glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MuzzleFlash, Projectile.Center);
            glowDrawer.color = Color.Yellow * MathHelper.Lerp(0f, 1f, Timer / 120f) * 0.5f;
            glowDrawer.color.A = 0;
            glowDrawer.scale.Y *= 32f;
            glowDrawer.scale.X *= 0.3f;
            glowDrawer.rotation = MathHelper.PiOver2;
            glowDrawer.worldPosition = worldPosition;
            Main.spriteBatch.Draw(glowDrawer);

        }


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _squishScale * _targetScale * 1.5f;
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
        return MathHelper.SmoothStep(64, 0, ratio) * _scale.X * 1.5f;
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
        shadowDrawer.color *= 0.58f;
        shadowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = _outlineColor;
        outlineDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Point tile = Projectile.Center.ToTileCoordinates();
            tile.Y -= 6;
            tile = TileUtilities.FallToSolidTile(tile);
            tile.Y -= 1;
            Vector2 pos = tile.ToWorldCoordinates();


            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
