using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class VampireTorchMinionProj : ModProjectile,
            IPixelPrimitiveDrawer
    {
        public Vector2[] CirclePos = new Vector2[48];
        public const float Beam_Width = 8;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)// Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<VampireTorchMinionBuff>(owner, Projectile))
                return;

            //This minion doesn't attack
            Projectile.Center = owner.Center - new Vector2(0, 96);
            Visuals();
        }

        private void Visuals()
        {
            Player owner = Main.player[Projectile.owner];
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(12))
            {
                int count = 3;
                for (int k = 0; k < count; k++)
                {
                    Dust.NewDust(Projectile.position, 8, 8, DustID.Blood);
                }
            }

            DrawHelper.DrawCircle(owner.Center, 320, CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * Beam_Width;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Red;
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
