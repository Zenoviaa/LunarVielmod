using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Steins;

public class ShadingShot : ModProjectile
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("MeatBall");
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        //The recording mode
    }

    public override void SetDefaults()
    {
        Projectile.damage = 12;
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.light = 1.5f;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ownerHitCheck = true;
        Projectile.timeLeft = 360;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
    }
    private ref float Timer => ref Projectile.ai[0];
    public override void OnKill(int timeLeft)
    {
        if (this.OwnedByLocalClient())
        {
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<Shit1>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
        }

        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
    }

    public float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(5, 3, completionRatio);
    }
    public float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 3f;
    }
    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.LightBlue, Color.Transparent, completionRatio) * 0.7f;
    }

    private void DrawPixelatedBloomTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.LightBlue;
        laserShader.OuterColor = Color.DeepSkyBlue;
        laserShader.InnerColor = Color.LightSkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader bloomTrail = BloomTrailShader.Instance;
        bloomTrail.InnerColor = Color.LightBlue;
        bloomTrail.OuterColor = Color.DeepSkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloomTrail, Projectile.Size * 0.5f);
    }

    private void DrawPixelatedFlash(SpriteBatch sb, Vector2 sp)
    {
        Asset<Texture2D> flashTexture = AssetManager.GlowMask.StarFlare1;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(flashTexture, Projectile.Center);
        drawer.scale = new Vector2(1f, 0.06f) * 0.25f * ExtraMath.Osc(0.5f, 1f, speed: 4f, offset: Projectile.whoAmI);
        drawer.color = Color.Lerp(Color.LightSkyBlue * 0.5f, Color.LightSkyBlue, ExtraMath.Osc(0f, 1f, speed: 3));
        drawer.color.A = 0;
        sb.Draw(drawer);
        drawer.rotation = MathHelper.PiOver2;
        sb.Draw(drawer);

        drawer.rotation = 0;
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.8f;
        sb.Draw(drawer);

        drawer.rotation = MathHelper.PiOver2;
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBloomTrail);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlash);
        return false;
    }



    public override void AI()
    {

        Timer++;


        float maxDetectRadius = 2f; // The maximum radius at which a projectile can detect a target
        float projSpeed = 30f; // The speed at which the projectile moves towards the target





        if (Timer > 80)
        {
            maxDetectRadius = 3000f;
        }

















        // Trying to find NPC closest to the projectile
        NPC closestNPC = FindClosestNPC(maxDetectRadius);
        if (closestNPC == null)
            return;

        // If found, change the velocity of the projectile and turn it in the direction of the target
        // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
        Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);

    }
    // Finding the closest NPC to attack within maxDetectDistance range
    // If not found then returns null
    public NPC FindClosestNPC(float maxDetectDistance)
    {
        NPC closestNPC = null;

        // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        // Loop through all NPCs(max always 200)
        for (int k = 0; k < Main.maxNPCs; k++)
        {
            NPC target = Main.npc[k];
            // Check if NPC able to be targeted. It means that NPC is
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            if (target.CanBeChasedBy())
            {
                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }
        }

        Projectile.rotation += 0.1f;
        {


            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y > 25f)
            {
                Projectile.velocity.Y = 25f;
            }
        }
        return closestNPC;
    }



}
