using System;
using System.Reflection;
using System.Collections.Generic;

namespace Aiv.Tween
{

    public class Tween
    {

        public delegate void StartHandler(Tween sender);

        public delegate void StopHandler(Tween sender);

        public delegate void UpdateHandler(Tween sender);

        public delegate void KeyFrameHandler(Tween sender);

        public event StartHandler OnStart;
        public event StopHandler OnStop;
        public event UpdateHandler OnUpdate;

        private class KeyFrame
        {

            private class Iteration
            {
                public object target;
                public string fieldName;
                public PropertyInfo pInfo;
                public FieldInfo fInfo;
                public object startValue;
                public object endValue;
            }

            public float startedAt;

            private float duration;

            public float Duration
            {
                get
                {
                    return duration;
                }
            }

            private List<Iteration> iterations;
            private EasingFunction easing;
            private KeyFrameHandler handler;

            public KeyFrame(float duration, EasingFunction easing, KeyFrameHandler handler = null)
            {
                this.duration = duration;
                this.startedAt = 0;
                this.easing = easing;
                this.handler = handler;
                iterations = new List<Iteration>();
            }

            public float Ease(float k)
            {
                return this.easing(k);
            }

            public void RunHandler(Tween tween)
            {
                if (this.handler == null)
                    return;
                this.handler(tween);
            }

            public void AddIteration(object target, string name, object value)
            {
                PropertyInfo pInfo = target.GetType().GetProperty(name);
                FieldInfo fInfo = target.GetType().GetField(name);
                if (pInfo == null && fInfo == null)
                {
                    throw new Exception("invalid field: " + name);
                }
                Iteration iteration = new Iteration { target = target, pInfo = pInfo, fInfo = fInfo };
                iteration.fieldName = name;
                iteration.endValue = value;
                this.iterations.Add(iteration);
            }

            public void SetupIterations()
            {
                foreach (Iteration iteration in this.iterations)
                {
                    if (iteration.pInfo != null)
                    {
                        iteration.startValue = iteration.pInfo.GetValue(iteration.target, null);
                        continue;
                    }

                    if (iteration.fInfo != null)
                    {
                        iteration.startValue = iteration.fInfo.GetValue(iteration.target);
                        continue;
                    }
                }
            }

            private static object Interpolate(object num1, object num2, float gradient)
            {
                return Sum(num1, Mul(Sub(num2, num1), gradient));
            }

            private static object Sum(object num1, object num2)
            {
                var sumOp = num1.GetType().GetMethod("op_Addition");
                if (sumOp == null)
                {
                    return System.Convert.ToSingle(num1) + System.Convert.ToSingle(num2);
                }
                return sumOp.Invoke(null, new object[] { num1, num2 });
            }

            private static object Sub(object num1, object num2)
            {
                var subOp = num1.GetType().GetMethod("op_Subtraction");
                if (subOp == null)
                {
                    return System.Convert.ToSingle(num1) - System.Convert.ToSingle(num2);
                }
                return subOp.Invoke(null, new object[] { num1, num2 });
            }

            private static object Mul(object num1, object num2)
            {
                var mulOp = num1.GetType().GetMethod("op_Multiply", new[] { num1.GetType(), typeof(float) });
                if (mulOp == null)
                {
                    return System.Convert.ToSingle(num1) * System.Convert.ToSingle(num2);
                }
                return mulOp.Invoke(null, new object[] { num1, num2 });
            }

            public void Step(float gradient)
            {
                foreach (Iteration iteration in this.iterations)
                {
                    if (iteration.pInfo != null)
                    {
                        iteration.pInfo.SetValue(iteration.target, Interpolate(iteration.startValue, iteration.endValue, gradient), null);
                        continue;
                    }
                    if (iteration.fInfo != null)
                    {
                        iteration.fInfo.SetValue(iteration.target, Interpolate(iteration.startValue, iteration.endValue, gradient));
                        continue;
                    }
                }
            }

        }

