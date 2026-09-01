using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;


namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;

public class MoonramDashProjectile : ModProjectile,
    IDrawToRenderTarget
{
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 64;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.Center = Owner.Center;

        if (Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var dp = DustParticle.Spawn(pos, Vector2.Zero, DustParticleSpawnParams.Default);
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.outerColor = Color.Blue;
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var dp = SparkleParticle.Spawn(pos, Vector2.Zero);
            dp.Scale *= 1f;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.outerColor = Color.Blue;
            dp.flickering = true;
            dp.fast = true;
        }
        if (Timer % 7 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(96, 144);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer % 2 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = -Owner.velocity * 0.3f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Blue;
            fx.VectorScale *= 0.5f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    //    return base.PreDraw(ref lightColor);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 18, ratio);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.5f;
    }

    private Color GetTrailColor(float ratio)
    {
        float fade = EasingFunction.InOutSine((float)Projectile.timeLeft / 60f);
        return Color.Lerp(Color.White, Color.SkyBlue, ratio) * fade;
    }

    private void RenderTrail(GraphicsDevice graphicDevice)
    {
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.White;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, offset: Projectile.Size * 0.5f);


        var shader2 = ShaderContent.GetInstance<BasicLaserShader>();
        shader2.SetDefaults();
        shader2.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth2, shader2, offset: Projectile.Size * 0.5f);
    }

    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 96, completionRatio) * EasingFunction.QuadraticBump(completionRatio) * 0.5f;
    }
    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }
    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * EasingFunction.QuadraticBump((float)Projectile.timeLeft / 60f);
    }

    private void DrawSpiralDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
        bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
        bloomShader.InnerColor = Color.SkyBlue;
        bloomShader.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = Color.SkyBlue;
        basicLaserShader.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);


        basicLaserShader.InnerColor = Color.White;
        basicLaserShader.OuterColor = Color.DarkGray;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, basicLaserShader, Projectile.Size * 0.5f);
    }


    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSpiralDashTrail, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(RenderTrail);
    }
}

public class MoonramShield : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<MoonramShieldHeld>());
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MoonramPlayer>().hasMoonramShield = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankCard>();
    }
}

public class MoonramShieldHeld : AbstractShieldProjectile
{
    
    public override void OnBlockMovement(NPC npc)
    {
        base.OnBlockMovement(npc);
        Owner.GetModPlayer<MoonramPlayer>().Ram(npc);
      //  npc.AddBuff(ModContent.BuffType<GhastlyWeakness>(), 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
    }
}

public class MoonramBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 384;
        Projectile.height = 384;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3") with { PitchVariance = 0.5f, Volume = 0.5f };
            SoundEngine.PlaySound(explosionSound, Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.White, 15, 256);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var sp = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                sp.dampening = 0.09f;
                sp.fadeToColor = Color.Black * 0.5f;
                sp.initialColor = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 32, offset: i), Color.Purple, Color.LightBlue, Color.Pink, Color.LightSkyBlue);
                sp.Scale *= 3f;
            }
            for (int i = 0; i < 14; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(pos, vel);
                sp.dampening = 0.09f;
                sp.fadeToColor = Color.Black * 0.5f;

                sp.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 32, offset: i), Color.Purple, Color.LightBlue, Color.Pink, Color.LightSkyBlue);
                sp.color = Color.Lerp(sp.color, Color.Black, 0.6f);
                sp.color *= 0.5f;

                sp.Scale *= 0.9f;
                sp.behindLayer = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                Vector2 vel = Main.rand.NextVector2Circular(32, 32);
                var dp = SparkleParticle.Spawn(pos, vel);
                dp.dampening = 0.1f;
                dp.innerColor = Color.White;
                dp.fast = true;
                dp.gravity = 0;

            }
            ShakeScreenPosition.Shake = 8;
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightSkyBlue, Color.Purple, duration: 12, baseSize: 0.24f);
            fx.Scale *= 2;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { PitchVariance = 0.6f }, Projectile.position);
        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MoonramMoon : ModProjectile
{
    private float Time => 60;
    private Vector2 _targetScale;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    private Asset<Texture2D> _outlineMoonTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle inSound = new SoundStyle("Stellamod/Assets/Sounds/Starrer") with { Pitch = -0.5f , Volume = 0.15f, PitchVariance = 0.3f };
            SoundEngine.PlaySound(inSound, Projectile.position);
        }
        if(Timer < 30 && Timer % 5 == 0)
        {
            PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White, Color.Transparent, 45, 256);
        }

        Projectile.velocity *= 0.999f;

   //     Projectile.velocity.Y += 0.05f;
        if(Timer >= Time - 20)
        {
            Projectile.Kill();
        }
        _targetScale = Vector2.Lerp(Vector2.Zero, Vector2.One * 0.6f + Vector2.Lerp(Vector2.Zero, Vector2.One * 0.4f, EasingFunction.InOutExpo((Timer - (Time / 2))/ 20f)), EasingFunction.InOutExpo(Timer / Time));
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
        glowDrawer.scale *= _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);

        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.SkyBlue, Color.Black, 0.48f);
        moonSprite.scale *= _targetScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


        Player player = Main.LocalPlayer;
        Point tile = player.Center.ToTileCoordinates();
        tile.Y -= 8;
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 worldPosition = tile.ToWorldCoordinates();


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _targetScale * 1.5f;
        Main.spriteBatch.Draw(glowDrawer);


    //    moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        _outlineMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");


        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.58f;
        shadowDrawer.scale *=  _targetScale;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Green * ExtraMath.Osc(0.6f, 1f, speed: 16);
        outlineDrawer.scale *= _targetScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<MoonramBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
public class MoonramPlayer : ModPlayer
{
    public bool hasMoonramShield;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasMoonramShield = false;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (!hasMoonramShield)
            return;
        DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();
        dashPlayer.DashVelocity += 7;
        dashPlayer.DashDuration += 6;
        if (Player.whoAmI != Main.myPlayer)
            return;

        if (!dashPlayer.IsDashing)
            return;

        foreach (var npc in Main.ActiveNPCs)
        {
            Vector2 pos = (npc.Center);
            float dist = Vector2.Distance(Player.Center, pos);
            if(dist < 100)
            {
                Ram(npc);
            }
        }
    }

    public void Ram(NPC npc)
    {
        DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();

        Rectangle playerRectangle = Player.getRect();
        Rectangle npcRectangle = npc.getRect();

        int type = ModContent.ProjectileType<MoonramMoon>();
        int damage = Player.HeldItem.damage;

        if (!dashPlayer.IsDashing)
            return;

        if (dashPlayer.DashedThroughSet.Contains(npc))
            return;

        PixelPrimitiveCircleFactory.CreateGenericInBoom(npc.Center, Color.SkyBlue, Color.Transparent, 25, 128);
        dashPlayer.DashedThroughSet.Add(npc);
        //Spawn falling projectile
        Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Top - new Vector2(0, 64),
            Vector2.UnitY, type, damage * 3, 1, Player.whoAmI);
    }
}