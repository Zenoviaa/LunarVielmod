using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class PotionOfLifeWand : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<LifeWandMinionProj>());
        Item.damage = 2;
        Item.knockBack = 3f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(),
            material: ModContent.ItemType<Mushroom>());
    }
}

public class LifeWandMinionProj : AbstractBellSummon
{
    private ref float Heart => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 8;
        Projectile.StaticDefaultToMinionProjectile();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DefaultToMinionProjectile();
        Projectile.WidthAndHeight = 8;
        Projectile.width = 12;
        Projectile.LocalPiercingImmunityTime = 20;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.light = 0.67f;
    }

    public override bool? CanCutTiles()
    {
        return true;
    }

    // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
    public override bool MinionContactDamage()
    {
        return false;
    }

    // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
    public override void AI()
    {
        base.AI();
        var xOfffset = MathHelper.Lerp(-64, 64, ExtraMath.Osc(0f, 1f, speed: 0, Projectile.minionPos * 2));
        xOfffset += MathHelper.Lerp(-32f, 32f, MathF.Sin(Heart * 0.025f) * 0.5f + 0.5f);
        var targetPos = Owner.Center + new Vector2(0, -48) + new Vector2(xOfffset, 0);
        MoonUtils.AI_FloatAbove(Projectile.Center, ref Projectile.velocity, targetPos);
        Visuals();
        Heart++;
        if (Heart == 1260)
        {
            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -12), TriColorPalette.Health, new Vector2(5, 15f));
            factory.particleCount = 16;
            ParticleUtils.CreateSwirlingDustCircle(factory);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black, duration: 22f, baseSize: 0.16f);
            fx.Scale *= 1.5f;
            var enchant = AssetReferences.Assets.Sounds.PrimeMagicCast.Asset with { PitchVariance = 0.7f, Pitch = -0.5f };
            SoundEngine.PlaySound(enchant, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                int itemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(10, 10), ItemID.Heart, 1);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }

            Heart = 0;
        }
    }

    private void Visuals()
    {
        // So it will lean slightly towards the direction it's moving
        Projectile.rotation = Projectile.velocity.X * 0.05f;

        // This is a simple "loop through all frames from top to bottom" animation
        int frameSpeed = 5;

        Projectile.frameCounter++;

        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        Texture2D closingCircle = ModContent.Request<Texture2D>(TextureRegistry.ThinCircle).Value;
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;

        Color drawColor = Color.Red;
        drawColor.A = 0;
        float drawScale = MathHelper.Lerp(1f, 0f, Heart / 1260f);
        spriteBatch.Draw(closingCircle, drawPosition, null, drawColor, Projectile.rotation, closingCircle.Size() / 2f, drawScale * 0.5f, SpriteEffects.None, 0);
        return false;
    }
}