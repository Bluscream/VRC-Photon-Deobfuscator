﻿using System;
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
            Console.WriteLine("Starting Deobfuscation");
            Deobfuscator d = new Deobfuscator(@"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat_Data\Managed\Assembly-CSharp.dll");
            d.RenameAllClasses();
            d.RenameAllClassProperties();

            Console.WriteLine("Saving Assembly... DO NOT close deobfuscator...");
            d.LogAllRenamedItems();
         //   Deobfuscator.assembly.Write(@"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat_Data\Managed\deobfuscatedAssembly.dll");
            Console.WriteLine("Saving finished. You may now close the deobfuscator.");
            Console.ReadLine();
        }
    }
}
