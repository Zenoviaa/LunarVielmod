using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class VerliaPaletteRenderer : ModSystem
{
    public static readonly Queue<Action<SpriteBatch>> DrawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += Render;
    }

    private void Render(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if(DrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(ApplyVerlia);
        }
    }

    private void ApplyVerlia(SpriteBatch sb, Vector2 sp)
    {
        sb.EndOut(out var oldParameters);

        using var target = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(target, Color.Transparent))
        {
            using (sb.Ctx(SB.InWorldUnscaled))
            {
                while (DrawQueue.Count > 0)
                {
                    DrawQueue.Dequeue()(sb);
                }
            }
        }

        var palettizerShader = PalettizerShader.Instance;
        palettizerShader.PaletteTexture = PaletteAssets.FromPaletteFile(PaletteAssets.MOONSPIRALTOWER).Value.ColorAtlas;//PaletteHelper.GetColorSpectrum("MoonspiralTower.pal");
        palettizerShader.Progress = 1f;
        palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
        palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
        palettizerShader.DitherAlpha = 0.125f;
        using (sb.Ctx(oldParameters with { effect = palettizerShader }))
        {
            sb.Draw(target, Vector2.Zero, Color.White);
        }
        sb.Begin(oldParameters);    
    }
}
public class DeepGreatBlade : ModProjectile
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
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.
        float length = 212 * 1.5f;
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
        Projectile.timeLeft = 9000;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 8;
        Projectile.friendly = true;
    }
    private float Fixer => Projectile.extraUpdates + 1;
    private float SwingTime => 110 * Fixer;
    private float ChargeTime => 240 * Fixer;
    private Vector2 AimingDirection => Projectile.velocity.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
    public override bool? CanHitNPC(NPC target)
    {
        if (State == SwingState.Charge)
            return false;
        return base.CanHitNPC(target);
    }

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
        var rot = Projectile.rotation;
        rot += ExtraMath.Osc(-0.05f, 0.05f);
        rot -= MathHelper.PiOver2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot);
        Owner.ChangeDir((int)AimingDirection.X);
        float targetScale = 0f;
        switch (_growthIndex)
        {
            case 0:
                targetScale = 0.13f;
                break;
            case 1:
                targetScale = 0.25f;
                break;
            case 2:
                targetScale = 0.35f;
                break;
            case 3:
                targetScale = 0.5f;
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
            _stretchAlpha = MathHelper.Lerp(1f, 1.05f, EasingFunction.QuadraticBump((Timer - growth1) / (ChargeTime / 2)));
        }
        else
        {

        }

        if (!Owner.channel && Timer < growth3)
            Projectile.Kill();

        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutCirc(ratio));
        float ease = EasingFunction.InOutCirc(ratio);
        float startRotation = -Vector2.UnitY.ToRotation();
        float endRotation = rotation - MathHelper.ToRadians(208 * AimingDirection.X);
        float interpolatedRotation = Utils.AngleLerp(startRotation, endRotation, ease);
        Projectile.rotation = interpolatedRotation;
        Projectile.Center = Owner.Center;

        if (Timer >= ChargeTime + 30)
        {
            SwitchState(SwingState.Swing);
        }
    }

    private void Smoke()
    {
        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 144, Main.rand.NextFloat(0f, 1f));
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
            Vector2 offset = Projectile.rotation.ToRotationVector2() * 177;
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
        Projectile.Center = Owner.Center;
        if (Timer >= SwingTime)
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
        return MathHelper.SmoothStep(64, 0, ratio) * _swingTrailAlpha;
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
        for (int i = 0; i < swingPos.Length; i++)
        {
            swingPos[i] = (Projectile.oldRot[i]).ToRotationVector2() * 144 * 2f + Projectile.Center;
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
            swingPos[i] = (Projectile.oldRot[i]).ToRotationVector2() * 144 * 1.25f + Projectile.Center;
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
        glowDrawer.worldPosition += Projectile.rotation.ToRotationVector2() * 144;
        spriteBatch.Draw(glowDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float e = 32;
        for (float d = 0; d < e; d++)
        {
            Vector2 spawnPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(64, 252);
            spawnPos += Main.rand.NextVector2Circular(64, 64);
            Vector2 spawnVelocity = Projectile.rotation.ToRotationVector2();
            spawnVelocity = spawnVelocity.RotatedByRandom(MathHelper.ToRadians(35));
            spawnVelocity *= Main.rand.NextFloat(5f, 14f);
            var sp = DustParticle.Spawn(spawnPos, spawnVelocity);
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.Scale *= 2;
            sp.gravity = 0;

            sp.innerColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
            sp.outerColor = Color.Blue;
        }

    }
}


public class DeepswordArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.shoot = ModContent.ProjectileType<DeepGreatBlade>();
        Item.mana = 50;
        Item.damage = 200;
        Item.knockBack = 1;
        Item.useTime = Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.channel = true;
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.shootSpeed = 5;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

}
