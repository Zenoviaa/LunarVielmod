using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;

public class VerliaGreatBlade : ModProjectile
{
    private enum SwingState
    {
        Charge,
        Swing,
        Out
    }

    private float _growthScale;
    private float _stretchAlpha;
    private float _inScale;
    private int _growthIndex;
    private float _flashAlpha;
    private float _swingTrailAlpha;
    private Asset<Texture2D> _smallBladeTextureAsset;
    private Asset<Texture2D> _bigBladeTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private SwingState State
    {
        get => (SwingState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.
        float length = 512 * 1.5f;
        float rotation = Projectile.rotation;
        //  rotation -= MathHelper.PiOver4;
        Vector2 start = Projectile.Center;
        Vector2 end = Projectile.Center + rotation.ToRotationVector2() * length;
        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 16, ref collisionPoint);
        return check;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _stretchAlpha = 1f;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 6000;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 8;
        Projectile.hostile = true;
    }
    private float Fixer => Projectile.extraUpdates + 1;
    private float SwingTime => 60f * Fixer;
    private float ChargeTime => 240 * Fixer;
    private float OutTime => 30f * Fixer;
    private Vector2 AimingDirection => Projectile.velocity.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
    public override void AI()
    {
        base.AI();

        switch (State)
        {
            case SwingState.Charge:
                AI_Charge();
                break;
            case SwingState.Swing:
                AI_Swing();
                break;
            case SwingState.Out:
                AI_Out();
                break;
        }
        float targetScale = 0f;
        switch (_growthIndex)
        {
            case 0:
                targetScale = 0.3f;
                break;
            case 1:
                targetScale = 0.5f;
                break;
            case 2:
                targetScale = 0.75f;
                break;
            case 3:
                targetScale = 1f;
                break;
        }
        _growthScale = MathHelper.Lerp(_growthScale, targetScale, 0.1f);
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 1f, 0.1f);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void Grow()
    {
        if (_growthIndex >= 3)
            return;

        _flashAlpha = 0;
        _growthIndex++;
    }
    private void SwitchState(SwingState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void AI_Charge()
    {
        Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
        {

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"), Projectile.position);
            // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StarCharge"), Projectile.position);
        }
        int growth1 = (int)ChargeTime / 4;
        int growth2 = growth1 * 2;
        int growth3 = growth1 * 3;
        if (Timer == growth1)
        {
            SoundStyle growSound1 = AssetRegistry.Sounds.Verlia.SwordGrowSmall;
            growSound1.Pitch = -0.5f;
            SoundEngine.PlaySound(growSound1, Projectile.position);
            Grow();
        }
        if (Timer == growth2)
        {
            SoundStyle growSound1 = AssetRegistry.Sounds.Verlia.SwordGrowSmall;
            SoundEngine.PlaySound(growSound1, Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), Projectile.position);
            Grow();
        }
        if (Timer == growth3)
        {
            SoundStyle growSound1 = AssetRegistry.Sounds.Verlia.SwordGrowBigga;
            SoundEngine.PlaySound(growSound1, Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), Projectile.position);
            Grow();
        }



        float rotation = AimingDirection.ToRotation();
        float ratio = Timer / ChargeTime;

