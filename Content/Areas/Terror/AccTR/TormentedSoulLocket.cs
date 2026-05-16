using Microsoft.Xna.Framework;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.AccTR
{
    public class TormentedSoulLocketPlayer : ModPlayer
    {
        public bool hasSoulLocket;
        public int soulTimer;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSoulLocket = false;
            if(soulTimer > 0)
                soulTimer--;
        }
    }

    public class TormentedGlobalProjectile : GlobalProjectile
    {

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
            Player owner = Main.player[projectile.owner];
            if (owner.GetModPlayer<TormentedSoulLocketPlayer>().soulTimer <= 0)
                return;
            if (projectile.ModProjectile is not AbstractBellSummon)
                return;
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Suckler"));
            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomKaev>(),
                (int)(projectile.damage * 2), 0f, projectile.owner, 0f, 0f);


            float Speed = Main.rand.Next(4, 7);
            float offsetRandom = Main.rand.Next(0, 50);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(projectile.Center, 2048f, 32f);

            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;

            owner.Heal(10);
            for (int i = 0; i < 2; i++)
            {

                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, projectile.owner);
                Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, projectile.owner);
            }
        }
    }

    public class TormentedSoulLocket : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<TormentedSoulLocketPlayer>().soulTimer = 3;
            player.GetDamage(DamageClass.Summon) += 0.05f;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankAccessory>();
        }
    }
}
