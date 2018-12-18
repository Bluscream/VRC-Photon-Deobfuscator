using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator.ClassComparer
{
    public static class Comparer
    {
        /// <summary>
        /// Finds the class most similar to the obfuscated class specified based on strings inside of the class. CURRENTLY BROKEN!
        /// </summary>
        /// <param name="obfuscatedClass"></param>
        /// <param name="deobfuscatedTypes"></param>
        /// <returns></returns>
        public static KeyValuePair<TypeDef, int> FindMostLikelyDeobfuscatedClass(TypeDef obfuscatedClass, List<TypeDef> deobfuscatedTypes)
        {
            Dictionary<TypeDef, List<string>> classStrings = new Dictionary<TypeDef, List<string>>();

            foreach(var deobfuscatedType in deobfuscatedTypes)
            {
                classStrings.Add(deobfuscatedType, deobfuscatedType.GetStrings());
            }

            // calculate amount of shared strings

            Dictionary<TypeDef, int> classAndAmountOfMatchingItems = new Dictionary<TypeDef, int>();
            foreach(var pair in classStrings)
            {
                int totalMatchingStrings = 0;
                foreach(var text in obfuscatedClass.GetStrings())
                {
                    if(pair.Value.Contains(text))
                    {
                        totalMatchingStrings++;
                    }
                }
                classAndAmountOfMatchingItems.Add(pair.Key, totalMatchingStrings);

            }


            return classAndAmountOfMatchingItems.First(x => x.Value == classAndAmountOfMatchingItems.Values.Max());

        }

        /// <summary>
        /// Compares two classes and looks for identical methods based on the strings they contain.
        /// </summary>
        /// <param name="classOne">obfuscated class</param>
        /// <param name="classTwo"> deobfuscated class</param>
        /// <returns></returns>
        public static Dictionary<MethodDef, List<MethodDef>> CompareClassMethods(TypeDef classOne, TypeDef classTwo)
        {
            List<MethodDef> classOneStringMethods = new List<MethodDef>();

            List<MethodDef> classTwoStringMethods = new List<MethodDef>();

            foreach(var method in classOne.Methods)
            {
                if(method.HasStrings())
                {
                    classOneStringMethods.Add(method);
                }
            }

            foreach(var method in classTwo.Methods)
            {
                if(method.HasStrings())
                {
                    classTwoStringMethods.Add(method);
                }
            }

            Dictionary<MethodDef, List<MethodDef>> methodsThatContainString = new Dictionary<MethodDef, List<MethodDef>>();

            foreach(var method in classOneStringMethods)
            {
                foreach(var methodTwo in classTwoStringMethods)
                {
                    var methodTwoStrings = methodTwo.GetMethodStrings();
                    if (!methodsThatContainString.ContainsKey(methodTwo))
                    {
                        var identicalMethods = classOneStringMethods.Where(x => x.GetMethodStrings().SequenceEqual(methodTwoStrings)).ToList();
                        if (identicalMethods.Count > 0)
                        {
                            methodsThatContainString.Add(methodTwo, identicalMethods);
                        }
                    }
                }
            }

            return methodsThatContainString;

        }

        /// <summary>
        /// Renames all unique fields in a class based off a deobfuscated class
        /// </summary>
        /// <param name="obfuscatedClass"></param>
        /// <param name="deobfuscatedClass"></param>
        /// <returns></returns>
        public static Dictionary<TypeDef, FieldDef> CompareClassFields(TypeDef obfuscatedClass, TypeDef deobfuscatedClass)
        {
            Dictionary<TypeDef, FieldDef> fieldsToRename = new Dictionary<TypeDef, FieldDef>();
            foreach (var field in deobfuscatedClass.Fields)
            {
                var possibleFields = obfuscatedClass.Fields.Where(x => x.Access == field.Access && x.IsStatic == field.IsStatic && x.HasCustomAttributes == field.HasCustomAttributes && x.Attributes == field.Attributes && x.FieldType == field.FieldType);
                if(possibleFields.Count() == 1)
                {
                    Console.WriteLine(possibleFields.First().Name + "IS FIELD THAT CAN BE RENAMED");
                    fieldsToRename.Add(possibleFields.First().DeclaringType, possibleFields.First());
                }
            }
            return fieldsToRename;

           
        }

        /// <summary>
        /// Looks for identical methods by comparing their IL instruction count and opcodes. It looks for exactly identical methods in the deobfuscated assembly and then renames them.
        /// </summary>
        /// <param name="obfuscatedClass"></param>
        /// <param name="deobfuscatedClass"></param>
        /// <returns></returns>
        public static Dictionary<MethodDef, List<MethodDef>> CompareClassIL(TypeDef obfuscatedClass, TypeDef deobfuscatedClass)
        {
            List<MethodDef> obfuscatedMethods = new List<MethodDef>();

            List<MethodDef> deobfuscatedMethods = new List<MethodDef>();

            Dictionary<MethodDef, List<MethodDef>> identicalMethods = new Dictionary<MethodDef, List<MethodDef>>();

            foreach(var method in deobfuscatedClass.Methods.Where(x => x.Body.Instructions.Count > 6 && x.IsGetter == false && x.IsSetter == false))
            {
                if(method.Body != null && method.Body.Instructions.Count > 0)
                {
                    var matches = obfuscatedClass.Methods.Where(x => x.Body.Instructions.Count == method.Body.Instructions.Count && x.IsStatic == method.IsStatic && method.IsVirtual == x.IsVirtual && method.Parameters.Count == x.Parameters.Count && method.IsPublic == x.IsPublic && method.Access == x.Access && method.Attributes == x.Attributes && method.CallingConvention == x.CallingConvention);

                    List<MethodDef> methods = new List<MethodDef>();

                    int totalIdenticalInstructions = 0;
                    foreach(var match in matches )
                    {
                        for(int i = 0; i < match.Body.Instructions.Count; i++)
                        {
                            if(match.Body.Instructions[i].OpCode == method.Body.Instructions[i].OpCode)
                            {
                                totalIdenticalInstructions++;

                                if(totalIdenticalInstructions == method.Body.Instructions.Count)
                                {
                                    methods.Add(match);
                                }
                            }
                        }
                    }

                    if(!identicalMethods.ContainsKey(method) && methods.Count == 1)
                    {
                        identicalMethods.Add(method, methods);
                    }
                }
            }

            return identicalMethods;

        }

        /// <summary>
        /// Calls this.Compare and renames all identical methods. Uses strings to identify methods
        /// </summary>
        /// <param name="obfuscatedClass"></param>
        /// <param name="deobfuscatedClass"></param>
        /// <returns></returns>
        public static Dictionary<MethodDef, List<MethodDef>> RewriteComparedClassMethods(TypeDef obfuscatedClass, TypeDef deobfuscatedClass)
        {
            var comparedClasses = CompareClassMethods(obfuscatedClass, deobfuscatedClass);
            foreach (var pair in comparedClasses)
            {
                if (pair.Value.Count == 1)
                {
                    pair.Value.First().Name = pair.Key.Name;
                    Console.WriteLine("Renamed " + pair.Value.First().DeclaringType.Name + "." + pair.Value.First().Name + " to " + pair.Key.Name);

                    // key is the deobfuscated method, value is the obfuscated method
                    for (int i = 0; i < pair.Key.Parameters.Count; i++)
                    {
                        pair.Value.First().Parameters.ElementAt(i).Name = pair.Key.Parameters.ElementAt(i).Name;
                    }
                }

            }

            return comparedClasses;
        }

        /// <summary>
        /// Calls CompareClassIL, renames all identical methods in the obfuscated assembly, and then returns the results of CompareClassIL.
        /// </summary>
        /// <param name="obfuscatedClass"></param>
        /// <param name="deobfuscatedClass"></param>
        /// <returns></returns>
        public static Dictionary<MethodDef, List<MethodDef>> RewriteComparedClassIL(TypeDef obfuscatedClass, TypeDef deobfuscatedClass)
        {
            var comparedClass = CompareClassIL(obfuscatedClass, deobfuscatedClass);

            foreach(var pair in comparedClass)
            {
                if(pair.Value.Count > 1)
                {
                    throw new Exception("support for multiple MethodResults has not yet been implemented.");
                }

                // key is the deob  scated method, value is the obfuscated method
                for(int i = 0; i < pair.Key.Parameters.Count; i++)
                {
                    pair.Value.First().Parameters.ElementAt(i).Name = pair.Key.Parameters.ElementAt(i).Name;
                }
            }

            return comparedClass;
        }


    }
}
