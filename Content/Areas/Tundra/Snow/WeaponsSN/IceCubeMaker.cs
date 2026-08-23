using Microsoft.Xna.Framework;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class IceCubeMaker : BaseGun
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 72;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;

            //Damage
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ModContent.ProjectileType<IceCubeMakerProj>();
            Item.shootSpeed = 25;

            //Use
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.NPCHit11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, -2);
        }

        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<IceCubeMakerProj>();
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            SoundStyle soundStyle = SoundRegistry.ExplosionCrystalShard;
            soundStyle.PitchVariance = 0.33f;
            SoundEngine.PlaySound(soundStyle, position);
            Vector2 offset = new Vector2(2, -0f * player.direction).RotatedBy(rot);
            float distance = 32;
            int numProjectiles = 3;
            for (int p = 0; p < numProjectiles; p++)
            {
                Dust.NewDustPerfect(position + offset * distance, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightCyan, 1);
                Dust.NewDustPerfect(player.Center + offset * distance, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.LightCyan * 0.5f, Main.rand.NextFloat(0.5f, 1));



                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }


    public class IceCubeMakerProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float DamageModifier => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = 12;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.light = 0.78f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                DamageModifier = 1;
            }
            if(Projectile.velocity.Length() > 1f)
                Projectile.velocity *= 0.94f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            AI_Collide();
            Visuals();
        }

        private void AI_Collide()
        {
            if (Timer < 45)
                return;

            if (!this.OwnedByLocalClient())
                return;
            Rectangle myRect = Projectile.getRect();
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.type != Projectile.type)
                    continue;
                if (p == Projectile)
                    continue;

                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    float scale = Main.rand.NextFloat(0.3f, 0.5f);

                    SoundStyle soundStyle = SoundID.NPCHit11;
                    soundStyle.Pitch = 0.5f;
                    soundStyle.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);

                    Vector2 directionToProjectile = Projectile.Center.DirectionTo(p.Center);
                    p.velocity = directionToProjectile * 16;
                    p.ai[0] = 20;

                    Vector2 bounceVelocity = -Projectile.velocity * 1.5f;
                    Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                    Projectile.ai[1] += 0.2f;
                    Projectile.netUpdate = true;
                    p.netUpdate = true;
                }
            }
        }

        private void Visuals()
        {
            //Animations
            Projectile.frameCounter++;

            int frameSpeed = 7;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return base.PreDraw(ref lightColor);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            DamageModifier = MathHelper.Clamp(DamageModifier, 1f, 3f);
            modifiers.FinalDamage *= DamageModifier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 bounceVelocity = -Projectile.velocity * 1.5f;
            Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 16);
            Projectile.velocity += -Vector2.UnitY * 8;
            Projectile.netUpdate = true;
            DamageModifier += 0.1f;

            if (Main.rand.NextBool(8))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                }
            }
        }
    }
}
