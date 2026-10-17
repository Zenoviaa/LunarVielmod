using ReLogic.Content;
using Stellamod.Common.Animations;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.ProjectileHelpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;


public static class AnimationExtensions
{
    public static readonly AseAnimator DummyAnimator = new();

    extension(ModProjectile modProjectile)
    {
        public AseAnimator AseAnimator
        {
            get
            {
                if (modProjectile.Projectile.TryGetGlobalProjectile<AnimatorGlobalProjectile>(out var animator))
                    return animator.Animator;
                return DummyAnimator;
            }
        }
    }

    extension(NPC npc)
    {
        /// <summary>
        /// Queues an outline draw if the outliner is visible enough, drawing with the animator
        /// </summary>
        /// <param name="outliner"></param>
        public void DrawOutlineByAnimator(Outliner outliner)
        {
            if (!outliner.VisibleEnough)
                return;
            void DrawOutline(SpriteBatch sb)
            {
                npc.DrawAnimator(sb, outliner.outlineColor);
            }
            OutlineRenderer.Queue(DrawOutline);
        }

        /// <summary>
        /// Draw Origin for rotation and scaling
        /// </summary>
        public Vector2 DrawOrigin
        {
            get
            {
                if (!npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animatorNPC))
                    return Vector2.One;
                if (animatorNPC.Animator.drawEffects.DrawOrigin == null)
                    return Vector2.One;
                return animatorNPC.Animator.drawEffects.DrawOrigin.Value;
            }
            set
            {
                if (!npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animatorNPC))
                    return;
                animatorNPC.Animator.drawEffects.DrawOrigin = value;
            }
        }


        /// <summary>
        /// Scales the X and Y axises independently
        /// </summary>
        public Vector2 VectorScale
        {
            get
            {
                if (!npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animatorNPC))
                    return Vector2.One;
                if (animatorNPC.Animator.drawEffects.Scale == null)
                    return Vector2.One;
                return animatorNPC.Animator.drawEffects.Scale.Value;
            }
            set
            {
                if (!npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animatorNPC))
                    return;
                animatorNPC.Animator.drawEffects.Scale = value;
            }
        }


        /// <summary>
        /// Plays an animation that will not loop
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimationOneShot(string name) => npc.PlayAnimation(name, AnimationParams.NoLooping);
        
        /// <summary>
        /// Plays an animation that will loop
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimationLooped(string name) => npc.PlayAnimation(name, AnimationParams.Default);
        
        /// <summary>
        /// Plays an animation with the specified parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="animationParams"></param>
        public void PlayAnimation(string name, AnimationParams animationParams)
        {
            if (!npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animatorNPC))
                return;
            animatorNPC.Animator.PlayAnimation(name, animationParams);
        }
    }

    extension(ModNPC modNpc)
    {
        public AseAnimator AseAnimator
        {
            get
            {
                if (modNpc.NPC.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animator))
                    return animator.Animator;
                return DummyAnimator;
            }
        }
    }

    public static AseAnimator GetAnimator(this ModNPC modNpc)
    {
        return modNpc.NPC.GetAnimator();
    }

    public static AseAnimator GetAnimator(this NPC npc)
    {
        if (npc.TryGetGlobalNPC<AnimatorGlobalNPC>(out var animator))
            return animator.Animator;
        return DummyAnimator;
    }
    public static AseAnimator GetAnimator(this Projectile npc)
    {
        if (npc.TryGetGlobalProjectile<AnimatorGlobalProjectile>(out var animator))
            return animator.Animator;
        return DummyAnimator;
    }

    public static void SetDrawOrigin(this ModNPC modNpc, Vector2 drawOrigin)
    {
        ref DrawEffects drawEffects = ref modNpc.GetAnimator().drawEffects;
        drawEffects.DrawOrigin = drawOrigin;
    }

    public static void SetSpriteEffects(this ModNPC modNpc, SpriteEffects spriteEffects)
    {
        var Animator = modNpc.GetAnimator().spriteEffects = spriteEffects;
    }
    public static SpritebatchDrawer GetAnimatorDrawInfo(this Projectile npc, Color drawColor)
    {
        var Animator = npc.GetAnimator();
        SpritebatchDrawer drawer = Animator.GetSprite(npc.Center);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition += offset;
        return drawer;
    }

    public static SpritebatchDrawer GetAnimatorDrawInfo(this NPC npc, Color drawColor, Vector2? position = null)
    {
        var Animator = npc.GetAnimator();
        if (position == null)
            position = npc.Center;
        SpritebatchDrawer drawer = Animator.GetSprite(position.Value);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition += offset;
        return drawer;
    }

    public static void DrawAnimator(this NPC npc, SpriteBatch spriteBatch, Color drawColor)
    {
        var Animator = npc.GetAnimator();
        SpritebatchDrawer drawer = Animator.GetSprite(npc.Center);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition += offset;
        drawer.worldPosition.Y += npc.gfxOffY;
        spriteBatch.Draw(drawer);
    }

    public static void DrawAnimator(this NPC npc, SpriteBatch spriteBatch, Color drawColor, Vector2 position)
    {
        var Animator = npc.GetAnimator();
        SpritebatchDrawer drawer = Animator.GetSprite(npc.Center);
        drawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawer.spriteEffects |= Animator.spriteEffects;
        drawer.rotation = npc.rotation;
        drawer.color = drawColor;
        if (npc.spriteDirection == -1)
        {
            drawer.drawOrigin.X = drawer.sourceRect!.Value.Width - drawer.drawOrigin.X;
        }

        //Offset it even with the draw origin so the sprite is still in the center of the hitbox
        Vector2 offset = drawer.drawOrigin - Animator.centerDrawOrigin;
        drawer.worldPosition = position + offset;
        spriteBatch.Draw(drawer);
    }
}

