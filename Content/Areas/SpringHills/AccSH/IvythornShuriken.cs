using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class IvythornShuriken : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Assassin's Shuriken");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<IvythornShurikenPlayer>().hasShuriken = true;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-3f, -2f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankAccessory>(),
            material: ModContent.ItemType<Ivythorn>());
    }
}

public class IvythornShurikenPlayer : ModPlayer
{
    private bool _trySummonSaw;
    public bool hasShuriken = false;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasShuriken = false;
    }
    public override void Load()
    {
        base.Load();
        DashPlayer.OnStaminaEffects += TriggerSaw;
    }
    public override void Unload()
    {
        base.Unload();
        DashPlayer.OnStaminaEffects -= TriggerSaw;
    }
    private void TriggerSaw(Player player)
    {
        player.GetModPlayer<IvythornShurikenPlayer>()._trySummonSaw = true;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (!hasShuriken)
            return;
        if (!_trySummonSaw)
            return;
        _trySummonSaw = false;
        if (Player.whoAmI == Main.myPlayer)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = (Main.MouseWorld - Player.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel = vel.RotatedByRandom(0.5f);
                vel *= Main.rand.NextFloat(10, 15);
                vel.Y -= 5;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, vel,
                    ModContent.ProjectileType<IvythornShurikenProj>(), 8, 1, Player.whoAmI);
            }

        }
    }
}
public class IvythornShurikenProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.ArmorPenetration = 5;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer < 25)
        {
            Projectile.velocity.Y += 0.35f;

        }
        else
        {
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if (nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 5);
                Projectile.velocity *= 1.015f;
            }
            else
            {
                Projectile.velocity.Y += 0.35f;

            }

        }

        if (Main.rand.NextBool(4))
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(pos, DustID.JunglePlants, Main.rand.NextVector2Circular(2, 2));
        }

        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.15f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.White, Color.Transparent, 0.4f);
        SpritebatchDrawer shurikenDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(shurikenDrawer);

        shurikenDrawer.VerticalFrame(1, 2);
        shurikenDrawer.color = Color.LightGreen * ExtraMath.Osc(0.5f, 1f, speed: 16);
        Main.spriteBatch.Draw(shurikenDrawer);
        return false;
    }


    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustPerfect(base.Projectile.Center, DustID.JunglePlants, (Vector2.One).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
        }
        for (int i = 0; i < 5; i++)
        {
            int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num1].noGravity = true;
            Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num1].position != Projectile.Center)
                Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num].noGravity = true;
            Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num].position != Projectile.Center)
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
        }

    }


}


