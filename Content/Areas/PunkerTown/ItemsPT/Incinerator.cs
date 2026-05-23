using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class Incinerator : BaseGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 62;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 280;
            Item.knockBack = 4;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<IncineratorProj>();
            Item.useTime = Item.useAnimation = 4;
            Item.shootSpeed = 1;
            // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
            Item.value = Item.sellPrice(gold: 25);
            muzzleOrigin = new Vector2(45, 10);
        }

        public override void SetMagazine(ref GunReloadParams fireParams)
        {
            base.SetMagazine(ref fireParams);
            fireParams.maxAmmo = 48;
            fireParams.reloadWindow = 120;
        }
        public override Vector2? HoldoutOffset()
        {
            muzzleOrigin = new Vector2(45, 24);
            return new Vector2(16, 0);
        }
        public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            type = ModContent.ProjectileType<IncineratorProj>();

            for(int i = 0; i < 2; i++)
            {
                Vector2 fvelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                fvelocity *= Main.rand.NextFloat(1.8f, 2.2f);
                Projectile.NewProjectile(source, position + velocity * 9, fvelocity, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            Color innerColor = Color.Yellow;
            Color outerColor = Color.Red;
            var sp = SmokeParticle.SpawnInAlphaLayer(position, velocity * 0.2f, Color.DarkGray);
            sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
            sp.fast = true;

            MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(position, velocity, innerColor);
            flashParticle.innerColor = innerColor;
            flashParticle.bloomColor = outerColor;
            flashParticle.Scale *= Main.rand.NextFloat(0.2f, 0.4f);



            for (float f = 0; f < 2; f++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    gravity = 0f,
                    innerColor = innerColor,
                    outerColor = outerColor,
                    scaleRange = new Vector2(0.8f, 1f)
                };
                var dp = DustParticle.Spawn(position, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1.5f, 3f), spawnParams);
                dp.dampening = 0.1f;
                dp.Scale *= 1;
            }

            for (int i = 0; i < 1; i++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(position, velocity.RotatedByRandom(MathHelper.ToRadians(22)) * Main.rand.NextFloat(3f, 8f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1.35f));
                dp.gravity = 0;
                dp.dampening = 0.1f;
                dp.innerColor = Color.Yellow;
            }
        }

    }

    public class IncineratorProj : ModProjectile,
        IDrawToRenderTarget
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationDir => ref Projectile.ai[1];
        private ref float DeathTimer => ref Projectile.ai[2];
        private float LifeTime => 60;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 7;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = (int)LifeTime;

        }


        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void AI()
        {

            Timer++;
            if (Timer == 1 && Main.rand.NextBool(8))
            {
                SoundStyle fireballShoot = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.5f };
                SoundEngine.PlaySound(fireballShoot, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }

            if (Main.rand.NextBool(32))
            {
                float time = (float)Projectile.timeLeft / LifeTime;
                FaintSmokeParticle faintSmoke = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center, Projectile.velocity, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                faintSmoke.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f) * 0.5f;
                faintSmoke.fadeToColor = Color.DarkGray * 0.5f;
                faintSmoke.Scale = Main.rand.NextFloat(0.5f, 0.9f) * time;
                faintSmoke.behindLayer = true;
            }

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            if(this.OwnedByLocalClient() && Main.rand.NextBool(128))
            {
                RotationDir = Main.rand.NextFloat(-1f, 1f);
                Projectile.netUpdate = true;
            }
            if (RotationDir != 0)
            {
                Timer++;
                Projectile.velocity = Projectile.velocity.RotatedBy(RotationDir * 0.05f);
            }
            if(DeathTimer == 1)
            {
                Timer++;
                Projectile.velocity *= 0.98f;
            }
            if (Timer >= LifeTime)
                Projectile.Kill();
            Lighting.AddLight(Projectile.Center, TorchID.Torch);
            Projectile.rotation += 0.05f;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            DeathTimer = 1;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
          //  PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
            return false;
        }

        public void DrawToRenderTargets()
        {
            float time = (float)Timer / LifeTime;
            time = 1f - time;
            float inverseTime = 1f - time;
            float maxRadius = 0.065f;
            float radius1 = MathHelper.Lerp(0f, maxRadius, EasingFunction.OutExpo(inverseTime));
            float radius2 = MathHelper.Lerp(maxRadius, 0f, EasingFunction.InExpo(inverseTime));
            float radius = MathHelper.Lerp(radius1, radius2, inverseTime);
            FlamethrowerRenderer.AddMetaball(Projectile.Center, time, radius);
            PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 120);
        }

        private void DrawBloom(SpriteBatch sb, Vector2 screenPos)
        {
            float time = (float)Timer / LifeTime;
            time = 1f - time;
            float inverseTime = 1f - time;
            float maxRadius = 1f;
            float radius1 = MathHelper.Lerp(0f, maxRadius, EasingFunction.OutExpo(inverseTime));
            float radius2 = MathHelper.Lerp(maxRadius, 0f, EasingFunction.InExpo(inverseTime));
            float radius = MathHelper.Lerp(radius1, radius2, inverseTime);

            SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            bloomDrawer.color = Color.DarkRed * 0.15f * MathHelper.Lerp(1f, 0f, Timer / LifeTime); 
            bloomDrawer.color.A = 0;
            bloomDrawer.scale *= 0.6f * radius;
            sb.Draw(bloomDrawer);
        }
    }
}
