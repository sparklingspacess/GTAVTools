# GTAVTools
GTAVTools is an extension to SHVDN built in C# used to mainly improve my coding experience but anyone else can use it, credit helps! 

Updates are published within the source code and then into the compiled DLL.

# How to install
Step 1: Make sure you have the latest ScriptHookV (http://www.dev-c.com/gtav/scripthookv/) and ScriptHookV .NET (https://github.com/scripthookvdotnet/scripthookvdotnet-nightly), if not, install them.
Step 2: Make a folder called "scripts" in the same location as GTA5.exe (GTA5_Enhanced.exe if you are using enhanced) and drag-and-drop GTAVTools.dll and GTAVMemScanner.dll into that folder.
Thats it! Now you can use scripts using .NET scripts that require GTAVTools.

# How to use within your scripts
## Referencing the DLL
**Please make Copy Local on the DLL properties to false to prevent replacing the GTAVTools dll.**
![Referencing the DLL](reference.gif)

## Creating entities within the game.
### Vehicles
```csharp
//The vehicles spawn point.
Vector3 spawnpoint = GTAV.player.character.position + GTAV.player.character.forwardvector * 3;
//The model for the vehicle.
GTAVModel mdl = new GTAVModel(VehicleCache.tailgater);
mdl.LoadIntoMemory();
//The vehicle itself.
GTAVVehicle vehicle = new GTAVVehicle(mdl, spawnpoint, GTAV.player.character.heading);
//Opening the door for the drivers seat.
vehicle.OpenVehicleDoor(DoorId.DriverSideFront);
//Making the car fully metallic black.
vehicle.primarycolor = VehicleColorID.MetallicBlack;
vehicle.secondarycolor = VehicleColorID.MetallicBlack;
```
### Pedestrians
```csharp
//The pedestrians spawn point.
Vector3 spawnpoint = GTAV.player.character.position + GTAV.player.character.forwardvector * 3;
//The model for the pedestrian.
GTAVModel mdl = new GTAVModel(PedCache.Player_One);
mdl.LoadIntoMemory();
//The pedestrian itself.
GTAVPed ped = new GTAVPed(mdl, spawnpoint, GTAV.player.character.heading);
//Gives the ped a Micro SMG.
ped.GiveWeapon(WeaponCache.WEAPON_MICROSMG, 256, false, true);
//Gives the ped armour.
ped.armour = 200;
```
### Props
```csharp
//The props spawn point.
Vector3 spawnpoint = GTAV.player.character.position + GTAV.player.character.forwardvector * 3;
//The model for the prop.
GTAVModel mdl = new GTAVModel(PropCache.prop_chair_06);
mdl.LoadIntoMemory();
//The prop itself.
GTAVProp prop = new GTAVProp(mdl, spawnpoint, GTAV.player.character.heading, true);
//Sets its velocity
prop.velocity = new Vector3(0f, 0f, 3f);
```
### Pickups
```csharp
//The pickups spawn point.
Vector3 spawnpoint = GTAV.player.character.position + GTAV.player.character.forwardvector * 3;
//The pickup itself.
GTAVPickup pickup = new GTAVPickup(PickupCache.PICKUP_WEAPON_PISTOL, spawnpoint); //Pickup currently lacks changeable arguments, will have alot more arguments soon.
```

## Memory
**Please do not attempt to modify any type of memory that you are not familiar with, memory modifying is unsafe if you don't know what you're doing**
### Creating a memory patch
```csharp
IntPtr address = //You insert a memory address into here, to find patterns use GTAV.FindPattern\\
byte[] newbytes = new byte[] { 0x01 };
MemoryPatch patch = GTAV.CreateNewMemoryPatch(address, newbytes); //A memory patch can be used to overwrite bytes in game memory.
```
### Applying an already existing MemoryPatch
```csharp
patch.Patch()
```
### Reverting an applied MemoryPatch
```csharp
patch.Revert()
```
### Getting patterns
```csharp
IntPtr pattern = GTAV.FindPattern("pattern123", "patternmask123");  //"pattern123" is the pattern it scans for in memory, "pattermask123" specifies which bytes in the pattern must match exactly and which bytes are ignored.
```
### Getting the memory address of entities
```csharp
IntPtr memaddr = [ENTITY].memaddress
```

## Code examples
**More code examples will be coming soon*
### Basic player ragdoller
```csharp
using System;
using GTAVTools;
using GTA;

namespace GTAVToolsExample1
{
    public class GTAVToolsExample1 : Script
    {

        public GTAVPlayer me;

        public GTAVToolsExample1()
        {
            Tick += OnTick;
            this.me = GTAV.player;
        }

        public void OnTick(object sender, EventArgs e)
        {
            if (GTAV.GetCtrlUtilization(ControlPriority.Frontend, ControlUtilization.JustPressed, Control.MultiplayerInfo))
            {
                switch (me.character.isragdoll)
                {
                    case false:
                        //Makes the players character balance and move their arms around in a windmill motion.
                        me.character.SendNMMessage(NMMessage.BodyBalance, true);
                        me.character.SendNMMessage(NMMessage.ArmsWindmill);
                        break;
                    case true:
                        //Unragdolls the players character.
                        me.character.Unragdoll();
                        break;
                }
            }
        }
    }
}
```
### Better bullet impacts (one of my mods)
```csharp
using GTA;
using GTA.Math;
using GTA.Native;
using GTAVTools;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterBulletImpacts
{
    public class BetterBulletImpacts : Script
    {

        public GTAVPlayer me;
        public bool ptfx = false;
        public List<WeaponCache> powerfulweapons = new List<WeaponCache>();
        public List<WeaponCache> critweapons = new List<WeaponCache>();
        public List<int> fatalbones = new List<int>();

        public BetterBulletImpacts()
        {
            Tick += OnTick;
            this.me = GTAV.player;
            string scriptsfolder = AppContext.BaseDirectory;
            string configlocation = Path.Combine(scriptsfolder, "BBIConfig.ini");
            foreach (string weapon in GTAV.ReadCategoryOfINIFile(configlocation, "powerfulweapons"))
            {
                WeaponCache cached;
                bool parsed = Enum.TryParse<WeaponCache>(weapon, out cached);
                if (parsed)
                {
                    powerfulweapons.Add(cached);
                }
            }
            foreach (string weapon in GTAV.ReadCategoryOfINIFile(configlocation, "criticalweapons"))
            {
                WeaponCache cached;
                bool parsed = Enum.TryParse<WeaponCache>(weapon, out cached);
                if (parsed)
                {
                    critweapons.Add(cached);
                }
            }
            foreach (string bone in GTAV.ReadCategoryOfINIFile(configlocation, "fatalboneids"))
            {
                if (int.TryParse(bone, out int boneid))
                {
                    fatalbones.Add(boneid);
                }
            }
        }

        public void OnTick(object sender, EventArgs e)
        {
            if (!ptfx)
            {
                ptfx = true;
                GTAV.RequestPTFXAsset("scr_fbi1");
                GTAV.RequestPTFXAsset("core");
                GTAV.RequestPTFXAsset("cut_michael2");
            }
            List<GTAVPed> peds = GTAV.GetAllGTAVPeds();
            foreach (GTAVPed ped in peds)
            {
                if (me.IsTargettingEntity(ped.handle) && me.character.isshooting && !me.character.isinvehicle)
                {
                    bool isusingheavyweapon = powerfulweapons.Contains((WeaponCache)me.character.currentweapon);
                    bool isusingcritweapon = critweapons.Contains((WeaponCache)me.character.currentweapon);
                    int bone = ped.lastdamagebone;
                    bool didhitfatalbone = fatalbones.Contains(bone);
                    Vector3 bulletcoord = me.character.lastweaponhitcoord;
                    Vector3 pos = Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x2274BC1C4885E333), ped, bulletcoord.X, bulletcoord.Y, bulletcoord.Z);
                    BoneTransform bonetransform = ped.GetBoneTransformForBone(ped.GetBoneIndexFromBoneID(bone));
                    Vector3 rot = me.character.forwardvector;
                    Vector3 finalrot = rot - bonetransform.rot;
                    Vector3 corrected = new Vector3(finalrot.Z, finalrot.X, finalrot.Y);
                    if (!isusingheavyweapon)
                    {
                        GTAV.SetPTFXAssetForNextCall("scr_fbi1");
                        GTAV.StartPTFXOnPedBone("scr_fbi1", "scr_fbi_autopsy_blood", ped, pos, corrected, ped.GetBoneIndexFromBoneID(bone), 0.3f, false);
                        GTAV.SetPTFXAssetForNextCall("core");
                        GTAV.StartPTFXOnPedBone("core", "blood_stab", ped, ped.GetBoneIndexFromBoneID(bone), 1f, false);
                    }
                    else if (isusingheavyweapon)
                    {
                        if (!didhitfatalbone)
                        {
                            GTAV.SetPTFXAssetForNextCall("scr_fbi1");
                            GTAV.StartPTFXOnPedBone("scr_fbi1", "scr_fbi_autopsy_blood", ped, pos, corrected, ped.GetBoneIndexFromBoneID(bone), 0.37f, false);
                            GTAV.SetPTFXAssetForNextCall("core");
                            GTAV.StartPTFXOnPedBone("core", "blood_stab", ped, ped.GetBoneIndexFromBoneID(bone), 1f, false);
                        }
                        else if (didhitfatalbone)
                        {
                            GTAV.SetPTFXAssetForNextCall("scr_fbi1");
                            GTAV.StartPTFXOnPedBone("scr_fbi1", "scr_fbi_autopsy_blood", ped, pos, corrected, ped.GetBoneIndexFromBoneID(bone), 0.46f, false);
                            GTAV.SetPTFXAssetForNextCall("core");
                            GTAV.StartPTFXOnPedBone("core", "blood_stab", ped, ped.GetBoneIndexFromBoneID(bone), 1f, false);
                        }
                    }
                    if (isusingcritweapon)
                    {
                        if (!didhitfatalbone)
                        {
                            GTAV.SetPTFXAssetForNextCall("cut_michael2");
                            GTAV.StartPTFXOnPedBone("cut_michael2", "cs_mich2_blood_head_leak", ped, pos, corrected, ped.GetBoneIndexFromBoneID(bone), 0.45f, false);
                        }
                        else if (didhitfatalbone)
                        {
                            GTAV.SetPTFXAssetForNextCall("cut_michael2");
                            GTAV.StartPTFXOnPedBone("cut_michael2", "cs_mich2_blood_head_leak", ped, pos, corrected, ped.GetBoneIndexFromBoneID(bone), 0.7f, false);
                        }
                    }
                    Script.Wait(200);
                }
            }
        }
    }
}
```
### Mobile radio
```csharp
using System;
using GTAVTools;
using GTA;
using GTA.Native;

namespace MobileRadio
{
    public class MobileRadio : Script
    {

        public bool on = false;

        public MobileRadio()
        {
            Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Checking if the player is just pressed Context (normally E on keyboard)
            if (GTAV.GetCtrlUtilization(ControlPriority.Frontend, ControlUtilization.JustPressed, GTA.Control.Context) && GTAV.player.character.isusingmobilephone)
            {
                //Toggling the "on" value.
                on = !on;
                //The soundset used for the "toggle noise"
                string soundset = "Phone_Soundset_Default";
                GTAVPlayer plr = GTAV.player;
                //Making the players finger touch a button on their cellphone.
                Function.Call(GTAV.HexHashToNativeHashU(0x95C9E72F3D7DEC9B), 5);
                //Changing the soundset for the main 3 protagonists phones.
                if (plr.cellphone == Cellphone.iFruit)
                {
                    soundset = "Phone_Soundset_Michael";
                }
                else if (plr.cellphone == Cellphone.Badger)
                {
                    soundset = "Phone_Soundset_Franklin";
                }
                else if (plr.cellphone == Cellphone.Facade)
                {
                    soundset = "Phone_Soundset_Trevor";
                }
                //Playing the "toggle noise"
                GTAV.PlaySoundFrontend(soundset, "Menu_Accept");
                //Toggling the audio flag for the mobile radio.
                Function.Call(GTAV.HexHashToNativeHashU(0xB9EFD5C25018725A), "MobileRadioInGame", on);
                //Toggling the mobile radio actually being active.
                Function.Call(GTAV.HexHashToNativeHashU(0xBF286C554784F3DF), on);
            }
        }
    }
}
```
### GTA IV Style Pushing
```csharp
using System;
using System.Collections.Generic;
using GTA;
using GTAVTools;

namespace IVPushing
{
    public class IVPushing : Script
    {

        public List<uint> pushed = new List<uint>();

        public IVPushing()
        {
            Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Getting every ped.
            List<GTAVPed> peds = GTAV.GetAllGTAVPeds();
            foreach (GTAVPed ped in peds)
            {
                //Making sure all requirements are met:
                // -- This ped is not the player
                // -- The player is touching the ped
                // -- The ped is not ragdolled
                // -- The ped is human
                if (ped.handle != GTAV.player.character.handle && GTAV.player.character.IsTouchingEntity(ped.handle) && !ped.isragdoll && ped.ishuman)
                {
                    //Making the ped balance, swing their arms around in a windmill motion and makes them try to grab things.
                    ped.SendNMMessage(NMMessage.BodyBalance, true, 2000);
                    ped.SendNMMessage(NMMessage.ArmsWindmill);
                    ped.SendNMMessage(NMMessage.Grab);
                    //If the "pushed" list does not contain this ped already.
                    if (!pushed.Contains(ped.handle))
                    {
                        //Adds this ped to the "pushed" list.
                        pushed.Add(ped.handle);
                    }
                }
            }
        }
    }
}
```
