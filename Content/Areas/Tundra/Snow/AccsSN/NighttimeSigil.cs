using Stellamod.Core.PlayerLevelingSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN;

public class NighttimeSigilPlayer : ModPlayer
{
    public bool hasSigil;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSigil = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSigil)
            return;
        if (Main.myPlayer != Player.whoAmI)
            return;
        if (Player.ownedProjectileCounts[ModContent.ProjectileType<NighttimeSigilCrown>()] > 0)
            return;

        ProjFirer firer = ProjFirer.From<NighttimeSigilCrown>(Player);
        firer.New();
    }
}

public class NighttimeSigil : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        if (!Main.dayTime)
        {
            player.GetModPlayer<NighttimeSigilPlayer>().hasSigil = true;
            LevelingPlayer levelingPlayer = player.GetModPlayer<LevelingPlayer>();
            for (int i = 0; i < levelingPlayer.statModifiers.Length; i++)
            {
                levelingPlayer.statModifiers[i] += 5;
            }
        }
    }
}

public class NighttimeSigilCrown : ModProjectile
{
    private Vector2 HoldPosition
    {
        get
        {
            Vector2 position = Owner.Center;
            position.Y += Owner.gfxOffY;
            position.Y += ExtraMath.Osc(0, -8, speed: 2);
            position.Y -= 62;
            return position;
        }
    }

    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 120;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        if (Owner.GetModPlayer<NighttimeSigilPlayer>().hasSigil)
        {
            Projectile.timeLeft = 120;
        }

        Timer++;
        Projectile.Center = HoldPosition;
        if(Timer % 32 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, Scale: 0.7f);
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.worldPosition = HoldPosition;
        drawer.color = Color.Lerp(Color.Purple, Color.SkyBlue, ExtraMath.Osc(0f, 1f)) * EasingFunction.InOutSine(Timer / 60f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 60f);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}