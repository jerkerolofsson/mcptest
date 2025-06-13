using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AI.Xunit.Tools;
public interface IToolConfigurator
{
    void Add(Delegate method);
    void AddMcpServerToolsFromAssembly(Assembly toolAssembly);
    void AddMcpServerToolsFromType(Type toolType);
    void AddMcpServerToolsFromType<T>();
}
