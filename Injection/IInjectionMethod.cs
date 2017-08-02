using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadlibrayy.Injection
{
    public enum ExecutionType
    {
        CreateThread,
        HijackThread
    }
    
    interface IInjectionMethod
    {
        ExecutionType TypeOfExecution { get; }
        Process TargetProcess { get; }
        bool ShouldElevateHandle { get; }
        bool ShouldEraseHeaders { get; }

        bool InjectImage(string imagePath);
        bool InjectImage(byte[] rawImage);
    }
}
