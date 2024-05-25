using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Paint
{
    public class InkingSThrow : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public byte Timer;
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
            Projectile.localNPCHitCooldown = 5;
        }

       

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation -= 0.5f;

            if (Main.myPlayer == Projectile.owner && Main.mouseRight)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
                Projectile.netUpdate = true;
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
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(2))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage / 2) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(1))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), (Projectile.damage / 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(10))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }


            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), (Projectile.damage / 2) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), Projectile.damage + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), (Projectile.damage / 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(4))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(7))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
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

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Main.DiscoColor, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
     
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}

