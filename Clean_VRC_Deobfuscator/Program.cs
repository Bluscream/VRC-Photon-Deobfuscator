using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            var inFile = new FileInfo(args.Length > 0 ? args[0] : @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat_Data\Managed\Assembly-CSharp.dll");
            Deobfuscator d = new Deobfuscator(inFile.FullName);
            Console.WriteLine("Starting Deobfuscation on " + inFile.FullName);
            d.RenameAllClasses();
            d.RenameAllClassProperties();
            d.RenameAllEnums();

            Console.WriteLine("Saving Assembly... DO NOT close deobfuscator...");
            d.RenameClassMethods();
            d.LogAllRenamedItems(); // dass

            try {
                d.CompareClasses();
            } catch (Exception ex) {
                Console.WriteLine($"Failed to CompareClasses: {ex.Message}");
            }
            var outFile = new FileInfo(args.Length > 0 ? Path.Combine(inFile.DirectoryName, Path.GetFileNameWithoutExtension(inFile.Name) + "_deobfuscated.dll") : @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat_Data\Managed\deobfuscatedAssemblyFinal.dll");
            Console.WriteLine($"Saving Assembly to \"{outFile.FullName}\" ... DO NOT close deobfuscator...");
            Deobfuscator.assembly.Write(outFile.FullName);
            Console.WriteLine("Saving finished. You may now close the deobfuscator.");
            Console.ReadLine();
        }
    }
}
