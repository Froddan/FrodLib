using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.IoC
{
    public static class IoCScannerExtensions
    {
        /// <summary>
        /// Adds the calling assembly to be included with automatic type registration
        /// </summary>
        public static void ScanCallingAssembly(this IIoCScanner scanner)
        {
            if (scanner != null)
            {
                var callingAsm = Assembly.GetCallingAssembly();
                scanner.ScanAssembly(callingAsm);
            }
        }


        public static void ScanAssembliesFromApplicationBaseDirectory(this IIoCScanner scanner)
        {
            ScanAssembliesFromApplicationBaseDirectory(scanner, null);
        }

        public static void ScanAssembliesFromApplicationBaseDirectory(this IIoCScanner scanner, Predicate<Assembly> filter)
        {
            if (scanner != null)
            {
                var entryAsm = Assembly.GetEntryAssembly();
                var applicationLocation = Path.GetDirectoryName(entryAsm.Location);
                var assemblyFiles = Directory.EnumerateFiles(applicationLocation, "*.dll", SearchOption.TopDirectoryOnly);
                foreach (var assemblyFile in assemblyFiles)
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(assemblyFile);
                        if (filter == null || filter(asm))
                        {
                            scanner.ScanAssembly(asm);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Error occured when loading assembly: " + assemblyFile);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    }

                }
            }
        }
    }
}
