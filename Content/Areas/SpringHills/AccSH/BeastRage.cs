using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class BeastRagePlayer : ModPlayer
    {
        public bool hasBeastRage;
        public override void Load()
        {
            base.Load();
            FlaskPlayer.OnProc += ApplyBeastRage;
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasBeastRage = false;
        }
        public override void Unload()
        {
            base.Unload();
            FlaskPlayer.OnProc -= ApplyBeastRage;
        }
        private void ApplyBeastRage(Player player)
        {
            BeastRagePlayer beastRagePlayer = player.GetModPlayer<BeastRagePlayer>();
            if (!beastRagePlayer.hasBeastRage)
                return;
            player.AddBuff(ModContent.BuffType<RagingBeast>(), 240);
            SoundStyle rageSound = new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Spawn");
            rageSound.Volume = 0.4f;
            SoundEngine.PlaySound(rageSound, player.position);
            FXUtil.ShakeCamera(player.position, 1024, 8);
            ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.Red, 0.2f, 15);
        }

    }

    public class RagingBeast : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetAttackSpeed(DamageClass.Generic) += 0.2f;
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetModPlayer<DashPlayer>().DashRegenerationBonus += 0.5f;
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                LegacyParticle.NewParticle<EmberParticle>(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            }
        }
    }
    public class BeastRage : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<BeastRagePlayer>().hasBeastRage = true;
        }
    }
}
