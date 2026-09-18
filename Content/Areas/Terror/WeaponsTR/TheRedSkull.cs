using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Magic;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class RedSkullPlayer : ModPlayer
    {
        public int npcWhoAmI;
        public int hitCount;
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);

            //Need to have this bow too
            if (Player.HeldItem.type != ModContent.ItemType<TheRedSkull>())
                return;

            //Arrows only
            if (!proj.arrow)
                return;

            if (npcWhoAmI != target.whoAmI)
            {
                npcWhoAmI = target.whoAmI;
                hitCount = 0;
            }
            else
            {
                hitCount++;
                SoundStyle sound = new SoundStyle("Stellamod/Assets/Sounds/Pericarditis");
                sound.Pitch = MathHelper.Lerp(0f, 1f, hitCount / 3f);
                SoundEngine.PlaySound(sound, target.position);
                if (hitCount < 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (Main.rand.NextBool(4))
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, default, 1f).noGravity = true;
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, Color.Red, 1f).noGravity = true;
                    }
                }

                if (hitCount == 2)
                {
                    for (float f = 0; f < 16; f++)
                    {
                        Vector2 vel = (f / 16f * MathHelper.ToRadians(360)).ToRotationVector2() * -4;
                        SparkleParticle sp = SparkleParticle.Spawn(target.Center - vel * 16, vel, Scale: 0.25f);
                        sp.innerColor = Color.Red;
                        sp.gravity = 0;
                        sp.noTileCollide = true;
                    }
                }

                if (hitCount >= 3)
                {
                    for (float f = 0; f < 32; f++)
                    {
                        Vector2 vel = -Vector2.UnitY * 8 * Main.rand.NextFloat(0.2f, 1f);
                        Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(32, 32), DustID.RedTorch, vel, Scale: 1.5f, newColor: Color.Red);

                    }
                    for (int i = 0; i < 7; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default, 1f).noGravity = true;
                    }
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), proj.position);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), proj.position);
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        float progress = i / 8f;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 vel = rot.ToRotationVector2() * 4;
                        Dust.NewDustPerfect(proj.Center, DustID.RedTorch, vel, Scale: 1f);
                    }


                    Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.Zero,
                        ModContent.ProjectileType<PericarditisBoom>(), proj.damage, proj.knockBack, proj.owner);

                    FXUtil.ShakeCamera(target.Center, 1024, 4);
                    modifiers.FinalDamage *= 2;
                    hitCount = 0;
                }
            }
        }
    }

    public class TheRedSkull : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 23;
            Item.rare = ItemRarityID.Green;
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.BurstShot(3, 5, shootParams.velocity, shootParams.chargeStrength);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<TerrorFragments>());
        }
    }
}
