using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.GunSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;

namespace Stellamod.Content.Areas.Jungle.WeaponsRadiant
{
    //Use class swap item
    public class FireflyCannon : BaseGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            remainingAmmo = maxAmmo = 3;
            reloadWindow = 120;
            Item.damage = 54;
            Item.width = 94;
            Item.height = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 15;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<FireflyBomb>();
            Item.shootSpeed = 30;
            Item.UseSound = SoundID.Pixie;
            Item.useAnimation = 70;
            Item.useTime = 70;
            Item.noMelee = true;
        }

        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {            
            //Funny Recoil
            float recoilStrength = 10;
            Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
            player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);
           type = ModContent.ProjectileType<FireflyBomb>();
            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            int numProjectiles = Main.rand.Next(8, 12);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            //Dust Burst Towards Mouse
            int count = 16;
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.CopperCoin, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            //Dust Burst in Circle at Muzzle
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 8;
                Dust.NewDust(position, 0, 0, DustID.CopperCoin, vel.X * 0.5f, vel.Y * 0.5f);
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), player.position);
            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class FireflyBomb : ModProjectile
    {
        public Vector3 HuntrianColorXyz;
        public float HuntrianColorOffset;
        public float Timer;
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 180;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if(Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y; 
            return false;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Goldenrod, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 10;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);


            Texture2D texture = TextureRegistry.DimLight.Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color glowColor = Color.Yellow;
            glowColor.A = 0;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, null, glowColor, 0, texture.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, glowColor, 0, texture.Size() / 2, 0.5f, SpriteEffects.None, 0);

            glowColor = Color.White;
            glowColor.A = 0;
            spriteBatch.Draw(texture, drawPos, null, glowColor, 0, texture.Size() / 2, 0.25f, SpriteEffects.None, 0);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            base.AI();
            Projectile.ai[0] += 1f;

            HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(85, 45, 15),
                new Vector3(15, 60, 60),
                new Vector3(3, 3, 3), HuntrianColorOffset);

            Timer++;
            if (Timer <= 2)
            {
                HuntrianColorOffset = Main.rand.NextFloat(-1f, 1f);
            }

            SummonHelper.SearchForTargets(Main.player[Projectile.owner], Projectile,
                out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget && distanceFromTarget < 384)
            {
                AI_Movement(targetCenter, 15);
            }

            Projectile.velocity *= 0.98f;

            //Despawn after 3 seconds.
            if (Projectile.ai[0] >= 180f)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            int count = 8;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 2;
                Dust.NewDust(Projectile.position, 0, 0, DustID.CopperCoin, vel.X, vel.Y);
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starblast"), Projectile.position);
        }
    }
}
