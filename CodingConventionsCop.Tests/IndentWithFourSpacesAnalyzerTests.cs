using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingConventionsCop;

namespace CodingConventionsCop.Tests
{
    [TestClass]
    public class IndentWithFourSpacesAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void IndentWithFourSpaces()
        {
            string test = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {   
    }
}";
            VerifyNoDiagnostic(test);
        }

        [TestMethod]
        public void IndentWithTabs()
        {
            string test = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
	class Program // there is a ""Tab"" before ""class""
    {   
    }
}";
            VerifyDiagnostic(test, DiagnosticIds.IndentWithFourSpaces);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new IndentWithFourSpacesAnalyzer();
        }
    }
}
