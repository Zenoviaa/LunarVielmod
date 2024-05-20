using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class ClimateTornadoProj : ModProjectile
    {
        private static Asset<Texture2D> VorTexture;
        private Projectile Parent
        {
            get => Main.projectile[(int)Projectile.ai[0]];
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 18;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.scale = 1.3f;
            Projectile.usesLocalNPCImmunity = true;

            //5 ticks local npc immunity
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.ownedProjectileCounts[ModContent.ProjectileType<CloudMinionProj>()] <= 6)
            {
                Projectile.Kill();
            }
            Projectile.Center = Parent.Center;
            Projectile.rotation += 0.5f;

            Lighting.AddLight(Projectile.position, 1.5f, 0.7f, 2.5f);
            Lighting.Brightness(2, 2);

            float suckingStrength = 0.95f;
            float suckingDistance = 128;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= suckingDistance)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * suckingStrength;
                    }
                }
            }
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 18)
                {
                    Projectile.frame = 0;
                }
            }

            return true;
        }

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
            VorTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/VoxTexture");
        }

        public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
          // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
            VorTexture = null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = new(100, 255, 255, 0);
            Main.EntitySpriteDraw(VorTexture.Value, Projectile.Center - Main.screenPosition,
                          VorTexture.Value.Bounds, drawColor, Projectile.rotation,
                          VorTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            return true;
        }
    }
}
