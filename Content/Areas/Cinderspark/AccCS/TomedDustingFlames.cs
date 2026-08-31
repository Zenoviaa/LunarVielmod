using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.Generic;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using System.Collections.Generic;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class TomedDustingFlames : AbstractIgniterAddon
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<IgniterPlayer>().addons.Add(this);
    }
    public override void OnExplode(IgniterBoom cardProj)
    {
        base.OnExplode(cardProj);
        if (Main.rand.NextBool(10))
        {
            var proj = cardProj.Projectile;
            Projectile.NewProjectile(proj.GetSource_FromThis(), proj.Center, Vector2.Zero, ModContent.ProjectileType<DustingFlameBoom>(),
                proj.damage, proj.knockBack, proj.owner);
        }

    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankAccessory>();
    }
}

public class DustingFlameBoom : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 60;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.timeLeft = (int)Time;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 60);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var smokeParitcle = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                smokeParitcle.dampening = 0.09f;
                smokeParitcle.fadeToColor = Color.Black * 0.5f;
                smokeParitcle.initialColor = Color.DarkRed * 0.5f;
                smokeParitcle.Scale *= 2f;
                smokeParitcle.behindLayer = true;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var dp = DustParticle.Spawn(pos, vel);
                dp.dampening = 0.05f;
                dp.innerColor = Color.OrangeRed;
                dp.fast = true;
            }

            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red, duration: 12, baseSize: 0.24f);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, Projectile.position);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    private void DrawPixelatedFlameBoom(SpriteBatch sb, Vector2 sp)
    {
        NoisyBoomShader boomShader = ShaderContent.GetInstance<NoisyBoomShader>();
        boomShader.Time = Main.GlobalTimeWrappedHourly * 8;
        boomShader.NoiseColor = Color.Red;
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = boomShader };

        float time = Timer / Time;
        float ease = EasingFunction.OutExpo(time);
        float ease2 = EasingFunction.InExpo(time);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.4f * ExtraMath.Osc(0.6f, 1f, speed: 6) * MathHelper.Lerp(1f, 0f, ease2);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.2f * MathHelper.Lerp(0f, 1f, ease);
        sb.Draw(glowDrawer);
        using(SpritebatchStarter.Begin(sb, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.FlameVortexNoise.Asset, Projectile.Center);
            drawer.scale = Vector2.One * MathHelper.Lerp(0.2f, 1.56f, ease);
            drawer.color = Color.Lerp(Color.Gold, Color.Transparent, ease2);
            sb.Draw(drawer);


        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlameBoom);
    }
}