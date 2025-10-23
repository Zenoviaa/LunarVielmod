using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class BurningAngel : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 10;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 51;
            Item.useAnimation = 51;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.DD2_FlameburstTowerShot;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 4;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.crit = 26;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<BurningAngelProj>();
            Item.shootSpeed = 4f;
            Item.value = 10000;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(), material: ModContent.ItemType<AlcadizScrap>());
        }
    }












    public class BurningAngelProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.height = 32;
            Projectile.width = 32;
            Projectile.friendly = true;
            Projectile.scale = 1f;    
            Projectile.timeLeft = 100;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private ref float Timer => ref Projectile.ai[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation -= 0.5f;
            Projectile.velocity *= 0.97f;

            if (Timer < 30)
            {
                if(Main.myPlayer == Projectile.owner && player.controlUseItem)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
                    Projectile.netUpdate = true;
                }

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemTime = 10;
                player.itemAnimation = 10;
                player.itemRotation = rotation * player.direction;
            }

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);
            //Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
        }
    }
}












