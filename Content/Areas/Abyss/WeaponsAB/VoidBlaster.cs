using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Players;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Abyss.WeaponsAB;

public class VoidBlaster : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.damage = 10;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 10000;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item11;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<VoidBlasterProj>();
        Item.shootSpeed = 20f;
        Item.useAmmo = AmmoID.Bullet;

        Item.useAnimation = 21;
        Item.useTime = 3; // one third of useAnimation
        Item.reuseDelay = 60;
    }
    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 7;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 0);
    }

    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int numberProjectiles = 1; // 4 or 5 shots
        for (int i = 0; i < numberProjectiles; i++)
        {
            type = ModContent.ProjectileType<VoidBlasterProj>();
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(8));                                                                                                         // perturbedSpeed = perturbedSpeed * scale; 
            Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
        }
     //   float recoilStrength = 1;
       // player.AddRecoil(-velocity.SafeNormalize(Vector2.Zero) * recoilStrength);
        FXUtil.ShakeCamera(player.Center, 1024, 8f);
        //Dust Burst Towards Mouse
        float rot = velocity.ToRotation();
        float spread = 0.24f;
        Vector2 offset = new Vector2(1.3f, 0f * player.direction).RotatedBy(rot);
        for (int k = 0; k < 2; k++)
        {
            Vector2 direction = offset.RotatedByRandom(spread);
            var dp = DustParticle.Spawn(position + offset * 43, direction * Main.rand.NextFloat(3, 15f));
            dp.outerColor = Color.Blue;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.Scale *= 0.75f;
            dp.dampening = 0.08f;
            //   Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(50, 74, 255), Main.rand.NextFloat(0.2f, 0.5f));
        }

        Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(50, 74, 255), 1);
        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
        return false; // return false because we don't want tmodloader to shoot projectile
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<PearlescentScrap>());
    }

}



public class VoidBlasterExplosionBomb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            ShakeScreenPosition.Shake = 4;
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), Projectile.position);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSeaGreen, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.SeaGreen, 1f).noGravity = true;
            }

            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.SeaGreen,
               outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.3f);


            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.SeaGreen,
               outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.2f);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.AliceBlue,
                    outerGlowColor: Color.Black, baseSize: 0.2f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), Projectile.position);
            ShakeScreenPosition.Shake = 4;
            for (int i = 0; i < 6; i++)
            {

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSeaGreen, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkSeaGreen, 0.5f).noGravity = true;
            }
        }
    }
}



public class VoidBlasterExsplosion : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.alpha = 255;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = 10;
        Projectile.timeLeft = 150;
        Projectile.height = 28;
        Projectile.width = 60;
        Projectile.extraUpdates = 1;
    }

    private NPC Owner => Main.npc[(int)Projectile.ai[1]];
    public override void AI()
    {
        Projectile.Center = Owner.Center;
        Timer++;
        if (Timer == 50)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DodgerBlue, 1f).noGravity = true;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DodgerBlue, 1.5f).noGravity = true;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<VoidBlasterExplosionBomb>(), Projectile.damage * 4, 1, Projectile.owner, 0, 0);
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2524f, 40f);
            Projectile.alpha = 0;
        }
        if (Projectile.ai[0] >= 50)
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0, 0.4f);
        }
    }
}


