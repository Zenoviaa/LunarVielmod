using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Lights;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class TheShrieker : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 45;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShriekerBoom>();
            Item.shootSpeed = 6f;
            Item.mana = 50;
            Item.noMelee = true;

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
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
                for (float f = 0; f < 16; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 15f)).RotatedByRandom(19.0), 0, Color.LightBlue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
            }

            float progress = Timer / TimeLeft;
            DrawColor = Color.Lerp(Color.LightBlue, Color.Transparent, progress);
            DrawScale = MathHelper.Lerp(0f, 5f, progress);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float rotation = Projectile.rotation;
            float drawScale = DrawScale;
            Color drawColor = DrawColor;
            drawColor = drawColor.MultiplyRGB(lightColor);
            drawColor.A = 0;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}