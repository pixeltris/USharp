using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    class Program
    {
        static string additionalAssemblySearchPath;

        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            UnrealTypes.Load();
            AssemblyRewriter rewriter = new AssemblyRewriter();

            if (System.Diagnostics.Debugger.IsAttached && args.Length == 0)
            {
                ManagedUnrealReflectionBase.UpdateSerializerCode();
                //RunTests(rewriter);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            foreach (string filePath in args)
            {
                ProcessAssembly(rewriter, filePath);
                additionalAssemblySearchPath = null;
            }

            stopwatch.Stop();
            Console.WriteLine("AssemblyRewriter finished " + stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyPath = Path.Combine(additionalAssemblySearchPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            return null;
        }

        private static void RunTests(AssemblyRewriter rewriter)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(dir, "../", "UnrealEngine.Runtime.dll");
            if (File.Exists(assemblyPath))
            {
                ProcessAssembly(rewriter, assemblyPath);
            }
        }

        private static bool ProcessAssembly(AssemblyRewriter rewriter, string filePath)
        {
            if (File.Exists(filePath))
            {
                System.Reflection.Assembly assembly = null;

                string pdbFilePath = Path.ChangeExtension(filePath, ".pdb");
                if (Path.GetFileNameWithoutExtension(filePath) == "UnrealEngine.Runtime")
                {
                    // Required so that type comparisons work correctly (this should only be used for testing purposes)
                    assembly = typeof(ManagedUnrealModuleInfo).Assembly;
                }
                else if (File.Exists(pdbFilePath))
                {
                    assembly = System.Reflection.Assembly.Load(File.ReadAllBytes(filePath), File.ReadAllBytes(pdbFilePath));
                }
                else
                {
                    assembly = System.Reflection.Assembly.Load(File.ReadAllBytes(filePath));                    
                }

                additionalAssemblySearchPath = Path.GetDirectoryName(filePath);

                if (ManagedUnrealModuleInfo.AssemblyHasSerializedModuleInfo(assembly))
                {
                    if (ManagedUnrealModuleInfo.LoadModuleFromAssembly(assembly) == null)
                    {
                        throw new Exception("Assembly already rewritten but failed to deserialize module info " + assembly.GetName().Name);
                    }
                    Console.WriteLine("Skip \"" + assembly.GetName().Name + "\"");
                    return true;
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ManagedUnrealModuleInfo moduleInfo = null;
                try
                {
                    moduleInfo = ManagedUnrealModuleInfo.CreateModuleFromAssembly(assembly);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when parsing module \"" + assembly.GetName().Name + "\"");
                    Console.WriteLine();
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine(e.ToString());
                }
                stopwatch.Stop();
                Console.WriteLine("Read \"" + assembly.GetName().Name + "\" " + stopwatch.Elapsed);

                if (moduleInfo != null)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    try
                    {
                        rewriter.RewriteModule(moduleInfo, filePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error when rewriting module \"" + assembly.GetName().Name + "\"");
                        Console.WriteLine();
                        Console.WriteLine(e.Message);
                        Console.WriteLine();
                        Console.WriteLine(e.ToString());
                    }

                    stopwatch.Stop();
                    Console.WriteLine("Write \"" + assembly.GetName().Name + "\" " + stopwatch.Elapsed);
                    return true;
                }
            }
            return false;
        }
    }
}
