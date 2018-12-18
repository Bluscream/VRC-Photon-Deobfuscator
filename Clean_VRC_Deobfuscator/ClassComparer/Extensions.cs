using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator.ClassComparer
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a list of all strings inside of the method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static List<string> GetMethodStrings(this MethodDef method)
        {
            List<string> methodStrings = new List<string>();
            if (method.Body != null && method.HasStrings())
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldstr)
                    {
                        methodStrings.Add(instruction.ToString());
                    }
                }
            }

            return methodStrings;
        }

        /// <summary>
        /// Returns true if the method has the string specified in the text parameter. If not, then it returns false.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ContainsString(this MethodDef method, string text)
        {
            if (method.Body == null || method.Body.Instructions.Count == 0)
                return false;

            foreach (var instruction in method.Body.Instructions)
            {
                
                if (instruction.OpCode == OpCodes.Ldstr && instruction.ToString().Contains(text))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the method has ANY strings. Otherwise, it returns false
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool HasStrings(this MethodDef method)
        {
            if (method.Body != null && method.Body.Instructions.Count > 0)
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldstr)
                        return true;
                }
            }

            return false;
        }

        public static List<string> GetStrings(this TypeDef classWithStrings)
        {
            List<string> classStrings = new List<string>();
            foreach(var method in classWithStrings.Methods)
            {
                foreach(var text in method.GetMethodStrings())
                {
                    classStrings.Add(text);
                }
            }

            return classStrings;
        }
    }
}
