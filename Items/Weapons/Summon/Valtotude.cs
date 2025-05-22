using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Runes;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.NPCs.Bosses.Gustbeak.Projectiles;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class Valtotude : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
			Item.damage = 4;
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 33, 0);
           
			Item.damage = 7; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.DamageType = DamageClass.Summon;
			Item.mana = 30;
			Item.useTime = 30; // The Item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true; //so the Item's animation doesn't do damage
			Item.knockBack = 0; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.value = 10000; // how much the Item sells for (measured in copper)
			Item.UseSound = SoundID.Item46; // The sound that this Item plays when used.
			Item.autoReuse = false; // if you can hold click to automatically use it again
			Item.shoot = ModContent.ProjectileType<WindPortal>();
			Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
			Item.channel = true;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 2;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
		}
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }

	public class WindPortal : BaseWindProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 24;
        }

        public override void AI()
        {
            base.AI();
            float chargeProgress = Timer / 60f;
            int divisor = (int)MathHelper.Lerp(6, 6, chargeProgress);
            if (Timer % divisor == 0)
            {
                //Spawn new slashes on our little wind orb
                float range = MathHelper.Lerp(16, 16, chargeProgress);
                Vector2 offset = Main.rand.NextVector2CircularEdge(range, range);
                float rotation = offset.ToRotation();
                // rotation += Main.rand.NextFloat(-1f, 1f);
                Wind.NewSlash(offset, rotation);

                offset = Main.rand.NextVector2CircularEdge(range, range);
                rotation = offset.ToRotation();
                //rotation += Main.rand.NextFloat(-1f, 1f);
                Wind.NewSlash(offset, rotation);
            }

            if(Timer % 6 == 0)
            {
                Vector2 spawnPoint = Projectile.Center + (Vector2.UnitY * 256).RotatedBy(Timer / 10f);
                Vector2 vel = (Projectile.Center - spawnPoint) * 0.05f;
                Dust.NewDustPerfect(spawnPoint, ModContent.DustType<GlyphDust>(),vel, 0, Color.White, 1f).noGravity = true;

                spawnPoint = Projectile.Center - (Vector2.UnitY * 256).RotatedBy(Timer / 10f);
                vel = (Projectile.Center - spawnPoint) * 0.05f;
                Dust.NewDustPerfect(spawnPoint, ModContent.DustType<GlyphDust>(), vel, 0, Color.White, 1f).noGravity = true;
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy())
                    continue;

                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                float maxPullDistance = 512;
                if (distance <= maxPullDistance)
                {
                    Vector2 blowVelocity = Projectile.Center - npc.Center;
                    float p = distance / maxPullDistance;
                    p = 1f - p;
                    blowVelocity *= 0.0052f * p * MathHelper.Clamp(Timer / 60f, 0f, 1f);
                    npc.GetGlobalNPC<RuneOfWindBlowNPC>().BlowVelocity = blowVelocity;
                }
            }
            Wind.ExpandMultiplier = 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                rotation += Main.GlobalTimeWrappedHourly * 64;
                Vector2 offset = rotation.ToRotationVector2() * 10;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }
            DrawWindBall(Projectile.Center - Main.screenPosition, ref lightColor);
            spriteBatch.RestartDefaults();

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<WindBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            for (float f = 0; f < 15; f++)
            {
                Color glyphColor = Color.White;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, glyphColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

        }
    }
}