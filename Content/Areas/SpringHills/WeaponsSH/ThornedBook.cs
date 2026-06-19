using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class ThornedBookPlayer : ModPlayer
    {
        public bool hasThornedBook;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasThornedBook = false;
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            base.PostHurt(info);
            if (!hasThornedBook)
                return;
            if (info.DamageSource.SourceNPCIndex != -1)
            {
                NPC npc = Main.npc[info.DamageSource.SourceNPCIndex];
                float damage = info.Damage * 5;
                if (npc.boss)
                    damage *= 0.5f;
                damage = MathHelper.Clamp(damage, 0, 200);
                npc.SimpleStrikeNPC((int)damage, -info.HitDirection);

                SoundStyle sound = AssetManager.GetSound("Thorny");
                sound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(sound, Player.position);

                Vector2 velocity = (npc.Center - Player.Center);
                velocity = velocity.SafeNormalize(Vector2.Zero);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 thornVel = velocity;
                    thornVel = thornVel.RotatedByRandom(0.1f);
                    thornVel *= Main.rand.NextFloat(2f, 10f) * 10;

                    Vector2 pos = Player.position;
                    pos.X += Main.rand.Next(0, Player.width);
                    pos.Y += Main.rand.Next(0, Player.height);
                    var fx = FXUtil.GlowStretch(pos, thornVel);
                    fx.VectorScale.X *= 1.5f;
                    fx.VectorScale.Y *= 0.75f;
                    fx.OuterGlowColor = Color.Red;
                }
            
            }
        }
    }

    public class ThornedBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ThornedBookPlayer>().hasThornedBook = true;
            player.statDefense -= 10;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }
}