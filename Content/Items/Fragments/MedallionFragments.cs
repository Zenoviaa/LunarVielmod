using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.CollectionSystem.Medallion;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Fragments
{
    internal abstract class BaseMedallionFragment : ModItem
    {
        public virtual int Number { get; }
        public LocalizedText Effect { get; set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Effect = this.GetLocalization("Effect");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.UseSound = SoundID.AchievementComplete;
            Item.maxStack = 1;
            Item.consumable = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Goldenrod);
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }


        public override bool? UseItem(Player player)
        {
            float num = 12;
            for (float i = 0; i < num; i++)
            {
                float rot = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(5f, 25f);
                var particle = FXUtil.GlowStretch(player.position, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.Goldenrod;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.04f, 0.07f);
                particle.VectorScale *= 0.5f;
            }
            return true;
        }
        public virtual bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return true;
        }
    }

    internal class MakMedal : BaseMedallionFragment
    {
        public override int Number => 1;
        public override bool? UseItem(Player player)
        {
            FragmentPlayer fragmentPlayer = player.GetModPlayer<FragmentPlayer>();
            fragmentPlayer.hasMak = true;
            return base.UseItem(player);
        }
        public override bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return fragmentPlayer.hasMak;
        }
    }
    internal class DraekusMedal : BaseMedallionFragment
    {
        public override int Number => 2;
        public override bool? UseItem(Player player)
        {
            FragmentPlayer fragmentPlayer = player.GetModPlayer<FragmentPlayer>();
            fragmentPlayer.hasDraekus = true;
            return base.UseItem(player);
        }
        public override bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return fragmentPlayer.hasDraekus;
        }
    }
    internal class ApilithyMedal : BaseMedallionFragment
    {
        public override int Number => 3;
        public override bool? UseItem(Player player)
        {
            FragmentPlayer fragmentPlayer = player.GetModPlayer<FragmentPlayer>();
            fragmentPlayer.hasApilithy = true;
            return base.UseItem(player);
        }
        public override bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return fragmentPlayer.hasApilithy;
        }
    }
    internal class GothiviaMedal : BaseMedallionFragment
    {
        public override int Number => 4;
        public override bool? UseItem(Player player)
        {
            FragmentPlayer fragmentPlayer = player.GetModPlayer<FragmentPlayer>();
            fragmentPlayer.hasGothivia = true;
            return base.UseItem(player);
        }
        public override bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return fragmentPlayer.hasGothivia;
        }
    }
    internal class KariMedal : BaseMedallionFragment
    {
        public override int Number => 5;
        public override bool? UseItem(Player player)
        {
            FragmentPlayer fragmentPlayer = player.GetModPlayer<FragmentPlayer>();
            fragmentPlayer.hasKari = true;
            return base.UseItem(player);
        }
        public override bool HasUnlocked(FragmentPlayer fragmentPlayer)
        {
            return fragmentPlayer.hasKari;
        }
    }
}