public class AnimatorGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public AseAnimator Animator;
    public override void SetDefaults(Projectile entity)
    {
        base.SetDefaults(entity);
        Animator = new AseAnimator();
        if (AsepriteAssets.Projectile == null)
            return;
        if (Main.netMode == NetmodeID.Server)
            return;
        Animator.SetSpriteAsset(AsepriteAssets.Projectile[entity.type]);
    }

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        if (ProjectileID.Sets.UsesAseprite[entity.type])
            return true && lateInstantiation;
        return false;
    }
}
public class AnimatorGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;


    public AseAnimator Animator;

    public override void FindFrame(NPC npc, int frameHeight)
    {
        base.FindFrame(npc, frameHeight);
        if (NPCSets.UseAseprite[npc.type])
        {
            Animator?.Update();
        }
    }

    public override void SetDefaults(NPC entity)
    {
        base.SetDefaults(entity);
        Animator = new AseAnimator();
        if (AsepriteAssets.Npc == null)
            return;
        if (Main.netMode == NetmodeID.Server)
            return;
        Animator.SetSpriteAsset(AsepriteAssets.Npc[entity.type]);
    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {

        if (NPCSets.UseAseprite[entity.type])
            return true && lateInstantiation;
        return false;
    }
}

public record struct AnimationParams(bool IsLooping = false)
{
    public static readonly AnimationParams NoLooping = new AnimationParams(IsLooping: false);
    public static readonly AnimationParams Default = new AnimationParams(IsLooping: true);
}
public record struct DrawEffects(Vector2? DrawOrigin, Vector2? Scale)
{

};

public class AseAnimator
{
    private float _frameCounter;
    private int _frameIndex;
    public AseAnimator()
    {

        drawEffects = default;

    }


    public Asset<AseSprite> Sprite;
    public AseTags playingTag;
    public bool isLooping;
    public DrawEffects drawEffects;
    public SpriteEffects spriteEffects;
    public Vector2 centerDrawOrigin;
    public Vector2? DrawOrigin
    {
        set => drawEffects.DrawOrigin = value;
        get => drawEffects.DrawOrigin;
    }

    public void SetSpriteAsset(Asset<AseSprite> sprite)
    {
        Sprite = sprite;
        centerDrawOrigin = new Vector2(sprite.Value.FrameWidth * 0.5f, sprite.Value.FrameHeight * 0.5f);
    }

    public SpritebatchDrawer GetSprite(Vector2 worldPosition)
    {
        var sprite = Sprite.Value.GetSprite(_frameIndex, worldPosition);
        sprite.drawOrigin = drawEffects.DrawOrigin.HasValue ? drawEffects.DrawOrigin.Value : sprite.drawOrigin;
        sprite.scale = drawEffects.Scale.HasValue ? drawEffects.Scale.Value : sprite.scale;
        sprite.spriteEffects = spriteEffects;
        return sprite;
    }
    public void PlayAnimation(string name, AnimationParams? animationParams = null)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        if (animationParams == null)
        {
            animationParams = AnimationParams.Default;
        }


        if (playingTag != null && playingTag.name == name)
            return;
        playingTag = Sprite.Value.tags.Find(x => x.name == name);
        _frameCounter = 0;
        _frameIndex = playingTag.from;
        this.isLooping = animationParams.Value.IsLooping;
    }

    public void Update()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        if (playingTag == null)
            return;
        //TODO: instead take in an elapsed time and calculate the current frame
        //That would be better net synced.

        //rn just increasing by 1 / 60 since that's the game's tick rate
        _frameCounter += 1f / 60f;
        float ft = Sprite.Value.frames[_frameIndex].frameTime;
        while (_frameCounter >= ft)
        {
            _frameCounter -= ft;
            _frameIndex++;
            if (isLooping)
            {
                //0 is infinite, so repeat the animation
                if (_frameIndex > playingTag.to)
                {
                    _frameIndex = playingTag.from;
                }
            }
            else
            {
                if (_frameIndex > playingTag.to)
                {
                    _frameIndex = playingTag.to;
                }
            }
        }
    }
}
