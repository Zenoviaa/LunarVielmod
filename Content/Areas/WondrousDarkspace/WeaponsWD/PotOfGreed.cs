using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class PotOfGreed : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 61;
            Item.knockBack = 3f;
            Item.mana = 40;
            Item.width = 54;
            Item.height = 34;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightPurple;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.buffType = ModContent.BuffType<PotOfGreedMinionBuff>();
            Item.shoot = ModContent.ProjectileType<PotOfGreedMinionProj>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<HypnotizedSoul>());
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            //Spawn at the mouse cursor position
            if (player.ownedProjectileCounts[type] > 0)
            {
                //Desummon it
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile projectile = Main.projectile[p];
                    if (projectile.owner != player.whoAmI)
                        continue;

                    if (projectile.type == type && projectile.active)
                    {
                        projectile.Kill();
                    }
                }
            }
            player.AddBuff(Item.buffType, 2);
            position = Main.MouseWorld;
            SoundEngine.PlaySound(SoundID.Item82, player.position);
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
    }
    public class PotOfGreedMinionProj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;

            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 80;

            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            //Projectile.friendly = true;

            // Only determines the damage type

            //I DON'T KNOW IF I NEED TO SET minion to true for sentries, I'm not going to
            //	Projectile.minion = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0f;

            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Void Pre-Draw Effects
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            return true;
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<PotOfGreedMinionBuff>(Main.player[Projectile.owner], Projectile))
                return;

            float distance = Vector2.Distance(Owner.Center, Projectile.Center);
            Timer++;
            if (distance < 252 && Timer % 12 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<PotOfGreedMinionProjBat>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                for (float f = 0; f < 24; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                    Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, vel, Scale: 0.2f);
                }
                SoundEngine.PlaySound(SoundID.Item117, Projectile.position);
            }

            //This is the ring that shows where the shadow minions spawn
            float hoverSpeed = 5;
            float rotationSpeed = 2.5f;
            float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
            float rotation = VectorHelper.Osc(MathHelper.ToRadians(-5), MathHelper.ToRadians(5), rotationSpeed);
            Projectile.velocity = new Vector2(0, yVelocity);
            Projectile.rotation = rotation;
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }

    public class PotOfGreedMinionProjBat : ScarletProjectile
    {
        private ITrailer _trailer;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Scale => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 7;
        }



        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 2;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 120;
        }



        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                Scale = Main.rand.NextFloat(0.5f, 1f);

            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            DrawHelper.AnimateTopToBottom(Projectile, 5);
            NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 4);
                if (Projectile.velocity.Length() < 15)
                {
                    Projectile.velocity *= 1.5f;
                }

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.Y -= 1;
                }
            }
        }



        public override bool PreDraw(ref Color lightColor)
        {
            _trailer ??= TrailPresets.HypnotizedSoul;
            _trailer.DrawTrail(ref lightColor, OldCenterPos);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale * Scale;
            float interpolant = Timer / 30f;
            float eased = EasingFunction.InOutSine(interpolant);
            scale *= eased;

            float rotation = Projectile.rotation;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0; f < MathHelper.TwoPi; f += 0.4f)
            {
                Vector2 offset = f.ToRotationVector2() * 2;
                spriteBatch.Draw(texture, drawPos + offset, drawFrame, Color.Pink.MultiplyRGB(lightColor), rotation, drawOrigin, scale, spriteEffects, 0);
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, drawFrame, Color.Black.MultiplyRGB(lightColor), rotation, drawOrigin, scale, spriteEffects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Vector2 position = Projectile.Center;
            Vector2 velocity = -Vector2.UnitY;
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.LightPink,
                    outerColor: Color.Pink,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;
                }
            }
        }
    }
}
