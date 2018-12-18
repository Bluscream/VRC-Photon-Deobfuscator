using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clean_VRC_Deobfuscator.ClassComparer;
using dnlib.DotNet.Emit;

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
        public Dictionary<string, List<string>> renamedMethods = new Dictionary<string, List<string>>();

        public Deobfuscator(string assemblyPath)
        {
            assembly = ModuleDefMD.Load(assemblyPath);
        }

        public void Log(string text)
        {
            Console.WriteLine(text);
        }

        public void RenameMethod(string newMethodName, MethodDef method)
        {
            Console.WriteLine("Found " + newMethodName + " - " + method.Name);
            method.Name = newMethodName;
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

                    type.Name = "RaiseEventOptions";

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
                            prop.SetMethod.Name = "set_" + newPropertyName;
                        }

                      
                    }

                    catch(Exception ex)
                    {
                        // this is thrown when the property has no getter
                    }
                }
                
                if(type.Name == "PhotonPlayer")
                {
                    var viewIDField = type.Fields.Where(x => x.FieldType.GetName() == "Int32");
                    var name = type.Fields.Where(x => x.FieldType.GetName() == "String");

                    if(viewIDField.Count() == 1)
                    {
                        Console.WriteLine("renamed ViewID");
                        viewIDField.First().Name = "viewID";
                    }

                    else
                    {
                        Console.WriteLine("Could not rename PhotonNetwork.ViewId - there was more than one viable field.");
                    }

                    if(name.Count() == 1)
                    {
                        Console.WriteLine("renamed name field");
                        name.First().Name = "name";
                    }

                    else
                    {
                        Console.WriteLine("could not rename PhotonNetwork.Name - there was more than one viable field.");
                    }
                }

                if (type.Name == "PhotonNetwork")
                {
                    var possibleNetworkingPeers = type.Fields.Where(x => x.Access == FieldAttributes.Assembly && x.IsStatic == true && x.IsInitOnly == false);

                    List<FieldDef> likelyNetworkingPeers = new List<FieldDef>();
                    foreach(var possibility in possibleNetworkingPeers)
                    {
                        if(possibility.FieldType.TryGetTypeDef() != null)
                        {
                            likelyNetworkingPeers.Add(possibility);
                        }
                    }

                    if(likelyNetworkingPeers.Count == 1)
                    {
                        // renames networking peer class
                        var networkingPeer = likelyNetworkingPeers.Single().FieldType.TryGetTypeDef();
                        Console.WriteLine("FOUND NetworkingPeer - " + networkingPeer.Name + ". Renamed to NetworkingPeer");
                        networkingPeer.Name = "NetworkingPeer";
                        renamedClasses.Add("NetworkingPeer", networkingPeer.FullName);

                        //renames networingPeer property in Photonnetwork
                        typeProps.Add(likelyNetworkingPeers.Single().FullName + "," + "networkingPeer");
                        likelyNetworkingPeers.Single().Name = "networkingPeer";

                        // renames NetworkingPeer's base class, LoadBalancingPeer
                        renamedClasses.Add("LoadBalancingPeer", networkingPeer.BaseType.FullName);
                        networkingPeer.BaseType.Name = "LoadBalancingPeer";
                    }

                    else
                    {
                        Console.WriteLine("Could NOT find Networking Peer!!!!!!. There were " + likelyNetworkingPeers.Count + " possible classes that could not be Networking Peer. Please Narrow Options");
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

        public void CompareClasses()
        {
            ModuleDefMD preObfuscation = ModuleDefMD.Load(@"preObfuscation.dll");

            var deobfuscatedPhotonNetwork = preObfuscation.Types.Single(x => x.Name == "PhotonNetwork");
            var obfuscatedPhotonNetwork = assembly.Types.First(x => x.Name == "PhotonNetwork");
            ClassComparer.Comparer.RewriteComparedClassMethods(obfuscatedPhotonNetwork, deobfuscatedPhotonNetwork);
            ClassComparer.Comparer.RewriteComparedClassIL(obfuscatedPhotonNetwork, deobfuscatedPhotonNetwork);

            var deobfuscatedNetworkingPeer = preObfuscation.Types.Single(x => x.Name == "NetworkingPeer");
            var obfuscatedNetworkingPeer = assembly.Types.Single(x => x.Name == "NetworkingPeer");
            ClassComparer.Comparer.RewriteComparedClassMethods(obfuscatedNetworkingPeer, deobfuscatedNetworkingPeer);
            var result = ClassComparer.Comparer.RewriteComparedClassIL(obfuscatedPhotonNetwork, deobfuscatedPhotonNetwork);

            string indent = "    ";

            foreach(var pair in result)
            {
                Console.WriteLine(pair.Key);
                foreach(var method in pair.Value)
                {
                    Console.WriteLine(indent + method.Name);
                    method.Name = pair.Key.Name;
                }
            }
            Console.WriteLine(result.Count); // for logging


            var deobfuscatedPhotonPlayer = preObfuscation.Types.Single(x => x.Name == "PhotonPlayer");
            var obfuscatedPhotonPlayer = assembly.Types.Single(x => x.Name == "PhotonPlayer");
            ClassComparer.Comparer.RewriteComparedClassMethods(obfuscatedPhotonPlayer, deobfuscatedPhotonPlayer);
            ClassComparer.Comparer.RewriteComparedClassIL(obfuscatedPhotonPlayer, deobfuscatedPhotonPlayer);

            var deobfuscatedPhotonView = preObfuscation.Types.Single(x => x.Name == "PhotonView");
            var obfuscatedPhotonView = assembly.Types.Single(x => x.Name == "PhotonView");
            ClassComparer.Comparer.RewriteComparedClassMethods(obfuscatedPhotonView, deobfuscatedPhotonView);
            ClassComparer.Comparer.RewriteComparedClassIL(obfuscatedPhotonView, deobfuscatedPhotonView);

            var deobfuscatedLoadBalancingPeer = preObfuscation.Types.Single(x => x.Name == "LoadBalancingPeer");
            var obfuscatedLoadBalancingPeer = assembly.Types.Single(x => x.Name == "LoadBalancingPeer");
            ClassComparer.Comparer.RewriteComparedClassMethods(obfuscatedLoadBalancingPeer, deobfuscatedLoadBalancingPeer);
            ClassComparer.Comparer.RewriteComparedClassIL(obfuscatedLoadBalancingPeer, deobfuscatedLoadBalancingPeer);
            // (not functional atm)     var classPair = ClassComparer.Comparer.FindMostLikelyDeobfuscatedClass(assembly.Types.Single( x => x.FullName == "VRC.Player"), preObfuscation.Types.ToList());
            //     Console.WriteLine(classPair.Key + "is key " + classPair.Value);


      //   (not function atm either)   ClassComparer.Comparer.CompareClassFields(obfuscatedPhotonNetwork, deobfuscatedPhotonNetwork);
        }

        public void RenameClassMethods() // NOTE: Methods that are not static usually have a hidden 'this' parameter
        {
            foreach(var type in assembly.Types)
            {
                // PhotonNetwork Counters
                int totalRaiseEventMethods = 0;
                int totalJoinOrCreateRoomMethods = 0;
                int totalJoinRandomRoomMethods = 0;
                int totalSwitchProtocolMethods = 0;
                int totalSetMasterClientMethods = 0;
                int totalSetPlayerCustomPropertiesMethods = 0;
                int totalDestroyMethods = 0;

                // PhotonView Counters
                int totalGetComponentPhotonViewMethods = 0;
                int totalGetGameObjectPhotonViewMethods = 0;
                int totalIntFindPhotonViewMethods = 0;
                int totalTransferOwnershipMethods = 0;

                // PhotonPlayer counters
                int totalIntFindPhotonPlayerMethods = 0;
                int totalGetNextForPhotonPlayerMethods = 0;
                int totalGetNextForIdMethods = 0;
                foreach (var method in type.Methods)
                {

                    if (type.Name == "PhotonNetwork")
                    {
                        List<string> tempRenamedMethods = new List<string>();

                        // Finds RaiseEvent
                        if (method.IsStatic == true && method.Parameters.Count() == 4 && method.ReturnType.GetName() == "Boolean" && method.Parameters.First().Type.GetName() == "Byte")
                        {
                            totalRaiseEventMethods++;
                            Log("Found PhotonNetwork.RaiseEvent()");
                            tempRenamedMethods.Add(method.Name + "," + "RaiseEvent" + totalRaiseEventMethods.ToString());
                            method.Name = "RaiseEvent" + totalRaiseEventMethods.ToString();
                        }

                        // find JoinOrCreateRoom and CreateRoom, and rename RoomOptions
                        if(method.Parameters.Count == 4 && method.IsStatic == true && method.Parameters.ElementAt(0).Type.GetName() == "String" && method.Parameters.ElementAt(3).Type.GetName() == "String[]")
                        {
                            totalJoinOrCreateRoomMethods++;

                            method.Parameters.ElementAt(0).Name = "roomName";

                            method.Parameters.ElementAt(1).Name = "roomOptions";
                            method.Parameters.ElementAt(1).Type.TryGetTypeDef().Name = "RoomOptions";
                            method.Parameters.ElementAt(2).Name = "typedLobby";
                            method.Parameters.ElementAt(3).Name = "expectedUsers";

                            Console.WriteLine("found PhotonNetwork.JoinOrCreateRoom OR PhotonNetwork.CreateRoom (no way to differentiate currently, so both were named JoinOrCreateRoom)");
                            tempRenamedMethods.Add(method.Name + "," + "JoinOrCreateRoom" + totalJoinOrCreateRoomMethods.ToString());
                            method.Name = "JoinOrCreateRoom" + totalJoinOrCreateRoomMethods.ToString();
                           
                        }

                        // find JoinRandomRoom and renames class RoomOptions and TypedLobby
                        if(method.Parameters.Count == 6 && method.Parameters.ElementAt(0).Type.GetName() == "Hashtable" && method.Parameters.ElementAt(1).Type.GetName() == "Byte" && method.Parameters.ElementAt(5).Type.GetName() == "String[]")
                        {
                            totalJoinRandomRoomMethods++;
                            Console.WriteLine("Found PhotonNetwork.JoinRandomRoom");
                            method.Parameters.ElementAt(0).Name = "expectedCustomRoomProperties";
                            method.Parameters.ElementAt(1).Name = "expectedMaxPlayers";

                            method.Parameters.ElementAt(2).Name = "matchingType";
                            method.Parameters.ElementAt(2).Type.TryGetTypeDef().Name = "MatchmakingMode";

                            method.Parameters.ElementAt(3).Name = "typedLobby";
                            method.Parameters.ElementAt(3).Type.TryGetTypeDef().Name = "TypedLobby"; ;

                            method.Parameters.ElementAt(4).Name = "sqlLobbyFilter";
                            method.Parameters.ElementAt(5).Name = "expectedUsers";
                            tempRenamedMethods.Add(method.Name + "," + "JoinRandomRoom" + totalJoinRandomRoomMethods.ToString());
                            method.Name = "JoinRandomRoom" + totalJoinRandomRoomMethods.ToString();
                        }

                        // find callEvent(which I currently don't think exists in VRC PhotonNetwork)
                        if(method.Parameters.Count == 3 && method.Parameters.ElementAt(0).Type.GetName() == "Byte"  && method.Parameters.ElementAt(2).Type.GetName() == "Int32")
                        {
                            Console.WriteLine((method.Parameters.ElementAt(1).Type.GetName()));
                            Console.WriteLine("Found PhotonNetwork.CallEvent");
                        }

                        // find SwitchToProtocol
                        if(method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "ConnectionProtocol")
                        {
                            Console.WriteLine("Found PhotonNetwork.SwitchToProtocol");
                            totalSwitchProtocolMethods++;
                            method.Parameters.ElementAt(0).Name = "cp";
                            tempRenamedMethods.Add(method.Name + "," + "SwitchToProtocol" + totalSwitchProtocolMethods.ToString());
                            method.Name = "SwitchToProtocol" + totalSwitchProtocolMethods.ToString();
                        }

                        if(method.ContainsString("Can not SetMasterClient(). Not in room or in offlineMode."))
                        {
                            Console.WriteLine("Found SetMasterClient - " + method.Name);
                            totalSetMasterClientMethods++;
                            tempRenamedMethods.Add(method.Name + "," + "SetMasterClient" + totalSetMasterClientMethods.ToString());
                            method.Name = "SetMasterClient";
                            method.Name = "SetMasterClient" + totalSetMasterClientMethods.ToString();
                        }

                        if(method.Parameters.Count == 1 && method.IsStatic && method.Parameters.First().Type.GetName() == "Hashtable" && method.ReturnType.GetName() == "Void")
                        {
                            Console.WriteLine("Found SetPlayerCustomProperties - " + method.Name);
                            totalSetPlayerCustomPropertiesMethods++;
                            tempRenamedMethods.Add(method.Name + "," + "SetPlayerCustomProperties" + totalSetPlayerCustomPropertiesMethods.ToString());
                            method.Name = "SetPlayerCustomProperties" + totalSetPlayerCustomPropertiesMethods.ToString();
                        }

                        if(method.ContainsString("Destroy(targetPhotonView) failed, cause targetPhotonView is null."))
                        {
                            Console.WriteLine("Found Destroy - " + method.Name);
                            totalDestroyMethods++;
                            tempRenamedMethods.Add(method.Name + "," + "Destroy" + totalDestroyMethods.ToString());
                            method.Name = "Destroy" + totalDestroyMethods.ToString();
                        }

                    }

                    if(type.Name == "PhotonView")
                    {
                        List<string> tempRenamedMethods = new List<string>();
                        // Locates DeserializeView method and renames the PhotonStream and PhotonMessageInfo classes
                        if (method.Name == "DeserializeView" && method.Parameters.Count == 3 && method.Parameters.First().IsHiddenThisParameter == true)
                        {
                            Console.WriteLine("Located DeserializeView method");

                            renamedClasses.Add("PhotonStream", method.Parameters.ElementAt(1).Type.GetFullName());
                            method.Parameters.ElementAt(1).Type.TryGetTypeDef().Name = "PhotonStream";
                            method.Parameters.ElementAt(1).Name = "stream";

                            renamedClasses.Add("PhotonMessageInfo", method.Parameters.ElementAt(2).Type.GetFullName());
                            method.Parameters.ElementAt(2).Type.TryGetTypeDef().Name = "PhotonMessageInfo";
                            method.Parameters.ElementAt(2).Name = "info";

                        }

                        // Find TransferOwnership methods
                   //     Console.WriteLine(method.Name + " : " + method.ReturnType.GetName() + " : " + method.Parameters.First().Type.GetName());


                        // Finds public static PhotonView Get(Component)
                        if (method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "Component" && method.ReturnType.GetName() == "PhotonView")
                        {
                            totalGetComponentPhotonViewMethods++;
                            Console.WriteLine("Found PhotonView.GetComponentPhotonView()");
                            tempRenamedMethods.Add(method.Name + "," + "GetComponentPhotonView" + totalGetComponentPhotonViewMethods.ToString());
                            method.Name = "GetComponentPhotonView" + totalGetComponentPhotonViewMethods.ToString();
                            // 

                        }

                        // Rename all Find(int viewID) methods
                        if (method.ReturnType.GetName() == "PhotonView" && method.Name != "Find" && method.IsStatic == true && method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "Int32")
                        {
                            totalIntFindPhotonViewMethods++;
                            Console.WriteLine("Found Possible Find Photon View: " + method.Name + " - renamed to Find with a random number at the end");
                            tempRenamedMethods.Add(method.Name + "," + "Find" + totalIntFindPhotonViewMethods.ToString());
                            method.Name = "Find" + totalIntFindPhotonViewMethods.ToString();
                        }
                        // Finds PhotonView(Get(GameObject) methods
                        if (method.ReturnType.GetName() == "PhotonView" && method.IsStatic == true && method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "GameObject")
                        {
                            totalGetGameObjectPhotonViewMethods++;
                            Console.WriteLine("Found Possible Get(GameObject) method" + method.Name + " - renamed to Find with a random number at the end");
                            tempRenamedMethods.Add(method.Name + "," + "GetGameObjectPhotonView" + totalGetGameObjectPhotonViewMethods.ToString());
                            method.Name = "GetGameObjectPhotonView" + totalGetGameObjectPhotonViewMethods.ToString();
                        }

                        if (method.IsStatic == false && method.Parameters.Count == 2 && method.Parameters.First().IsHiddenThisParameter == true && method.Parameters.ElementAt(1).Type.GetName() == "PhotonPlayer")
                        {
                            totalTransferOwnershipMethods++;
                            Console.WriteLine("Found TransferOwnership method(there is alot usually)");
                            tempRenamedMethods.Add(method.Name + "," + "TransferOwnership" + totalTransferOwnershipMethods.ToString());
                            method.Name = "TransferOwnership" + totalTransferOwnershipMethods.ToString();
                            
                        }

                    }

                    if(type.Name == "PhotonPlayer")
                    {
                        List<string> tempRenamedMethods = new List<string>();
                        if (method.Parameters.Count == 1 && method.Parameters.First().Type.GetName() == "Int32" && method.IsStatic == true && method.ReturnType.GetName() == "PhotonPlayer")
                        {
                            totalIntFindPhotonPlayerMethods++;
                            tempRenamedMethods.Add(method.Name + "," + "Find" + totalIntFindPhotonPlayerMethods);
                            Console.WriteLine("Found PhotonPlayer.Find() method(there is usually alot)" + " : " + method.Name + " : " + method.Parameters.Count);
                            method.Name = "Find" + totalIntFindPhotonPlayerMethods.ToString();
                            method.Parameters.First().Name = "id";
                        }

                        if (method.Parameters.Count == 1 && method.IsStatic == true && method.Parameters.First().Type.GetName() == "PhotonPlayer" && method.ReturnType.GetName() == "PhotonPlayer")
                        {
                            totalGetNextForPhotonPlayerMethods++;
                            Console.WriteLine("Found PhotonPlayer.GetNextFor(PhotonPlayer) method");
                            tempRenamedMethods.Add(method.Name + "," + "GetNextFor" + totalGetNextForPhotonPlayerMethods);
                            method.Name = "GetNextFor" + totalGetNextForPhotonPlayerMethods.ToString();
                        }

                        if (method.Parameters.Count == 1 && method.IsStatic == true && method.Parameters.First().Type.GetName() == "Int32" && method.ReturnType.GetName() == "PhotonPlayer")
                        {
                            totalGetNextForIdMethods++;
                            Console.WriteLine("Found PhotonPlayer.GetNextFor(int) method");
                            tempRenamedMethods.Add(method.Name + "," + "GetNextFor" + totalGetNextForIdMethods);
                            method.Name = "GetNextFor" + totalGetNextForIdMethods.ToString();
                        }
                    }

                    if(type.Name == "PunTurnManager")
                    {
                        if(method.Name == "OnEvent" && method.Parameters.ElementAt(0).IsHiddenThisParameter == true && method.Parameters.ElementAt(1).Type.GetName() == "Byte")
                        {
                            Console.WriteLine("Renamed PunTurnManager.OnEvent parameters");
                            method.Parameters.ElementAt(1).Name = "eventCode";
                            method.Parameters.ElementAt(2).Name = "content";
                            method.Parameters.ElementAt(3).Name = "senderId";

                        }
                    }
                }

            }
        }

    }
}
