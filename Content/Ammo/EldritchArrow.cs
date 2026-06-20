using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ammo;

public class EldritchArrow : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 12; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
        Item.knockBack = 1.5f;
        Item.value = 10;
        Item.rare = ItemRarityID.LightPurple;
        Item.shoot = ModContent.ProjectileType<EldritchArrowProj>(); // The projectile that weapons fire when using this item as ammunition.
        Item.shootSpeed = 16f; // The speed of the projectile.
        Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
    }



}


public class EldritchArrowProj : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }
    public override void SetDefaults()
    {
        Projectile.width = 32; // The width of projectile hitbox
        Projectile.height = 18; // The height of projectile hitbox
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.hostile = false; // Can the projectile deal damage to the player?
        Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
        Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        Projectile.light = 0.5f; // How much light emit around the projectile
        Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
        Projectile.tileCollide = true; // Can the projectile collide with tiles?
        AIType = 1;
    }

    public override void AI()
    {
        base.AI();
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        for (int i = 0; i < Main.rand.Next(1, 3); i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                ModContent.ProjectileType<EldritchPlanetoid>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
        for (int j = 0; j < 8; j++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.UnusedWhiteBluePurple, speed, Scale: 3f);
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(93, 203, 243), Color.Transparent, ref lightColor);
        return base.PreDraw(ref lightColor);
    }

}

public class EldritchBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }

    public override void SetDefaults()
    {
        Projectile.width = 4; // The width of projectile hitbox
        Projectile.height = 4; // The height of projectile hitbox
        Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.hostile = false; // Can the projectile deal damage to the player?
        Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
        Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
        Projectile.light = 0.5f; // How much light emit around the projectile
        Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
        Projectile.tileCollide = true; // Can the projectile collide with tiles?
        Projectile.extraUpdates = 3; // Set to above 0 if you want the projectile to update multiple time in a frame
        AIType = ProjectileID.Bullet; // Act exactly like default Bullet
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
    }
}

public class EldritchPlanetoid : ModProjectile
{
    public override void SetStaticDefaults()
    {
        // Total count animation frames
        Main.projFrames[Projectile.type] = 4;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 34;
        Projectile.height = 34;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.5f;
        Projectile.penetrate = 2;
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.03f);
        Rectangle myRect = Projectile.getRect();
        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile p = Main.projectile[i];
            if (p.friendly && p.type != Projectile.type)
            {
                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect) && p.active)
                {


                    //Shoot the projectile
                    NPC npc = NPCHelper.FindClosestNPC(Projectile.Center, float.MaxValue);
                    if (npc != null)
                    {
                        if (this.OwnedByLocalClient())
                        {
                            Vector2 velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                                ModContent.ProjectileType<EldritchBolt>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner);
                        }
                    }

                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                        var d = Dust.NewDustPerfect(Projectile.Center, DustID.UnusedWhiteBluePurple, speed, Scale: 3f);
                        d.noGravity = true;
                    }

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon") { PitchVariance = 0.15f }, Projectile.position);
                    Projectile.Kill();
                    break;
                }
            }
        }

        //Animate It
        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(93, 203, 243), Color.Transparent, ref lightColor);
        return base.PreDraw(ref lightColor);
    }
}