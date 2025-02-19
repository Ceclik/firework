using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
    // Small wrapper on top of AnimationCurve to handle zero-key curves and keyframe looping
    [Serializable]
    public sealed class Spline
    {
        public const int k_Precision = 128;
        public const float k_Step = 1f / k_Precision;

        public AnimationCurve curve;

        [SerializeField] private bool m_Loop;

        [SerializeField] private float m_ZeroValue;

        [SerializeField] private float m_Range;

        // Instead of trying to be smart and blend two curves by generating a new one, we'll simply
        // store the curve data in a float array and blend these instead.
        internal float[] cachedData;

        // Used to track frame changes for data caching
        private int frameCount = -1;

        private AnimationCurve m_InternalLoopingCurve;

        public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
        {
            Assert.IsNotNull(curve);
            this.curve = curve;
            m_ZeroValue = zeroValue;
            m_Loop = loop;
            m_Range = bounds.magnitude;
            cachedData = new float[k_Precision];
        }

        public void Cache(int frame)
        {
            // Only cache once per frame
            if (frame == frameCount)
                return;

            var length = curve.length;

            if (m_Loop && length > 1)
            {
                if (m_InternalLoopingCurve == null)
                    m_InternalLoopingCurve = new AnimationCurve();

                var prev = curve[length - 1];
                prev.time -= m_Range;
                var next = curve[0];
                next.time += m_Range;
                m_InternalLoopingCurve.keys = curve.keys;
                m_InternalLoopingCurve.AddKey(prev);
                m_InternalLoopingCurve.AddKey(next);
            }

            for (var i = 0; i < k_Precision; i++)
                cachedData[i] = Evaluate(i * k_Step);

            frameCount = Time.renderedFrameCount;
        }

        public float Evaluate(float t)
        {
            if (curve.length == 0)
                return m_ZeroValue;

            if (!m_Loop || curve.length == 1)
                return curve.Evaluate(t);

            return m_InternalLoopingCurve.Evaluate(t);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 +
                       curve.GetHashCode(); // Not implemented in Unity, so it'll always return the same value :(
                return hash;
            }
        }
    }
}