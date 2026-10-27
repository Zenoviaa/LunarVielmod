using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.StaminaSystem;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
        if (Main.rand.NextBool(2))
        {
            Vector2 pos = player.position;
            pos.X += Main.rand.Next(0, player.width);
            pos.Y += Main.rand.Next(0, player.height);
            Vector2 vel = Main.rand.NextVector2Circular(8, 8);
            vel.Y -= 4;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Yellow.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }

    }
}
public class StarFlowerPlayer : ModPlayer
{
    int mana;
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (Main.GameUpdateCount % 3 == 0 && Player.HasBuff<StarFlowerMana>() && Player.GetModPlayer<StaminaNDashPlayer>().CanConsume(1))
        {
            Player.statMana += 1;
            mana++;
            if(mana >= 40)
            {
                mana = 0;
                var staminaNDashPlayer = Player.GetModPlayer<StaminaNDashPlayer>();
                staminaNDashPlayer.Consume(1);
            }
        }
    }
}
public class StarFlowerStaff : ModItem
{
    private int _dir;
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.StarFlowerStaff.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 24;
        Item.mana = 125;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useAnimation = Item.useTime = 32;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shootSpeed = 12;
        Item.UseSound = null;
        Item.shoot = ModContent.ProjectileType<StarFlowerAura>();
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        position = Main.MouseWorld;
        velocity = Vector2.Zero;
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
        SoundStyle castSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                castSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/HolyCast1");
                break;
            case 1:
                castSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/HolyCast2");
                break;
        }
        castSound.PitchVariance = 0.4f;
        castSound.Volume = 0.3f;
        castSound.MaxInstances = 1;
        SoundEngine.PlaySound(castSound, player.position);


        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);

        return true;
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
    float Time => 600;
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
        Projectile.hide = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    private float EnchantDistanceSquared => 128 * 128;
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        behindNPCs.Add(index);
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Dust.QuickDustLine(Projectile.Center, Main.player[Projectile.owner].Center, 32, Color.Gold);
            var sound = AssetReferences.Assets.Sounds.StarFlower3.Asset with { PitchVariance = 0.5F };
            SoundEngine.PlaySound(sound, Projectile.position);
            var impact = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Fiery, new(5, 15));
            impact.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(impact);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Gold, Color.DarkOrange, Color.Black, 25);
            fx.Scale *= 1.5f;
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.Gold, Color.DarkOrange, 25, 128);
        }
        if (Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
            var sp = SparkleParticle.Spawn(pos, Main.rand.NextVector2Circular(2, 2));
            sp.dampening = 0.15f;
            sp.gravity = 0;
            sp.innerColor = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.Gold, Color.DarkOrange, Color.White);
            sp.outerColor = Color.Lerp(sp.innerColor, Color.Black, 0.5f);
            sp.Scale = Main.rand.NextFloat(0.45f, 0.8f) * 0.5f;
        }

        foreach (var player in Main.ActivePlayers)
        {
            float sqrtDistance = Vector2.DistanceSquared(player.Center, Projectile.Center);
            if (sqrtDistance <= EnchantDistanceSquared)
            {
                player.AddBuff(ModContent.BuffType<StarFlowerMana>(), 2);
            }
        }

    }
    void DrawPixelatedAura(SpriteBatch sb, Vector2 sp)
    {

    }
    public override bool PreDraw(ref Color lightColor)
    {
        var sb = Main.spriteBatch;
        var pass = AssetReferences.Effects.Fable.FableStarAura.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        var stars = AssetReferences.Assets.NoiseTextures.StarNoise2.Asset.Value;
        pass.Parameters.starSampler = new()
        {
            Texture = stars,
            Sampler = SamplerState.PointWrap
        };

        pass.Parameters.spriteSize = Projectile.Drawer.Size;
        pass.Parameters.noiseTexelSize = stars.GetTexelSize();
        pass.Apply();

        using (sb.Ctx(sb.Parameters with { effect = pass.Shader }))
        {
            sb.Draw(Projectile.Drawer with { scale = new Vector2(Scale), color = Color.DarkOrange });
        }

        var star = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.FivePointedStar.Asset, Projectile.Center);
        star.color = Color.DarkOrange * 0.65f;
        star.color.A = 0;
        star.rotation = Main.GlobalTimeWrappedHourly;
        star.scale = new Vector2(Scale);// * MathHelper.Lerp(4f, 1f, EasingFunction.InOutSine(Timer / 12f));
        sb.Draw(star);
        return false;
    }
}