using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class StarFlowerMana : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.StarFlowerMana.KEY;
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (Main.rand.NextBool(4))
        {
            Vector2 pos = player.position;
            pos.X += Main.rand.Next(0, player.width);
            pos.Y += Main.rand.Next(0, player.height);
            Vector2 vel = Main.rand.NextVector2Circular(8, 8);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Yellow.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }
        player.manaRegenBonus += 4;
    }
}
public class StarFlowerStaff : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.StarFlowerStaff.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 24;
        Item.mana = 125;
        Item.useStyle = ItemUseStyleID.Guitar;
        Item.useTime = Item.useAnimation = 24;
        Item.UseSound = null;
        Item.shoot = ModContent.ProjectileType<StarFlowerAura>();
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        position = Main.MouseWorld;
        velocity = Vector2.Zero;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankStaff>();
    }
}


public class StarFlowerAura : ModProjectile
{
    float Scale => EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
    float Time => 300;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.StarFlowerAuraMask.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 128;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6f;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    private float EnchantDistanceSquared => 252 * 252;
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
            var sp = SparkleParticle.Spawn(pos, Main.rand.NextVector2Circular(2, 2));
            sp.dampening = 0.15f;
            sp.gravity = 0;
            sp.innerColor = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.DarkBlue, Color.SkyBlue, Color.White);
            sp.outerColor = Color.Lerp(sp.innerColor, Color.Black, 0.5f);
            sp.Scale = Main.rand.NextFloat(0.45f, 0.8f) * 0.5f;
        }

        foreach (var player in Main.ActivePlayers)
        {
            float sqrtDistance = Vector2.DistanceSquared(player.Center, Projectile.Center);
            if (sqrtDistance <= EnchantDistanceSquared)
            {
                player.AddBuff(ModContent.BuffType<StarFlowerMana>(), 120);
            }
        }

    }
    public override bool PreDraw(ref Color lightColor)
    {
        var pass = AssetReferences.Effects.Fable.FableStarAura.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        var stars = AssetReferences.Assets.NoiseTextures.Stars.Asset.Value;
        pass.Parameters.starSampler = new()
        {
            Texture = stars,
            Sampler = SamplerState.PointWrap
        };

        pass.Parameters.spriteSize = Projectile.Drawer.Size;
        pass.Parameters.noiseTexelSize = stars.GetTexelSize();
        pass.Apply();

        using(Main.spriteBatch.Ctx(Main.spriteBatch.Parameters with { effect = pass.Shader }))
        {
            Main.spriteBatch.Draw(Projectile.Drawer with { scale = new Vector2(Scale) });
        }

        return false;
    }
}