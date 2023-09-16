using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExamples.IntegrationTests.Application.Examine
{
    public class CustomRebuildOnStartupHandlerState
    {
        public bool _isReady;
        public bool _isReadSet;
        public object? _isReadyLock;
    }
}
