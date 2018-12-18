using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator
{
    class OnEventDetour
    {

        public void ReplacementMethod()
        {
            string targetExePath = @"C:\Users\colton\source\repos\MethodReplaceTarget\MethodReplaceTarget\bin\Debug\MethodReplaceTarget.exe";


            ModuleDefMD target = ModuleDefMD.Load(targetExePath);


            var targetClass = target.Types.Single(x => x.Name == "Program");
            var targetMethod = targetClass.Methods.Single(x => x.Name == "MethodToBeReplaced");
            var sourceClass = target.Types.Single(x => x.Name == "Program");
            var sourceMethod = sourceClass.Methods.Single(x => x.Name == "MethodToEdit");

            
            List<Instruction> instructions = new List<Instruction>();
            instructions.Add(targetMethod.Body.Instructions.ElementAt(1));
            instructions.Add(targetMethod.Body.Instructions.ElementAt(2));
            instructions.Add(targetMethod.Body.Instructions.ElementAt(3));
            instructions.Add(targetMethod.Body.Instructions.ElementAt(4));

            /*
            foreach (var instruction in instructions)
            {
                Console.WriteLine(instruction.ToString());
            }

            int i = 1;
            foreach(var instruction in instructions)
            {
                sourceMethod.Body.Instructions.Insert(i, instruction);
                i++;
            }
            target.Write(@"C:\Users\colton\source\repos\MethodReplaceTarget\MethodReplaceTarget\bin\Debug\ReplacedFinal.exe");
            */

        }
    }
}
