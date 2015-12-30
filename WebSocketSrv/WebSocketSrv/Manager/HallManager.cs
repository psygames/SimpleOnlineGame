using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    class HallManager
    {
        private static HallManager m_instance;
        public static HallManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new HallManager();
                return m_instance;
            }
        }
        private HallForm m_hallForm = null;
        public void SetHallForm(HallForm form)
        {
            m_hallForm = form;
        }

        public void Start()
        {
            HallPhx phx = new HallPhx();
            phx.Init();
            phx.Start();
        }

        public void Print(string str)
        {
            m_hallForm.PrintText(str);
        }
    }
}
