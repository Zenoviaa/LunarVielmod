using Microsoft.Xna.Framework;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.AccCL
{
    public class BarryPlayer : ModPlayer
    {
        public bool hasBarry;
        public float regenTimer;
        public override void ResetEffects()
        {
            hasBarry = false;
        }

        public override void PostUpdateEquips()
        {
            regenTimer--;
            if (regenTimer > 0)
                return;

            if (hasBarry && Player.ownedProjectileCounts[ModContent.ProjectileType<BarrySpike>()] == 0 && Main.myPlayer == Player.whoAmI)
            {
                float count = 9;
                for (float i = 0; i < count; i++)
                {
                    float progress = i / count;
                    float rot = progress * 360;
                    Projectile barryProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<BarrySpike>(), 40, 20, Player.whoAmI, ai0: rot);
                }
            }
        }
    }

    public class Barry : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BarryPlayer>().hasBarry = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }


    public class BarrySpike : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            if (!Owner.GetModPlayer<BarryPlayer>().hasBarry || Owner.GetModPlayer<BarryPlayer>().regenTimer > 0)
            {

                Projectile.Kill();
                return;
            }


            Timer++;
            float degrees = Timer;
            float circleDistance = 64;
            Vector2 circlePosition = Owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees));

            Projectile.timeLeft = 2;
            Projectile.Center = circlePosition;
            Projectile.rotation = Owner.Center.DirectionTo(circlePosition).ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // The hitDirection is always set to hit away from the player
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float count = 32;
            float degreesPer = 360 / count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 4;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, vel.X, vel.Y);
            }

            Owner.GetModPlayer<BarryPlayer>().regenTimer = 300;
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"));
            switch (Main.rand.Next(0, 2))
            {
                case 0:
                    target.AddBuff(BuffID.Poisoned, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Venom, 120);
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 8; i++)
            {
                float f = i;
                float num = 8;
                float progress = f / num;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 3;
                Dust.NewDustPerfect(Projectile.Center, DustID.Silver, vel);
            }
        }
    }
}
