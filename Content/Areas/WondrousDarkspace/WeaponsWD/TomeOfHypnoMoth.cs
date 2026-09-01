using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class TomeOfHypnoMoth : BaseMagicTomeItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.shoot = ModContent.ProjectileType<TomeOfHypnoMothHold>();
        Item.shootSpeed = 10f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankStaff>();
    }
}
public class TomeOfHypnoMothHold : BaseMagicTomeProjectile
{
    private float _dustTimer;
    public override void SetDefaults()
    {
        base.SetDefaults();
        //How often it shoots
        AttackRate = 36;

        //How fast it drains mana, better to change the mana use in the item instead of this tho
        ManaConsumptionRate = 4;

        //How far the tome is held from the player
        HoldDistance = 36;

        //The glow effect around it
        GlowDistanceOffset = 4;
        GlowRotationSpeed = 0.05f;
    }

    public override void AI()
    {
        base.AI();
        _dustTimer++;
        if (_dustTimer % 16 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.LightPink, Main.rand.NextFloat(1f, 1.5f));
        }
    }

    protected override void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
    {
        base.Shoot(player, source, position, velocity, damage, knockback);
        int type = ModContent.ProjectileType<LarveinScriputeProg>();
        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Projectile.owner);
        }
    }
}
public class LarveinScriputeProg : ModProjectile
{
    private ITrailer _trailer;
    private ref float Timer => ref Projectile.ai[0];
    private ref float AlphaCounter => ref Projectile.ai[1];
    private ref float Red => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        AlphaCounter = 1;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.timeLeft = 100;
        Projectile.alpha = 0;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }


    public override void AI()
    {
        Projectile.velocity *= 0.98f;
        Timer++;
        if (Timer == 1)
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.alpha = 255;
        }

        if (Timer <= 1)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                float A = Main.rand.Next(0, 2);

                if (A == 0)
                {
                    Red = 15;
                }
                else
                {
                    Red = 65;
                }
                Projectile.velocity.X += offsetX;
                Projectile.velocity.Y += offsetY;
                Projectile.netUpdate = true;
            }

            Projectile.scale = 1.5f;
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
        }

        if (Main.rand.NextBool(3))
        {
            int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
            Main.dust[dustnumber].velocity *= 0.3f;
            Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
            Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
            Main.dust[dustnumber].noGravity = true;
            Main.dust[dustnumber].noLight = false;
        }

        if (Timer >= 90)
        {
            if (Projectile.scale >= 0)
            {
                Projectile.scale -= 0.22f;
            }
            if (AlphaCounter >= 0)
            {
                AlphaCounter -= 0.08f;
            }
        }


        Projectile.spriteDirection = Projectile.direction;
        Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.0f * Main.essScale);
    }

    public override void OnKill(int timeLeft)
    {
        var EntitySource = Projectile.GetSource_Death();
        for (int i = 0; i < 3; i++)
        {
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-4, 5), Main.rand.Next(-4, 5),
                ModContent.ProjectileType<LarveinScriputeProg2>(), Projectile.damage, 1, Projectile.owner, 0, 0);
        }


        ShakeScreenPosition.Shake = 7;
        float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
        float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0, speedYa * 0,
            ModContent.ProjectileType<MooningKaboom>(), 0, 0f, Projectile.owner, 0f, 0f);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
        }
        for (int i = 0; i < 7; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.VenomStaff, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _trailer ??= TrailPresets.HypnotizedSoul;
        _trailer.DrawTrail(ref lightColor, Projectile.oldPos);
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }
}
public class LarveinScriputeProg2 : ModProjectile
{
    private ITrailer _trailer;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Red => ref Projectile.ai[1];
    private ref float AlphaCounter => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.timeLeft = 250;
        Projectile.alpha = 0;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        Projectile.velocity.Y -= 0.08f;
        Projectile.velocity.X *= 0.97f;
        Timer++;
        if (Timer == 1)
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.alpha = 255;
        }

        if (Timer <= 1)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                Projectile.velocity.X += offsetX;
                Projectile.velocity.Y += offsetY;
                Projectile.netUpdate = true;
            }

            float A = Main.rand.Next(0, 2);

            if (A == 0)
            {
                Red = 15;
            }
            else
            {
                Red = 65;
            }

            Projectile.scale = 1.5f;

        }
        if (Main.rand.NextBool(3))
        {
            int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
            Main.dust[dustnumber].velocity *= 0.3f;
            Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
            Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
            Main.dust[dustnumber].noGravity = true;
            Main.dust[dustnumber].noLight = false;
        }

        Projectile.spriteDirection = Projectile.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _trailer ??= TrailPresets.HypnotizedSoul;
        _trailer.DrawTrail(ref lightColor, Projectile.oldPos);
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

}
