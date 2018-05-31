using System;
using System.Collections;
using System.Linq;

namespace TServer.Common.Content
{
    public interface IContent
    {
        object GetField(ContentParam param);
    }
}
