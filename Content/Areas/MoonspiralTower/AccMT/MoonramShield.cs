using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.MoonspiralTower.AccMT;

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
        Projectile.width = 128;
        Projectile.height = 128;
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

        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MoonramMoon : ModProjectile
{
    private float _flashAlpha;
    private Vector2 _targetScale;
    private Asset<Texture2D>? _shadowMoonTextureAsset;
    private Asset<Texture2D>? _outlineMoonTextureAsset;
    private Asset<Texture2D>? _scrollingMoonTextureAsset;
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
        Projectile.timeLeft = 100;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        _targetScale = Vector2.Lerp(_targetScale, Vector2.One, 0.15f);
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
        moonSprite.color = Color.Lerp(Color.White, Color.Black, 0.18f);
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


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
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
        outlineDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 16);
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


        Rectangle playerRectangle = Player.getRect();
        int type = ModContent.ProjectileType<MoonramMoon>();
        int damage = Player.HeldItem.damage;
        foreach (var npc in Main.ActiveNPCs)
        {
            Rectangle npcRectangle = npc.getRect();
            if (!playerRectangle.Intersects(npcRectangle))
                continue;
            if (dashPlayer.DashedThroughSet.Contains(npc))
                continue;

            dashPlayer.DashedThroughSet.Add(npc);
            //Spawn falling projectile
            Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Top - new Vector2(0, 250),
                Vector2.UnitY, type, damage * 3, 1, Player.whoAmI);
        }
    }
}