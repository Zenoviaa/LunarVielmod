using Stellamod.Common.ArmorRework;
using Stellamod.Content.Gores;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Ravaging
{
    public class RavagerRockFriendly : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // this.Outline(Color.Red, ref lightColor);
            Projectile.scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 30f));
            this.DrawCentered(ref lightColor);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
            }
            if (Projectile.velocity.Y < 5)
            {
                Projectile.velocity.Y = 5;
            }
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.15f;
            Projectile.velocity.Y *= 1.01f;
            Projectile.rotation += 0.015f;
            Projectile.rotation += Projectile.velocity.Length() * 0.002f;
            NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 1000);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }

            SoundStyle smashSound;
            int sound = Main.rand.Next(3);
            switch (sound)
            {
                default:
                case 1:
                    smashSound = new SoundStyle("Stellamod/Assets/Sounds/RockBreak1");
                    break;
                case 2:
                    smashSound = new SoundStyle("Stellamod/Assets/Sounds/RockBreak2");
                    foreach (int g in gores)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                    }
                    FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                    break;
            }



            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);
            float numDust = 8;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(1f);
                SmokeParticle sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(48, 48), velocity, Scale: Main.rand.NextFloat(1f, 1.5f));
                sp.initialColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            }
        }
    }

    public class RavagingPlayer : ModPlayer
    {
        public bool hasRavagingSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasRavagingSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (!hasRavagingSetBonus)
                return;
            DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 7;
            dashPlayer.DashDuration += 6;
            if (Player.whoAmI != Main.myPlayer)
                return;

            if (dashPlayer.IsDashing)
            {
                Rectangle playerRectangle = Player.getRect();
                int type = ModContent.ProjectileType<RavagerRockFriendly>();
                int damage = Player.HeldItem.damage;
                foreach (var npc in Main.ActiveNPCs)
                {
                    Rectangle npcRectangle = npc.getRect();
                    if (!playerRectangle.Intersects(npcRectangle))
                        continue;
                    if (dashPlayer.DashedThroughSet.Contains(npc))
                        continue;

                    dashPlayer.DashedThroughSet.Add(npc);
                    //Spawn falling projectile
                    Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Top - new Vector2(0, 500),
                        Vector2.UnitY, type, damage, 1, Player.whoAmI);
                }
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class RavagingHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<RavagingHelmet, RavagingChestplate, RavagingLegs>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.criticalStrikeDamage += 1.75f;
            stats.defenseBonus += 3;
            stats.accessorySlots += 1;
            stats.stamina += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<RavagingChestplate>()
                && legs.type == ModContent.ItemType<RavagingLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<RavagingPlayer>().hasRavagingSetBonus = true;

        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class RavagingChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
        }


        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.accessorySlots += 1;
            stats.stamina += 2;
            stats.defenseBonus += 3;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class RavagingLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 2;
            stats.criticalStrikeChance += 0.07f;
            stats.movementSpeedBonus += 0.2f;
        }
    }
}
