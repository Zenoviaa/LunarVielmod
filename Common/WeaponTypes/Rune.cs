using Stellamod.Common.SummonerSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{

    public class ExpandingRuneTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            RuneGlobalItem runeGlobalItem = item.GetGlobalItem<RuneGlobalItem>();
            if (runeGlobalItem.isRune)
            {
                TooltipLine line = new TooltipLine(Mod, "RuneHelp", LangText.Common("RuneHelp"));
                lines.Add(line);
            }
        }


    }

    public class RuneGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isRune;

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (isRune)
            {
                float numClones = 8;
                for(float n = 0; n < numClones; n++)
                {
                    float ratio = n / numClones;
                    float rot = ratio * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 4;
                    Vector2 offset = rot.ToRotationVector2() * ExtraMath.Osc(0.8f, 1f);
                    offset *= 32;

                    //color.A = 0;
                    Color glowColor = Color.White * 0.6f;
                  //  glowColor.A = 0;
                    Main.DrawItemIcon(spriteBatch, item, position, glowColor, 32);
                }

                if (Main.rand.NextBool(32) && !Main.gameInactive)
                {
                    DustParticle dp = DustParticle.SpawnInUI(position + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    dp.gravity = 0;
                    dp.innerColor = Color.White;
                    dp.outerColor = Color.White;
                }
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if (isRune && player.whoAmI == Main.myPlayer)
            {
                position = Main.MouseWorld;
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (isRune)
            {
                if (player.ownedProjectileCounts[type] > 0)
                    return false;
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public abstract class AbstractRuneProjectile : ModProjectile
    {
        private Vector2 _oldShapePoint;
        private List<Vector2> _shapePointsList;
        private ref float Timer => ref Projectile.ai[0];
        protected Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        public Vector2 StartDrawingPosition { get; private set; }
        public Vector2 DrawingPosition { get; private set; }
        public Vector2[] OldDrawingCache { get; private set; }
        public float EaseInRatio { get; private set; }
        public bool MatchedShape { get; private set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
       

        public sealed override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle castSound = new SoundStyle("Stellamod/Assets/Sounds/Frosty");
                castSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(castSound, Projectile.position);
                for (float n = 0; n < 4; n++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                    FXUtil.GlowStretch(position, vel);
                }
            }

            if (Timer == 1 && this.OwnedByLocalClient())
            {
                StartDrawingPosition = Main.MouseScreen;
            }
            EaseInRatio = Timer / 30f;
            EaseInRatio = MathHelper.Clamp(EaseInRatio, 0f, 1f);

            if (this.OwnedByLocalClient())
            {
                DrawingPosition = Main.MouseScreen;
            }

            _shapePointsList ??= new List<Vector2>();
            float distanceToOldPosition = Vector2.Distance(DrawingPosition, _oldShapePoint);
            if (Timer > 2 && distanceToOldPosition > 2)
            {
                _shapePointsList.Add(DrawingPosition);
                _oldShapePoint = DrawingPosition;
            }

            if (OldDrawingCache == null)
            {
                OldDrawingCache = new Vector2[500];
                for (int n = 0; n < OldDrawingCache.Length; n++)
                {
                    OldDrawingCache[n] = DrawingPosition;
                }
            }

            for (int i = OldDrawingCache.Length - 1; i > 0; i--)
            {
                OldDrawingCache[i] = OldDrawingCache[i - 1];
            }
            OldDrawingCache[0] = DrawingPosition;

            bool shouldRelease = !Owner.controlUseItem && Timer > 2;
            if (shouldRelease)
            {
                Vector2[] shapePointsArr = _shapePointsList.ToArray();
                MatchedShape = MatchShapeCheck(shapePointsArr);
                if (MatchedShape)
                {
                    //Main.NewText("Success");
                    foreach (var projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.owner != Projectile.owner)
                            continue;
                        if (projectile.ModProjectile is AbstractBellSummon minion)
                        {
                            ApplyMagic(minion);
                        }
                    }

                    Projectile.Kill();
                }
                else
                {
                    //Main.NewText("Fail");
                    Projectile.Kill();
                }
            }
            DustEffects();
        }

        public override bool PreKill(int timeLeft)
        {
            RuneLineParticle runeLineParticle = RuneLineParticle.Spawn(Owner.Center, Vector2.Zero, Color.White);
            runeLineParticle.trailCache = OldDrawingCache;
            runeLineParticle.bloomColor = MatchedShape ? Color.Green : Color.Red;
            return base.PreKill(timeLeft);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < OldDrawingCache.Length && i < Timer; i++)
            {
                Vector2 oldPos = OldDrawingCache[i];
                OnDissipate(oldPos);
            }
        }

        /// <summary>
        /// Returns whether the array of points matches the shape for this rune
        /// </summary>
        /// <param name="shapePoints"></param>
        /// <returns></returns>
        public abstract bool MatchShapeCheck(Vector2[] shapePoints);

        /// <summary>
        /// Does something to the minion!
        /// </summary>
        /// <param name="minion"></param>
        public abstract void ApplyMagic(AbstractBellSummon minion);

        /// <summary>
        /// Called at the end of the AI logic for the rune, originally used for dust effects
        /// </summary>
        public virtual void DustEffects() { }

        /// <summary>
        /// Called for every point in the old position trail
        /// </summary>
        /// <param name="trailPoint"></param>
        public virtual void OnDissipate(Vector2 trailPoint) { }
    }
}
