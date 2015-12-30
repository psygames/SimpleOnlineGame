using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    class FrameSyncManager
    {
        public const int maxFrameBuffedSize = 2;
        public const int syncInOutMuti = 4;

        public FrameSyncManager()
        {
            m_frameStateDic = new Dictionary<long, List<FrameState>>();
        }

        public Dictionary<long, List<FrameState>> m_frameStateDic = null;


        public void FrameAdd(long guid, FrameState state)
        {
            if (!m_frameStateDic.ContainsKey(guid))
                m_frameStateDic.Add(guid, new List<FrameState>());
            m_frameStateDic[guid].Add(state);
        }


        public FrameState GetPlayerState(long guid)
        {
            if (!m_frameStateDic.ContainsKey(guid) || m_frameStateDic[guid].Count <= 0)
                return FrameState.zero;

            int curFrame = Math.Min(m_frameStateDic[guid].Count, syncInOutMuti + 1);
            FrameState result = m_frameStateDic[guid][curFrame - 1];
            int willRemoveFrams = 0;
            if (m_frameStateDic[guid].Count <= syncInOutMuti)
            {
                willRemoveFrams = curFrame - 1;
            }
            else if (m_frameStateDic[guid].Count > syncInOutMuti + maxFrameBuffedSize)
            {
                willRemoveFrams = m_frameStateDic[guid].Count - 1;
            }
            else
            {
                willRemoveFrams = syncInOutMuti;
            }
            for (int i = 0; i < willRemoveFrams; i++)
            {
                m_frameStateDic[guid].RemoveAt(0);
            }
            return result;
        }




        private static FrameSyncManager s_instance;
        public static FrameSyncManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new FrameSyncManager();
                }
                return s_instance;
            }
        }
    }

    public class FrameState
    {
        public Vector3 pos;
        public Vector3 dir;

        public FrameState(Vector3 pos, Vector3 dir)
        {
            this.pos = pos;
            this.dir = dir;
        }

        public static FrameState zero = new FrameState(Vector3.zero, Vector3.zero);
    }
}
