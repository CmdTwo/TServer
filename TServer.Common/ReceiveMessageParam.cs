using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common
{
    public enum ReceiveMessageParam:byte
    {
        CommandType = 1,
        IsRequest = 2,
        Params = 3
    }
}
