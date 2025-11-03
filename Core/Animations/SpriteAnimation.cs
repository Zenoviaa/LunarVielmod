using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Animations
{
    public class Animator
    {
        private Dictionary<string, SpriteAnimation> _animations;
        private SpriteAnimation _currentAnimation;
        public Animator()
        {
            _animations = new Dictionary<string, SpriteAnimation>();
        }

        public void AddAnimation(string name, SpriteAnimation animation)
        {
            _animations.Add(name, animation);
        }

        public void PlayAnimation(string name)
        {
            SpriteAnimation animation = _animations[name];
            _currentAnimation = animation;
            _currentAnimation.Start();
        }

        public void Stop()
        {
            _currentAnimation?.Stop();
            _currentAnimation = null;
        }
        public void Update()
        {
            _currentAnimation?.Update();
        }

        public int GetFrameY(int frameHeight)
        {
            if (_currentAnimation == null)
                return 0;
            return _currentAnimation.GetFrameY(frameHeight);
        }

        public Vector2? GetDrawOrigin()
        {
            if (_currentAnimation == null)
                return null;
            return _currentAnimation.drawOriginOverride;
        }
    }

    public class SpriteAnimation
    {
        public SpriteAnimation(int startFrame, int endFrame, bool isLooping, Vector2? drawOriginOverride = null)
        {
            this.startFrame = startFrame;   
            this.endFrame = endFrame;
            this.isLooping = isLooping;
            this.drawOriginOverride = drawOriginOverride;
        }
        private int _frame;
        private float _frameCounter;
        public int startFrame;
        public int endFrame;
        public bool isLooping;
        public bool isPlaying;
        public Vector2? drawOriginOverride;
        public void Start()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
        }
        public void Update()
        {
            if (isPlaying)
            {
                _frameCounter += 0.15f;
                if (_frameCounter >= 1f)
                {
                    _frame++;
                    _frameCounter = 0f;
                    if (_frame >= endFrame + 1 && isLooping)
                    {
                        _frame = startFrame;
                    }
                    else if (_frame >= endFrame + 1 && !isLooping)
                    {
                        _frame = endFrame;
                    }
                }
            }

        }

        public int GetFrameY(int frameHeight)
        {
           return frameHeight * _frame;
        }
    }
}
