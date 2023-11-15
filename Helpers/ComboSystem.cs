using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    // See Common/Systems/KeybindSystem for keybind registration.
    public class ComboSystem : ModPlayer
    {
        public static int StyleResetTimerMax = 420;
        public static int MaxStyle = 4;

        public int StyleResetTimer = StyleResetTimerMax;
        public float Style = 1; // the style score
        public int CurrentStyle = 0; // ranges from 0 to inf
        public int inactiveCharge = 0; // the charge for the next weapon

        public int thisProjStyle = -1;
        public int lastProjStyle = -1;

        public bool didHitThisProj = false;

        public int currentProjectile = -1;

        public void NewAttack()
        {
            lastProjStyle = thisProjStyle;
            thisProjStyle = CurrentStyle;

            didHitThisProj = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            if (Player.HeldItem.ModItem as IComboSystem == null)
                return;

            if (didHitThisProj == false)
            {
                if (lastProjStyle == thisProjStyle)
                    Style *= 0.9f;
                else
                    Style *= (1f / 0.9f);
            }

            StyleResetTimer = StyleResetTimerMax;
            //Style = Math.Min(Style, MaxStyle);

            didHitThisProj = true;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.ModItem as IComboSystem == null)
                return true;

            IComboSystem comboItem = item.ModItem as IComboSystem;

            type = comboItem.ComboProjectiles[CurrentStyle];

            NewAttack();

            Mod.Logger.Info(item.damage + " | " + Style);

            float Damage = item.damage * comboItem.ComboProjectilesDamageMultiplers[CurrentStyle];

            currentProjectile = Projectile.NewProjectile(source, Player.Center, new Vector2(0, 0), type, (int)(Style * Damage), item.knockBack, Player.whoAmI);
            Player.heldProj = currentProjectile;
            Player.channel = true;

            return false;
        }

        /*(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.ModItem as IComboSystem == null) {
                base.ModifyShootStats(item, ref position, ref velocity, ref type, ref damage, ref knockback);
                return;
            }
            IComboSystem comboItem = item.ModItem as IComboSystem;
            type = comboItem.ComboProjectiles[CurrentStyle];
        }
        */

        private bool didCountMouseDown = false;

        public override void PostUpdate()
        {
            /*
            if (currentProjectile != -1)
                Mod.Logger.Info(Main.projectile[currentProjectile].direction);
            */

            if (currentProjectile != -1)
                Main.projectile[currentProjectile].direction = Player.direction;

            StyleResetTimer -= Math.Min(Math.Max((int)Math.Ceiling(Style), 1), MaxStyle); // make it drop x stlye
            if (StyleResetTimer <= 0)
            {
                StyleResetTimer = 0;
                lastProjStyle = -1;
                Style = 1;
            }

            Item item = Player.HeldItem;
            if (item.ModItem as IComboSystem == null)
                return;
            IComboSystem comboItem = item.ModItem as IComboSystem;


            if (Main.mouseRight)
            {
                if (!didCountMouseDown)
                {
                    CurrentStyle++;
                    if (CurrentStyle >= comboItem.ComboProjectiles.Length)
                        CurrentStyle = 0;

                    inactiveCharge = 0;
                }
            }
            didCountMouseDown = Main.mouseRight;


            if (currentProjectile != -1)
            {
                if (Main.mouseLeft)
                    inactiveCharge++;
                else
                    inactiveCharge = 0;

                if (Main.projectile[currentProjectile].active == false)
                {
                    if (inactiveCharge != 0)
                    { // spawn next projectile at charge

                        int type = comboItem.ComboProjectiles[CurrentStyle];

                        float Damage = item.damage * comboItem.ComboProjectilesDamageMultiplers[CurrentStyle];

                        currentProjectile = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(0, 0), type, (int)(Style * Damage), item.knockBack, Player.whoAmI);
                        Player.direction = (Main.mouseX > Main.screenWidth / 2) ? 1 : -1;
                        Main.projectile[currentProjectile].direction = Player.direction;
                        Player.heldProj = currentProjectile;


                        NewAttack();

                        Main.projectile[currentProjectile].ai[0] = inactiveCharge;

                        Player.channel = true;
                    }
                    else
                    {
                        currentProjectile = -1;
                        Player.channel = false;
                    }
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {

        }
    }
}