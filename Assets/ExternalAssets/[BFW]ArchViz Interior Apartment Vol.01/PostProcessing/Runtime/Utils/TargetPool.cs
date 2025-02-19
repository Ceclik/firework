using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
    internal class TargetPool
    {
        private readonly List<int> m_Pool;
        private int m_Current;

        internal TargetPool()
        {
            m_Pool = new List<int>();
            Get(); // Pre-warm with a default target to avoid black frame on first frame
        }

        internal int Get()
        {
            var ret = Get(m_Current);
            m_Current++;
            return ret;
        }

        private int Get(int i)
        {
            int ret;

            if (m_Pool.Count > i)
            {
                ret = m_Pool[i];
            }
            else
            {
                // Avoid discontinuities
                while (m_Pool.Count <= i)
                    m_Pool.Add(Shader.PropertyToID("_TargetPool" + i));

                ret = m_Pool[i];
            }

            return ret;
        }

        internal void Reset()
        {
            m_Current = 0;
        }
    }
}