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
        private float _hitTimer;
        private NPC _target;
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float Speed => ref Projectile.ai[1];
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
            Timer++;
            BellMasterPlayer bellMasterPlayer = Owner.GetModPlayer<BellMasterPlayer>();
            if (bellMasterPlayer.hasBellMasterSetBonus)
                Projectile.timeLeft = 2;

            //Movement code
            if(_target == null || !_target.active)
            {
                NPC nearest = NPCHelper.FindClosestNPC(Owner.position, 1500);
                _target = nearest;
            }

            if (Main.rand.NextBool(12))
            {
                Vector2 pos = Projectile.position;
                pos.X += Main.rand.Next(0, Projectile.width);
                pos.Y += Main.rand.Next(0, Projectile.height);
                SparkleParticle sp = SparkleParticle.Spawn(pos, Vector2.Zero);
                sp.innerColor = Color.Goldenrod;
                sp.outerColor = Color.DarkGoldenrod;
                sp.flickering = true;
                sp.Scale *= 0.5f;
                sp.gravity *= 0.5f;
            }


            Vector2 targetCenter = Owner.Center;
            float offset = 128;
            if(_target != null && _target.active)
            {
 
                targetCenter = _target.Center;
                offset = MathF.Max(_target.width, _target.height) + 64;
                if (Vector2.Distance(targetCenter, Owner.Center) > 800)
                    _target = null;
            } else
            {

            }
      
            float speedValue1 = MathHelper.Max(12, Owner.velocity.Length());
            float speedValue2 = Vector2.Distance(Projectile.Center, targetCenter) * 0.05f;
            float s = MathHelper.Max(speedValue1, speedValue2);

            if (Vector2.Distance(Projectile.Center, Owner.Center) > 2500)
                Projectile.Center = Owner.Center;
            Speed = MathHelper.Lerp(Speed, s, 0.1f);

                
            Vector2 rotatedCenter = targetCenter + Vector2.UnitY.RotatedBy(Timer * 0.015f) * offset;
            Vector2 velocityToMove = rotatedCenter - Projectile.Center ;
            Vector2 normalVelocity = velocityToMove.SafeNormalize(Vector2.Zero);
            float speed = MathF.Min(Vector2.Distance(rotatedCenter, Projectile.Center), Speed);
            Projectile.velocity = normalVelocity * speed;
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if (_hitTimer > 0)
            {
                _hitTimer--;
            }

            Projectile.scale = MathHelper.SmoothStep(1f, 0.5f, _hitTimer / 15f);
            //Ricochet
            Rectangle myRect = Projectile.getRect();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type == Type)
                    continue;
                if (proj.hostile)
                    continue;

                Rectangle projRect = proj.getRect();
                if (!proj.Colliding(myRect, projRect))
                    continue;

                //Needs to be reflected
                GoldenBellGlobalProjectile goldenBellGlobalProjectile = proj.GetGlobalProjectile<GoldenBellGlobalProjectile>();
                if (goldenBellGlobalProjectile.isReflected)
                    continue;

                Vector2 reflectedCenter;
                if (_target != null)
                    reflectedCenter = _target.Center;
                else
                {
                    reflectedCenter = Projectile.Center + proj.velocity.RotatedBy(MathHelper.PiOver2);
                }

                    Vector2 newVelocity = (reflectedCenter - proj.Center).SafeNormalize(Vector2.Zero) * proj.velocity.Length();
                proj.velocity = newVelocity;

                _hitTimer = 15;
                SoundStyle hitSound = AssetRegistry.Sounds.Bishinine.BellHit1;
                hitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hitSound, Projectile.position);

                ThrustParticle tp = ThrustParticle.Spawn(Projectile.Center, proj.velocity * 0.15f);
                tp.Scale *= 0.66f;
                tp.innerColor = Color.Goldenrod;
                tp.bloomColor = Color.DarkGoldenrod;
                for(float f =0; f < 3f; f++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        scaleRange = new Vector2(0.3f, 0.6f)
                    };
                    DustParticle.Spawn(Projectile.Center, proj.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.5f, 1f) * 0.2f, spawnParams);

                    if (Main.rand.NextBool(3))
                    {
                        var dp = SparkleParticle.Spawn(Projectile.Center, proj.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f) * 0.2f);
                        dp.Scale *= 0.5f;
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
            for(float f = 0f; f < 4f; f++)
            {
                float rot = MathHelper.TwoPi * f / 4f;
                Vector2 offset = rot.ToRotationVector2() * ExtraMath.Osc(0f, 8);
                Color glowCOlor = Color.Gold;
                glowCOlor *= 0.1f;
                glowCOlor.A = 0;
                spriteBatch.Draw(texture, drawCenter + offset, null, glowCOlor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowOrigin = glowMask.Size() * 0.5f;
            Color glowMaskColor = Color.Gold;
            glowMaskColor *= 0.25f;
            glowMaskColor.A = 0;
            spriteBatch.Draw(glowMask, drawCenter , null, glowMaskColor, Projectile.rotation, glowOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
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
            if (!hasBellMasterSetBonus)
                return;
            int bellType = ModContent.ProjectileType<GoldenBell>();
            if (Player.ownedProjectileCounts[bellType] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, bellType, 1, 1, Player.whoAmI);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class BellMasterHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<BellMasterHood, BellMasterCloak, BellMasterLegs>(ArmorGroup.Act_II);
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
