using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingConventionsCop;

namespace CodingConventionsCop.Tests
{
    [TestClass]
    public class OneFileOneTypeDeclarationAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void OneClassOnly()
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
        public void OneStructOnly()
        {
            string test = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    Struct MyStruct
    {   
    }
}";
            VerifyNoDiagnostic(test);
        }

        [TestMethod]
        public void OneClassAndOneStruct()
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

    struct MyStruct
    {
    }
}";
            VerifyDiagnostic(test, DiagnosticIds.OneFileOneTypeDeclaration);
        }

        [TestMethod]
        public void OneClassAndOneInterface()
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

    interface IMyInterface
    {
    }
}";
            VerifyDiagnostic(test, DiagnosticIds.OneFileOneTypeDeclaration);
        }

        [TestMethod]
        public void OneClassAndOneEnum()
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

    enum MyEnum
    {
        Value1,
        Value2
    }
}";
            VerifyDiagnostic(test, DiagnosticIds.OneFileOneTypeDeclaration);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new OneFileOneTypeDeclarationAnalyzer();
        }
    }
}