        private List<KeyFrame> keyFrames;
        private bool isStarted;
        public bool IsPlaying
        {
            get
            {
                return this.isStarted;
            }
        }

        private bool isPaused;
        public bool IsPaused
        {
            get
            {
                return this.isPaused;
            }
        }

        private int currentKeyFrame;
        public int CurrentKeyFrameIndex
        {
            get
            {
                return this.currentKeyFrame;
            }
        }

        public int KeyFramesCount
        {
            get
            {
                return this.keyFrames.Count;
            }
        }

        public float CurrentKeyFrameStartedAt
        {
            get
            {
                return this.keyFrames[this.currentKeyFrame].startedAt;
            }
        }

        /// <summary>
        /// Easing function.
        /// </summary>
        public delegate float EasingFunction(float n);

        private EasingFunction easing;
        private int repeat;
        private int currentRound;
        private float currentGradient;
        private float now;
        private float deltaNowAccumulator;

        /// <summary>
        /// Get the current computed Tween gradient.
        /// </summary>
        /// <value>The current gradient.</value>
        public float Gradient
        {
            get
            {
                return this.currentGradient;
            }
        }

        /// <summary>
        /// Get the current Tween update time
        /// </summary>
        /// <value>The current update time.</value>
        public float Now
        {
            get
            {
                return this.now;
            }
        }

        /// <summary>
        /// Get the current Tween round.
        /// </summary>
        /// <value>The current round.</value>
        public int Round
        {
            get
            {
                return this.currentRound;
            }
        }

        public Tween()
        {
            this.keyFrames = new List<KeyFrame>();
            this.easing = (n => n);
        }


        /// <summary>
        /// Create a KeyFrame with the specified values
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="values">Values.</param>
        /// <param name="duration">Duration.</param>
        public Tween To(object target, object values, float duration)
        {
            return To(target, values, (object)duration);
        }


        /// <summary>
        /// Create a KeyFrame with multiple transformations.
        /// </summary>
        /// <param name="items">Items (target, properties, target, properties, ... , duration).</param>
        public Tween To(params object[] items)
        {
            if (items.Length < 3)
                throw new ArgumentException("invalid number of arguments");
            float duration = System.Convert.ToSingle(items[items.Length - 1]);

            KeyFrame keyFrame = new KeyFrame(duration, this.easing);

            for (int i = 0; i < items.Length - 1; i += 2)
            {
                object target = items[i];
                object values = items[i + 1];
                foreach (PropertyInfo pInfo in values.GetType().GetProperties())
                {
                    object endValue = pInfo.GetValue(values, null);
                    keyFrame.AddIteration(target, pInfo.Name, endValue);
                }
            }

            this.keyFrames.Add(keyFrame);
            return this;
        }

        /// <summary>
        /// Set the easing function.
        /// </summary>
        /// <param name="easing">Easing function.</param>
        public Tween SetEasing(EasingFunction easing)
        {
            this.easing = easing;
            return this;
        }

        /// <summary>
        /// Repeat the Tween animation n times.
        /// </summary>
        /// <param name="n">N.</param>
        public Tween Repeat(int n)
        {
            // check for loops
            if (n < 0)
            {
                bool safe = false;
                foreach (KeyFrame keyFrame in this.keyFrames)
                {
                    if (keyFrame.Duration > 0)
                    {
                        safe = true;
                        break;
                    }
                }
                if (!safe)
                {
                    throw new Exception("Loop detected");
                }
            }
            this.repeat = n;
            return this;
        }

        /// <summary>
        /// Loop this Tween.
        /// </summary>
        public Tween Loop()
        {
            return this.Repeat(-1);
        }

        /// <summary>
        /// Add a sleeping keyframe of the specified duration.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public Tween Delay(float duration)
        {
            KeyFrame keyFrame = new KeyFrame(duration, this.easing);
            this.keyFrames.Add(keyFrame);
            return this;
        }

