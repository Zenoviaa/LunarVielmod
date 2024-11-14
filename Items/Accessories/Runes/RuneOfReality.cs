using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RealityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public Vector2 PushVelocity;
        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            projectile.velocity += PushVelocity;
            PushVelocity *= 0.6f;
        }
    }

    internal class RuneOfRealityField : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private Vector2[] _fieldPos;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        private RuneOfRealityPlayer RealityPlayer => Owner.GetModPlayer<RuneOfRealityPlayer>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            _fieldPos = new Vector2[12];
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            base.AI();
            if (RealityPlayer.hasRuneOfReality && RealityPlayer.manaPercent < 0.25f)
            {
                Timer++;
                Projectile.timeLeft = 60;
            }
            else
            {
                Timer--;
            }

            if (Timer > 60)
            {
                Timer = 60;
            }
            else if (Timer < 0)
            {
                Timer = 0;
            }

            Projectile.Center = Owner.Center;
            for (int i = 0; i < _fieldPos.Length; i++)
            {
                float f = i;
                float length = _fieldPos.Length;
                float progress = f / length;
                float offset = progress * MathHelper.TwoPi;
                Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + (Timer / 20f)).RotatedByRandom(MathHelper.PiOver4 / 24f);
                Vector2 rotatedVector = (rotatedOffset * 150 * VectorHelper.Osc(0.95f, 1f, 9, offset: f * 0.5f));
                if (i % 2 == 0)
                {
                    _fieldPos[i] = rotatedVector * VectorHelper.Osc(0.8f, 1f, speed: 8, offset: f * 4f) + Projectile.position;
                }
                else
                {
                    _fieldPos[i] = rotatedVector + Projectile.position;
                }
            }

        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0f, 64, Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.BlueViolet, Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            Projectile.oldPos = _fieldPos;
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            }

            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            Vector2 trailOffset = -Main.screenPosition;
            TrailDrawer.DrawPrims(_fieldPos, trailOffset, 155);

            return false;
        }
    }

    internal class RuneOfRealityPlayer : ModPlayer
    {
        public bool hasRuneOfReality;
        public float manaPercent;
        public override void ResetEffects()
        {
            hasRuneOfReality = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (hasRuneOfReality)
            {
                manaPercent = (float)Player.statMana / (float)Player.statManaMax2;
                if (manaPercent <= 0.25f)
                {
                    //OKAY
                    foreach (var projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.hostile)
                        {
                            RealityGlobalProjectile realityGlobalProjectile = projectile.GetGlobalProjectile<RealityGlobalProjectile>();
                            float distance = Vector2.Distance(Player.Center, projectile.Center);
                            if (distance <= 150)
                            {
                                realityGlobalProjectile.PushVelocity = -projectile.velocity * 0.85f;
                            }
                        }
                    }

                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RuneOfRealityField>()] == 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                            ModContent.ProjectileType<RuneOfRealityField>(), 0, 0, Player.whoAmI);
                    }
                }
            }
        }
    }

    internal class RuneOfReality : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 20;
            RuneOfRealityPlayer realityPlayer = player.GetModPlayer<RuneOfRealityPlayer>();
            realityPlayer.hasRuneOfReality = true;
        }
    }
}