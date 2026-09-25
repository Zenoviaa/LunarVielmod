using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class ThrowingMoon : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Throw => ref Projectile.ai[1];
    private float _growthCount;
    private float _shootFlash;
    private float _scale;
    private float _flashAlpha;
    private Player Owner => Main.player[Projectile.owner];
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    private Asset<Texture2D> _magicCircleTextureAsset;
    public override string Texture => AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.VerliaDesperationMoon.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_growthCount);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _growthCount = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _flashAlpha = 1f;
        Projectile.width = 192;
        Projectile.height = 192;
        Projectile.friendly = false;
        Projectile.timeLeft = 2000;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();

        if(Throw == 0 && !Owner.CheckMana(1, pay: false))
        {
            Throw = 1;
        }
        if ( Throw == 0)
        {
            if(_growthCount < 4)
            {
                if (Timer % 2 == 0)
                    Owner.CheckMana(1, true);
            }
   

            if(this.OwnedByLocalClient() && Owner.channel)
            {
                Vector2 posToMoveTo = Owner.Center - new Vector2(0, 64 * _growthCount + 64);
                Vector2 targetVelocity = posToMoveTo - Projectile.Center;
                Projectile.velocity = targetVelocity * 0.2f;
                Projectile.netUpdate = true;
            }
            if(this.OwnedByLocalClient() && !Owner.channel)
            {
                Throw = 1;
                Vector2 posToMoveTo =Main.MouseWorld;
                Vector2 targetVelocity = posToMoveTo - Projectile.Center;
                targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                targetVelocity *= 12;
                targetVelocity.Y -= 12;
                Projectile.velocity = targetVelocity ;
                Projectile.netUpdate = true;
            }

            Timer++;
            if (Timer == 1)
            {
                if (_growthCount == 0)
                {
                    SoundStyle e = new SoundStyle($"Stellamod/Assets/Sounds/StarCharge");
                    SoundEngine.PlaySound(e, Projectile.position);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.TintScreen(Color.LightBlue, 0.2f, 15);
                    PixelPrimitiveCircleFactory.CreateVerliaMoonBoom(Projectile.Center);
                }

                SoundStyle inSound = AssetRegistry.Sounds.Verlia.BigMoonGrow;
                inSound.Pitch = MathHelper.Lerp(0f, 1f, (_growthCount + 1f) / 3f);
                SoundEngine.PlaySound(inSound);
                _flashAlpha = 1f;
            }

            if(_growthCount < 4)
            {
                if (Timer % 2 == 0)
                {
                    float range = Main.rand.NextFloat(252, 512);
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                    Vector2 vel = (Projectile.Center - pos);
                    vel *= 0.1f;
                    FXUtil.GlowStretch(pos, vel);
                }

                if (Timer % 2 == 0)
                {
                    float range = Main.rand.NextFloat(384, 666);
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                    Vector2 vel = (Projectile.Center - pos);
                    vel *= 0.1f;
                    var fx = FXUtil.GlowStretch(pos, vel);
                    fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
                    fx.VectorScale *= 0.5f;
                }
            }
   
            float maxScale = MathHelper.Lerp(0f, 1f, (_growthCount + 1) / 3f);
            _scale = MathHelper.Lerp(_scale, maxScale, 0.1f);
            _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
            if (Timer >= 90 && _growthCount < 3)
            {
                Timer = 0;
                _growthCount++;
            }
            if(Timer >= 180 && _growthCount == 3)
            {
                Timer = 0;
                _growthCount++;
            }
            if (_growthCount >= 3)
            {
                ShakeScreenPosition.Shake = 3;
            }

            var rot = (Projectile.Center - Owner.Center).ToRotation();
            rot += ExtraMath.Osc(-0.05f, 0.05f);
            rot -= MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot);
            return;
        }

        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity -= Vector2.UnitY * 12;
        }

        if(Timer == 15)
        {
            SoundEngine.PlaySound(AssetRegistry.Sounds.Bishinine.BishinineFastfall, Projectile.position);
        }

        if (Timer % 8 == 0)
        {
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
            p2.Scale *= 3f * _scale;
        }
        if (Projectile.velocity.Y < 1)
            Projectile.velocity.Y += 0.5f;
        else
        {
            Projectile.velocity.Y *= 1.05f;
            Projectile.tileCollide = true;
        }

        _scale = MathHelper.Lerp(_scale, ((_growthCount+1) / 3f) * 1f, 0.1f);
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
        _shootFlash = MathHelper.Lerp(_shootFlash, 0f, 0.1f);
    }
    private void PrepareAssets()
    {
        _outlineTextureAsset ??= AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.VerliaDesperationMoon_Outline.Asset;//ModContent.Request<Texture2D>(Texture + "_Outline");
        _scrollingMoonTextureAsset ??= AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.VerliaDesperationMoon_ScrollingMoon.Asset;
        _shadowMoonTextureAsset ??= AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.VerliaDesperationMoon_Shadow.Asset;
        _magicCircleTextureAsset ??= AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.VerliaDesperationMoon_Sigil.Asset;
    }

    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        PrepareAssets();
        Vector2 scale = Vector2.One * _scale;

        var circleSprite = SpritebatchDrawer.FromTextureAsset(_magicCircleTextureAsset, Projectile.Center);
        circleSprite.color = Color.Lerp(Color.Black, Color.White, _flashAlpha + _shootFlash);// * ExtraMath.Osc(0.5f, 1f, speed: 6);
        circleSprite.color.A = 0;
        circleSprite.rotation = Main.GlobalTimeWrappedHourly;
        circleSprite.scale *= 1.2f;
        sb.Draw(circleSprite);

        var moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Blue, Color.White, _flashAlpha + _shootFlash) * 0.8f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.8f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);



        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.White * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * 1;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.White; // Color.Lerp(Color.White, Color.DarkBlue, 0.5f);
        moonSprite.scale *= scale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.White * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= scale * 3f;
        Main.spriteBatch.Draw(glowDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrepareAssets();
        Vector2 scale = Vector2.One * _scale;
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);

        Color flashColor = Color.White;
        Color darkColor = Color.Lerp(Color.Blue, Color.Black, 0.8f) * 0.5f;
        shadowDrawer.color = Color.Lerp(darkColor, flashColor, _flashAlpha + _shootFlash);
        shadowDrawer.scale *= scale * 1.05f;
        Main.spriteBatch.Draw(shadowDrawer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= scale;

        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        moonSprite.scale = scale * 1.05f;
        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha + _shootFlash);
        Main.spriteBatch.Draw(moonSprite);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        Point tile = Projectile.Center.ToTileCoordinates();
        tile.Y -= 6;
        tile = TileUtilities.FallToSolidTile(tile);
        tile.Y -= 1;
        Vector2 pos = tile.ToWorldCoordinates();
        if (this.OwnedByLocalClient())
        {
            var ratio = _growthCount / 4f;
            var originalRatio = ratio;
            ratio = MathHelper.Lerp(-0.8f, 1f, ratio);
            var style = 0;
            if (ratio > 0)
                style = 1;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwaveFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: style, ai2: ratio);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoomFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: originalRatio);
        }

        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 8f;
        float numDust = 32;
        for (float f = 0; f < numDust; f++)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            spawnParams.scaleRange *= 2;
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16), spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
        FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 32, 2, 32);
    }
}

public class ThrowingMoonArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.shoot = ModContent.ProjectileType<ThrowingMoon>();
        Item.mana = 50;
        Item.damage = 200;
        Item.knockBack = 1;
        Item.useTime = Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.channel = true;
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        velocity = Vector2.UnitY * 0.2f;
        position = player.Center + new Vector2(0, -64);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
}



