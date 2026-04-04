using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Stellamod.Common.Animations
{
    public class Animator
    {
        private string _name;
        private Dictionary<string, SpriteAnimation> _animations;
        private SpriteAnimation _currentAnimation;
        public Animator()
        {
            _animations = new Dictionary<string, SpriteAnimation>();
        }

        public float extraUpdates;
        public void AddAnimation(string name, SpriteAnimation animation)
        {
            _animations.Add(name, animation);
        }

        public bool IsFinished()
        {
            if (_currentAnimation == null)
                return true;
            return _currentAnimation.isFinished;
        }
        public void PlayAnimation(string name)
        {
            _name = name;
            SpriteAnimation animation = _animations[name];
            if (_currentAnimation == animation)
                return;
            if (_currentAnimation != null)
                _currentAnimation.isPlaying = false;
            _currentAnimation = animation;
            _currentAnimation.Start();
            for (int i = 0; i < extraUpdates; i++)
                _currentAnimation?.Update();
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

        public int GetFrame()
        {
            if (_currentAnimation == null)
                return 0;
            return _currentAnimation.GetFrame();
        }
        public int GetFrameY(int frameHeight)
        {
            if (_currentAnimation == null)
                return 0;
            return _currentAnimation.GetFrameY(frameHeight);
        }

        public string GetAnimation()
        {
            if (_currentAnimation == null)
                return string.Empty;
            return _name;
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
        public SpriteAnimation(int startFrame, int endFrame, bool isLooping, Vector2? drawOriginOverride = null, float frameSpeed = 0.15f)
        {
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.isLooping = isLooping;
            this.drawOriginOverride = drawOriginOverride;
            this.frameSpeed = frameSpeed;
        }

        private int _frame;
        private float _frameCounter;
        public int startFrame;
        public int endFrame;
        public float frameSpeed;
        public bool isLooping;
        public bool isPlaying;
        public bool reverse;
        public Vector2? drawOriginOverride;
        public bool isFinished;

        public int GetFrameCount()
        {
            return (endFrame - startFrame) + 1;
        }
        public void Start()
        {
            if (!isPlaying)
            {

                isFinished = false;
                _frame = startFrame;
                if (reverse)
                    _frame = endFrame;
                _frameCounter = 0;
            }

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

                _frameCounter += frameSpeed;

                if (_frameCounter >= 1f)
                {
                    _frameCounter = 0f;
                    if (reverse)
                    {
                        _frame--;

                        if (_frame <= startFrame - 1 && isLooping)
                        {
                            _frame = endFrame;
                        }
                        else if (_frame <= startFrame - 1 && !isLooping)
                        {
                            _frame = startFrame;
                            isFinished = true;
                        }
                    }
                    else
                    {
                        _frame++;

                        if (_frame >= endFrame + 1 && isLooping)
                        {
                            _frame = startFrame;
                        }
                        else if (_frame >= endFrame + 1 && !isLooping)
                        {
                            _frame = endFrame;
                            isFinished = true;
                        }
                    }

                }
            }

        }
        public int GetFrame()
        {
            return _frame;
        }
        public int GetFrameY(int frameHeight)
        {
            return frameHeight * _frame;
        }
    }
}