        /// <summary>
        /// Call the specified handler as KeyFrame.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public Tween Call(KeyFrameHandler handler)
        {
            KeyFrame keyFrame = new KeyFrame(0, this.easing, handler);
            this.keyFrames.Add(keyFrame);
            return this;
        }

        /// <summary>
        /// Empty KeyFrame.
        /// </summary>
        public Tween Nop()
        {
            KeyFrame keyFrame = new KeyFrame(0, this.easing, null);
            this.keyFrames.Add(keyFrame);
            return this;
        }


        /// <summary>
        /// Start the Tween.
        /// </summary>
        public Tween Start()
        {
            if (this.keyFrames.Count < 1)
            {
                throw new Exception("Tween without keyframes");
            }
            this.isPaused = false;
            this.currentRound = 0;
            this.isStarted = true;
            this.currentKeyFrame = 0;
            this.keyFrames[currentKeyFrame].startedAt = -1;
            this.deltaNowAccumulator = 0;

            if (OnStart != null)
            {
                OnStart(this);
            }

            return this;
        }

        /// <summary>
        /// Stop the Tween.
        /// </summary>
        public Tween Stop()
        {
            isStarted = false;
            if (OnStop != null)
            {
                OnStop(this);
            }
            return this;
        }

        public Tween Pause()
        {
            this.isPaused = true;
            return this;
        }

        public Tween Resume()
        {
            this.isPaused = false;
            return this;
        }

        /// <summary>
        /// Update the Tween with a deltaTime.
        /// </summary>
        /// <param name="deltaTime">deltaTime.</param>
        public Tween DeltaUpdate(float deltaTime)
        {
            // redundancy, required for avoiding increasing timesteps
            if (!isStarted)
                return this;
            if (isPaused)
                return this;
            // special condition for first round
            KeyFrame keyFrame = this.keyFrames[currentKeyFrame];
            if (keyFrame.startedAt < 0)
            {
                keyFrame.startedAt = 0;
                keyFrame.SetupIterations();
            }
            this.deltaNowAccumulator += deltaTime;
            return this.Update(this.deltaNowAccumulator);
        }

        /// <summary>
        /// Update the Tween.
        /// </summary>
        /// <param name="now">Now.</param>
        public Tween Update(float now)
        {
            if (!isStarted)
                return this;
            if (isPaused)
                return this;
            // allow the Tween class to export the current time
            this.now = now;

            bool nextFrame = false;
            KeyFrame keyFrame = this.keyFrames[currentKeyFrame];
            // first round ?
            if (keyFrame.startedAt < 0)
            {
                keyFrame.startedAt = now;
                keyFrame.SetupIterations();
            }


            float gradient = 1;
            if (keyFrame.Duration > 0)
                gradient = (this.now - keyFrame.startedAt) / keyFrame.Duration;

            // avoid overflow
            gradient = gradient > 1 ? 1 : gradient;
            if (gradient >= 1)
                nextFrame = true;

            // apply easing
            this.currentGradient = keyFrame.Ease(gradient);
            // foreach iteration update values
            keyFrame.Step(this.currentGradient);

            keyFrame.RunHandler(this);

            if (OnUpdate != null)
            {
                OnUpdate(this);
            }

            if (nextFrame)
            {
                if (this.currentKeyFrame + 1 >= this.keyFrames.Count)
                {
                    // restart ?
                    this.currentRound++;
                    if (this.repeat > -1 && this.currentRound >= this.repeat)
                    {
                        this.Stop();
                        return this;
                    }
                    this.currentKeyFrame = 0;
                }
                else {
                    this.currentKeyFrame++;
                }
                this.keyFrames[this.currentKeyFrame].startedAt = this.now;
                this.keyFrames[this.currentKeyFrame].SetupIterations();
                // call non-time-based following keyframes as soon as possible
                if (this.keyFrames[this.currentKeyFrame].Duration <= 0)
                {
                    this.Update(now);
                }
            }


            return this;
        }


    }
}
