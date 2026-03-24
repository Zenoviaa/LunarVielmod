using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Steins;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class Hultinstein : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 7;
        Item.useTime = 8;
        Item.useAnimation = 8;
        Item.shoot = ModContent.ProjectileType<HultinsteinBarrage>();
        staminaProjectileShoot = ModContent.ProjectileType<HultFist>();
        meleeWeaponType = MeleeWeaponType.Stein;
        staminaDamageMultiplier = 2;
        staminaCost = 3;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStein>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}

public class HultinsteinBarrage : ModProjectile
{
    private Vector2 _start;
    private Vector2 _end;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 12;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_start);
        writer.WriteVector2(_end);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _start = reader.ReadVector2();
        _end = reader.ReadVector2();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private Vector2 CalculateSwingPoint(float time)
    {
        float ratio = time / 12f;
        float ease = EasingFunction.QuadraticBump(ratio);
        Vector2 pos = Vector2.Lerp(_start, _end, ease);
        return pos;
    }
    public override void AI()
    {
        base.AI();
     //   ProjectileID.Sets.TrailCacheLength[Type] = 8;
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                _start = Owner.Center + Main.rand.NextVector2Circular(45, 45);
                _end = _start + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80;
                Projectile.netUpdate = true;
            }
        }
        if (Timer == 2)
        {

            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
            ThrustParticle ts = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity);
            ts.bloomColor = Color.LightGray;
            ts.Scale *= 0.5f;
        }

        if(Timer % 8 == 0)
        {
            var ts = ThickSmokeParticle.Spawn(Projectile.Center, Vector2.Zero);
            ts.expand = true;
            ts.color *= 0.5f;
            ts.Scale *= 0.2f;
        }
        Projectile.Center = CalculateSwingPoint(Timer);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = (float)(i + 1) / (float)Projectile.oldPos.Length;
            SpritebatchDrawer fadeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            fadeDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            fadeDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.3f;
            Main.spriteBatch.Draw(fadeDrawer);
        }
        return false;
        
    }
}