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
        /// Key = The class name of properties renamed, Value = List of properties that were renamed(with the old and new property seperated by commas)
        /// </summary>
        public Dictionary<string, List<string>> renamedProperties = new Dictionary<string, List<string>>();

        /// <summary>
        ///  Key = New Enum Name, Value = Old Enum Full Name
        /// </summary>
        public Dictionary<string, string> renamedEnums = new Dictionary<string, string>();

        /// <summary>
        ///  Key = New Class Name, Value = the new and old method name seperated by commas like: newName, oldName
        /// </summary>
        public Dictionary<string, string> renamedMethods = new Dictionary<string, string>();

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
            foreach(var type in assembly.Types) // loops through all classes
            {

                // Finds PhotonPlayer
                if(type.FullName == "VRC.Player" && type.Namespace.String == "VRC")
                {
                    Log("Found VRC.Player");
                    foreach(var prop in type.Properties)
                    {
                        if(prop.GetMethod.Name.Contains("PhotonPlayer"))
                        {
                            Log("Found PhotonPlayer");
                            renamedClasses.Add("PhotonPlayer", prop.GetMethod.ReturnType.GetFullName());
                            prop.GetMethod.ReturnType.TryGetTypeDef().Name = "PhotonPlayer";
                        }
                    }
                }

                // Finds RaiseEventOptions and renames the classes EventCaching/ReceiverGroup
                if (type.Fields.Count == 8 && type.Fields.ElementAt(0).IsStatic == true && type.Fields.ElementAt(1).IsPublic == true && type.Fields.ElementAt(2).FieldType.GetName() == "Byte" && type.Fields.ElementAt(5).FieldType.GetName() == "Byte" && type.Fields.ElementAt(7).FieldType.GetName() == "Boolean")
                {
                    Console.WriteLine("found RaiseEventOptions");
                    renamedClasses.Add("RaiseEventOptions", type.FullName);
                    renamedClasses.Add("EventCaching", type.Fields.ElementAt(1).FieldType.TryGetTypeDef().FullName);
                    renamedClasses.Add("ReceiverGroup", type.Fields.ElementAt(4).FieldType.TryGetTypeDef().FullName);

                    type.Fields.ElementAt(0).Name = "Default";

                    type.Fields.ElementAt(1).Name = "CachingOption";
                    type.Fields.ElementAt(1).FieldType.TryGetTypeDef().Name = "EventCaching";
                    type.Fields.ElementAt(2).Name = "InterestGroup";
                    type.Fields.ElementAt(3).Name = "TargetActorIDs";

                    type.Fields.ElementAt(4).Name = "Receivers";
                    type.Fields.ElementAt(4).FieldType.TryGetTypeDef().Name = "ReceiverGroup";

                    type.Fields.ElementAt(5).Name = "SequenceChannel";
                    type.Fields.ElementAt(6).Name = "ForwardToWebhook";


                }

                foreach(var method in type.Methods)
                {
                    // Finds PhotonNetwork
                    if (method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "ConnectionProtocol")
                    {
                        Log("FOUND PHOTON NETWORK: " + type.Name);
                        renamedClasses.Add("PhotonNetwork", type.FullName);
                        type.Name = "PhotonNetwork";
                    }
                }
            }

        }

        public void RenameAllEnums()
        {
            List<string> photonNetworkingMessageItems = new List<string>();
            photonNetworkingMessageItems.Add("OnConnectedToPhoton");
            photonNetworkingMessageItems.Add("OnLeftRoom");
            photonNetworkingMessageItems.Add("OnMasterClientSwitched");
            photonNetworkingMessageItems.Add("OnLeftLobby");
            photonNetworkingMessageItems.Add("OnJoinedLobby");
            photonNetworkingMessageItems.Add("OnFailedToConnectToPhoton");
            photonNetworkingMessageItems.Add("OnPhotonRandomJoinFailed");
            photonNetworkingMessageItems.Add("OnPhotonSerializeView");
            photonNetworkingMessageItems.Add("OnPhotonPlayerPropertiesChanged");
            FindEnumByNamesAndRename(photonNetworkingMessageItems, "PhotonNetworkingMessage");

            List<string> photonTargetItems = new List<string>();
            photonNetworkingMessageItems.Add("All");
            photonNetworkingMessageItems.Add("Others");
            photonNetworkingMessageItems.Add("MasterClient");
            photonNetworkingMessageItems.Add("AllBuffered");
            photonNetworkingMessageItems.Add("OthersBuffered");
            photonNetworkingMessageItems.Add("AllViaServer");
            photonNetworkingMessageItems.Add("AllBufferedViaServer");
            FindEnumByNamesAndRename(photonTargetItems, "PhotonTargets");

            List<string> clientStateItems = new List<string>();
            photonNetworkingMessageItems.Add("Leaving");
            FindEnumByNamesAndRename(clientStateItems, "ClientState");

            List<string> eventCachingItems = new List<string>();
            photonNetworkingMessageItems.Add("RemoveFromRoomCache");
            FindEnumByNamesAndRename(eventCachingItems, "EventCaching");

            List<string> statusCodeItems = new List<string>();
            photonNetworkingMessageItems.Add("DisconnectByServerUserLimit");
            FindEnumByNamesAndRename(statusCodeItems, "StatusCode");

            List<string> roomStuffItems = new List<string>();
            photonNetworkingMessageItems.Add("JoinOrCreateRoom");
            FindEnumByNamesAndRename(roomStuffItems, "RoomStuff");

            List<string> gvr_Keyboard_Stuff = new List<string>();
            photonNetworkingMessageItems.Add("GVR_KEYBOARD_ERROR_NO_LOCALES_FOUND");
            FindEnumByNamesAndRename(gvr_Keyboard_Stuff, "GVR_Keyboard_Stuff");

            List<string> regionsItems = new List<string>();
            photonNetworkingMessageItems.Add("asia");
            FindEnumByNamesAndRename(regionsItems, "regions");

            List<string> bonesItems = new List<string>();
            photonNetworkingMessageItems.Add("FootRight");
            FindEnumByNamesAndRename(bonesItems, "Bones");

            List<string> secondClientStateItems = new List<string>();
            photonNetworkingMessageItems.Add("ConnectedToFrontEnd");
            FindEnumByNamesAndRename(secondClientStateItems, "SecondClientState");

            List<string> trustOptionsItems = new List<string>();
            photonNetworkingMessageItems.Add("NegativeTrustLevel2");
            FindEnumByNamesAndRename(trustOptionsItems, "TrustOptions");

            List<string> serverOptionsItems = new List<string>();
            photonNetworkingMessageItems.Add("MasterServer");
            FindEnumByNamesAndRename(serverOptionsItems, "vrcErrorsItems");

            List<string> vrcErrorsItems = new List<string>();
            photonNetworkingMessageItems.Add("AssetBundleInvalidOrNull");
            FindEnumByNamesAndRename(vrcErrorsItems, "PhotonNetworkingMessage");



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
            string indent = new string(' ', 4);

            translations.WriteLine("Classes Renamed: ");
            foreach(var classPair in renamedClasses)
            {
                translations.WriteLine( indent + classPair.Value + " : " + classPair.Key);
            }

            foreach(var pair in renamedProperties)
            {
                if (pair.Value.Count() > 0)
                {
                    if(renamedClasses.Keys.Contains(pair.Key))
                    {
                        translations.WriteLine(renamedClasses.First(x => x.Key == pair.Key).Value + " : " + pair.Key);
                    }

                    else
                    {
                        translations.WriteLine(pair.Key);
                    }
                    
                    foreach (var prop in pair.Value)
                    {
                        var oldPropertyName = prop.Split(',').ElementAt(0);
                        var newPropertyName = prop.Split(',').ElementAt(1);
                        translations.WriteLine(indent + oldPropertyName + " : " + newPropertyName, 4);
                    }
                }
            }
        }

        public void FindEnumByNamesAndRename(List<string> enumItemNames, string newEnumName)
        {
            List<string> viableEnumNames = new List<string>();
            foreach(var type in assembly.Types)
            {
                List<string> enumNames = new List<string>();
                if(type.IsEnum)
                {
                    foreach(var field in type.Fields)
                    {
                        enumNames.Add(field.Name);
                    }
                    int totalSameEnumsCount = 0;

                    foreach(var e in enumItemNames)
                    {
                        if(enumNames.Contains(e))
                        {
                            totalSameEnumsCount++;
                        }
                    }

                    if(totalSameEnumsCount == enumItemNames.Count && totalSameEnumsCount > 0)
                    {
                        viableEnumNames.Add(type.FullName);
                    }
                   
                }
            }

            if(viableEnumNames.Count == 1)
            {
                renamedEnums.Add(newEnumName, viableEnumNames.First());
                assembly.Types.Where(x => x.FullName == viableEnumNames.First()).Single().Name = newEnumName;
            }

            else if(viableEnumNames.Count == 0)
            {
                Console.WriteLine("Found no viable enum for " + newEnumName);
            }

            else
            {
                string indent = new string(' ', 4);
                Log("Found more than one viable enum for " + newEnumName + ". Conflicting Enum Items are: ");
                foreach(var enumName in viableEnumNames)
                {
                    Log(indent + enumName);
                }

            }
        }

        public void renameClassMethods()
        {
            foreach(var type in assembly.Types)
            {
                foreach(var method in type.Methods)
                {
                    if (type.Name == "PhotonNetwork")
                    {
                        int totalRaiseEventMethods = 0;
                        if (method.IsStatic == true && method.Parameters.Count() == 4 && method.ReturnType.GetName() == "Boolean" && method.Parameters.First().Type.GetName() == "Byte")
                        {
                            totalRaiseEventMethods++;
                            Log("Found PhotonNetwork.RaiseEvent()");
                            renamedMethods.Add("PhotonNetwork", method.Name + "," + "RaiseEvent" + totalRaiseEventMethods.ToString());
                            method.Name = "RaiseEvent" + totalRaiseEventMethods.ToString();
                        }
                    }

                    if(type.Name == "PhotonPlayer")
                    {

                    }
                }

            }
        }

    }
}
