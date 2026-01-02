using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IvynBow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 4;
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            for (float f = 0; f < 6; f++)
            {
                Vector2 position = shootParams.position;
                Vector2 velocity = shootParams.velocity * shootParams.chargeStrength * 32;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(0.2f, 1f);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LeafShot>(), (int)bowDamage, 0, player.whoAmI);
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<Ivythorn>());
        }
    }

    public class LeafShot : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 12 == 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_LivingWood);
                Main.dust[d].noGravity = true;
            }

            Projectile.velocity.X *= 0.93f;
            Projectile.velocity.Y += 0.05f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int trailLength = Projectile.oldPos.Length;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;
            for (int t = 0; t < trailLength; t++)
            {
                float l = trailLength;
                float interpolant = (float)t / l;
                Vector2 oldPos = Projectile.oldPos[t];
                oldPos -= Main.screenPosition;
                oldPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldPos, frame, drawColor * MathHelper.SmoothStep(0.5f, 0f, interpolant), Projectile.oldRot[t], drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}