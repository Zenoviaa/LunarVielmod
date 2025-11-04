using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.AccFB
{
    public class BonfirePlayer : ModPlayer
    {
        public bool hasBonfire;
        public bool hideVisual;
        public float bonfireCooldown;
        public float DamageBonus => MathHelper.Clamp(bonfireCooldown / 900f, 0f, 1f);
        public override void ResetEffects()
        {
            hasBonfire = false;
            hideVisual = false;
        }

        public override void PostUpdateEquips()
        {
            if (hasBonfire)
            {
                bonfireCooldown++;
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<BonfireProj>()] == 0 && Main.myPlayer == Player.whoAmI && !hideVisual)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<BonfireProj>(), 1, 1, Player.whoAmI);
                }
            }
        }
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(item, ref damage);
            if (!hasBonfire)
                return;
            StatModifier m = new StatModifier(1f + MathHelper.Lerp(0f, 0.5f, DamageBonus), 1f);
            damage = damage.CombineWith(m);
            
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!hasBonfire)
                return;

         
            bonfireCooldown = 0;
        }

    }

    public class Bonfire : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BonfirePlayer>().hasBonfire = true;
            player.GetModPlayer<BonfirePlayer>().hideVisual = hideVisual;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), 
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }

    public class BonfireProj : ModProjectile,
        IDrawOutlines
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 4;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            BonfirePlayer bonfirePlayer = owner.GetModPlayer<BonfirePlayer>();
            if(bonfirePlayer.DamageBonus >= 1f)
            {
                this.OutlineNoRestart(Color.White, ref lightColor, Projectile.scale * Vector2.One);
            }
        
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
               new Vector3(255, 0, 68),
               new Vector3(252, 191, 84),
               new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(255, 0, 68), lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(255, 0, 68), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BonfirePlayer bonfirePlayer = owner.GetModPlayer<BonfirePlayer>();
            if (bonfirePlayer.hideVisual)
                return;
            if (bonfirePlayer.hasBonfire)
            {
                Projectile.timeLeft = 2;
            }

            float aiSizeMult = bonfirePlayer.DamageBonus;
            float offset = aiSizeMult * 16;

            Projectile.timeLeft = 2;
            Projectile.Center = owner.Center + new Vector2(0, -80 + VectorHelper.Osc(0, 8 + offset, 2));

            float targetScale = 1.25f;
            float targetLightSize = 7;
            float lightSize = aiSizeMult * targetLightSize;
            lightSize = MathHelper.Clamp(lightSize, 1f, targetLightSize);


            Lighting.AddLight(Projectile.Center, new Vector3(lightSize, lightSize, lightSize));
            Projectile.scale = aiSizeMult * targetScale;
            Projectile.scale = MathHelper.Lerp(0f, 1f, aiSizeMult);
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

    }
}
