using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Dread
{
    internal class BloodyHeartEnchantment : BaseEnchantment
    {

        public override float GetStaffManaModifier()
        {
            return 0.8f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DreadElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.Red);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            float Speed = Main.rand.Next(4, 7);
            float offsetRandom = Main.rand.Next(0, 50);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;

            Player Player = Main.player[Projectile.owner];
            for (int i = 0; i < 2; i++)
            {
                Player.Heal(1);
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Projectile.owner);
                Projectile.netUpdate = true;
            }
        }
    }
}
