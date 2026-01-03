using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class OrbitingLeaf : ModProjectile
    {
        private float Distance => 42;
        private ref float Timer => ref Projectile.ai[0];
        private ref float RadiansOffset => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
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
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.tileCollide = false;
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
            BoxOfLeavesPlayer leavesPlayer = Owner.GetModPlayer<BoxOfLeavesPlayer>();
            if (leavesPlayer.hasBoxOfLeaves)
                Projectile.timeLeft = 2;
            float rot = Timer * 0.04f;
            rot += RadiansOffset;
            Vector2 vel = rot.ToRotationVector2();
            vel *= Distance;
            Vector2 targetCenter = Owner.Center + vel;
            Projectile.velocity = targetCenter - Projectile.Center;
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
    public class BoxOfLeavesPlayer : ModPlayer
    {
        public bool hasBoxOfLeaves;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasBoxOfLeaves = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasBoxOfLeaves)
                return;
            if (Main.myPlayer != Player.whoAmI)
                return;
            int projType = ModContent.ProjectileType<OrbitingLeaf>();
            if (Player.ownedProjectileCounts[projType] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projType, 4, 1, Player.whoAmI, ai1: MathHelper.ToRadians(180));
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projType, 4, 1, Player.whoAmI);
            }
        }
    }

    public class BoxOfLeaves : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<BoxOfLeavesPlayer>().hasBoxOfLeaves = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Ivythorn>());
        }
    }
}
