using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator.ClassComparer
{
    class Signature
    {
        public static bool CreateSignature(TypeDef targetClass, string path)
        {
            List<MethodDef> classMethods = targetClass.Methods.ToList();

            throw new NotImplementedException();
        }
    }
}
