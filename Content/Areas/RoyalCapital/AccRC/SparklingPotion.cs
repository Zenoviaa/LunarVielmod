using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.AccRC
{
    public class SparklingPotion : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<SparklingStarPlayer>().hasSparklingPotion = true;
            player.GetModPlayer<ArmorStatsPlayer>().insourceSlots += 1;
        }
    }

    public class SparklingStarPlayer : ModPlayer
    {
        public bool hasSparklingPotion;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSparklingPotion = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            int sparklingStarType = ModContent.ProjectileType<SparklingStar>();
            if(Player.whoAmI == Main.myPlayer && hasSparklingPotion && Player.ownedProjectileCounts[sparklingStarType] == 0)
            {
                int damage = 100;
                float num = 3f;
                for(int i = 0; i < num; i++)
                {
                    float offset = (float)i / num;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, sparklingStarType, damage, 1, Player.whoAmI, ai1: offset);
                }
                
            }
        }
    }

    public class SparklingStar : ModProjectile
    {
        private float Interp;
        private Vector2 OldVelocity;
        private Vector2 HomingVelocity;
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationOffset => ref Projectile.ai[1];
        private ref float HomingTimer => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            var player = Owner.GetModPlayer<SparklingStarPlayer>();
            if (player.hasSparklingPotion)
                Projectile.timeLeft = 2;
            Timer++;
            if(Timer % 48 == 0)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams();
                spawnParams.outerColor = Color.Goldenrod;
                spawnParams.gravity = 0f;
                DustParticle dp = DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
                dp.fast = true;
                dp.Scale *= .7f;
            }

            float radians = Timer * 0.05f;
            radians += MathHelper.TwoPi * RotationOffset;
            Vector2 vel = Vector2.UnitY.RotatedBy(radians) * 64;
            Vector2 targetPosition = Owner.Center + vel;
            Vector2 velocityTo = targetPosition - Projectile.Center;

            NPC nearest = NPCHelper.FindClosestNPC(targetPosition, 384);

            float interp = 0;
            float time = 90f;
            if(nearest != null)
            {
                HomingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, HomingVelocity, HomingTimer / time);
                OldVelocity = Projectile.velocity;
                HomingTimer++;
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(velocityTo, OldVelocity, HomingTimer / time);
                Interp = MathHelper.Lerp(Interp, 0, 0.01f);
                HomingTimer--;
            }
            HomingTimer = MathHelper.Clamp(HomingTimer, 0, time);
    
            Projectile.rotation += 0.05f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
