using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Summons
{

    public class VoltuxPortal : ModProjectile
    {
        private static Asset<Texture2D> VorTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 280;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = -1;
            Projectile.stepSpeed = 1f;
            Projectile.alpha = 255;
            Projectile.scale = 0;
        }

        public override void AI()
        {
            float timeLeft = Projectile.timeLeft;
            if(timeLeft > 250)
            {
                float progress = (280f - timeLeft) / 30f;
                float easedProgress = Easing.OutCirc(progress);
                Projectile.scale = 1 * easedProgress;
            } 
            else if (timeLeft < 30)
            {
                float progress = timeLeft / 30f;
                float easedProgress = Easing.OutCirc(progress);
                Projectile.scale = 1 * easedProgress;
            }

            Projectile.rotation += 0.12f;
            Projectile.velocity.X *= 0.0f;
            Projectile.velocity.Y *= 0.01f;
            Lighting.AddLight(Projectile.position, 1.5f, 0.7f, 2.5f);
            Lighting.Brightness(2, 2);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 200)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.5f;
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
            Projectile.ownerHitCheck = true;

            int radius = 250;

            // Damage enemies within the splash radius
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {
                    int damage = Projectile.damage * 2;
                    target.SimpleStrikeNPC(damage: 12, 0);
                }
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed * 11, Scale: 3f);
                d.noGravity = true;
            }
        }

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
            VorTexture = Request<Texture2D>("Stellamod/Assets/Effects/VoxTexture");
        }
  
        public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
          // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
            VorTexture = null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = new(255, 255, 255, 0);
            Main.EntitySpriteDraw(VorTexture.Value, Projectile.Center - Main.screenPosition,
                          VorTexture.Value.Bounds, drawColor, Projectile.rotation,
                          VorTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
