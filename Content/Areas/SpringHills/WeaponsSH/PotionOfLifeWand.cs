using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Summons.Minions;
using Stellamod.Trails;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class PotionOfLifeWand : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToBellMinion(ModContent.ProjectileType<LifeWandMinionProj>());
            Item.damage = 2;
            Item.knockBack = 3f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(), 
                material: ModContent.ItemType<Mushroom>());
        }
    }

    public class LifeWandMinionProj : AbstractBellSummon
    {
        private ref float Heart => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 8;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.scale = 1f;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return true;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            base.AI();
            SummonHelper.CalculateIdleValues(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
            Heart++;
            if (Heart == 1260)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int itemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(10, 10), ItemID.Heart, 1);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }

                Heart = 0;
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D closingCircle = ModContent.Request<Texture2D>(TextureRegistry.ThinCircle).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Color drawColor = Color.Red;
            drawColor.A = 0;
            float drawScale = MathHelper.Lerp(1f, 0f, Heart / 1260f);
            spriteBatch.Draw(closingCircle, drawPosition, null, drawColor, Projectile.rotation, closingCircle.Size() / 2f, drawScale * 0.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}