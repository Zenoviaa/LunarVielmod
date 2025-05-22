using Microsoft.Xna.Framework;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MorrowBroochGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Hit;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);
            Hit = true;
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
            Player owner = Main.player[projectile.owner];
            if (!Hit && projectile.friendly)
            {
                owner.GetModPlayer<MorrowBroochPlayer>().hitCount = 0;
            }
        }
    }

    public class MorrowBroochPlayer : ModPlayer
    {
        public int npcWhoAmI;
        public float hitCount;

        public bool Hit;
        public bool MorrowBroochActive => Player.GetModPlayer<BroochSpawnerPlayer>().BroochActive(ModContent.ItemType<MorrowedBroochA>());
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!MorrowBroochActive)
                return;
            if (npcWhoAmI != target.whoAmI)
            {
                npcWhoAmI = target.whoAmI;
                hitCount = 0;
                return;
            }

            hitCount++;
            if (hitCount >= 10f)
            {
                hitCount = 10f;
            }

            float progress = hitCount / 10f;
            float maxDamageBoost = 15;
            float maxKnockbackBoost = 5;


            modifiers.Knockback += MathHelper.Lerp(0, maxKnockbackBoost, progress);
            modifiers.FinalDamage.Base += MathHelper.Lerp(0, maxDamageBoost, progress);

            if (hitCount >= 10f)
            {
                Hit = true;
                modifiers.HideCombatText();

                for (int i = 0; i < 2; i++)
                {
                    Color color = Color.RosyBrown;
                    Dust.NewDustPerfect(target.Center,
                        ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, color, 1f).noGravity = true;
                }
                target.AddBuff(BuffID.OnFire, 180);
                FXUtil.ShakeCamera(target.Center, 1024, 1);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!MorrowBroochActive)
                return;
            if (Hit)
            {
                CombatText.NewText(target.getRect(), Color.Lerp(Color.Red, Color.White, 0.25f), hit.Damage, dramatic: hit.Crit);
                Hit = false;
            }
            //Uhh
            //hmm
        }
    }
    public class MorrowedBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<Morrow>();
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBrooch>(), material: ModContent.ItemType<AlcadizScrap>());
        }
    }
}