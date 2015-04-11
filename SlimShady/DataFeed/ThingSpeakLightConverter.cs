using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SlimShady.DataFeed
{
    public static class ThingSpeakLightConverter
    {
        private static String code =
              "using System;"
            + "namespace DynamicSlimShady"
            + "{"
            + "   public class DynamicGraphEval"
            + "   {"
            + "       public static double eval(double x)"
            + "       {"
            + "           double y = 50;"
            + "           /* INSERT CODE HERE */;"
            + "           return y;"
            + "       }"
            + "   }"
            + "}";
        private static String mFunction = "y = 42";
        public static String Function
        {
            get { return mFunction; }
            set
            {
                mFunction = value;
                evalImplReset = true;
            }
        }
        private static MethodInfo mEvalImpl;
        private static bool evalImplReset = true;
        private static MethodInfo MEvalImpl
        {
            get
            {
                if (evalImplReset)
                {
                    MainWindow.Trace("New light Function: " + Function);
                    String codeToCompile = code.Replace("/* INSERT CODE HERE */", Function);
                    //Assembly compiledAssembly = Compile(codeToCompile);
                    Assembly compiledAssembly = CompileRoslyn(codeToCompile);
                    if (compiledAssembly != null)
                    {
                        Module module = compiledAssembly.GetModules()[0];
                        Type mt = module.GetType("DynamicSlimShady.DynamicGraphEval");
                        mEvalImpl = mt.GetMethod("eval");
                    }
                }
                evalImplReset = false;
                return mEvalImpl;
            }
        }

        public static double DefaultLightLevel = 50;
        public static double Eval(double x)
        {
            try
            {
                var f = MEvalImpl;
                if (f == null)
                    return DefaultLightLevel;
                double y = (double)f.Invoke(null, new Object[] { x });
                //MainWindow.Trace("Evaluated(" + x + ") --> " + y);
                return Math.Max(0, Math.Min(100, y));
            }
            catch (Exception e)
            {
                MainWindow.Error("An Error occured while calculating monitor settings: " + e);
            }
            return DefaultLightLevel;
        }

        private static Assembly CompileRoslyn(String codeToCompile)
        {
            string assemblyFileName = "gen" + Guid.NewGuid().ToString().Replace("-", "") + ".dll";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            MetadataReference[] references = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    //MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.dll")),
                };
            
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyFileName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    foreach (Diagnostic diagnostic in failures)
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    return null;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }

        /*
        [Obsolete]
        private static Assembly Compile(String codeToCompile)
        {
            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                GenerateExecutable = false,
                IncludeDebugInformation = false
            };
            //CompilerParams.CompilerOptions = "/optimize";
            compilerParams.ReferencedAssemblies.AddRange(new String[] { "System.dll" });

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(compilerParams, codeToCompile);
            if (compile.Errors.HasErrors)
            {
                MainWindow.Error("Compile Error: ");
                foreach (CompilerError ce in compile.Errors)
                    MainWindow.Error("- " + ce);
                evalImplReset = false;
                return null;
            }
            return compile.CompiledAssembly;
        }
        */
    }
}
