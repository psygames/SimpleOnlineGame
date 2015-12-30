using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ShitMan
{
    [Serializable]
    public class MessageBase
    {
        public MessageTypeEnum type;
        public MessageBase() { }
        public MessageBase(MessageTypeEnum type)
        {
            this.type = type;
        }

        public override string ToString()
        {
            return ""+type;
        }
    }
}
