
using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Gothivia
{
    public class Helios : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ThornBall);
            AIType = ProjectileID.ThornBall;
            Projectile.penetrate = 9999;
            Projectile.width = 138;
            Projectile.height = 140;
            Projectile.timeLeft = 500;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
         
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame);
            }

            ShakeModSystem.Shake = 1;
            return false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Timer2;
        int moveSpeed = 0;
        public override void AI()
        {
            Projectile.rotation += 0.3f;
            Timer2++;
            Timer++;

            float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
            Player closestplayer = FindClosestNPC(maxDetectRadius);
            if (Projectile.Center.X >= closestplayer.Center.X && moveSpeed >= -90) // flies to players x position
                moveSpeed--;
            else if (Projectile.Center.X <= closestplayer.Center.X && moveSpeed <= 90)
                moveSpeed++;

            Projectile.velocity.X = moveSpeed * 0.05f;


            closestplayer.RotatedRelativePoint(Projectile.Center);

            // Trying to find NPC closest to the projectile

            if (closestplayer == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
       
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public Player FindClosestNPC(float maxDetectDistance)
        {
            Player closestplayer = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)

                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestplayer = target;
                }
            }

            return closestplayer;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OasisCactus);
            }
        }
    }
}