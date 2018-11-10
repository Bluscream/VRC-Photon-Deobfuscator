using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_VRC_Deobfuscator
{
    class Deobfuscator
    {
        public static ModuleDefMD assembly;

        /// <summary>
        ///  Key = New Class Full Name, Value = Old Class Full Name
        /// </summary>
        public Dictionary<string, string> renamedClasses = new Dictionary<string, string>();

        /// <summary>
        /// Key = The new class full name of properties, Value = List of properties that were renamed(with the old and new property seperated by commas)
        /// </summary>
        public Dictionary<string, List<string>> renamedProperties = new Dictionary<string, List<string>>();

        public Deobfuscator(string assemblyPath)
        {
            assembly = ModuleDefMD.Load(assemblyPath);
        }

        public void Log(string text)
        {
            Console.WriteLine(text);
        }

        public void RenameAllClasses()
        {
            foreach(var type in assembly.Types)
            {
                if(type.FullName == "VRC.Player" && type.Namespace.String == "VRC")
                {
                    Log("Found VRC.Player");
                    foreach(var prop in type.Properties)
                    {
                        if(prop.GetMethod.Name.Contains("PhotonPlayer"))
                        {
                            renamedClasses.Add("PhotonPlayer", prop.GetMethod.ReturnType.GetFullName());
                            prop.GetMethod.ReturnType.TryGetTypeDef().Name = "PhotonPlayer";
                            Console.WriteLine("Renamed PhotonPlayer");
                        }
                    }
                }

                if(type.Name == "PhotonPlayer")
                {

                }
            }

        }
        
        public void RenameAllClassProperties()
        {
            foreach(var type in assembly.Types)
            {
                // List of properties being renamed. Formatted like: OldPropertyName,NewPropertyName
                List<string> typeProps = new List<string>();
                foreach(var prop in type.Properties)
                {
                    try
                    {
                        if (prop.GetMethod.Name.Contains("get_"))
                        {
                            string newPropertyName = prop.GetMethod.Name.Replace("get_", "");
                            typeProps.Add(prop.FullName + "," + newPropertyName);
                            prop.Name = newPropertyName;
                        }
                    }

                    catch(Exception ex)
                    {
                        // this is thrown when the property has no getter
                    }
                }

                renamedProperties.Add(type.FullName, typeProps);

            }
            Log(renamedProperties.Count.ToString() + "Properties Were Renamed.");
        }

        public void LogAllRenamedItems()
        {
            StreamWriter translations = new StreamWriter("translations.txt");

          

            // Logs All Renamed Properties
            var duplicateItems = renamedProperties.GroupBy(p => p.Key).SelectMany(grp => grp).Distinct();
            foreach(var pair in duplicateItems)
            {
                if (duplicateItems.Count() > 0 && pair.Value.Count > 0)
                {
                    translations.WriteLine(pair.Key);
                    
                    
                    if (renamedClasses.Keys.Contains(pair.Key))
                    {
                        Console.WriteLine(renamedClasses.First(x => x.Key == pair.Key).Value + " : " + renamedClasses.First(x => x.Key == pair.Key).Key);
                    }

                    else
                    {
                    //    Console.WriteLine(pair.Key);
                    }
                    foreach (var prop in pair.Value)
                    {
                        var oldPropertyName = prop.Split(',').ElementAt(0);
                        var newPropertyName = prop.Split(',').ElementAt(1);
                        string indent = new string(' ', 4);
                        translations.WriteLine(indent + oldPropertyName + " : " + newPropertyName, 4);
                    }
                }
            }


        }

    }
}
