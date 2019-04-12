# VRC-Photon-Deobfuscator

Current Classes being Deobfuscated:
1. PhotonPlayer
2. PhotonNetwork
3. PhotonView
4. A few other unnamed classes with relevant methods like OnEvent.
  
(enum functionality is temporarily disabled)
Current Enums being deobfuscated:
1.  PhotonNetworkingMessage
2. PhotonTargets
3.  ClientState
4.  EventCaching
5.  StatusCode
6.  RoomStuff(vrc exclusive - not sure if photon actually has this class.)
7.  GVR_Keyboard_Stuff(vrc exlcusive)
8.  regions
9.  Bones(vrc exclusive)
10.  SecondClientState(an enum with almost the exact same properties as client state. Not sure what to call it)
11.  TrustOptions
12.  ServerOptions
13.  vrcErrors
  
 Current VRC-Exclusive classes being deobfuscated
1. VRC.Player
2. vrcPlayer
3. Some of PunTurnManager
4.  About 2500 other properties throughout different classes.


There are hundreds of other classes that get deobfuscated due to renamed classes making code more clear.


After Deobfuscator:

![After Deobfuscator](https://cdn.discordapp.com/attachments/356125271767908354/510174996677787658/unknown.png "After Deobfuscator")



Before Deobfuscator:

![Before Deobfuscator](https://cdn.discordapp.com/attachments/501091178641621012/509914352816488448/unknown.png "Before Deobfuscator")


