using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.XixianFlaskSystem
{


    public class InsourceDefaultProjectile : ModProjectile
    {
        public override string Texture => AssetHelper.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float InsourceType => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 60;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 3;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            FlaskPlayer flaskPlayer = owner.GetModPlayer<FlaskPlayer>();
            if (flaskPlayer.Insource.type == (int)InsourceType)
            {
                Projectile.timeLeft = 3;
            }

            Timer++;
            Projectile.Center = CalculateCirclePosition(owner);
            Visuals();
        }

        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int count = 1;
            float between = 360 / (float)count;
            float degrees = between * minionIndex;
            float circleDistance = 96;
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + Timer * 0.2f));
            return circlePosition;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int insourceType = (int)InsourceType;
            ModItem item = ModContent.GetModItem(insourceType);
            string projectileTexturePath = item.Texture + "Proj";

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(projectileTexturePath).Value;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            spriteBatch.Draw(texture, drawPos, Projectile.Frame(overrideTexture: texture), drawColor, drawRotation,
                Projectile.Frame(overrideTexture: texture).Size() / 2f, drawScale, SpriteEffects.None, 0);
            return false;

        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 1);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}