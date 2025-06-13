using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AI.Xunit.Tools
{
    public interface IAIFunctionAdapter
    {
        AIFunction AIFunction { get; }
    }
}