        if (Timer > growth1)
        {

            Smoke();
            ShakeModSystem.Shake = 2;
            _stretchAlpha = MathHelper.Lerp(1f, 1.05f, EasingFunction.QuadraticBump((Timer - growth1) / (ChargeTime / 2)));
        }
        else
        {
            ShakeModSystem.Shake = 1;
        }


        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutCirc(ratio));
        float ease = EasingFunction.InOutCirc(ratio);
        float startRotation = -Vector2.UnitY.ToRotation();
        float endRotation = rotation - MathHelper.ToRadians(208 * AimingDirection.X);
        float interpolatedRotation = Utils.AngleLerp(startRotation, endRotation, ease);
        Projectile.rotation = interpolatedRotation;
        Projectile.Center = Parent.Center;

        if (Timer >= ChargeTime + 30)
        {
            SwitchState(SwingState.Swing);
        }
    }

    private void Smoke()
    {
        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, Main.rand.NextFloat(0f, 1f));
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
    }
    private void AI_Swing()
    {
        _swingTrailAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / SwingTime));
        _stretchAlpha = 1f;
        Projectile.hostile = true;
        _inScale = MathHelper.Lerp(_inScale, 1f, 0.1f);
        Timer++;


        if (Timer == 120)
        {
            SoundStyle growSound1 = AssetRegistry.Sounds.Verlia.BigSwordSwing;
            SoundEngine.PlaySound(growSound1);
        }
        if (Timer % 8 == 0 && Timer > 120)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = SparkleParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            //sp.flickering = true;
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
            //  sp.Scale *= 2;
            sp.outerColor = Color.Blue;
        }
        Smoke();


        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * 444f;
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, Main.rand.NextFloat(0f, 1f));
            sp.color = Color.Lerp(sp.color, Color.Black, 0.5f);
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
        float ratio = Timer / SwingTime;
        float ease = EasingFunction.InOutExpo(ratio);
        float midRotation = AimingDirection.ToRotation();
        float startRotation = midRotation - MathHelper.ToRadians(208 * AimingDirection.X);
        float endRotation = midRotation + MathHelper.ToRadians(160 * AimingDirection.X);
        float interpolatedRotation = MathHelper.Lerp(startRotation, endRotation, ease);
        Projectile.rotation = interpolatedRotation;
        Projectile.Center = Parent.Center;
        if (Timer >= SwingTime - 260)
        {
            SwitchState(SwingState.Out);
        }
    }

    private void AI_Out()
    {
        Projectile.hostile = false;
        Projectile.Kill();
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Black, ratio);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(192, 0, ratio) * _swingTrailAlpha;
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.05f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 3;
    }
    private float GetTrailWidth4(float ratio)
    {
        return GetTrailWidth3(ratio) * 1.05f;
    }
    private void DrawTrails(GraphicsDevice gDevice)
    {
        Vector2[] swingPos = new Vector2[Projectile.oldRot.Length];
        float radians = MathHelper.PiOver4 * 0.5f;
        for (int i = 0; i < swingPos.Length; i++)
        {
            swingPos[i] = (Projectile.oldRot[i]).ToRotationVector2() * 484 * 2f + Projectile.Center;
        }

        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth, laserShader);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth2, b);


        swingPos = new Vector2[Projectile.oldRot.Length];
        for (int i = 0; i < swingPos.Length; i++)
        {
            swingPos[i] = (Projectile.oldRot[i]).ToRotationVector2() * 484 * 1.25f + Projectile.Center;
        }

        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth3, laserShader);
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth4, b);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (State == SwingState.Swing)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        }

        _smallBladeTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_0");
        _bigBladeTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_1");

        SpriteBatch spriteBatch = Main.spriteBatch;
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.WhispyTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 1;
        shader.InnerColor = Color.LightBlue;
        shader.OuterColor = Color.Blue;
        spriteBatch.Restart(effect: shader.Effect);


        Asset<Texture2D> bladeAsset;
        switch (_growthIndex)
        {
            default:
            case 0:
                bladeAsset = _smallBladeTextureAsset;
                break;
            case 1:
                bladeAsset = _smallBladeTextureAsset;
                break;
            case 2:
                bladeAsset = _smallBladeTextureAsset;
                break;
            case 3:
                bladeAsset = _bigBladeTextureAsset;
                break;
        }

        float scale = 1.5f * _growthScale;
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(bladeAsset, Projectile.Center);
        sbDrawer.LeftCenterOrigin();
        sbDrawer.rotation = Projectile.rotation;
        sbDrawer.color.A = 0;
        sbDrawer.scale = Projectile.scale * Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, _flashAlpha) * _inScale;
        sbDrawer.scale.X *= _stretchAlpha;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);
        sbDrawer.scale *= 1.25f;
        sbDrawer.scale.X *= _stretchAlpha;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);
        spriteBatch.RestartDefaults();

        Asset<Texture2D> glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.color = Color.Blue * ExtraMath.Osc(0.5f, 1f, speed: 2);
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Projectile.rotation;
        glowDrawer.scale.X *= 2 * _inScale;
        glowDrawer.scale.Y *= 0.5f;
        glowDrawer.scale *= scale;
        glowDrawer.worldPosition += Projectile.rotation.ToRotationVector2() * 384;
        spriteBatch.Draw(glowDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 32;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = DustParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            //sp.flickering = true;
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
            sp.Scale *= 2;
            sp.outerColor = Color.Blue;
        }
        for (int i = 0; i < Projectile.oldRot.Length; i++)
        {
            if (!Main.rand.NextBool(8))
                continue;
            Vector2 pos = Projectile.oldRot[i].ToRotationVector2() * 484 + Projectile.Center;
            FXUtil.GlowStretch(pos, (pos - Parent.Center).SafeNormalize(Vector2.Zero) * 8);
        }

        if (this.OwnedByLocalClient())
        {
            float d = 600 * 1.5f;
            Vector2 position = Projectile.Center + Projectile.rotation.ToRotationVector2() * d * 0.5f;
            Point tile = position.ToTileCoordinates();
            tile = TileUtilities.FallToSolidTile(tile);
            tile.Y -= 5;
            Vector2 fallPosition = tile.ToWorldCoordinates();
           // fallPosition.Y -= 48;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), fallPosition, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 2);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), fallPosition, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
