using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Rendering.MoonMagic;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class MoonEnchanted : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (Main.rand.NextBool(4))
        {
            Vector2 pos = player.position;
            pos.X += Main.rand.Next(0, player.width);
            pos.Y += Main.rand.Next(0, player.height);
            Vector2 vel = Main.rand.NextVector2Circular(8, 8);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Blue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }
    }
}

public class MoonEnchantedGlobalItem : GlobalItem
{
    private Projectile FindClosestAura(Player player)
    {
        int type = ModContent.ProjectileType<MoonEnchantAura>();
        foreach(var proj in Main.ActiveProjectiles)
        {
            if (proj.type == type)
                return proj;
        }
        return null;
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.HasBuff<MoonEnchanted>())
        {
            Projectile aura = FindClosestAura(player);
            if(aura != null)
            {
                ProjFirer firer = ProjFirer.From<MoonEnchantedShot>(player);
                float dmg = (float)damage;
                dmg *= 0.66f;
                firer.damage = (int)dmg;
                firer.knockback = knockback;
                firer.position = player.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 shootVelocity = (Main.MouseWorld - firer.position).SafeNormalize(Vector2.Zero);
                shootVelocity *= 12;
                firer.position += shootVelocity;

                firer.velocity = shootVelocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.4f, 0.8f);
                firer.New();
            }
        }
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}

public class MoonEnchantedShot : ModProjectile,
    IDrawToRenderTarget
{
    private float _inScale;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.ignoreWater = true;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 480;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        //   ProjectileID.Sets.TrailCacheLength[Type] = 128;
        Timer++;
        if (Timer == 1)
        {

            float numDust = 6;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
                spawnparams.outerColor = Color.Blue;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnparams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.fast = true;
                d.noTileCollide = true;
                d.innerColor = Color.SkyBlue;
            }

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { PitchVariance = 0.6f, Volume = 0.4f }, Projectile.position);
        }

        if (Timer % 18 == 0)
        {
            var sp = CrescentMoonParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.Scale *= 0.3f;
            sp.gravity = 0;
        }


        if (Timer % 14 == 0)
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.125f, 0.25f);
            sp.behindLayer = true;
            sp.noShrink = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
        }

        float targetScale = 1f * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        _inScale = MathHelper.Lerp(_inScale, targetScale, 0.1f);

        NPC npc = NPCHelper.FindClosestNPC(Projectile.Center, 512);
        if(npc != null)
        {
            Vector2 vel = ProjectileHelper.SimpleHomingVelocity(Projectile, npc.Center, degreesToRotate: 2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 0.2f);
        }
    }

    private float GetTrailWidth2(float ratio)
    {
        return MathHelper.SmoothStep(13, 0, ratio) * _inScale * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(13, 0, ratio) * _inScale;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Blue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float globalScale = 0.4f * _inScale * 0.4f;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        flareDrawer.color = Color.Blue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        flareDrawer.color = Color.LightSkyBlue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale * 0.8f;
        flareDrawer.rotation = -Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.White;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        MoonshotTrailRenderer renderer = ModContent.GetInstance<MoonshotTrailRenderer>();
        renderer.PrepareForRendering(DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetTrailColor, GetTrailWidth, Projectile.Size * 0.5f));
        renderer.PrepareForBigRendering(DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetTrailColor, GetTrailWidth2, Projectile.Size * 0.5f));
    }
}

public class MoonEnchantAura : ModProjectile
{
    private float InScale
    {
        get
        {
            return EasingFunction.OutExpo(Timer / 80f);
        }
    }
    private float Fade
    {
        get
        {
            float inAlpha = EasingFunction.InOutSine(Timer / 30f);
            float outAlpha = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
            return inAlpha * outAlpha * 0.7f;
        }
    }

    private float EnchantDistanceSquared => 252 * 252;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.timeLeft = 420;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var supernovaSitar = AssetReferences.Assets.Sounds.Verlia.SupernovaSitarCast.Asset with 
            { PitchVariance = 0.2f };
            SoundEngine.PlaySound(supernovaSitar, Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Blue, 30, 252);
        }
        if(Projectile.timeLeft == 30)
        {
            var magicDowner = AssetReferences.Assets.Sounds.Verlia.MagicDowner.Asset with
            { PitchVariance = 0.6f };
            SoundEngine.PlaySound(magicDowner, Projectile.position);
        }
        if(Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
            var sp = SparkleParticle.Spawn(pos, Main.rand.NextVector2Circular(2, 2));
            sp.dampening = 0.15f;
            sp.gravity = 0;
            sp.innerColor = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.DarkBlue, Color.SkyBlue, Color.White);
            sp.outerColor = Color.Lerp(sp.innerColor, Color.Black, 0.5f);
            sp.Scale = Main.rand.NextFloat(0.45f, 0.8f) * 0.5f;
        }

        foreach(var player in Main.ActivePlayers)
        {
            float sqrtDistance = Vector2.DistanceSquared(player.Center, Projectile.Center);
            if(sqrtDistance <= EnchantDistanceSquared)
            {
                player.AddBuff(ModContent.BuffType<MoonEnchanted>(), 120);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var pass = AssetReferences.Effects.Abyss.MoonEnchantedAura.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        using (new SpritebatchContext(Main.spriteBatch, Main.spriteBatch.Parameters with { effect = pass.Shader, samplerState = SamplerState.LinearWrap }))
        {
            float alpha = 0.3f;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.WaterTrail.Asset, Projectile.Center);
            drawer.color = Color.SkyBlue * Fade * alpha;
            drawer.rotation = Main.GlobalTimeWrappedHourly * 0.3f;
        //    drawer.color.A = 0;
            drawer.scale *= InScale * 2;
            Main.spriteBatch.Draw(drawer);

            drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
            drawer.color = Color.DarkBlue * Fade  * alpha;
            drawer.color.A = 0;

            Main.spriteBatch.Draw(drawer);

            drawer.color = Color.SkyBlue * Fade * alpha;
            drawer.color.A = 0;
          

            Main.spriteBatch.Draw(drawer);
            Main.spriteBatch.Draw(drawer);
            Main.spriteBatch.Draw(drawer);

        }

        PixelationManager.QueueSpritebatchDrawAction(DrawSigilOverEverybody, DrawLayer.OverPlayers);

        return false;
    }

    private void DrawSigilOverEverybody(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer sigil = SpritebatchDrawer.FromProjectile(Projectile);
        sigil.color = Color.SkyBlue;
        sigil.color *= Fade;
        sigil.color *= ExtraMath.Osc(0.7f, 1f);
        sigil.color.A = 0;
        sigil.scale *= InScale;
        spriteBatch.Draw(sigil );
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class SupernovaSitar : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 24;
        Item.mana = 125;
        Item.useStyle = ItemUseStyleID.Guitar;
        Item.useTime = Item.useAnimation = 24;
        Item.UseSound = null;
        Item.shoot = ModContent.ProjectileType<MoonEnchantAura>();
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        position = Main.MouseWorld;
        velocity = Vector2.Zero;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankStaff>();
    }
}
