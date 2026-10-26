using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class BurningFlask : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningFlask.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToCombatTool(0, 0, ammoCount: 3);
        Item.shoot = ModContent.ProjectileType<BurningFlaskThrow>();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankJuggler>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}


public class BurningFlaskOil : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 300;
        Projectile.friendly = true;

        Projectile.StaticPiercingImmunityTime = 30;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(3))
        {
            Vector2 pos = Projectile.Bottom;
            pos.X += Main.rand.NextFloat(-Projectile.width / 2, Projectile.width / 2);
            pos.Y += Main.rand.NextFloat(-3f, 3f);
            Particles.RagingFlameDust.Spawn(RagingFlameDustData.Default with { position = pos, timeleft = 70, style = 2 });
        }
        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y += 0.25f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 120);
    }
}

public class BurningFlaskThrow : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningFlaskThrow.KEY;
    public override void SetStaticDefaults()
    {
        Projectile.SetTrailCacheLength(12);
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.light = 0.5f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle useSound = SoundID.DD2_GhastlyGlaivePierce;
            useSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(useSound, Projectile.position);
        }


        Projectile.velocity.Y += 0.3f;
        Projectile.rotation += Projectile.velocity.X * 0.015f;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        CreateImpactEffects();
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        CreateImpactEffects();
        return true;
    }
    private void CreateImpactEffects()
    {
        for (int i = 0; i < 6; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
        }
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
        }

        int numDust = 8;
        for (int n = 0; n < numDust; n++)
        {
            var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
            sp.initialColor = Color.Brown;
        }

        for (int n = 0; n < numDust; n++)
        {
            var dp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 25), Scale: Main.rand.NextFloat(1f, 2f));
        }

        ShakeScreenPosition.Shake = 3;
        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"), Projectile.position);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            for(var i= 0; i < 4; i++)
            {
                var velocity = -Vector2.UnitY;
                velocity = velocity.RotatedByRandom(1.5f);
                velocity *= Main.rand.NextFloat(10, 15);
                var pos = Projectile.Center;
                var firer = ProjFirer.From<BurningFlaskOil>(Projectile);
                firer.position = pos;
                firer.velocity = velocity;
                firer.New();
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, 0.24f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}