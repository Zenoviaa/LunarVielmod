using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.Vixyl;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    internal class VixylPlayer : ModPlayer
    {
        public int parryCooldown;
        public int parryTimer;

        public override void PostUpdateEquips()
        {
            parryTimer--;
            if(parryTimer <= 0)
            {
                parryTimer = 0;
            }

            parryCooldown--;
            if(parryCooldown <= 0)
            {
                parryCooldown = 0;
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (parryTimer > 0 && parryCooldown <= 0)
            {
                parryTimer = 0;
                ParryEffects();
                return true;
            }

            return false;
        }

        public void ParryEffects()
        {
            //Brief invulnerability after parrying
            // Some sound and visual effects
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.BlueCrystalShard, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f }, Player.position);

            //Spawn the big verlia slash projectile here
            //Setting the immune time
            Player.SetImmuneTimeForAllTypes(60);
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Add the buff and assigning the cooldown time
            int time = 180;
            Player.AddBuff(ModContent.BuffType<VixylDodgeBuff>(), time);

            Vector2 velocity = Player.Center.DirectionTo(Main.MouseWorld);
            Projectile.NewProjectile(Player.GetSource_FromThis(),
                Player.Center, velocity, ModContent.ProjectileType<VixylParryProj>(), Player.HeldItem.damage * 4, Player.HeldItem.knockBack, Player.whoAmI);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"), Player.position);

            parryCooldown = time;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendExampleDodgeMessage(Player.whoAmI);
            }
        }

        public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

            VixylPlayer vixylPlayer = Main.player[player].GetModPlayer<VixylPlayer>();
            vixylPlayer.ParryEffects();

            if (Main.netMode == NetmodeID.Server)
            {
                // If the server receives this message, it sends it to all other clients to sync the effects.
                SendExampleDodgeMessage(player);
            }
        }

        public static void SendExampleDodgeMessage(int whoAmI)
        {
            // This code is called by both the initial 
            ModPacket packet = ModContent.GetInstance<Stellamod>().GetPacket();
            packet.Write((byte)MessageType.Dodge);
            packet.Write((byte)whoAmI);
            packet.Send(ignoreClient: whoAmI);
        }
    }

    internal class Vixyl : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 34;
            Item.DamageType = DamageClass.Generic;

            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(gold: 30);
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.rare = ModContent.RarityType<GothiviaSpecialRarity>();
            Item.shoot = ModContent.ProjectileType<VixylSwordProj>();
            Item.shootSpeed = 15;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            VixylPlayer vixylPlayer = player.GetModPlayer<VixylPlayer>();
            if(player.HasBuff(ModContent.BuffType<VixylDodgeBuff>()))
            {
                //Verli spam slashes
                type = ModContent.ProjectileType<VixylSlashProj>();
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), position);
            } 
            else if (vixylPlayer.parryCooldown <= 0 && !player.immune)
            {
                vixylPlayer.parryTimer = 18;
                //Normal slash
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSheethe"), position);
            }
        }
    }
}
