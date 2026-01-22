using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Content.Armors.Jackler;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.BellMaster
{
    public class GoldenBellGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool isReflected;
        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
            isReflected = false;
        }

        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            if (isReflected && projectile.velocity.Length() < 35)
                projectile.velocity *= 1.1f;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(projectile, target, ref modifiers);
            if (isReflected)
                modifiers.FinalDamage *= 1.5f;
        }
    }


    public class GoldenBell : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
        }
        public override void AI()
        {
            base.AI();
            BellMasterPlayer bellMasterPlayer = Owner.GetModPlayer<BellMasterPlayer>();
            if (bellMasterPlayer.hasBellMasterSetBonus)
                Projectile.timeLeft = 2;

            //Movement code
            NPC nearest = NPCHelper.FindClosestNPC(Owner.position, 1500);
            Vector2 targetCenter = Owner.Center;
            float offset = 128;
            if(nearest != null)
            {
                targetCenter = nearest.Center;
                offset = MathF.Max(nearest.width, nearest.height) + 64;
            }

            Vector2 rotatedCenter = targetCenter + Vector2.UnitY.RotatedBy(Timer * 0.05f) * offset;
            Vector2 velocityToMove = rotatedCenter - Projectile.Center ;
            Vector2 diff = velocityToMove - Projectile.velocity;
            Projectile.velocity += diff * 0.3f;
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            //Ricochet
            Rectangle myRect = Projectile.getRect();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type == Type)
                    continue;

                Rectangle projRect = proj.getRect();
                if (!myRect.Intersects(projRect))
                    continue;

                //Needs to be reflected
                GoldenBellGlobalProjectile goldenBellGlobalProjectile = proj.GetGlobalProjectile<GoldenBellGlobalProjectile>();
                if (goldenBellGlobalProjectile.isReflected)
                    continue;

                Vector2 newVelocity = (nearest.Center - proj.Center).SafeNormalize(Vector2.Zero) * proj.velocity.Length();
                proj.velocity = newVelocity;

                SoundStyle hitSound = AssetRegistry.Sounds.Bishinine.BellHit1;
                hitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hitSound, Projectile.position);

                ThrustParticle.Spawn(Projectile.Center, proj.Center - Projectile.Center);
                for(float f =0; f < 3f; f++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        scaleRange = new Vector2(0.3f, 0.6f)
                    };
                    DustParticle.Spawn(Projectile.Center, proj.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);

                    if (Main.rand.NextBool(3))
                    {
                        var dp = SparkleParticle.Spawn(Projectile.Center, proj.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f));
                        dp.Scale *= 0.85f;
                        dp.dampening = 0.1f;
                        dp.flickering = true;
                        dp.innerColor = Color.White;
                        dp.outerColor = Color.Gold;
                        dp.gravity = 0f;
                    }
                }
               
                goldenBellGlobalProjectile.isReflected = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            drawCenter.Y += ExtraMath.Osc(-1f, 1f);
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class BellMasterPlayer : ModPlayer
    {
        public bool hasBellMasterSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasBellMasterSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.myPlayer != Player.whoAmI)
                return;

        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class BellMasterHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<BellMasterHood, BellMasterCloak, BellMasterLegs>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.criticalStrikeDamage += 0.45f;
            stats.rangedGunAmmoAmountPct -= 0.5f;
            stats.accessorySlots++;
            stats.defenseBonus += 10;

        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == Type && body.type == ModContent.ItemType<BellMasterCloak>() && legs.type == ModContent.ItemType<BellMasterLegs>();
        }
        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
            player.GetModPlayer<BellMasterPlayer>().hasBellMasterSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class BellMasterCloak : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.accessorySlots++;
            stats.rangedDamage += 0.15f;
            stats.defenseBonus += 12;

        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class BellMasterLegs : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.accessorySlots++;
            stats.rangedBowChargeTime += 0.2f;
            stats.defenseBonus += 5;
        }
    }
}
