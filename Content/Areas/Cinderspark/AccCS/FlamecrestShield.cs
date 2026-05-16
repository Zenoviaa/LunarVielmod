using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS
{
    public class FlamecrestGlobalProjectile : GlobalProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            int[] resistedProjectiles = new int[]
            {
                    ProjectileID.Flames,
                    ProjectileID.FlamethrowerTrap,
                    ProjectileID.Fireball,
                    ProjectileID.EyeBeam,
                    ProjectileID.FlamingScythe,
                    ProjectileID.CultistBossFireBall,
                    ProjectileID.CultistBossFireBallClone,
                    ProjectileID.DD2BetsyFireball,
                    ProjectileID.DD2BetsyFlameBreath,
                    ProjectileID.GreekFire1,
                    ProjectileID.GreekFire2,
                    ProjectileID.GreekFire3,
                    ProjectileID.InfernoHostileBlast,
                    ProjectileID.InfernoHostileBolt
            }; 
            for (int n = 0; n < resistedProjectiles.Length; n++)
            {
                ProjectileSets.ResistedByFlamecrestShield[resistedProjectiles[n]] = true;
            }
        }
    }
    public class FlamecrestGlobalNPC : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
    }

    public class FlamecrestShieldBubble : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle fireSound = AssetRegistry.Sounds.MagicWand.FireCharge;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }

            if(Timer % 64 == 0)
            {
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Top + Main.rand.NextVector2Circular(8, 8),
                      innerColor: Color.Yellow,
                      glowColor: Color.Orange,
                      outerGlowColor: Color.Red,
                      baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                      duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = -Vector2.UnitY.RotatedByRandom(0.6f) * 8;
                particle.Scale *= 0.5f;
                particle.Rotation = particle.Velocity.ToRotation();
            }
            Projectile.Center = Owner.Center;
            FlamecrestPlayer flamecrestPlayer = Owner.GetModPlayer<FlamecrestPlayer>();
            if (flamecrestPlayer.hasFlamecrestShield && flamecrestPlayer.resistCooldown <= 0 && !flamecrestPlayer.hideVisual)
            {
                Projectile.timeLeft = 2;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            BubbleShader bubbleShader = BubbleShader.Instance;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: bubbleShader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White * EasingFunction.InOutSine(Timer / 60f), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
    }
    public class FlamecrestPlayer : ModPlayer
    {
        public bool hasFlamecrestShield;
        public float resistCooldown;
        public bool hideVisual;
        public override void ResetEffects()
        {
            hasFlamecrestShield = false;
            hideVisual = false;        
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            resistCooldown--;
            if(hasFlamecrestShield && resistCooldown <= 0)
            {
                Player.AddBuff(ModContent.BuffType<FlamecrestShieldBuff>(), 2);
            }

            if(hasFlamecrestShield && Player.ownedProjectileCounts[ModContent.ProjectileType<FlamecrestShieldBubble>()] == 0 && 
                Main.myPlayer == Player.whoAmI && resistCooldown <= 0 && !hideVisual)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, 
                    ModContent.ProjectileType<FlamecrestShieldBubble>(), 1, 1, Player.whoAmI);
            }
        }

        private void BlockVisuals()
        {
            SoundEngine.PlaySound(SoundID.NPCHit42, Player.position);
            SoundEngine.PlaySound(SoundID.Item45, Player.position);
            SoundStyle fireSound = AssetRegistry.Sounds.MagicWand.FireChargeShot;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Player.position);

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                //     rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Player.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: 0.1f,
                    duration: 15);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (float f = 0; f < 16; f++)
            {
                float lerp = f / 16f;

                Vector2 pos = Player.Center;
                Vector2 velocity = Vector2.UnitY.RotatedBy(lerp * MathHelper.TwoPi) * 16;
                var part = LegacyParticle.NewParticle<FlareParticle>(pos, velocity);
            }

            int combatText = CombatText.NewText(Player.getRect(), Color.OrangeRed, LangText.Misc("FlamecrestPlayer"), true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 60;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (resistCooldown > 0)
                return;
            bool isResisted = NPCSets.ResistedByFlamecrestShield[npc.type];
            if (!isResisted)
                return;

            if (hasFlamecrestShield)
            {
                resistCooldown = 600;
                //50% less damage from fire-based sources
                modifiers.FinalDamage *= 0.9f;
                BlockVisuals();
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (resistCooldown > 0)
                return;
            bool isResisted = ProjectileSets.ResistedByFlamecrestShield[proj.type];
            if (!isResisted)
                return;

            if (hasFlamecrestShield)
            {
                resistCooldown = 600;
                //50% less damage from fire-based sources
                modifiers.FinalDamage *= 0.9f;
                BlockVisuals();
            }
        }

        public bool ConsumeShield()
        {
            if (!hasFlamecrestShield)
                return false;
            if (resistCooldown > 0)
                return false;

            resistCooldown = 600;
            BlockVisuals();
            return true;
        }
    }

    public class FlamecrestShieldBuff : ModBuff
    {

    }

    public class FlamecrestShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FlamecrestPlayer>().hideVisual = hideVisual;
            player.GetModPlayer<FlamecrestPlayer>().hasFlamecrestShield = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
}
