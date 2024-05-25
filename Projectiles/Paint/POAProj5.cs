using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Stellamod.Projectiles.Visual;


namespace Stellamod.Projectiles.Paint
{
    public class POAProj5 : ModProjectile, IPixelPrimitiveDrawer
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public byte Timer2;
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }
        public override void SetDefaults()
        {
            Projectile.damage = 15;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 80;
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }



        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            player.RotatedRelativePoint(Projectile.Center);
            if (Main.myPlayer == Projectile.owner && Main.mouseLeft)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 3;
            }
            else
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 20;
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    Projectile.Kill();
                }
            }
            
         
            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = rotation * player.direction;
            //Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SplashProj>(), 0, 0, Projectile.owner);
            }

            if (Main.rand.NextBool(2))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), Projectile.damage / 2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(1))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage / 4, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), Projectile.damage * 5, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }


            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), Projectile.damage / 2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), Projectile.damage / 3, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(3))
            {

                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Projectile.oldPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public Color ColorFunction(float completionRatio)
        {
            if (completionRatio < 0.2f)
            {
                return Color.Red;
                // return Color.Lerp(Color.Red, new Color(255, 145, 0), completionRatio / 0.25f);
            }
            else if (completionRatio < 0.4f)
            {
                return new Color(255, 145, 0);
                // return Color.Lerp(new Color(255, 145, 0), new Color(1, 255, 149), (0.5f - completionRatio) / 0.25f);
            }
            else if (completionRatio < 0.6f)
            {
                return new Color(1, 255, 149);
                // return Color.Lerp(new Color(1, 255, 149), new Color(0, 239, 255), (0.75f - completionRatio) / 0.25f);
            }
            else if (completionRatio < 0.8f)
            {
                return new Color(0, 239, 255);
                //return Color.Lerp(new Color(0, 239, 255), new Color(255, 0, 252), (1f - completionRatio) / 0.25f);
            }
            else
            {
                return new Color(255, 0, 252);
            }
        }

        public Color ColorFunction2(float completionRatio)
        {
            return Color.Black;
        }


        public float WidthFunction(float completionRatio)
        {
            if (completionRatio < 0.5f)
            {
                return MathHelper.Lerp(0f, 48, completionRatio / 0.5f);
            }
            else
            {
                return MathHelper.Lerp(0f, 48, (1f - completionRatio) / 0.5f);
            }

        }

        public float WidthFunction2(float completionRatio)
        {
            if (completionRatio < 0.5f)
            {
                return MathHelper.Lerp(0f, 56, completionRatio / 0.5f);
            }
            else
            {
                return MathHelper.Lerp(0f, 56, (1f - completionRatio) / 0.5f);
            }
        }

        internal PrimitiveTrail BeamDrawer;
        internal PrimitiveTrail OutlineBeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true);
            OutlineBeamDrawer ??= new PrimitiveTrail(WidthFunction2, ColorFunction2, null, true);
            //  TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.SimpleTrail);

            OutlineBeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}

