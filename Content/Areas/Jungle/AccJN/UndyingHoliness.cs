using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.AccJN;

public class HolyManaKnifeGlobalProjectile : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        if (ProjSets.IsManasphere[projectile.type] && Main.player[projectile.owner].GetModPlayer<ManaSpherePlayer>().radiantKnives)
        {
            //Rain swords here
            Vector2 spawnPos = target.Center + new Vector2(0, -500);
            spawnPos += Main.rand.NextVector2Circular(80, 80);
            Vector2 vel = (target.Center - spawnPos).SafeNormalize(Vector2.Zero) * 8;
            Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, vel,
                ModContent.ProjectileType<HolyManaKnife>(), projectile.damage / 2, 1, projectile.owner);
        }
    }
}

public class HolyManaKnife : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float ScaleVariance => ref Projectile.ai[1];
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
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.light = 1.5f;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            softSummon.PitchVariance = 0.3f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
            ScaleVariance = Main.rand.NextFloat(0.8f, 1f);
            Projectile.scale = 0.001f;
        }

        if (Projectile.Top.Y < Main.player[Projectile.owner].Bottom.Y)
            Projectile.tileCollide = false;
        else
            Projectile.tileCollide = true;

        if (Timer % 12 == 0)
        {
            var ds = DustParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f);
            ds.outerColor = Color.Gold;
            ds.Scale *= 0.5f;
            ds.gravity = 0;
        }

        Projectile.scale = MathHelper.Lerp(Projectile.scale, ScaleVariance, 0.1f);
        if (Projectile.velocity.Length() < 15)
            Projectile.velocity *= 1.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer <= 1)
            return false;
        SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        afDrawer.scale.Y *= 0.5f;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.Gold, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 16));
        drawer.color.A = 0;
        drawer.scale.Y *= 0.5f;
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(int i = 0; i < 3; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with { position = Projectile.Center, velocity = Main.rand.NextVector2Circular(6, 6),
                outerColor = Color.Gold.ToVector4(), innerColor = Color.Yellow.ToVector4(), timeLeft = Main.rand.Next(20, 50), scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)) });
        }
    }
}

public class UndyingHoliness : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<ManaSpherePlayer>().radiantKnives = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<RadiantNectar, BlankAccessory>();
    }
}