public class VoidBlasterPlayer : ModPlayer
{
    public int hitCount;
    public int npcWhoAmI;
    public override void ResetEffects()
    {
        base.ResetEffects();
    }
}
public class VoidBlasterProj : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Granite MagmumProj");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Bullet);
        AIType = ProjectileID.Bullet;
        Projectile.penetrate = 1;
        Projectile.width = 15;
        Projectile.height = 15;
        Projectile.extraUpdates = 2;
    }


    public override void AI()
    {
        int num1222 = 74;
        if (Projectile.ai[1] == 1)
        {
            float rot = Projectile.velocity.ToRotation();
            float spread = 0.24f;
            for (int k = 0; k < 2; k++)
            {
                Vector2 direction = rot.ToRotationVector2().RotatedByRandom(spread);
                var dp = DustParticle.Spawn(Projectile.Center, direction * Main.rand.NextFloat(3, 15f));
                dp.outerColor = Color.Blue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.Scale *= 0.75f;
                dp.dampening = 0.08f;
                //   Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(50, 74, 255), Main.rand.NextFloat(0.2f, 0.5f));
            }
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.Black);
            fx.Scale *= 0.5f;
        }
        if (Projectile.ai[1] % 12 == 0)
        {
            for (int k = 0; k < 2; k++)
            {
                int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * k;
                Main.dust[index2].scale = .95f;
                Main.dust[index2].velocity *= 0f;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = false;
            }
        }

        Projectile.ai[1]++;
        if (Projectile.ai[1] >= 15)
        {
            Projectile.penetrate = 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

            bool flag25 = false;
            int jim = 1;
            for (int index1 = 0; index1 < 200; index1++)
            {
                if (Main.npc[index1].CanBeChasedBy(Projectile, false)
                    && Projectile.Distance(Main.npc[index1].Center) < 500
                    && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                {
                    flag25 = true;
                    jim = index1;
                }
            }

            if (flag25)
            {
                float num1 = 10f;
                Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num2 = Main.npc[jim].Center.X - vector2.X;
                float num3 = Main.npc[jim].Center.Y - vector2.Y;
                float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                float num5 = num1 / num4;
                float num6 = num2 * num5;
                float num7 = num3 * num5;
                int num8 = 10;
                Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
                Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        VoidBlasterPlayer voidBlasterPlayer = Owner.GetModPlayer<VoidBlasterPlayer>();
        if (target.whoAmI != voidBlasterPlayer.npcWhoAmI)
        {
            voidBlasterPlayer.npcWhoAmI = target.whoAmI;
            voidBlasterPlayer.hitCount = 0;
        }
        else
        {
            voidBlasterPlayer.hitCount++;
            if (voidBlasterPlayer.hitCount >= 6)
            {
                voidBlasterPlayer.hitCount = 0;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                    ModContent.ProjectileType<VoidBlasterExsplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, ai1: target.whoAmI);
                int Sound = Main.rand.Next(1, 3);

                SoundStyle fireSound;
                if (Sound == 1)
                {
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/VoidBlasterExplosionBomb");
                 //   SoundEngine.PlaySound(, Projectile.position);
                }
                else
                {
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/VoidBlasterExplosionBomb2");

                }
                fireSound.PitchVariance = 0.3f;
                fireSound.Volume = 0.1f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }
        }
    }
    public override void OnSpawn(IEntitySource source)
    {
        int Sound = Main.rand.Next(1, 3);
        SoundStyle fireSound;
        if (Sound == 1)
        {
            fireSound = new SoundStyle("Stellamod/Assets/Sounds/VoidBlaster2");
          
        }
        else
        {
            fireSound = new SoundStyle("Stellamod/Assets/Sounds/VoidBlaster1");

        }
        fireSound.PitchVariance = 0.3f;
        fireSound.Volume = 0.1f;
        SoundEngine.PlaySound(fireSound, Projectile.position);
    }

    public override bool PreDraw(ref Color lightColor)
    {

        Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            float ratio = (float)k / (float)Projectile.oldPos.Length;
            Color color = Color.Lerp(Color.White, Color.Blue, ratio);
            color *= MathHelper.Lerp(1f, 0f, ratio);
            color.A = 0;
            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
           
            Vector2 drawOrigin2 = AssetManager.GlowMask.SimpleGlowCircle.Value.Size() * 0.5f;

            Color color2 = Color.Lerp(Color.Blue, Color.DarkBlue, ratio);
            color2 *= MathHelper.Lerp(1f, 0f, ratio) * 0.33f;
            color2.A = 0;
            Main.spriteBatch.Draw(AssetManager.GlowMask.SimpleGlowCircle.Value, drawPos, null, color2, Projectile.rotation, drawOrigin2, 0.2f, SpriteEffects.None, 0f);
        }

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        for(int i = 0; i < 2; i++)
        {
            Vector2 vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5f, 8f);
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.1f;
            sp.outerColor = Color.Blue;
            if (Main.rand.NextBool(4))
            {
                Vector2 vel2 = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(20, 25);
                var fx = FXUtil.GlowStretch(Projectile.Center, vel2);
                fx.OuterGlowColor = Color.Blue;
                fx.VectorScale *= 0.4f;
            }
        }
    }
}