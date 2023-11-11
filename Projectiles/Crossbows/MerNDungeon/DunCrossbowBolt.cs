using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.MerNDungeon
{
    public class DunCrossbowBolt : ModProjectile
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
            Projectile.tileCollide = false;
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
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedX * 1, speedY * 1, ProjectileID.BoneDagger, Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedX * 1.2f, speedY * 0.8f, ProjectileID.BoneDagger, Projectile.damage * 3, 0f, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedX * 0.6f, speedY * 1.4f, ProjectileID.BoneDagger, Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedX * 0.8f, speedY * 1.2f, ProjectileID.BoneGloveProj, Projectile.damage * 4, 0f, Projectile.owner, 0f, 0f);
        }



    }

}