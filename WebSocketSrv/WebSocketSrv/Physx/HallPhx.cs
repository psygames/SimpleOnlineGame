using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ShitMan
{
    class HallPhx
    {
        Timer timer = null;
        
        public void Init()
        {
 
        }

        public void Start()
        {
            timer = new Timer();
            timer.Interval = PhysxConfig.physxCheckInterval * 1000f;
            timer.Elapsed += TimerTicks;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private long lastTicks = 0;

        private float m_fixedDtTimeCount = 0;
        private float m_dtTimeCount = 0;
        public void TimerTicks(object sender, ElapsedEventArgs e)
        {
            long ticks = DateTime.Now.Ticks;
            if (lastTicks == 0)
                lastTicks = ticks;
            float dt = (ticks - lastTicks) * 0.0000001f;
            lastTicks = ticks;


            m_fixedDtTimeCount += dt;
            if (m_fixedDtTimeCount >= 0.1f)
            {
                m_fixedDtTimeCount -= 0.1f;
                EntryManager.Instance.fixedDeltaTime = 0.1f;
                EntryManager.Instance.FixedUpdate();

            }

            m_dtTimeCount += dt;
            if (m_dtTimeCount >= 0.02f)
            {
                m_dtTimeCount -= 0.02f;
                EntryManager.Instance.deltaTime = 0.02f;
                EntryManager.Instance.Update();
            }
        }
    }
}
