using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Interfaces
{
    public delegate void OnExceptionDelegate(object sender, Exception ex);
    public interface IThrowsException
    {
        event OnExceptionDelegate OnException;
        void Throw(object sender, Exception ex);
    }
}
