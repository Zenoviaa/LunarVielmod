using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Crossbows.Gemmed
{
    public class PlatinumCrossbowBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Bolt");

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 10;

            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;
            Projectile.arrow = true;
            Projectile.timeLeft = 380;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void OnKill(int timeleft)
        {
            for (var i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 0, default, 0.5f);
            Collision.AnyCollision(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);

        }
        public override void AI()
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.WoodFurniture, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, Scale: 0.6f);   //spawns dust behind it, this is a spectral light blue dust


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)

        {
            float speedX = Projectile.velocity.X;
            float speedY = Projectile.velocity.Y;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 0.5f, ProjectileID.RubyBolt, Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 1.3f, speedY * 0, ProjectileID.RubyBolt, (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f) ;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 1.5f, ProjectileID.RubyBolt, (int)(Projectile.damage * 0.7), 0f, Projectile.owner, 0f, 0f);
        }



    }

}