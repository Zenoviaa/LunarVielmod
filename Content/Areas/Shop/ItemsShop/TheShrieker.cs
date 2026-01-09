using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class TheShrieker : AbstractMagicTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<ShriekerBoom>();
            Item.shootSpeed = 6f;
            Item.mana = 50;
            Item.damage = 60;
            Item.useAnimation = 90;
            Item.useTime = 90;
        }

    }

    public class ShriekerBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 90;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Projectile.Center = Owner.Center;
            Timer++;
            if (Timer % 15 == 0)
            {

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<ShriekerWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }


    public class ShriekerWave : ModProjectile
    {
        private float TimeLeft => 30f;
        private float DrawScale = 0f;
        private Color DrawColor;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => "Stellamod/Assets/NoiseTextures/Extra_67";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 768;
            Projectile.height = 768;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = (int)TimeLeft;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            SpecialEffectsPlayer specialEffectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
            specialEffectsPlayer.blurStrength = 0.66f;
            Timer++;
            if (Timer == 1)
            {
                ShakeModSystem.Shake = 4;
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                int Sound = Main.rand.Next(1, 3);
                SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/TheDeafen");
                if (Sound == 1)
                {

                }
                else
                {
                    mySound = new SoundStyle("Stellamod/Assets/Sounds/TheDeafen2");
                }
                mySound.PitchVariance = 0.3f;
     
                SoundEngine.PlaySound(mySound, Projectile.position);
                for (float f = 0; f < 4; f++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2);
                    velocity *= Main.rand.NextFloat(5, 10);
                    DustParticle dp = Particle<DustParticle>.Spawn(position, velocity, Color.White, Scale: Main.rand.NextFloat(0.3f, 1.3f));
                    dp.outerColor = Color.Blue;
                }
            }

            float progress = Timer / TimeLeft;
            DrawColor = Color.Lerp(Color.LightBlue, Color.Transparent, progress);
            DrawScale = MathHelper.Lerp(0f, 5f, progress);
        }

        private void DrawPixelatedShriek(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float rotation = Projectile.rotation;
            float drawScale = DrawScale;
            Color drawColor = DrawColor;
            drawColor.A = 0;
            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedShriek, DrawLayer.OverNPCsWithOutline);
            return false;
        }
    }
}