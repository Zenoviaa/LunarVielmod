using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class FallingMushroomArtifact : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 8;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 14;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item43 with { PitchVariance = 0.4f, Volume = 0.3f };
        Item.knockBack = 2;
        Item.shoot = ModContent.ProjectileType<FallingMushroom>();
        Item.shootSpeed = 12;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }

        position += Main.rand.NextVector2Circular(32, 32);
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<Mushroom>());
    }
}

public class FallingMushroomCloud : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            int rand = Main.rand.Next(4) + 1;
            string fungalFlace = $"Stellamod/Assets/Sounds/FungalFlaceBall{rand}";
            SoundStyle fungalSound = new SoundStyle(fungalFlace);
            fungalSound.PitchVariance = 0.4f;
            fungalSound.Volume = 0.3f;
            SoundEngine.PlaySound(fungalSound, Projectile.position);

            for (float f = 0; f < 4; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                fs.noShrink = true;
                fs.Scale *= Main.rand.NextFloat(0.25f, 0.5f) * 0.5f;
                fs.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f) * 0.4f;
                fs.fadeToColor = Color.Lerp(Color.OrangeRed, Color.Black, 0.8f) * 0.4f;
            }
        }
        if (Timer % 16 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, Vector2.Zero);
            fs.color = Color.OrangeRed * 0.8f;
            fs.fadeToColor = Color.Lerp(fs.color, Color.Black, 0.8f) * 0.8f;
            fs.Scale *= 0.1f * Projectile.scale;
        }
        Projectile.velocity.Y -= 0.05f;
    }

    private void DrawPixelatedSmog(SpriteBatch sb, Vector2 screenPos)
    {
        RadialShearShader radialSheer = RadialShearShader.Instance;
        radialSheer.Time = EasingFunction.InExpo((Timer / 30f) * 1.4f);
        sb.Restart(effect: radialSheer.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        float ratio = Timer / 30f;
        float ease = EasingFunction.QuadraticBump(ratio);
        drawer.color = Color.Lerp(Color.Transparent, Color.OrangeRed, ease) * 0.6f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        sb.Draw(drawer);
        sb.RestartDefaults();
    }


    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSmog, DrawLayer.OverNPCsWithOutline);
        return false;
        //  return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class FallingMushroom : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private enum AIState
    {
        ThrowOut,
        FallNWave
    }
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    public override void Unload()
    {
        base.Unload();
        _outlineTextureAsset = null;
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
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.timeLeft = 369;
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.ThrowOut:
                AI_ThrowOut();
                break;
            case AIState.FallNWave:
                AI_FallNWave();
                break;
        }
        Projectile.rotation = Projectile.velocity.X * 0.05f;
    }

    private void AI_ThrowOut()
    {
        Timer++;
        if (Timer % 8 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.8f, 1.2f));
        }
        Projectile.tileCollide = false;
        Projectile.velocity.X *= 0.978f;
        Projectile.velocity.Y -= 0.5f;
        if (Timer >= 15)
        {
            SwitchState(AIState.FallNWave);
        }
    }

    private void AI_FallNWave()
    {
        Timer++;
        if (Timer % 16 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.4f, 0.8f));
        }

        if (Timer % 16 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.8f, 1.2f));
        }

        if (Timer % 16 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, Vector2.Zero);
            fs.color = Color.OrangeRed * 0.3f;
            fs.fadeToColor = Color.Lerp(fs.color, Color.Black, 0.8f) * 0.3f;
            fs.Scale *= 0.25f * Projectile.scale;
        }

        if (Projectile.velocity.Y < 2)
            Projectile.velocity.Y += 0.5f;
        Projectile.tileCollide = true;
        float targetX = MathF.Sin(Timer * 0.1f) * 1f;
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetX, 0.1f);

        float outScale = Projectile.timeLeft / 60f;
        float easedOutScale = EasingFunction.InOutSine(outScale);
        Projectile.scale = easedOutScale;
    }

    private void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.rotation = Projectile.oldRot[i];
            afDrawer.color = Color.Lerp(Color.OrangeRed, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.texture = _outlineTextureAsset.Value;
        sbDrawer.color = Color.Lerp(Color.White, Color.White * 0.1f, ExtraMath.Osc(0f, 1f, speed: 6, offset: Projectile.whoAmI));
        Main.spriteBatch.Draw(sbDrawer);

        SpritebatchDrawer sporeDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        sporeDrawer.color = Color.OrangeRed * 0.3f;
        sporeDrawer.color.A = 0;
        sporeDrawer.scale = Vector2.One * 0.3f * ExtraMath.Osc(0.66f, 1f, speed: 2, offset: Projectile.whoAmI);
        sporeDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(sporeDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<FallingMushroomCloud>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
}
