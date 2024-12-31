using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;

namespace Stellamod.NPCs.Bosses.Fenix.Projectiles
{
    public class Angelu : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ThornBall);
            AIType = ProjectileID.ThornBall;
            Projectile.penetrate = 5;
            Projectile.width = 178;
            Projectile.height = 144;
            Projectile.scale = 1.5f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 0f), 0f);
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            var entitySource = Projectile.GetSource_FromThis();
            NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
            NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y - 600, ModContent.NPCType<STARBOMBERLASERWARN>());
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
            }

            ShakeModSystem.Shake = 5;
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY + 2 * 2, ModContent.ProjectileType<RuneSpawnEffect>(), 0, 0f, 0, 0f, 0f);
            return false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Timer2;
        int moveSpeed = 0;
        //int moveSpeedY = 0;
        public override void AI()
        {
            Projectile.rotation += 0.3f;
            Timer2++;
            Timer++;

            float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
            Player closestplayer = FindClosestNPC(maxDetectRadius);
            if (closestplayer != null)
            {
                if (Projectile.Center.X >= closestplayer.Center.X && moveSpeed >= -90) // flies to players x position
                    moveSpeed--;
                else if (Projectile.Center.X <= closestplayer.Center.X && moveSpeed <= 90)
                    moveSpeed++;
            }


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

        float alphaCounter = 1;
        Vector2 DrawOffset;
        

        public override void PostDraw(Color lightColor)
        {

            /*
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Color.Goldenrod;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            */
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
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

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = 1 * (Projectile.width / 4) * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightSkyBlue, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = new Color(255, 255, 255, 255);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale / 2, SpriteEffects.None, 0);

            return false;
        }
    }
}