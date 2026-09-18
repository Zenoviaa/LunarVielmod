using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Splitflame : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToArtifact();
            Item.damage = 18;
            Item.mana = 180;
            Item.width = 18;
            Item.height = 21;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SplitFlameBlast>();
            Item.shootSpeed = 4f;
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<ManaFire>());
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class ManaFire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {

        }
    }

    public class ManaFirePlayer : ModPlayer
    {
        public override void ResetEffects()
        {
            base.ResetEffects();

        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(item, ref reduce, ref mult);
            if (Player.HasBuff<ManaFire>())
                mult *= 0.5f;
        }
    }

    public class SplitFlameBlast : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(32, 32);
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1f, 3f));
                }
                SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.5f }, Projectile.position);
            }

            Owner.AddBuff(ModContent.BuffType<ManaFire>(), 10);

            Vector2 center = Owner.Center;
            center.X -= 14;
            Projectile.velocity = (center - Projectile.Center);

            if (Main.rand.NextBool(16))
            {
                var fs = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 32), -Vector2.UnitY.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f));
                fs.Center -= Vector2.UnitY.SafeNormalize(Vector2.Zero) * 64;
                fs.fadeToColor = Color.Black * 0.35f;
                fs.color = Color.RosyBrown * 0.35f;
                fs.Scale *= 0.25f;
            }
            //  Vector2.c
            // Projectile.rotation = _initialVelocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(8))
            {
                DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32) - new Vector2(0, 12),
                    -Vector2.UnitY * Main.rand.NextFloat(2, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                sp.innerColor = Color.Yellow;
                sp.outerColor = Color.Red;
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= 0.6f;
            }

            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.friendly)
                    continue;
                if (npc.townNPC)
                    continue;
                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist <= 256)
                    npc.AddBuff(BuffID.OnFire, 80);
            }
            //   Projectile.rotation = Projectile.velocity.X * 0.025f;
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.75f * Main.essScale);
        }

        private void DrawPixelatedFlames(SpriteBatch sb, Vector2 screenPos)
        {
            // var sb = Main.spriteBatch;
            float fade = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Projectile.timeLeft / 30f));
            float inScale = EasingFunction.OutExpo(Timer / 30f);
            Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
            WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
            waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI;
            waveShader.Amplitude = 0.3f;
            waveShader.Frequency = 8;
            waveShader.XStrength = 6;
            waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
            sb.Restart(effect: waveShader.Effect);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, Projectile.Center);
            drawer.rotation = Projectile.rotation;
            drawer.BottomCenterOrigin();
            drawer.color = Color.OrangeRed * fade * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI);
            drawer.color.A = 0;
            drawer.scale *= 0.5f * inScale;
            drawer.scale.Y *= ExtraMath.Osc(1f, 1.1f, offset: Projectile.whoAmI);
            sb.Draw(drawer);

            drawer.TopCenterOrigin();
            drawer.scale.Y *= 0.4f;
            drawer.spriteEffects |= SpriteEffects.FlipVertically;
            drawer.rotation = Projectile.rotation;
            sb.Draw(drawer);

            sb.RestartDefaults();

            Asset<Texture2D> bloomLine = AssetManager.GlowMask.SimpleGlowCircle;
            SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bloomLine, Projectile.Center + new Vector2(0f, 12));
            //      drawer2.BottomCenterOrigin();
            drawer2.scale *= new Vector2(0.55f, 0.55f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * inScale;
            drawer2.color = Color.Yellow * fade * 0.5f; ;
            drawer2.color.A = 0;
            drawer2.rotation = Projectile.rotation;
            sb.Draw(drawer2);

            drawer2.scale *= 2;
            drawer2.color = Color.Red * fade * 0.5f; ;
            drawer2.color.A = 0;
            sb.Draw(drawer2);

            drawer2.scale *= 2;
            drawer2.color = Color.Red * fade * 0.15f; ;
            drawer2.color.A = 0;
            sb.Draw(drawer2);

            SpritebatchDrawer blastPillar = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center + new Vector2(0f, 12));
            blastPillar.BottomCenterOrigin();
            blastPillar.color = Color.Red * 0.5f * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI) * fade;
            blastPillar.color.A = 0;
            blastPillar.scale *= 0.6f;
            blastPillar.rotation = Projectile.rotation;
            sb.Draw(blastPillar);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlames, DrawLayer.OverPlayers);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}