using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class StringMinion : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private List<Vector2> StringPos = new List<Vector2>();
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];

        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
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
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<StringMinionBuff>(Owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
        
            if (!foundTarget)
            {
                SummonHelper.CalculateIdleValues(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            else
            {
                Vector2 targetVelocity = (targetCenter - Projectile.Center) * 0.5f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
            }

            StringPos.Clear();
            float num = (int)Vector2.Distance(Projectile.Center, Owner.Center) / 16f;
            for (float f = 0; f < num; f++)
            {
                float progress = f / num;
                Vector2 myPos = Vector2.Lerp(Projectile.Center, Owner.Center, progress);
                StringPos.Add(myPos);
            }
            for (int i = 1; i < StringPos.Count - 1; i++)
            {
                float p = (float)i / (float)StringPos.Count - 1;
                Vector2 pos = StringPos[i];
                Vector2 nextPos = StringPos[i + 1];
                Vector2 vec = (nextPos - pos);
                vec = vec.RotatedBy(MathHelper.ToRadians(90));
                vec *= p;

                pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -12 + p * 24 + Projectile.whoAmI * 2);
                pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 4) * -12 + p * 12 + Projectile.whoAmI * 2);
    
                StringPos[i] = pos;
            }

            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;


            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }



        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true);
            BeamDrawer.DrawPixelated(StringPos.ToArray(), -Main.screenPosition, 64);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
