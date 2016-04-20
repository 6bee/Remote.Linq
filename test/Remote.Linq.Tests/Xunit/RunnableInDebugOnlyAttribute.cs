// source: http://lostechies.com/jimmybogard/2013/06/20/run-tests-explicitly-in-xunit-net/

namespace Xunit
{
    using System.Diagnostics;

    public class RunnableInDebugOnlyAttribute : FactAttribute
    {
        public RunnableInDebugOnlyAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only running in interactive mode.";
            }
        }
    }
}
