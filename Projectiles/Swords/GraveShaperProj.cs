using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{

    public class GraveShaperProj : ModProjectile
    {
        public bool Hit;
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 44;

            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.4f;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;

            Projectile.penetrate = 8;
            AIType = -1;
        }

        public override void AI()
        {
            if (Hit)
            {

                Projectile.velocity *= .96f;
                Projectile.ai[1]++;

                Vector2 ParOffset;
                if (Projectile.ai[1] >= 60)
                {
                    ParOffset.X = Projectile.Center.X - 18;
                    ParOffset.Y = Projectile.Center.Y;
  
                    Projectile.velocity.Y += 3.2f;
                }

            }
            else
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 100)
                {
                    Player owner = Main.player[Projectile.owner];
                    Vector2 directionToPlayer = Projectile.Center.DirectionTo(owner.Center);
                    float speed = Projectile.velocity.Length();
                    Projectile.velocity = directionToPlayer * speed;
                }
            }   
        }



        public override void PostDraw(Color lightColor)
        {
            Vector2 pos = base.Projectile.Center;
            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, pos - Main.screenPosition, null, lightColor, base.Projectile.rotation + (float)Math.PI / 4f + (float)Math.PI / 2f, TextureAssets.Projectile[Projectile.type].Value.Size() / 2f, base.Projectile.scale, SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Teal.ToVector3() * 1.75f * Main.essScale);

        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleManager.NewParticle(Projectile.Center, Projectile.velocity * 0, ParticleManager.NewInstance<GraveSpin>(), Color.Purple, 0.3f, Projectile.whoAmI);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Hit)
            {
                Projectile.velocity = Projectile.velocity / 3;

                Hit = true;
                target.position = Projectile.Center;
            }


        }
    }
}