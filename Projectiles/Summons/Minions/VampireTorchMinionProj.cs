using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class VampireTorchMinionProj : ModProjectile
    {
        public Vector2[] CirclePos = new Vector2[16];
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

            DrawHelper.DrawCircle(owner.Center, VectorHelper.Osc(280, 320), CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public float WidthFunction(float completionRatio)
        {
            float sin = MathF.Sin(Main.GlobalTimeWrappedHourly + (completionRatio * 48));
            if (sin < 0)
                sin = 0;
            float sin2 = MathF.Sin(Main.GlobalTimeWrappedHourly * 2 + completionRatio * 16) * VectorHelper.Osc(1f, 2f);
            float sin3 = MathF.Cos(Main.GlobalTimeWrappedHourly * 3 + completionRatio * 32) * VectorHelper.Osc(0.5f, 1f);
            return Projectile.scale * 8 * sin * sin2 * sin3;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Red;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.ColorFunc = ColorFunction;
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(CirclePos, -Main.screenPosition, 64);
            return true;
        }
    }
}
