using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Security;
using System.Text;
using GTA;
using GTA.Math;
using GTA.Native;
using GTAVMemScanner;
using Ctrl = GTA.Control;

namespace GTAVTools
{
    #region Internals
    /// <summary>
    /// A unique memory patcher.
    /// </summary>
    public class MemoryPatch
    {
        public IntPtr address;
        public int byteslength;
        public byte[] newbytes;
        byte[] oldbytes;

        public MemoryPatch(IntPtr address, int byteslength, byte[] newbytes)
        {
            this.address = address;
            this.byteslength = byteslength;
            this.newbytes = newbytes;
            this.oldbytes = new byte[this.byteslength];
        }

        public void Patch()
        {
            Marshal.Copy(this.address, this.oldbytes, 0, this.byteslength);
            Marshal.Copy(this.newbytes, 0, this.address, this.byteslength);
        }

        public void Revert()
        {
            Marshal.Copy(this.oldbytes, 0, this.address, this.byteslength);
        }
    }

    /// <summary>
    /// An internal identifier for an entities memory address.
    /// </summary>
    internal class InternalMemIdentifier
    {
        internal IntPtr address;
        internal GTAVEntity entity;

        internal InternalMemIdentifier(IntPtr address, GTAVEntity entity)
        {
            this.address = address;
            this.entity = entity;
        }
    }

    /// <summary>
    /// A bool inside an INI file.
    /// </summary>
    public class INIBool
    {
        public string name { get;  }
        public bool value { get;  }
        public INIBool(string name, bool value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public enum NMMessage
    {
        //I extracted these from jedijosh920s post on gtaforums about all the Euphoria messages, you can see it at https://gtaforums.com/topic/817563-v-euphorianaturalmotion-messages/
        StopAllBehaviours = 0,
        BodyBalance = 466,
        BodyFoetal = 507,
        BodyWrithe = 526,
        BodyRollUp = 515,
        ConfigureBalance = 42,
        HighFall = 715,
        CatchFall = 576,
        PedalLegs = 816,
        StaggerFall = 1151,
        FallToKnees = 1085,
        StayUpright = 292,
        RollDownStairs = 941,
        ShotNewBullet = 1060,
        ShotHeadLook = 1129,
        Shot = 983,
        ShotFallToKnees = 1083,
        ShotFromBehind = 1107,
        ShotRelax = 271,
        ArmsWindmill = 372,
        Electrocute = 613,
        BraceForImpact = 548,
        PointGun = 1140,
        Grab = 660,
        LeanRandom = 199,
        Teeter = 1221,
        PointArm = 838,
    }

    public enum ParkMode
    {
        None = 0,
        Forward = 1,
        Backwards = 2,
    }

    public enum Seat
    {
        Driver = -1,
        Passenger = 0,
        BackLeft = 1,
        BackRight = 2,
        OutsideLeft = 3,
        OutsideRight = 4,
    }

    public enum AnimFlags
    {
        Normal = 0,
        Repeat = 1,
        StopAtLastFrame = 2,
        OnlyUpperBody = 16,
        EnablePlayerControl = 32,
        Cancellable = 120,
    }

    public enum LookAtFlags
    {
        SlowTurnRate = 1,
        FastTurnRate = 2,
        ExtendYawLimit = 4,
        ExtendPitchLimit = 8,
        WidestYawLimit = 16,
        WidestPitchLimit = 32,
        NarrowYawLimit = 64,
        NarrowPitchLimit = 128,
        NarrowestYawLimit = 256,
        NarrowestPitchLimit = 512,
        UseTorso = 1024,
        WhileNotInFOV = 2048,
        UseCameraFocus = 4096,
        UseEyesOnly = 8192,
        UseLookDirection = 16384,
        FromScript = 32768,
        UseAbsoluteReferenceDirection = 65536
    }

    public enum LeaveVehicleFlags
    {
        NormalExit = 0,
        NormalExit2 = 1,
        TeleportOutside = 16,
        SlowNormalExit = 64,
        NormalExitNoCloseDoor = 256,
        ThrowSelfOut = 4160,
        MoveToPassengerThenExit = 262144
    }

    public enum VehicleEscortMode
    {
        Behind = -1,
        Ahead = 0,
        Left = 1,
        Right = 2,
        BackLeft = 3,
        BackRight = 4,
    }

    public enum Protagonist
    {
        None = -1,
        Michael = 0,
        Franklin = 1,
        Trevor = 2,
    }

    public enum Cellphone
    {
        iFruit = 0,
        Badger = 1,
        Facade = 2,
        iFruitGTAO = 3,
    }

    public enum ControlUtilization
    {
        Pressed,
        JustPressed,
        Released,
        JustReleased,
    }

    public enum ControlPriority
    {
        Gameplay = 0,
        Frontend = 2,
    }

    public enum MissileLockOnMode
    {
        NotLockedOn = 0,
        LockingOn = 1,
        LockedOn = 2,
    }

    public class BoneTransform
    {
        public Vector3 pos;
        public Vector3 rot;
        public BoneTransform(Vector3 pos, Vector3 rot)
        {
            this.pos = pos;
            this.rot = rot;
        }
    }

    public enum DoorLockState
    {
        None,
        Unlocked,
        Locked,
        OnlyLockOutPlayer,
        LockPlayerInside,
        InitiallyLocked,
        ForceShutDoors,
        LockedButCanBeDamaged,
        LockedButBootUnlocked,
        LockedNoPassengers,
        CannotEnter
    };

    public enum DoorId
    {
        Unknown = -1,
        DriverSideFront,
        DriverSideBack,
        PassengerSideFront,
        PassengerSideBack,
        Bonnet,
        Boot
    }

    public enum VehicleColorID
    {
        MetallicBlack = 0,
        MetallicGraphiteBlack = 1,
        MetallicBlackSteel = 2,
        MetallicDarkSilver = 3,
        MetallicSilver = 4,
        MetallicBlueSilver = 5,
        MetallicSteelGray = 6,
        MetallicShadowSilver = 7,
        MetallicStoneSilver = 8,
        MetallicMidnightSilver = 9,
        MetallicGunMetal = 10,
        MetallicAnthraciteGrey = 11,
        MatteBlack = 12,
        MatteGray = 13,
        MatteLightGrey = 14,
        UtilBlack = 15,
        UtilBlackPoly = 16,
        UtilDarkSilver = 17,
        UtilSilver = 18,
        UtilGunMetal = 19,
        UtilShadowSilver = 20,
        WornBlack = 21,
        WornGraphite = 22,
        WornSilverGrey = 23,
        WornSilver = 24,
        WornBlueSilver = 25,
        WornShadowSilver = 26,
        MetallicRed = 27,
        MetallicTorinoRed = 28,
        MetallicFormulaRed = 29,
        MetallicBlazeRed = 30,
        MetallicGracefulRed = 31,
        MetallicGarnetRed = 32,
        MetallicDesertRed = 33,
        MetallicCabernetRed = 34,
        MetallicCandyRed = 35,
        MetallicSunriseOrange = 36,
        MetallicClassicGold = 37,
        MetallicOrange = 38,
        MatteRed = 39,
        MatteDarkRed = 40,
        MatteOrange = 41,
        MatteYellow = 42,
        UtilRed = 43,
        UtilBrightRed = 44,
        UtilGarnetRed = 45,
        WornRed = 46,
        WornGoldenRed = 47,
        WornDarkRed = 48,
        MetallicDarkGreen = 49,
        MetallicRacingGreen = 50,
        MetallicSeaGreen = 51,
        MetallicOliveGreen = 52,
        MetallicGreen = 53,
        MetallicGasolineBlueGreen = 54,
        MatteLimeGreen = 55,
        UtilDarkGreen = 56,
        UtilGreen = 57,
        WornDarkGreen = 58,
        WornGreen = 59,
        WornSeaWash = 60,
        MetallicMidnightBlue = 61,
        MetallicDarkBlue = 62,
        MetallicSaxonyBlue = 63,
        MetallicBlue = 64,
        MetallicMarinerBlue = 65,
        MetallicHarborBlue = 66,
        MetallicDiamondBlue = 67,
        MetallicSurfBlue = 68,
        MetallicNauticalBlue = 69,
        MetallicBrightBlue = 70,
        MetallicPurpleBlue = 71,
        MetallicSpinnakerBlue = 72,
        MetallicUltraBlue = 73,
        MetallicBrightBlueLight = 74,
        UtilDarkBlue = 75,
        UtilMidnightBlue = 76,
        UtilBlue = 77,
        UtilSeaFoamBlue = 78,
        UtilLightningBlue = 79,
        UtilMauiBluePoly = 80,
        UtilBrightBlue = 81,
        MatteDarkBlue = 82,
        MatteBlue = 83,
        MatteMidnightBlue = 84,
        WornDarkBlue = 85,
        WornBlue = 86,
        WornLightBlue = 87,
        MetallicTaxiYellow = 88,
        MetallicRaceYellow = 89,
        MetallicBronze = 90,
        MetallicYellowBird = 91,
        MetallicLime = 92,
        MetallicChampagne = 93,
        MetallicPuebloBeige = 94,
        MetallicDarkIvory = 95,
        MetallicChocoBrown = 96,
        MetallicGoldenBrown = 97,
        MetallicLightBrown = 98,
        MetallicStrawBeige = 99,
        MetallicMossBrown = 100,
        MetallicBistonBrown = 101,
        MetallicBeechwood = 102,
        MetallicDarkBeechwood = 103,
        MetallicChocoOrange = 104,
        MetallicBeachSand = 105,
        MetallicSunBleechedSand = 106,
        MetallicCream = 107,
        UtilBrown = 108,
        UtilMediumBrown = 109,
        UtilLightBrown = 110,
        MetallicWhite = 111,
        MetallicFrostWhite = 112,
        WornHoneyBeige = 113,
        WornBrown = 114,
        WornDarkBrown = 115,
        WornStrawBeige = 116,
        BrushedSteel = 117,
        BrushedBlackSteel = 118,
        BrushedAluminium = 119,
        Chrome = 120,
        WornOffWhite = 121,
        UtilOffWhite = 122,
        WornOrange = 123,
        WornLightOrange = 124,
        MetallicSecuricorGreen = 125,
        WornTaxiYellow = 126,
        PoliceCarBlue = 127,
        MatteGreen = 128,
        MatteBrown = 129,
        WornOrangeBright = 130,
        MatteWhite = 131,
        WornWhite = 132,
        WornOliveArmyGreen = 133,
        PureWhite = 134,
        HotPink = 135,
        SalmonPink = 136,
        MetallicVermillionPink = 137,
        Orange = 138,
        Green = 139,
        Blue = 140,
        MetallicBlackBlue = 141,
        MetallicBlackPurple = 142,
        MetallicBlackRed = 143,
        HunterGreen = 144,
        MetallicPurple = 145,
        MetallicVeryDarkBlue = 146,
        ModshopBlack1 = 147,
        MattePurple = 148,
        MatteDarkPurple = 149,
        MetallicLavaRed = 150,
        MatteForestGreen = 151,
        MatteOliveDrab = 152,
        MatteDesertBrown = 153,
        MatteDesertTan = 154,
        MatteFoilageGreen = 155,
        DefaultAlloyColor = 156,
        EpsilonBlue = 157,
        PureGold = 158,
        BrushedGold = 159
    }

    public enum WindowIndex
    {
        FrontRight = 0,
        FrontLeft = 1,
        BackRight = 2,
        BackLeft = 3,
        Unknown = 4,
        Unknown2 = 5,
        Windscreen = 6,
        RearWindscreen = 7,
    }

    public enum VehicleWindow //because apparently rockstar made it so that WindowIndex and this are different? what the fuck.
    {
        FrontLeft = 0,
        FrontRight = 1,
        RearLeft = 2,
        RearRight = 3,
        FrontWindscreen = 4,
        RearWindscreen = 5,
        MidLeft = 6,
        MidRight = 7,
        Invalid = 8,
    }

    public class IntRGB
    {
        public int r;
        public int g;
        public int b;
        public IntRGB(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
    #endregion

    #region Players
    public class GTAVPlayer
    {
        /// <summary>
        /// The players ped.
        /// </summary>
        public GTAVPed character
        {
            //updated to use proper system to un-fuck this when the player's ped is swapped out, if this ever runs badly i'll optimize it but for now it's fine.
            get
            {
                uint pedhandle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x43A66C31C68491C0), this.handle);
                return new GTAVPed(pedhandle);
            }
        }

        /// <summary>
        /// The players handle.
        /// </summary>
        public uint handle
        {
            get => Function.Call<uint>(GTAV.HexHashToNativeHash(0x4F8644AF03D0E0D6));
        }

        /// <summary>
        /// The players name, this is the name you have on Rockstar Social Club, if you do not have a Social Club account (in which you are probably using a cracked or illegitimate version of the game) it will default to "Player".
        /// </summary>
        public string name
        {
            get
            {
                bool hasaccount = Function.Call<bool>(GTAV.HexHashToNativeHash(0x71EEE69745088DA0));
                string name = "Player";
                if (hasaccount)
                {
                    name = Function.Call<string>(GTAV.HexHashToNativeHash(0x6D0DE6A7B5DA71F8));
                }
                return name;
            }
        }

        /// <summary>
        /// The players wanted level, the game technically supports 6 stars but no police will show up for some reason.
        /// </summary>
        public int wantedlevel
        {
            set
            {
                if (value > 6)
                {
                    value = 6;
                }
                if (value < 0)
                {
                    value = 0;
                }
                Function.Call(GTAV.HexHashToNativeHash(0x39FF19C64EF7DA5B), this.handle, value);
                Function.Call(GTAV.HexHashToNativeHashU(0xE0A7D1E497FFCD6F), this.handle, false);
            }
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0xE28E54788CE8F12D), this.handle);
        }

        /// <summary>
        /// If the player is actively sounding the horn in any vehicle.
        /// </summary>
        public bool ispressinghorn
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xFA1E2BF8B10598F9), this.handle);
        }

        /// <summary>
        /// If the player is dead.
        /// </summary>
        public bool isdead
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x424D4687FA1E5652), this.handle);
        }

        /// <summary>
        /// If the player should be ignored by everyone (peds will still flee from gunshots and stuff like that).
        /// </summary>
        public bool ignoredbyeveryone
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x8EEDA153AD141BA4), this.handle, value);
            }
        }

        /// <summary>
        /// The players wanted level multiplier.
        /// </summary>
        public int wantedlevelmultiplier
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x020E5F00CDA207BA), this.handle, value);
            }
        }

        /// <summary>
        /// The players wanted level difficulty (max is 1.0).
        /// </summary>
        public float wantedleveldifficulty
        {
            set
            {
                if (value > 1.0f)
                {
                    value = 1.0f;
                }
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                Function.Call(GTAV.HexHashToNativeHashU(0x9B0BB33B04405E7A), this.handle, value);
            }
        }

        /// <summary>
        /// If the player is free-aiming.
        /// </summary>
        public bool isfreeaiming
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x2E397FD2ECD37C87), this.handle);
        }

        /// <summary>
        /// If the player can start missions or not.
        /// </summary>
        public bool canstartmissions
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xDE7465A27D403C06), this.handle);
        }

        /// <summary>
        /// If the player is ready to be used in cutscenes.
        /// </summary>
        public bool isreadyforcutscenes
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x908CBECC2CAA3690), this.handle);
        }

        /// <summary>
        /// If the player can do drive-bys.
        /// </summary>
        public bool candodrivebys
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x6E8834B52EC20C77), this.handle, value);
            }
        }

        /// <summary>
        /// The players current amount of money.
        /// </summary>
        public unsafe int money
        {
            set
            {
                uint hash = 0;
                if (this.character.modelcache == PedCache.Player_Zero)
                {
                    hash = GTAV.GenHashKey("SP0_TOTAL_CASH");
                }
                else if (this.character.modelcache == PedCache.Player_One)
                {
                    hash = GTAV.GenHashKey("SP1_TOTAL_CASH");
                }
                else if (this.character.modelcache == PedCache.Player_Two)
                {
                    hash = GTAV.GenHashKey("SP2_TOTAL_CASH");
                }
                Function.Call(GTAV.HexHashToNativeHashU(0xB3271D7AB655B441), hash, value, true);
            }
            get
            {
                uint hash = 0;
                int outvalue = 0;
                if (this.character.modelcache == PedCache.Player_Zero)
                {
                    hash = GTAV.GenHashKey("SP0_TOTAL_CASH");
                }
                else if (this.character.modelcache == PedCache.Player_One)
                {
                    hash = GTAV.GenHashKey("SP1_TOTAL_CASH");
                }
                else if (this.character.modelcache == PedCache.Player_Two)
                {
                    hash = GTAV.GenHashKey("SP2_TOTAL_CASH");
                }
                Function.Call(GTAV.HexHashToNativeHashU(0x767FBC2AC802EF3D), hash, &outvalue, -1);
                return outvalue;
            }
        }

        /// <summary>
        /// What protagonist the player is playing as.
        /// </summary>
        public Protagonist protagonist
        {
            get
            {
                if (this.character.modelcache == PedCache.Player_Zero)
                {
                    return Protagonist.Michael;
                }
                else if (this.character.modelcache == PedCache.Player_One)
                {
                    return Protagonist.Franklin;
                }
                else if (this.character.modelcache == PedCache.Player_Two)
                {
                    return Protagonist.Trevor;
                }
                return Protagonist.None;
            }
        }


        /// <summary>
        /// What type of cellphone the player has.
        /// </summary>
        public Cellphone cellphone
        {
            get
            {
                if (this.protagonist == Protagonist.Michael)
                {
                    return Cellphone.iFruit;
                }
                else if (this.protagonist == Protagonist.Franklin)
                {
                    return Cellphone.Badger;
                }
                else if (this.protagonist == Protagonist.Trevor)
                {
                    return Cellphone.Facade;
                }
                return Cellphone.iFruitGTAO;
            }
        }

        /// <summary>
        /// If the player can be hassled by gangs.
        /// </summary>
        public bool canbehassledbygangs
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xD5E460AD7020A246), this.handle, value);
            }
        }

        /// <summary>
        /// If the player is invincible.
        /// </summary>
        public bool isinvincible
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB721981B2B939E07), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x6BC97F4F4BB3C04B), this.handle, value);
            }
        }

        /// <summary>
        /// If the player has movement control whilst ragdolled.
        /// </summary>
        public bool hasragdollcontrol
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x3C49C870E66F0A28), this.handle, value);
            }
        }

        /// <summary>
        /// The players special ability multiplier.
        /// </summary>
        public bool specialabilitymultiplier
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xA49C426ED0CA4AB7), value);
            }
        }

        /// <summary>
        /// The players noise multiplier (noise is signalled by a blue circle around the player blip on the minimap).
        /// </summary>
        public float noisemultiplier
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xDB89EF50FF25FCE9), this.handle, value);
            }
        }

        /// <summary>
        /// A player helper class.
        /// </summary>
        public GTAVPlayer()
        {

        }

        //support for using GTAVPlayer as an InputArgument for native calls
        public static implicit operator InputArgument(GTAVPlayer plr)
        {
            return new InputArgument(plr.handle);
        }

        /// <summary>
        /// Changes the players model.
        /// </summary>
        public void ChangeModel(GTAVModel mdl)
        {
            mdl.LoadIntoMemory();
            Function.Call(GTAV.HexHashToNativeHash(0x00A1CADD00108836), handle, GTAV.GenHashKey(mdl.name));
        }

        /// <summary>
        /// If the player is targetting the entity (entity handle).
        /// </summary>
        public bool IsTargettingEntity(uint entityhandle)
        {
            bool targetting = Function.Call<bool>(GTAV.HexHashToNativeHash(0x3C06B5C839B38F7B), this.handle, entityhandle);
            if (targetting)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Teleports the player's ped to certain coordinates.
        /// </summary>
        public void Teleport(Vector3 coords)
        {
            uint tphandle = this.character.handle;
            if (this.character.isinvehicle)
            {
                tphandle = this.character.currentvehicle;
            }
            Function.Call(GTAV.HexHashToNativeHash(0x239A3351AC1DA385), tphandle, coords.X, coords.Y, coords.Z, false, false, false);
        }

        /// <summary>
        /// Teleports the player's ped to certain coordinates, if "autoplaceonground" is true it will auto place them on the ground.
        /// </summary>
        public void Teleport(Vector3 coords, bool autoplaceonground)
        {
            uint tphandle = this.character.handle;
            if (this.character.isinvehicle)
            {
                tphandle = this.character.currentvehicle;
            }
            Vector3 ogpos = GTAV.GetEntityCoordsFromHandle(tphandle);
            if (autoplaceonground)
            {
                Vector3 finalcoords = GTAV.GetGroundPosForTPFunction(tphandle, new Vector2(coords.X, coords.Y), ogpos);
                GTAV.SetEntityCoordsFromHandle(tphandle, finalcoords);
            }
            else if (!autoplaceonground)
            {
                GTAV.SetEntityCoordsFromHandle(tphandle, coords);
            }
        }

        /// <summary>
        /// Teleports the player's ped to certain coordinates, if "autoplaceonground" is true it will auto place them on the ground, if "screenfade" is true it will make the screen go black until it has found the correct tp position.
        /// </summary>
        public void Teleport(Vector3 coords, bool autoplaceonground, bool screenfade)
        {
            uint tphandle = this.character.handle;
            if (this.character.isinvehicle)
            {
                tphandle = this.character.currentvehicle;
            }
            Vector3 ogpos = GTAV.GetEntityCoordsFromHandle(tphandle);
            if (autoplaceonground)
            {
                if (screenfade)
                {
                    GTAV.FadeOut(100);
                }
                Vector3 finalcoords = GTAV.GetGroundPosForTPFunction(tphandle, new Vector2(coords.X, coords.Y), ogpos);
                GTAV.SetEntityCoordsFromHandle(tphandle, finalcoords);
                if (screenfade)
                {
                    GTAV.FadeIn(100);
                }
            }
            else if (!autoplaceonground)
            {
                GTAV.SetEntityCoordsFromHandle(tphandle, coords);
            }
        }
    }
    #endregion

    #region Pickups
    public class GTAVPickup
    {
        /// <summary>
        /// The handle of this pickup.
        /// </summary>
        public uint handle { get; }

        /// <summary>
        /// A pickup.
        /// </summary>
        public GTAVPickup(PickupCache pickup, Vector3 pos)
        {
            handle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0xFBA08C503DD5FA58), GTAV.GenHashKey(pickup.ToString()), pos.X, pos.Y, pos.Z, 1, 1, false, 0);
        }
    }
    #endregion

    #region Props
    public class GTAVProp
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong GSCED(uint handle);

        /// <summary>
        /// If this props memory address has been found.
        /// </summary>
        internal bool hasmemaddr = false;

        /// <summary>
        /// The handle of this prop.
        /// </summary>
        public uint handle { get; }

        private static IntPtr scea;
        private static GSCED gse;

        /// <summary>
        /// This props memory address.
        /// </summary>
        public unsafe IntPtr memaddress
        {
            get
            {
                if (!hasmemaddr)
                {
                    GetMemoryAddress();
                }
                if (gse == null)
                {
                    return IntPtr.Zero;
                }
                ulong ptr = gse(this.handle);
                return new IntPtr((long)ptr);
            }
        }

        /// <summary>
        /// This props current position.
        /// </summary>
        public Vector3 position
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x3FEF770D40960D5A), this.handle, true);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x06843DA7060A026B), this.handle, value.X, value.Y, value.Z, 0, 0, 0, false);
            }
        }

        /// <summary>
        /// This props current velocity.
        /// </summary>
        public Vector3 velocity
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x4805D2B1D8CF94A9), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x1C99BB7B6E96D16F), this.handle, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// This props current forward vector.
        /// </summary>
        public Vector3 forwardvector
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x0A794A5A57F8DF91), this.handle);
        }

        /// <summary>
        /// This props current rotation.
        /// </summary>
        public Vector3 rotation
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHashU(0xAFBD61CC738D9EB9), this.handle, 2);
        }

        /// <summary>
        /// The heading value of this prop.
        /// </summary>
        public float heading
        {
            get => Function.Call<float>(GTAV.HexHashToNativeHashU(0xE83D4F9BA2A38914), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x8E2530AA8ADA980E), this.handle, value);
            }
        }

        /// <summary>
        /// If this prop is attached to anything.
        /// </summary>
        public bool attachedtoanything
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB346476EF1A64897), this.handle);
        }

        /// <summary>
        /// This props model.
        /// </summary>
        public GTAVModel model
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                GTAVModel mdl = new GTAVModel(((PropCache)hash).ToString());
                return mdl;
            }
        }

        /// <summary>
        /// This props model cache.
        /// </summary>
        public PropCache modelcache
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                return (PropCache)hash;
            }
        }

        /// <summary>
        /// If this prop is a mission entity.
        /// </summary>
        public bool ismissionentity
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x0A7B270912999B3C), this.handle);
            set
            {
                if (!value)
                {
                    this.SetAsNoLongerNeeded();
                }
                else
                {
                    Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), this.handle, true, true);
                }
            }
        }

        /// <summary>
        /// A prop.
        /// </summary>
        public GTAVProp(GTAVModel model, Vector3 pos, float heading, bool dynamic)
        {
            model.LoadIntoMemory();
            handle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x509D5878EB39E842), model, pos.X, pos.Y, pos.Z, false, false, dynamic);
            this.heading = heading;
        }

        /// <summary>
        /// A prop. (Only use this to create a GTAVProp for an already existing prop)
        /// </summary>
        public GTAVProp(uint handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Does all the stuff to get this vehicles memory address
        /// </summary>
        internal unsafe void GetMemoryAddress()
        {
            bool existsindb = false;
            foreach (InternalMemIdentifier imi in GTAV.memidentifiers)
            {
                if (imi.entity.handle == handle)
                {
                    existsindb = true;
                }
            }
            if (!existsindb)
            {
                IntPtr address = GTAV.FindPattern("\x85\xED\x74\x0F\x8B\xCD\xE8\x00\x00\x00\x00\x48\x8B\xF8\x48\x85\xC0\x74\x2E", "xxxxxxx????xxxxxxxx");
                if (address == IntPtr.Zero)
                {
                    throw new AccessViolationException("Memory pattern not found or the memory is protected.");
                }
                int rel = Marshal.ReadInt32(address + 7);
                IntPtr fnptr = address + 11 + rel;
                gse = Marshal.GetDelegateForFunctionPointer<GSCED>(fnptr);
                this.hasmemaddr = true;
                GTAV.memidentifiers.Add(new InternalMemIdentifier(this.memaddress, this));
            }
        }

        //support for using GTAVProp as a GTAVEntity
        public static implicit operator GTAVEntity(GTAVProp ped)
        {
            return new GTAVEntity(ped);
        }

        //support for using GTAVProp as an InputArgument for native calls
        public static implicit operator InputArgument(GTAVProp ped)
        {
            return new InputArgument(ped.handle);
        }

        //support for using GTAVProp as a uint
        public static implicit operator uint(GTAVProp prop)
        {
            return prop.handle;
        }

        /// <summary>
        /// Deletes this prop.
        /// </summary>
        public unsafe void Delete()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), handle, false, true);
            Function.Call(GTAV.HexHashToNativeHashU(0x539E0AE3E6634B9F), &handle);
        }

        /// <summary>
        /// Makes this prop no longer needed. (the game will delete it when it seems fit and/or when the engine is running out of ram)
        /// </summary>
        public unsafe void SetAsNoLongerNeeded()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHash(0x3AE22DEB5BA5A3E6), &handle);
        }

        /// <summary>
        /// Attaches this prop to another entities bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity, bone, 0f, 0f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Attaches this prop to another entities bone with position being a local position offset to the bone and rotation being a local rotation offset (or world? not sure yet) to the bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone, Vector3 position, Vector3 rotation)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity.handle, bone, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Places this prop on the ground properly.
        /// </summary>
        public void PlaceOnGroundProperly()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x58A850EAEE20FAA3), this.handle);
        }
    }
#endregion

    #region Vehicles
    public class GTAVVehicle
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong GSCED(uint handle);

        /// <summary>
        /// If this vehicles memory address has been found.
        /// </summary>
        internal bool hasmemaddr = false;

        /// <summary>
        /// The handle of this vehicle.
        /// </summary>
        public uint handle { get; }

        private static IntPtr scea;
        private static GSCED gse;

        /// <summary>
        /// This vehicles memory address
        /// </summary>
        public unsafe IntPtr memaddress
        {
            get
            {
                if (!hasmemaddr)
                {
                    GetMemoryAddress();
                }
                if (gse == null)
                {
                    return IntPtr.Zero;
                }
                ulong ptr = gse(this.handle);
                return new IntPtr((long)ptr);
            }
        }

        /// <summary>
        /// This vehicles current position.
        /// </summary>
        public Vector3 position
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x3FEF770D40960D5A), this.handle, true);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x06843DA7060A026B), this.handle, value.X, value.Y, value.Z, 0, 0, 0, false);
            }
        }

        /// <summary>
        /// This vehicles current velocity.
        /// </summary>
        public Vector3 velocity
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x4805D2B1D8CF94A9), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x1C99BB7B6E96D16F), this.handle, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// This vehicles current forward vector.
        /// </summary>
        public Vector3 forwardvector
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x0A794A5A57F8DF91), this.handle);
        }

        /// <summary>
        /// This vehicles current rotation.
        /// </summary>
        public Vector3 rotation
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHashU(0xAFBD61CC738D9EB9), this.handle, 2);
        }

        /// <summary>
        /// The heading value of this vehicle.
        /// </summary>
        public float heading
        {
            get => Function.Call<float>(GTAV.HexHashToNativeHashU(0xE83D4F9BA2A38914), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x8E2530AA8ADA980E), this.handle, value);
            }
        }

        /// <summary>
        /// This vehicles model.
        /// </summary>
        public GTAVModel model
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                GTAVModel mdl = new GTAVModel(((VehicleCache)hash).ToString());
                return mdl;
            }
        }

        /// <summary>
        /// This vehicles model cache.
        /// </summary>
        public VehicleCache modelcache
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                return (VehicleCache)hash;
            }
        }

        /// <summary>
        /// If this vehicle is a mission entity.
        /// </summary>
        public bool ismissionentity
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x0A7B270912999B3C), this.handle);
            set
            {
                if (!value)
                {
                    this.SetAsNoLongerNeeded();
                }
                else
                {
                    Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), this.handle, true, true);
                }
            }
        }

        /// <summary>
        /// If this vehicle is attached to anything.
        /// </summary>
        public bool attachedtoanything
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB346476EF1A64897), this.handle);
        }

        /// <summary>
        /// Gets this current vehicles missile lock-on state.
        /// </summary>
        public MissileLockOnMode missilelockonmode
        {
            get
            {
                int state = Function.Call<int>(GTAV.HexHashToNativeHashU(0xE6B0E8CFC3633BF0), this.handle);
                switch (state)
                {
                    case 0:
                        return MissileLockOnMode.NotLockedOn;
                    case 1:
                        return MissileLockOnMode.LockingOn;
                    case 2:
                        return MissileLockOnMode.LockedOn;
                }
                return MissileLockOnMode.NotLockedOn;
            }
        }

        /// <summary>
        /// If this vehicle is stuck upside down (aka on its roof).
        /// </summary>
        public bool isstuckonroof
        {
            get
            {
                return Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB497F06B288DCFDF), this.handle);
            }
        }

        /// <summary>
        /// If this vehicle is idle (not moving).
        /// </summary>
        public bool isidle
        {
            get
            {
                return Function.Call<bool>(GTAV.HexHashToNativeHash(0x5721B434AD84D57A), this.handle);
            }
        }

        /// <summary>
        /// This vehicles max amount of passengers
        /// </summary>
        public int maxpassengers
        {
            get
            {
                return Function.Call<int>(GTAV.HexHashToNativeHashU(0xA7C4F2C6E744A550), this.handle);
            }
        }

        /// <summary>
        /// If this vehicle can be rappeled from by peds.
        /// </summary>
        public bool canberappeledfrom
        {
            get
            {
                return Function.Call<bool>(GTAV.HexHashToNativeHash(0x4E417C547182C84D), this.handle);
            }
        }

        /// <summary>
        /// If this vehicles sirens are muted.
        /// </summary>
        public bool sirensmuted
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xD8050E0EB60CF274), this.handle, value);
            }
        }

        /// <summary>
        /// The lock state of every door on this vehicle.
        /// </summary>
        public DoorLockState doorlockstate
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xB664292EAECF7FA6), this.handle, (int)value);
            }
        }

        /// <summary>
        /// This vehicles forward speed (in meters-per-second, not miles or kilometres).
        /// </summary>
        public float forwardspeed
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x83F969AA1EE2A664), this.handle, value);
            }
        }

        /// <summary>
        /// If this vehicle causes swerving.
        /// </summary>
        public bool causesswerving
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x9849DE24FCF23CCC), this.handle, value);
            }
        }

        /// <summary>
        /// This vehicles primary color.
        /// </summary>
        public VehicleColorID primarycolor
        {
            get
            {
                OutputArgument primary = new OutputArgument();
                OutputArgument secondary = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0xA19435F193E081AC), this.handle, primary, secondary);
                return (VehicleColorID)primary.GetResult<int>();
            }
            set
            {
                OutputArgument primary = new OutputArgument();
                OutputArgument secondary = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0xA19435F193E081AC), this.handle, primary, secondary);
                Function.Call(GTAV.HexHashToNativeHash(0x4F1D4BE3A7F24601), this.handle, (int)value, secondary.GetResult<int>());
            }
        }

        /// <summary>
        /// This vehicles secondary color.
        /// </summary>
        public VehicleColorID secondarycolor
        {
            get
            {
                OutputArgument primary = new OutputArgument();
                OutputArgument secondary = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0xA19435F193E081AC), this.handle, primary, secondary);
                return (VehicleColorID)secondary.GetResult<int>();
            }
            set
            {
                OutputArgument primary = new OutputArgument();
                OutputArgument secondary = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0xA19435F193E081AC), this.handle, primary, secondary);
                Function.Call(GTAV.HexHashToNativeHash(0x4F1D4BE3A7F24601), this.handle, primary.GetResult<int>(), (int)value);
            }
        }

        /// <summary>
        /// This vehicles primary color in RGB.
        /// </summary>
        public IntRGB primaryrgb
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x7141766F91D15BEA), this.handle, value.r, value.g, value.b);
            }
            get
            {
                OutputArgument r = new OutputArgument();
                OutputArgument g = new OutputArgument();
                OutputArgument b = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0xB64CF2CCA9D95F52), this.handle, r, g, b);
                return new IntRGB(r.GetResult<int>(), g.GetResult<int>(), b.GetResult<int>());
            }
        }

        /// <summary>
        /// This vehicles secondary color in RGB.
        /// </summary>
        public IntRGB secondaryrgb
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x36CED73BFED89754), this.handle, value.r, value.g, value.b);
            }
            get
            {
                OutputArgument r = new OutputArgument();
                OutputArgument g = new OutputArgument();
                OutputArgument b = new OutputArgument();
                Function.Call(GTAV.HexHashToNativeHashU(0x8389CD56CA8072DC), this.handle, r, g, b);
                return new IntRGB(r.GetResult<int>(), g.GetResult<int>(), b.GetResult<int>());
            }
        }


        /// <summary>
        /// If this vehicles alarm is active.
        /// </summary>
        public bool isalarmactive
        {
            get
            {
                return Function.Call<bool>(GTAV.HexHashToNativeHash(0x4319E335B71FFF34), this.handle);
            }
        }

        /// <summary>
        /// If this vehicle can be visibly damaged / deformed.
        /// </summary>
        public bool canbevisiblydamaged
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x4C7028F78FFD3681), this.handle, value);
            }
        }

        /// <summary>
        /// If this vehicles wheels can break (if they can fall off and get stuck).
        /// </summary>
        public bool canwheelsbreak
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x29B18B4FD460CA8F), this.handle, value);
            }
        }

        /// <summary>
        /// If this vehicles is invincible.
        /// </summary>
        public bool isinvincible
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x3882114BDE571AD4), this.handle, true, false);
            }
        }

        /// <summary>
        /// The amount of dirt this vehicle has, max value is 15.0.
        /// </summary>
        public float dirtlevel
        {
            get
            {
                return Function.Call<float>(GTAV.HexHashToNativeHashU(0x8F17BC8BA08DA62B), this.handle);
            }
            set
            {
                if (value > 15.0f)
                {
                    value = 15.0f;
                }
                if (value < 0f)
                {
                    value = 0f;
                }
                Function.Call(GTAV.HexHashToNativeHash(0x79D3B596FE44EE8B), this.handle, value);
            }
        }

        /// <summary>
        /// A vehicle.
        /// </summary>
        public GTAVVehicle(GTAVModel model, Vector3 pos, float heading)
        {
            model.LoadIntoMemory();
            handle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0xAF35D0D2583051B0), model, pos.X, pos.Y, pos.Z, heading, false, false, false);
        }

        /// <summary>
        /// A vehicle. (Only use this to create a GTAVVehicle for an already existing vehicle)
        /// </summary>
        public GTAVVehicle(uint handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Does all the stuff to get this vehicles memory address
        /// </summary>
        internal unsafe void GetMemoryAddress()
        {
            bool existsindb = false;
            foreach (InternalMemIdentifier imi in GTAV.memidentifiers)
            {
                if (imi.entity.handle == handle)
                {
                    existsindb = true;
                }
            }
            if (!existsindb)
            {
                IntPtr address = GTAV.FindPattern("\x85\xED\x74\x0F\x8B\xCD\xE8\x00\x00\x00\x00\x48\x8B\xF8\x48\x85\xC0\x74\x2E", "xxxxxxx????xxxxxxxx");
                if (address == IntPtr.Zero)
                {
                    throw new AccessViolationException("Memory pattern not found or the memory is protected.");
                }
                int rel = Marshal.ReadInt32(address + 7);
                IntPtr fnptr = address + 11 + rel;
                gse = Marshal.GetDelegateForFunctionPointer<GSCED>(fnptr);
                this.hasmemaddr = true;
                GTAV.memidentifiers.Add(new InternalMemIdentifier(this.memaddress, this));
            }
        }

        //support for using GTAVVehicle as an InputArgument for native calls
        public static implicit operator InputArgument(GTAVVehicle veh)
        {
            return new InputArgument(veh.handle);
        }

        //support for using GTAVVehicle as a uint
        public static implicit operator uint(GTAVVehicle veh)
        {
            return veh.handle;
        }

        //support for using GTAVVehicle as a GTAVEntity
        public static implicit operator GTAVEntity(GTAVVehicle veh)
        {
            return new GTAVEntity(veh);
        }

        /// <summary>
        /// Converts this vehicle into a SHVDN vehicle.
        /// </summary>
        public Vehicle ConvertToSHVDNVehicle()
        {
            return new Vehicle((int)this.handle);
        }

        /// <summary>
        /// Deletes this vehicle.
        /// </summary>
        public unsafe void Delete()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), handle, false, true);
            Function.Call(GTAV.HexHashToNativeHashU(0xEA386986E786A54F), &handle);
        }

        /// <summary>
        /// Makes this vehicle no longer needed. (the game will delete it when it seems fit and/or when the engine is running out of ram)
        /// </summary>
        public unsafe void SetAsNoLongerNeeded()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHash(0x629BFA74418D6239), &handle);
        }

        /// <summary>
        /// Repairs this vehicle.
        /// </summary>
        public void Repair()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x45F6D8EEF34ABEF1), this.handle, 1000);
            Function.Call(GTAV.HexHashToNativeHash(0x115722B1B9C14C1C), this.handle);
        }

        /// <summary>
        /// Attaches this vehicle to another entities bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity, bone, 0f, 0f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Attaches this vehicle to another entities bone with position being a local position offset to the bone and rotation being a local rotation offset (or world? not sure yet) to the bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone, Vector3 position, Vector3 rotation)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity.handle, bone, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Places this vehicle on the ground properly, if it fails to do so it will return false, if it succeeds it will return true
        /// </summary>
        public bool PlaceOnGroundProperly()
        {
            bool fuck = Function.Call<bool>(GTAV.HexHashToNativeHash(0x49733E92263139D1), this.handle, 5.0f);
            return fuck;
        }


        /// <summary>
        /// Gets this vehicles number of passengers occupying it.
        /// </summary>
        public int GetNumberOfPassengers()
        {
            return Function.Call<int>(GTAV.HexHashToNativeHash(0x24CB2137731FFE89), this.handle, false, true);
        }

        /// <summary>
        /// Gets this vehicles number of passengers occupying it (includes the driver if includedriver is true and includes dead passengers if includedead is true)
        /// </summary>
        public int GetNumberOfPassengers(bool includedriver, bool includedead)
        {
            return Function.Call<int>(GTAV.HexHashToNativeHash(0x24CB2137731FFE89), this.handle, includedriver, includedead);
        }

        /// <summary>
        /// Sets an individual door on this vehicles lock state.
        /// </summary>
        public void SetIndividualDoorLocked(DoorId id, DoorLockState lockstate)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBE70724027F85BCD), this.handle, (int)id, (int)lockstate);
        }

        /// <summary>
        /// Explodes this vehicle.
        /// </summary>
        public void Explode()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBA71116ADF5B514C), this.handle, true, false);
        }

        /// <summary>
        /// Explodes this vehicle, if audible is set to true the explosion will make sound, if invisible is true the explosion will not be visible by any player.
        /// </summary>
        public void Explode(bool audible, bool invisible)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBA71116ADF5B514C), this.handle, audible, invisible);
        }

        /// <summary>
        /// If the specified seat in this vehicle is free.
        /// </summary>
        public bool IsSeatFree(Seat seat)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHashU(0x8389CD56CA8072DC), this.handle, (int)seat, false);
        }

        /// <summary>
        /// Gets the ped in the specified vehicle seat, if no ped is in the specified seat it will return null.
        /// </summary>
        public GTAVPed GetPedInSeat(Seat seat)
        {
            uint pedhandle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0xBB40DD2270B65366), this.handle, (int)seat, false);
            if (pedhandle != 0)
            {
                return new GTAVPed(pedhandle);
            }
            return null;
        }

        /// <summary>
        /// Gets the last ped in the specified vehicle seat, if no ped was previously in the specified seat it will return null.
        /// </summary>
        public GTAVPed GetLastPedInSeat(Seat seat)
        {
            uint pedhandle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x83F969AA1EE2A664), this.handle, (int)seat);
            if (pedhandle != 0)
            {
                return new GTAVPed(pedhandle);
            }
            return null;
        }

        /// <summary>
        /// Opens the specified door on this vehicle.
        /// </summary>
        public void OpenVehicleDoor(DoorId doorid)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x7C65DAC73C35C862), this.handle, (int)doorid, false, false);
        }

        /// <summary>
        /// Opens the specified door on this vehicle, if loose is true the door becomes loose, if openinstantly is true it immediately snaps open as soon as this gets ran.
        /// </summary>
        public void OpenVehicleDoor(DoorId doorid, bool loose, bool openinstantly)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x7C65DAC73C35C862), this.handle, (int)doorid, loose, openinstantly);
        }

        /// <summary>
        /// Removes the specified window on this vehicle.
        /// </summary>
        public void RemoveWindow(WindowIndex window)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xA711568EEDB43069), this.handle, (int)window);
        }

        /// <summary>
        /// Rolls down every window in this vehicle
        /// </summary>
        public void RollDownWindows()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x85796B0549DDE156), this.handle);
        }

        /// <summary>
        /// Rolls down the specified window on this vehicle.
        /// </summary>
        public void RollDownWindow(VehicleWindow window)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x7AD9E6CE657D69E3), this.handle, (int)window);
        }

        /// <summary>
        /// Rolls up the specified window on this vehicle.
        /// </summary>
        public void RollUpWindow(VehicleWindow window)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x602E548F46E24D59), this.handle, (int)window);
        }

        /// <summary>
        /// Smashes the specified window on this vehicle.
        /// </summary>
        public void SmashVehicleWindow(VehicleWindow window)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x9E5B5E4D2CCD2259), this.handle, (int)window);
        }

        /// <summary>
        /// Fixes the specified window on this vehicle if it was broken/smashed.
        /// </summary>
        public void FixVehicleWindow(VehicleWindow window)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x772282EBEB95E682), this.handle, (int)window);
        }

        /// <summary>
        /// Starts this vehicles alarm.
        /// </summary>
        public void StartAlarm()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xB8FF7AB45305C345), this.handle);
        }

        /// <summary>
        /// Controls whether this vehicles engine is on.
        /// </summary>
        public void SetVehicleEngineOn(bool on)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x2497C4717C8B881E), this.handle, on, false, false);
        }

        /// <summary>
        /// Controls whether this vehicles engine is on, if instantly is set to true the driver will not attempt to turn it on and it will just snap on, if disableautostart is true the engine will not automatically start when it tries to.
        /// </summary>
        public void SetVehicleEngineOn(bool on, bool instantly, bool disableautostart)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x2497C4717C8B881E), this.handle, on, instantly, disableautostart);
        }

        /// <summary>
        /// Deforms the vehicles mesh and collision at the specified offset from the vehicle models origin, damage is the intensity of the deformation, radius is the range in which the vehicles model will be deformed at the offset.
        /// </summary>
        public void DeformAt(Vector3 offset, float damage, float radius)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xA1DD317EA8FD4F29), this.handle, offset.X, offset.Y, offset.Z, damage, radius, false);
        }

        /// <summary>
        /// Deforms the vehicles mesh and collision at the specified offset from the vehicle models origin, damage is the intensity of the deformation, radius is the range in which the vehicles model will be deformed at the offset, focus controls whether it makes sure the point actually fits with the vehicles mesh, so even if it isnt on the vehicles mesh it will still go to the closest position to it.
        /// </summary>
        public void DeformAt(Vector3 offset, float damage, float radius, bool focus)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xA1DD317EA8FD4F29), this.handle, offset.X, offset.Y, offset.Z, damage, radius, focus);
        }
    }
    #endregion

    #region Pedestrians
    public class GTAVPed
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong GSCED(uint handle);

        /// <summary>
        /// If this peds memory address has been found.
        /// </summary>
        internal bool hasmemaddr = false;

        /// <summary>
        /// The handle of this ped.
        /// </summary>
        public uint handle { get; }

        private static IntPtr scea;
        private static GSCED gse;


        /// <summary>
        /// The global player handle.
        /// </summary>
        public uint plrhandle
        {
            get => Function.Call<uint>(GTAV.HexHashToNativeHash(0x4F8644AF03D0E0D6));
        }

        /// <summary>
        /// The GTAVTaskHandler for this ped.
        /// </summary>
        public GTAVTaskHandler tasks;

        /// <summary>
        /// If this ped has non-temporary events blocked.
        /// </summary>
        public bool blocknontemporaryevents
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x9F8AA94D6D97DBF4), this.handle, value);
            }
        }

        /// <summary>
        /// If this ped can ragdoll.
        /// </summary>
        public bool canragdoll
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x128F79EDCECE4FD5), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xB128377056A54E2A), this.handle, value);
            }
        }

        /// <summary>
        /// This peds memory address
        /// </summary>
        public unsafe IntPtr memaddress
        {
            get
            {
                if (!hasmemaddr)
                {
                    GetMemoryAddress();
                }
                if (gse == null)
                {
                    return IntPtr.Zero;
                }
                ulong ptr = gse(this.handle);
                return new IntPtr((long)ptr);
            }
        }

        /// <summary>
        /// If this ped is ragdolled.
        /// </summary>
        public bool isragdoll
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x47E4E977581C5B55), this.handle);
        }

        /// <summary>
        /// The heading value of this ped.
        /// </summary>
        public float heading
        {
            get => Function.Call<float>(GTAV.HexHashToNativeHashU(0xE83D4F9BA2A38914), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x8E2530AA8ADA980E), this.handle, value);
            }
        }

        /// <summary>
        /// If this ped has been injured.
        /// </summary>
        public bool isinjured
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x84A2DD9AC37C35C1), this.handle);
        }

        /// <summary>
        /// If this ped has been hurt (apparently isinjured and this are different?).
        /// </summary>
        public bool ishurt
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x5983BB449D7FDB12), this.handle);
        }

        /// <summary>
        /// If this prop is attached to anything.
        /// </summary>
        public bool attachedtoanything
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB346476EF1A64897), this.handle);
        }

        /// <summary>
        /// If this ped has been fatally injured.
        /// </summary>
        public bool isfatallyinjured
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xD839450756ED5A80), this.handle);
        }

        /// <summary>
        /// If this ped is dead or dying.
        /// </summary>
        public bool isdeadordying
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x3317DEDB88C95038), this.handle, false);
        }

        /// <summary>
        /// If this ped is invincible.
        /// </summary>
        public bool isinvincible
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x3882114BDE571AD4), this.handle, value, false);
            }
        }

        /// <summary>
        /// This peds current position.
        /// </summary>
        public Vector3 position
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x3FEF770D40960D5A), this.handle, true);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x06843DA7060A026B), this.handle, value.X, value.Y, value.Z, 0, 0, 0, false);
            }
        }

        /// <summary>
        /// This peds current velocity.
        /// </summary>
        public Vector3 velocity
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x4805D2B1D8CF94A9), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x1C99BB7B6E96D16F), this.handle, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// This peds current position with no offset
        /// </summary>
        public Vector3 positionnooffset
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x239A3351AC1DA385), this.handle, value.X, value.Y, value.Z, false, false, false);
            }
        }

        /// <summary>
        /// This peds current forward vector.
        /// </summary>
        public Vector3 forwardvector
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x0A794A5A57F8DF91), this.handle);
        }

        /// <summary>
        /// This vehicles current rotation.
        /// </summary>
        public Vector3 rotation
        {
            get => Function.Call<Vector3>(GTAV.HexHashToNativeHashU(0xAFBD61CC738D9EB9), this.handle, 2);
        }

        /// <summary>
        /// The vehicle this ped is currently in.
        /// </summary>
        public GTAVVehicle currentvehicle
        {
            get => new GTAVVehicle(Function.Call<uint>(GTAV.HexHashToNativeHashU(0x6094AD011A2EA87D), this.handle));
        }

        /// <summary>
        /// If this ped is reloading a weapon.
        /// </summary>
        public bool isreloading
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x24B100C68C645951), this.handle);
        }

        /// <summary>
        /// If this ped is a player.
        /// </summary>
        public bool isplayer
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x12534C348C6CB68B), this.handle);
        }

        /// <summary>
        /// If this ped is in melee combat.
        /// </summary>
        public bool isinmeleecombat
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x4E209B2C1EAD5159), this.handle);
        }

        /// <summary>
        /// If this ped is shooting a weapon.
        /// </summary>
        public bool isshooting
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x34616828CD07F1A1), this.handle);
        }

        /// <summary>
        /// This peds aiming accuracy.
        /// </summary>
        public int accuracy
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHash(0x37F4AD56ECBC0CD6), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x7AEFB85C1D49DEB6), this.handle, value);
            }
        }

        /// <summary>
        /// This peds armour level.
        /// </summary>
        public int armour
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0x9483AF821605B1D8), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xCEA04D83135264CC), this.handle, value);
            }
        }

        /// <summary>
        /// If this ped is a male.
        /// </summary>
        public bool ismale
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x6D9F5FAA7488BA46), this.handle);
        }

        /// <summary>
        /// If this ped is human.
        /// </summary>
        public bool ishuman
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB980061DA992779D), this.handle);
        }

        /// <summary>
        /// If this ped is on a vehicle (eg. on a vehicle turret).
        /// </summary>
        public bool isonvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x67722AEB798E5FAB), this.handle);
        }

        /// <summary>
        /// This peds amount of money.
        /// </summary>
        public int money
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0x3F69145BBA87BAE7), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xA9C8960E8684C1B5), this.handle, value);
            }
        }

        /// <summary>
        /// If this ped is on foot.
        /// </summary>
        public bool isonfoot
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x01FEE67DB37F59B2), this.handle);
        }

        /// <summary>
        /// If this ped is on any bike.
        /// </summary>
        public bool isonanybike
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x94495889E22C6479), this.handle);
        }

        /// <summary>
        /// If this ped is planting a bomb.
        /// </summary>
        public bool isplantingbomb
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xC70B5FAE151982D8), this.handle);
        }

        /// <summary>
        /// If this ped is in any boat.
        /// </summary>
        public bool isinanyboat
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x2E0E1C2B4F6CB339), this.handle);
        }

        /// <summary>
        /// If this ped is in any submarine.
        /// </summary>
        public bool isinanysub
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xFBFC01CCFB35D99E), this.handle);
        }

        /// <summary>
        /// If this ped is in any helicopter.
        /// </summary>
        public bool isinanyheli
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x298B91AE825E5705), this.handle);
        }

        /// <summary>
        /// If this ped is in any plane.
        /// </summary>
        public bool isinanyplane
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x5FFF4CFC74D8FB80), this.handle);
        }

        /// <summary>
        /// If this ped is in any flying vehicle.
        /// </summary>
        public bool isinanyflyingvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x9134873537FA419C), this.handle);
        }

        /// <summary>
        /// If this ped dies in water.
        /// </summary>
        public bool diesinwater
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x65671A4FB8218930), this.handle);
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x56CEF0AC79073BDE), this.handle, value);
            }
        }

        /// <summary>
        /// Returns the hash of this peds current weapon.
        /// </summary>
        public unsafe uint currentweapon
        {
            get
            {
                int hash = 0;
                Function.Call(GTAV.HexHashToNativeHash(0x3A87E44BB9A01D54), this.handle, &hash, false);
                return (uint)hash;
            }
        }

        /// <summary>
        /// This peds last damaged bone (use (Bone)bone to convert it to a SHVDN bone), if no bone has been damaged it returns 0.
        /// </summary>
        public int lastdamagebone
        {
            get
            {
                OutputArgument arg = new OutputArgument();
                Function.Call<bool>(GTAV.HexHashToNativeHashU(0xD75960F6BD9EA49C), this.handle, arg);
                int bone = 0;
                bone = arg.GetResult<int>();
                return bone;
            }
        }

        /// <summary>
        /// The last point this ped hit with a weapon, returns Vector3.Zero if no hit point was found.
        /// </summary>
        public Vector3 lastweaponhitcoord
        {
            get
            {
                OutputArgument outarg = new OutputArgument();
                bool didhitanything = Function.Call<bool>(GTAV.HexHashToNativeHashU(0x6C4D0409BA1A2BC2), this.handle, outarg);
                Vector3 bulletcoord = outarg.GetResult<Vector3>();
                if (!didhitanything)
                {
                    return Vector3.Zero;
                }
                return bulletcoord;
            }
        }
        /// <summary>
        /// If this ped is in any police vehicle.
        /// </summary>
        public bool isinanypolicevehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x0BD04E29640C9C12), this.handle);
        }

        /// <summary>
        /// If this ped is freefalling with a parachute.
        /// </summary>
        public bool isinparachutefreefall
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x7DCE8BDA0F1C1200), this.handle);
        }

        /// <summary>
        /// If this ped is falling.
        /// </summary>
        public bool isfalling
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xFB92A102F1C4DFA3), this.handle);
        }

        /// <summary>
        /// If this ped is jumping.
        /// </summary>
        public bool isjumping
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xCEDABC5900A0BF97), this.handle);
        }

        /// <summary>
        /// If this ped is climbing.
        /// </summary>
        public bool isclimbing
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x53E8CB4F48BFE623), this.handle);
        }

        /// <summary>
        /// If this ped is vaulting over a ledge.
        /// </summary>
        public bool isvaulting
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x117C70D1F5730B5E), this.handle);
        }

        /// <summary>
        /// If this ped is diving.
        /// </summary>
        public bool isdiving
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x5527B8246FEF9B11), this.handle);
        }

        /// <summary>
        /// If this ped is jumping out of a vehicle.
        /// </summary>
        public bool isjumpingoutofvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x433DDFFE2044B636), this.handle);
        }

        /// <summary>
        /// If this ped is opening a door.
        /// </summary>
        public bool isopeningadoor
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x26AF0E8E30BD2A2C), this.handle);
        }

        /// <summary>
        /// If this ped is ducking.
        /// </summary>
        public bool isducking
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xD125AE748725C6BC), this.handle);
        }

        /// <summary>
        /// If this ped is in any taxi.
        /// </summary>
        public bool isinanytaxi
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x6E575D6A898AB852), this.handle);
        }

        /// <summary>
        /// If this ped is in stealth mode.
        /// </summary>
        public bool isstealthing
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x7C2AC9CA66575FBF), this.handle);
        }

        /// <summary>
        /// If this ped is hanging on to a vehicle.
        /// </summary>
        public bool ishangingontovehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x1C86D8AEF8254B78), this.handle);
        }

        /// <summary>
        /// If this ped is prone.
        /// </summary>
        public bool isprone
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xD6A86331A537A7B9), this.handle);
        }

        /// <summary>
        /// If this ped is doing a drive-by.
        /// </summary>
        public bool isdoingdriveby
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xB2C086CC1BF8F2BF), this.handle);
        }

        /// <summary>
        /// If this ped is jacking a vehicle.
        /// </summary>
        public bool isjackingvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x4AE4FF911DFB61DA), this.handle);
        }

        /// <summary>
        /// If this ped is currently a victim to a car jacking.
        /// </summary>
        public bool isbeingcarjacked
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x9A497FE2DF198913), this.handle);
        }

        /// <summary>
        /// If this ped is stunned.
        /// </summary>
        public bool isstunned
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x4FBACCE3B4138EE8), this.handle, 0);
        }

        /// <summary>
        /// If this ped is fleeing.
        /// </summary>
        public bool isfleeing
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xBBCCE00B381F8482), this.handle);
        }

        /// <summary>
        /// If this ped is in a vehicle.
        /// </summary>
        public bool isinvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x826AA586EDB9FEF8), this.handle);
        }
        /// <summary>
        /// If this ped is swimming.
        /// </summary>
        public bool isswimming
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x9DE327631295B4C2), this.handle);
        }

        /// <summary>
        /// If this ped is swimming underwater.
        /// </summary>
        public bool isswimmingunderwater
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xC024869A53992F34), this.handle);
        }

        /// <summary>
        /// If this ped is a passenger or driver of any train.
        /// </summary>
        public bool isinanytrain
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x6F972C1AB75A1ED0), this.handle);
        }

        /// <summary>
        /// If this ped is using their mobile phone.
        /// </summary>
        public bool isusingmobilephone
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x2AFE52F782F25775), this.handle);
        }

        /// <summary>
        /// If this ped is getting into any vehicle.
        /// </summary>
        public bool isgettingintovehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0xBB062B2B5722478E), this.handle);
        }

        /// <summary>
        /// If this ped is trying to enter a locked vehicle.
        /// </summary>
        public bool istryingtoenterlockedvehicle
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHashU(0x44D28D5DDFE5F68C), this.handle);
        }

        /// <summary>
        /// This peds alertness level.
        /// </summary>
        public int alertness
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0xF6AA118530443FD2), this.handle);
            set
            {
                if (value > 3)
                {
                    value = 3;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                Function.Call(GTAV.HexHashToNativeHashU(0xDBA71115ED9941A6), this.handle, value);
            }
        }

        /// <summary>
        /// This peds health.
        /// </summary>
        public int health
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0xEEF059FAD016D209), this.handle);
            set
            {
                if (value > 200)
                {
                    value = 200;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                Function.Call(GTAV.HexHashToNativeHashU(0x6B76DC1F3AE6E6A3), this.handle, value);
            }
        }

        /// <summary>
        /// This peds max health.
        /// </summary>
        public int maxhealth
        {
            get => Function.Call<int>(GTAV.HexHashToNativeHashU(0x15D757606D170C3C), this.handle);
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                Function.Call(GTAV.HexHashToNativeHashU(0x166E7CF68597D8B5), this.handle, value);
            }
        }

        /// <summary>
        /// If this ped is a mission entity.
        /// </summary>
        public bool ismissionentity
        {
            get => Function.Call<bool>(GTAV.HexHashToNativeHash(0x0A7B270912999B3C), this.handle);
            set
            {
                if (!value)
                {
                    this.SetAsNoLongerNeeded();
                }
                else
                {
                    Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), this.handle, true, true);
                }
            }
        }

        /// <summary>
        /// This peds model.
        /// </summary>
        public GTAVModel model
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                GTAVModel mdl = new GTAVModel(((PedCache)hash).ToString());
                return mdl;
            }
        }

        /// <summary>
        /// This peds model cache.
        /// </summary>
        public PedCache modelcache
        {
            get
            {
                uint hash = Function.Call<uint>(GTAV.HexHashToNativeHashU(0x9F47B058362C84B5), this.handle);
                return (PedCache)hash;
            }
        }

        /// <summary>
        /// A ped.
        /// </summary>
        public GTAVPed(GTAVModel model, Vector3 pos, float heading)
        {
            model.LoadIntoMemory();
            handle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0xD49F9B0955C367DE), 26, model, pos.X, pos.Y, pos.Z, heading, false, false);
            tasks = new GTAVTaskHandler(handle, this);
        }

        /// <summary>
        /// A ped. (Only use this to create a GTAVPed for an already existing ped)
        /// </summary>
        public GTAVPed(uint handle)
        {
            this.handle = handle;
            tasks = new GTAVTaskHandler(handle, this);
        }

        /// <summary>
        /// Does all the stuff to get this peds memory address
        /// </summary>
        internal unsafe void GetMemoryAddress()
        {
            bool existsindb = false;
            foreach (InternalMemIdentifier imi in GTAV.memidentifiers)
            {
                if (imi.entity.handle == handle)
                {
                    existsindb = true;
                }
            }
            if (!existsindb)
            {
                IntPtr address = GTAV.FindPattern("\x85\xED\x74\x0F\x8B\xCD\xE8\x00\x00\x00\x00\x48\x8B\xF8\x48\x85\xC0\x74\x2E", "xxxxxxx????xxxxxxxx");
                if (address == IntPtr.Zero)
                {
                    throw new AccessViolationException("Memory pattern not found or the memory is protected.");
                }
                int rel = Marshal.ReadInt32(address + 7);
                IntPtr fnptr = address + 11 + rel;
                gse = Marshal.GetDelegateForFunctionPointer<GSCED>(fnptr);
                this.hasmemaddr = true;
                GTAV.memidentifiers.Add(new InternalMemIdentifier(this.memaddress, this));
            }
        }

        //support for using GTAVPed as an InputArgument for native calls
        public static implicit operator InputArgument(GTAVPed ped)
        {
            return new InputArgument(ped.handle);
        }

        //support for using GTAVPed as a GTAVEntity
        public static implicit operator GTAVEntity(GTAVPed ped)
        {
            return new GTAVEntity(ped);
        }

        //support for using GTAVPed as a uint
        public static implicit operator uint(GTAVPed ped)
        {
            return ped.handle;
        }

        /// <summary>
        /// Sets the specified config flag to the specified value on this ped.
        /// </summary>
        public void SetConfigFlag(int flag, bool on)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x1913FE4CBF41C463), this.handle, flag, on);
        }

        /// <summary>
        /// Gets the specified config flag value on this ped.
        /// </summary>
        public bool GetConfigFlag(int flag)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHash(0x7EE53118C892B513), this.handle, flag, false);
        }

        /// <summary>
        /// Attaches this ped to another entities bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity, bone, 0f, 0f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Attaches this ped to another entities bone with position being a local position offset to the bone and rotation being a local rotation offset (or world? not sure yet) to the bone.
        /// </summary>
        public void Attach(GTAVEntity entity, int bone, Vector3 position, Vector3 rotation)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6B9BBD38AB0796DF), this.handle, entity.handle, bone, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, false, false, false, false, 2, true);
        }

        /// <summary>
        /// Clears/removes all of this peds props.
        /// </summary>
        public void ClearAllProps()
        {
            Function.Call((Hash)0xCD8A7537A9B52F06, this.handle, 0);
        }

        /// <summary>
        /// Converts this ped into a SHVDN ped.
        /// </summary>
        public Ped ConvertToSHVDNPed()
        {
            return new Ped((int)this.handle);
        }

        public void GiveWeapon(WeaponCache hash, int ammocount, bool hidden, bool forceinhand)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBF0FD6E56C964FCB), this.handle, (uint)hash, ammocount, hidden, forceinhand);
        }

        /// <summary>
        /// If this ped is on the set vehicle (from handle).
        /// </summary>
        public bool IsOnSpecificVehicle(GTAVVehicle vehiclehandle)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHashU(0xEC5F66E459AF3BB2), this.handle, vehiclehandle);
        }

        /// <summary>
        /// Murders this ped.
        /// </summary>
        public void Kill()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x2D05CED3A38D0F3A), this.handle, (ulong)WeaponHash.Minigun);
        }

        /// <summary>
        /// Sets the specified reset flag to the specified value on this ped.
        /// </summary>
        public void SetResetFlag(int flag, bool on)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xC1E8A365BF3B29F2), this.handle, flag, on);
        }

        /// <summary>
        /// Gets the specified reset flag value for this ped.
        /// </summary>
        public bool GetResetFlag(int flag)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHashU(0xAF9E59B1B1FBF2A0), this.handle, flag);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped.
        /// </summary>
        public void SendNMMessage(NMMessage message)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped.
        /// </summary>
        public void SendNMMessage(int message)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped and can auto-ragdoll them.
        /// </summary>
        public void SendNMMessage(NMMessage message, bool autoragdoll)
        {
            if (autoragdoll)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xAE99FB955581844A), this.handle, 0, -1, 1, true, true, false);
            }
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped and can auto-ragdoll them.
        /// </summary>
        public void SendNMMessage(int message, bool autoragdoll)
        {
            if (autoragdoll)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xAE99FB955581844A), this.handle, 0, -1, 1, true, true, false);
            }
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped, can auto-ragdoll them and can ragdoll them for a specific duration of time (in ms).
        /// </summary>
        public void SendNMMessage(NMMessage message, bool autoragdoll, int duration)
        {
            if (autoragdoll)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xAE99FB955581844A), this.handle, 0, duration, 1, true, true, false);
            }
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Sends the NaturalMotion message to this ped, can auto-ragdoll them and can ragdoll them for a specific duration of time (in ms).
        /// </summary>
        public void SendNMMessage(int message, bool autoragdoll, int duration)
        {
            if (autoragdoll)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xAE99FB955581844A), this.handle, 0, duration, 1, true, true, false);
            }
            Function.Call(GTAV.HexHashToNativeHash(0x418EF2A1BCE56685), true, (int)message);
            Function.Call(GTAV.HexHashToNativeHashU(0xB158DFCCC56E5C5B), this.handle);
        }

        /// <summary>
        /// Un-ragdolls this ped.
        /// </summary>
        public void Unragdoll()
        {
            if (isragdoll)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xAE99FB955581844A), this.handle, 0, 100, 1, true, true, false);
            }
        }

        /// <summary>
        /// Deletes this ped.
        /// </summary>
        public unsafe void Delete()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHashU(0xAD738C3085FE7E11), handle, false, true);
            Function.Call(GTAV.HexHashToNativeHashU(0x9614299DCB53E54B), &handle);
        }

        /// <summary>
        /// Makes this ped no longer needed. (the game will delete them when it seems fit and/or when the engine is running out of ram)
        /// </summary>
        public unsafe void SetAsNoLongerNeeded()
        {
            uint handle = this.handle;
            Function.Call(GTAV.HexHashToNativeHash(0x2595DD4236549CE3), &handle);
        }

        /// <summary>
        /// If this ped is touching the specified entity (entity handle)
        /// </summary>
        public bool IsTouchingEntity(uint entityhandle)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHash(0x17FFC1B2BA35A494), this.handle, entityhandle);
        }

        /// <summary>
        /// Clears this peds last damage bone.
        /// </summary>
        public void ClearLastDamageBone()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x8EF6B7AC68E2F01B), this.handle);
        }

        /// <summary>
        /// Gets the index for a bone, for example if you use this for 31086 (head bone) it returns the bone index for the head bone
        /// </summary>
        public int GetBoneIndexFromBoneID(int boneid)
        {
            return Function.Call<int>(GTAV.HexHashToNativeHash(0x3F428D08BE5AAE31), this.handle, boneid);
        }

        /// <summary>
        /// Gets the bone transform data for one of this peds bones.
        /// </summary>
        public BoneTransform GetBoneTransformForBone(int bone)
        {
            Vector3 pos = Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x17C07FC640E86B4E), this.handle, bone, 0f, 0f, 0f);
            Vector3 rot = Function.Call<Vector3>(GTAV.HexHashToNativeHashU(0xCE6294A232D03786), this.handle, bone);
            BoneTransform data = new BoneTransform(pos, rot);
            return data;
        }

        /// <summary>
        /// Gets the coordinates for the specified ped bone with a specified offset.
        /// </summary>
        public Vector3 GetBoneCoord(int bone, Vector3 offset)
        {
            int index = this.GetBoneIndexFromBoneID(bone);
            return Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x17C07FC640E86B4E), this.handle, index, offset.X, offset.Y, offset.Y);
        }
    }
    #endregion

    #region Tasks
    public class GTAVTaskHandler
    {
        public uint handle { get; }
        public GTAVPed thisped { get; }

        public GTAVTaskHandler(uint handle, GTAVPed thisped)
        {
            this.handle = handle;
            this.thisped = thisped;
        }

        /// <summary>
        /// Clears every task that is currently running immediately.
        /// </summary>
        public void ClearTasksImmediately()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xAAA34F8A7CB32098), this.handle);
        }

        /// <summary>
        /// Clears every task that is currently running when the engine seems fit.
        /// </summary>
        public void ClearTasks()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xE1EF3C1216AFF2CD), this.handle);
        }

        /// <summary>
        /// Makes the ped stand still.
        /// </summary>
        public void TaskStandStill()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x919BE13EED931959), this.handle, -1);
        }

        /// <summary>
        /// Makes the ped stand still for a set period of time (in ms).
        /// </summary>
        public void TaskStandStill(int duration)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x919BE13EED931959), this.handle, duration);
        }

        /// <summary>
        /// Makes the ped put their hands up.
        /// </summary>
        public void TaskHandsUp()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xF2EAB31979A7F910), this.handle, -1, 0, 0, 0);
        }

        /// <summary>
        /// Makes the ped put their hands up for a set period of time (in ms).
        /// </summary>
        public void TaskHandsUp(int duration)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xF2EAB31979A7F910), this.handle, duration, 0, 0, 0);
        }

        /// <summary>
        /// Makes the ped fight against the set enemy.
        /// </summary>
        public void TaskFightAgainst(GTAVPed enemy)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xF166E48407BAC484), this.handle, enemy.handle, 0, 16);
        }

        /// <summary>
        /// Makes the ped warp into the specified vehicle.
        /// </summary>
        public void TaskWarpIntoVehicle(GTAVVehicle vehicle, Seat seat)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x9A7D091411C5F684), this.handle, vehicle.handle, (int)seat);
        }

        /// <summary>
        /// Makes the ped put their hands up for a set period of time (in ms) and makes them face another ped aslong as they are still in the timeframe where they can face them (in ms).
        /// </summary>
        public void TaskHandsUp(int duration, GTAVPed facing, int timetofaceped)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xF2EAB31979A7F910), this.handle, duration, facing, timetofaceped, 0);
        }

        /// <summary>
        /// Makes the ped cower.
        /// </summary>
        public void TaskCower()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x3EB1FE9E8E908E15), this.handle, -1);
        }

        /// <summary>
        /// Makes the ped cower for a set period of time (in ms).
        /// </summary>
        public void TaskCower(int duration)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x3EB1FE9E8E908E15), this.handle, duration);
        }

        /// <summary>
        /// Makes the ped jump.
        /// </summary>
        public void TaskJump()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x0AE4086104E067B1), this.handle, false, false, false);
        }

        /// <summary>
        /// Makes the ped jump with the given arguments.
        /// </summary>
        public void TaskJump(bool useplayerlaunchforce, bool dosuperjump, bool usefullsuperjumpforce)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x0AE4086104E067B1), this.handle, useplayerlaunchforce, dosuperjump, usefullsuperjumpforce);
        }

        /// <summary>
        /// Makes the ped open a vehicle door (from vehicle handle) aslong as the seat exists, the timeout threshold hasn't exceeded (I think speed is how fast they go to the door?).
        /// </summary>
        public void TaskOpenVehicleDoor(GTAVVehicle vehicle, int timeout, Seat seat, float speed)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x965791A9A488A062), this.handle, vehicle.handle, timeout, (int)seat, speed);
        }

        /// <summary>
        /// Makes the ped enter a vehicle.
        /// </summary>
        public void TaskEnterVehicle(GTAVVehicle vehicle)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x965791A9A488A062), this.handle, vehicle.handle, -1, -1, 1.0f, 0, 0);
        }

        /// <summary>
        /// Makes the ped enter a vehicle aslong as the timeout threshold isn't met and the seat exists.
        /// </summary>
        public void TaskEnterVehicle(GTAVVehicle vehicle, int timeout, Seat seat, int speed)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x965791A9A488A062), this.handle, vehicle.handle, timeout, (int)seat, speed, 0, 0);
        }

        /// <summary>
        /// Makes the ped leave the vehicle they are currently in.
        /// </summary>
        public void TaskLeaveVehicle(GTAVVehicle vehicle)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xD3DBCE61A490BE02), this.handle, vehicle.handle, 0);
        }

        /// <summary>
        /// Makes the ped leave the vehicle they are currently in with the given flags.
        /// </summary>
        public void TaskLeaveVehicle(GTAVVehicle vehicle, LeaveVehicleFlags flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xD3DBCE61A490BE02), this.handle, vehicle.handle, (int)flags);
        }

        /// <summary>
        /// Makes the ped get off the boat they are currently on.
        /// </summary>
        public void TaskGetOffBoat(GTAVVehicle boat)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x9C00E77AF14B2DFF), this.handle, boat);
        }

        /// <summary>
        /// Makes the ped skydive.
        /// </summary>
        public void TaskSkydive(bool instant)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x601736CFE536B0A0), this.handle, instant);
        }

        /// <summary>
        /// Makes the ped parachute.
        /// </summary>
        public void TaskParachute(bool giveparachuteitem, bool instant)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x601736CFE536B0A0), this.handle, giveparachuteitem, instant);
        }

        /// <summary>
        /// Makes the ped parachute to certain coordinates.
        /// </summary>
        public void TaskParachute(Vector3 coords)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xB33E291AFA6BD03A), this.handle, coords.X, coords.Y, coords.Z);
        }

        /// <summary>
        /// Makes the ped rappel from the helicopter they are a passenger of.
        /// </summary>
        public void TaskRappelFromHelicopter(float minheightaboveground)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x09693B0312F91649), this.handle, minheightaboveground);
        }

        /// <summary>
        /// Makes the ped drive their vehicle to certain coordinates.
        /// </summary>
        public void TaskDriveToCoord(Vector3 coords, float speed)
        {
            if (thisped.currentvehicle.handle != 0)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x158BB33F920D360C), this.handle, thisped.currentvehicle.handle, coords.X, coords.Y, coords.Z, speed, 0, 0);
            }
        }

        /// <summary>
        /// Makes the ped drive their vehicle around randomly (still follows navigation paths).
        /// </summary>
        public void TaskDriveWander(float speed, int drivingstyle)
        {
            if (thisped.currentvehicle.handle != 0)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x158BB33F920D360C), this.handle, thisped.currentvehicle.handle, speed, drivingstyle);
            }
        }

        /// <summary>
        /// Makes the ped go straight to certain coordinates (ignores navigation paths).
        /// </summary>
        public void TaskGoStraightToCoords(Vector3 coords, float speed, int timeout)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xD76B57B44F1E6F8B), this.handle, coords.X, coords.Y, coords.Z, speed, timeout, 0, 0);
        }

        /// <summary>
        /// Makes the ped go straight to certain coordinates relative to another entity (from entity handle).
        /// </summary>
        public void TaskGoStraightToCoordsRelativeToEntity(uint entityhandle, Vector3 coords, float moveblendratio, int timeout)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x61E360B7E040D12E), this.handle, entityhandle, coords.X, coords.Y, coords.Z, moveblendratio, timeout);
        }

        /// <summary>
        /// Makes the ped look at the heading value aslong as the timeout threshold isn't met.
        /// </summary>
        public void TaskAchieveHeading(float heading, int timeout)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x93B93A37987F1F3D), this.handle, heading, timeout);
        }

        /// <summary>
        /// Makes the ped react to danger and flee from the specified ped (from handle).
        /// </summary>
        public void TaskReactAndFlee(uint fleeingfrom)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x72C896464915D1B1), this.handle, fleeingfrom);
        }

        /// <summary>
        /// Makes the ped wander around in a certain area within a given radius of that area.
        /// </summary>
        public void TaskWanderInArea(Vector3 coords, float radius, float minimumwalklength, float timebetweenwalks)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xE054346CA3A0F315), this.handle, coords.X, coords.Y, coords.Z, radius, minimumwalklength, timebetweenwalks);
        }

        /// <summary>
        /// Makes the ped wander around aimlessly.
        /// </summary>
        public void TaskWanderStandard()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBB9CE077274F6A1B), this.handle, 0f, 0);
        }

        /// <summary>
        /// Makes the ped wander around aimlessly.
        /// </summary>
        public void TaskWanderStandard(float heading, int flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBB9CE077274F6A1B), this.handle, heading, flags);
        }

        /// <summary>
        /// Makes the ped park their car at given coordinates maintaining the set heading direction in the process.
        /// </summary>
        public void TaskVehiclePark(Vector3 coords, float heading)
        {
            if (thisped.currentvehicle.handle != 0)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x0F3E34E968EA374E), this.handle, thisped.currentvehicle.handle, coords.X, coords.Y, coords.Z, heading, 0, 5f, false);
            }
        }

        /// <summary>
        /// Makes the ped park their car at given coordinates in the set radius maintaining the set heading direction in the process.
        /// </summary>
        public void TaskVehiclePark(Vector3 coords, float heading, ParkMode mode, float radius, bool keepengineon)
        {
            if (thisped.currentvehicle.handle != 0)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x0F3E34E968EA374E), this.handle, thisped.currentvehicle.handle, coords.X, coords.Y, coords.Z, heading, (int)mode, radius, keepengineon);
            }
        }

        /// <summary>
        /// Makes the ped plant a bomb at the peds coordinates.
        /// </summary>
        public void TaskPlantBomb()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x965FEC691D55E9BF), this.handle, thisped.position.X, thisped.position.Y, thisped.position.Z, thisped.heading);
        }

        /// <summary>
        /// Makes the ped plant a bomb at the given coordinates.
        /// </summary>
        public void TaskPlantBomb(Vector3 coords, float heading)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x965FEC691D55E9BF), this.handle, coords.X, coords.Y, coords.Z, heading);
        }

        /// <summary>
        /// Makes the ped go to the set coordinates by any means necessary
        /// </summary>
        public void TaskGoToCoordAnyMeans(Vector3 coords)
        {
            if (!thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x5BC448CB78FA3E88), this.handle, coords.X, coords.Y, coords.Z, 1, 0, false, 0, 10);
            }
            else if (thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x5BC448CB78FA3E88), this.handle, coords.X, coords.Y, coords.Z, 1, thisped.currentvehicle.handle, true, 0, 10);
            }
        }

        /// <summary>
        /// Makes the ped go to the set coordinates by any means necessary
        /// </summary>
        public void TaskGoToCoordAnyMeans(Vector3 coords, float moveblendratio, GTAVVehicle vehicle, bool uselongrangevehiclepathing, int drivingflags, float maxrangetoshoottargets)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x5BC448CB78FA3E88), this.handle, coords.X, coords.Y, coords.Z, moveblendratio, vehicle, uselongrangevehiclepathing, drivingflags, maxrangetoshoottargets);
        }

        /// <summary>
        /// Plays an animation on the ped from the given animation dictionary with the set blend in, blend out, duration and animation flags.
        /// </summary>
        public void TaskPlayAnim(string animdict, string animname, float blendin, float blendout, float duration, AnimFlags flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xEA47FE3719165B94), this.handle, animdict, animname, blendin, blendout, duration, (int)flags, 1.0f, false, false, false);
        }

        /// <summary>
        /// Plays an animation on the ped from the given animation dictionary with the set blend in, blend out, speed, lockx, locky, lockz, duration and animation flags.
        /// </summary>
        public void TaskPlayAnim(string animdict, string animname, float blendin, float blendout, float speed, float duration, AnimFlags flags, bool lockx, bool locky, bool lockz)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xEA47FE3719165B94), this.handle, animdict, animname, blendin, blendout, duration, (int)flags, speed, lockx, locky, lockz);
        }

        /// <summary>
        /// Plays an animation on the ped from the given animation dictionary with the set blend in, blend out, duration and animation flags.
        /// </summary>
        public void TaskPlayAnim(string animdict, string animname, float blendin, float blendout, float duration, int flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xEA47FE3719165B94), this.handle, animdict, animname, blendin, blendout, duration, flags, 1.0f, false, false, false);
        }

        /// <summary>
        /// Plays an animation on the ped from the given animation dictionary with the set blend in, blend out, speed, lockx, locky, lockz, duration and animation flags.
        /// </summary>
        public void TaskPlayAnim(string animdict, string animname, float blendin, float blendout, float speed, float duration, int flags, bool lockx, bool locky, bool lockz)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xEA47FE3719165B94), this.handle, animdict, animname, blendin, blendout, duration, flags, speed, lockx, locky, lockz);
        }

        /// <summary>
        /// Makes the ped look at specific coordinates given the priority is high enough.
        /// </summary>
        public void TaskLookAtCoord(Vector3 coords, int priority)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x6FA46612594F7973), this.handle, coords.X, coords.Y, coords.Z, -1, 2048, priority);
        }

        /// <summary>
        /// Makes the ped look at specific coordinates given the priority is high enough, duration (in ms) is long enough and the flags are valid.
        /// </summary>
        public void TaskLookAtCoord(Vector3 coords, int priority, int duration, LookAtFlags flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x6FA46612594F7973), this.handle, coords.X, coords.Y, coords.Z, duration, (int)flags, priority);
        }

        /// <summary>
        /// Makes the ped look at the given entity aslong as priority is high enough, duration (in ms) is long enough and the flags are valid.
        /// </summary>
        public void TaskLookAtEntity(GTAVEntity entity, int duration, LookAtFlags flags, int priority)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x69F4BE8C8CC4796C), this.handle, entity, duration, (int)flags, priority);
        }

        /// <summary>
        /// Makes the ped leave any vehicle.
        /// </summary>
        public void TaskLookAtEntity(int delay, LeaveVehicleFlags flags)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x504D54DF3F6F2247), this.handle, delay, (int)flags);
        }

        /// <summary>
        /// Makes the ped aim their gun at the given entity until the time they have been aiming has exceeded duration (in ms).
        /// </summary>
        public void TaskAimGunAtEntity(GTAVEntity entity, int duration, bool instantblendtoaim)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x9B53BB6E8943AF53), this.handle, entity, duration, instantblendtoaim);
        }

        /// <summary>
        /// Makes the ped turn to face the given entity until the time they have been facing the entity has exceeded duration (in ms).
        /// </summary>
        public void TaskTurnToFaceEntity(GTAVEntity entity, int duration)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x5AD23D40115353AC), this.handle, entity, duration);
        }

        /// <summary>
        /// Makes the ped aim their gun at the given coords until they have aimed at the coords longer than the duration value (in ms).
        /// </summary>
        public void TaskAimGunAtCoords(Vector3 coords, int duration)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x6671F3EEC681BDA1), this.handle, coords.X, coords.Y, coords.Z, duration, false, true);
        }

        /// <summary>
        /// Makes the ped shoot at the given coordinates until they have shot at the coords longer than the duration value (in ms).
        /// </summary>
        public void TaskShootAtCoords(Vector3 coords, int duration, FiringPattern pattern)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x46A6CC01E0826106), this.handle, coords.X, coords.Y, coords.Z, duration, (uint)pattern);
        }

        /// <summary>
        /// Makes the ped shuffle to the next vehicle seat (aslong as they are actually in a vehicle).
        /// </summary>
        public void TaskShuffleToNextVehicleSeat(bool usealternateshuffle)
        {
            if (thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x7AA80209BDA643EB), this.handle, thisped.currentvehicle.handle, usealternateshuffle);
            }
        }

        /// <summary>
        /// Makes the ped escort the target vehicle (vehicle handle) with the given arguments (I can't think of any good documentation for this, the value names should give you a good idea though).
        /// </summary>
        public void TaskEscortVehicle(GTAVVehicle targetvehicle, VehicleEscortMode escortmode, DrivingStyle drivingstyle, float mindistance, int minheightaboveterrain, float noroadsdistance)
        {
            if (thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x0FA6E4B75F302400), this.handle, thisped.currentvehicle.handle, targetvehicle.handle, (int)escortmode, (int)drivingstyle, mindistance, minheightaboveterrain, noroadsdistance);
            }
        }

        /// <summary>
        /// Makes the ped follow the target entity (entity handle) in a vehicle with the set speed, drivingstyle, and minimum distance (the minimum amount of distance the ped can be from the target entity).
        /// </summary>
        public void TaskFollowEntityInVehicle(GTAVEntity targetentity, float speed, DrivingStyle drivingstyle, int mindistance)
        {
            if (thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0xFC545A9F0626E3B6), this.handle, thisped.currentvehicle, targetentity, speed, (int)drivingstyle, mindistance);
            }
        }

        /// <summary>
        /// Makes the ped chase the given entity (entity handle) in their vehicle.
        /// </summary>
        public void TaskChaseEntityInVehicle(GTAVEntity targetentity)
        {
            if (thisped.isinvehicle)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x3C08A8E30363B353), this.handle, targetentity);
            }
        }

        /// <summary>
        /// Makes the ped use their mobile phone.
        /// </summary>
        public void TaskUseMobilePhone()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBD2A8EC3AF4DE7DB), this.handle, true, 0);
        }

        /// <summary>
        /// Makes the ped use their mobile phone with the desired phone mode.
        /// </summary>
        public void TaskUseMobilePhone(int phonemode)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xBD2A8EC3AF4DE7DB), this.handle, true, phonemode);
        }

        /// <summary>
        /// Makes the ped use their mobile phone until they have used it longer than duration (in ms).
        /// </summary>
        public void TaskUseMobilePhoneTimed(int duration)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x5EE02954A14C69DB), this.handle, duration);
        }

        /// <summary>
        /// Makes the ped climb.
        /// </summary>
        public void TaskClimb()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x89D9FCC2435112F1), this.handle);
        }

        /// <summary>
        /// Makes the ped climb a ladder.
        /// </summary>
        public void TaskClimbLadder(bool fast)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x89D9FCC2435112F1), this.handle, fast);
        }

        /// <summary>
        /// Makes the ped reload their current weapon.
        /// </summary>
        public void TaskReloadWeapon()
        {
            Function.Call(GTAV.HexHashToNativeHash(0x62D2916F56B9CD2D), this.handle, true);
        }

    }
    #endregion

    #region Models
    /// <summary>
    /// A model.
    /// </summary>
    public class GTAVModel
    {
        public string name;
        public object cache;
        public uint hash;

        public GTAVModel(object input)
        {
            switch (input)
            {
                case PedCache pcache:
                    this.name = pcache.ToString();
                    break;
                case VehicleCache vcache:
                    this.name = vcache.ToString();
                    break;
                case PropCache pcache:
                    this.name = pcache.ToString();
                    break;
                case string str:
                    this.name = str;
                    this.cache = null;
                    break;
                default:
                    throw new ArgumentException("Unsupported type, please use a different type and try again.");
            }
            hash = GTAV.GenHashKey(name.ToString());
        }

        public void LoadIntoMemory()
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x963D27A58DF860AC), this.hash);
            while (!Function.Call<bool>(GTAV.HexHashToNativeHashU(0x98A4EB5D89A0C952), this.hash))
            {
                Script.Yield();
            }
        }

        //support for using GTAVModel in native calls.
        public static implicit operator InputArgument(GTAVModel mdl)
        {
            return mdl.hash;
        }
    }
    #endregion

    #region Entities
    /// <summary>
    /// An entity. (not recommended for normal use, used internally for some functions, if a function requires one of these you can pass stuff like GTAVPed instead of GTAVEntity and it will still work)
    /// </summary>
    public class GTAVEntity
    {
        public uint handle;
        public object entity;

        public GTAVEntity(object input)
        {
            switch (input)
            {
                case GTAVPed gvp:
                    this.entity = gvp;
                    this.handle = gvp.handle;
                    break;
                case GTAVVehicle gvv:
                    this.entity = gvv;
                    this.handle = gvv.handle;
                    break;
                case GTAVProp prop:
                    this.entity = prop;
                    this.handle = prop.handle;
                    break;
                case Ped ped:
                    this.entity = new GTAVPed((uint)ped.Handle);
                    this.handle = (uint)ped.Handle;
                    break;
                case Vehicle veh:
                    this.entity = new GTAVVehicle((uint)veh.Handle);
                    this.handle = (uint)veh.Handle;
                    break;
                case Prop prop:
                    this.entity = new GTAVProp((uint)prop.Handle);
                    this.handle = (uint)prop.Handle;
                    break;
                default:
                    throw new ArgumentException("Unsupported type, please use a different type and try again.");
            }
        }

        //support for GTAVPed
        public static implicit operator GTAVPed(GTAVEntity ent)
        {
            return new GTAVPed(ent.handle);
        }

        //support for GTAVVehicle
        public static implicit operator GTAVVehicle(GTAVEntity ent)
        {
            return new GTAVVehicle(ent.handle);
        }

        //support for GTAVProp
        public static implicit operator GTAVProp(GTAVEntity ent)
        {
            return new GTAVProp(ent.handle);
        }

        //support for SHVDNs Ped
        public static implicit operator Ped(GTAVEntity ent)
        {
            return new Ped((int)ent.handle);
        }

        //support for SHVDNs Vehicle
        public static implicit operator Vehicle(GTAVEntity ent)
        {
            return new Vehicle((int)ent.handle);
        }

        //support for SHVDNs Prop
        public static implicit operator Prop(GTAVEntity ent)
        {
            return new Prop((int)ent.handle);
        }

        //support for native calls
        public static implicit operator InputArgument(GTAVEntity ent)
        {
            return ent.handle;
        }
    }
    #endregion

    #region Core
    public class GTAV : Script
    {

        internal static GTAVPlayer internalplayer = new GTAVPlayer();
        internal static List<InternalMemIdentifier> memidentifiers = new List<InternalMemIdentifier>();

        /// <summary>
        /// The player.
        /// </summary>
        public static GTAVPlayer player
        {
            get
            {
                return internalplayer;
            }
        }

        /// <summary>
        /// The games global time scale.
        /// </summary>
        public static float timescale
        {
            set
            {
                Function.Call(GTAV.HexHashToNativeHash(0x1D408577D440E81E), value);
            }
        }

        /// <summary>
        /// The budget for peds for functions like GTAV.GetAllGTAVPeds, GTAV.GetNearbyGTAVPeds and GTAV.GetNearestGTAVPed (to reduce the performance hit)
        /// </summary>
        public static int gvtpedbudget = 2048;
        /// <summary>
        /// The budget for vehicles for functions like GTAV.GetAllGTAVVehicles, GTAV.GetNearbyGTAVVehicles and GTAV.GetNearestGTAVVehicle (to reduce the performance hit)
        /// </summary>
        public static int gvtvehbudget = 2048;

        public static bool init = false;

        /// <summary>
        /// Generates a hash key from a string to be used in things like spawning pedestrians, vehicles, etc.
        /// </summary>
        public static uint GenHashKey(string hashing)
        {
            return Function.Call<uint>(HexHashToNativeHashU(0xD24D37CC275948CC), hashing);
        }

        /// <summary>
        /// GTAVTools.GTAV
        /// </summary>
        public GTAV()
        {
            Tick += OnTick;
        }

        /// <summary>
        /// Tick loop.
        /// </summary>
        private static void OnTick(object sender, EventArgs e)
        {
            uint handle = Function.Call<uint>(GTAV.HexHashToNativeHashU(0xD80958FC74E988A6));
            if (internalplayer.character.handle != handle)
            {
                internalplayer = new GTAVPlayer();
            }
        }


        /// <summary>
        /// Converts a hexadecimal hash to a native hash.
        /// </summary>
        public static Hash HexHashToNativeHash(long hexhash)
        {
            return (Hash)hexhash;
        }


        /// <summary>
        /// Returns the entity coordinates from the entities handle.
        /// </summary>
        public static Vector3 GetEntityCoordsFromHandle(uint handle)
        {
            return Function.Call<Vector3>(GTAV.HexHashToNativeHash(0x3FEF770D40960D5A), handle, true);
        }

        /// <summary>
        /// Makes the screen fade out, "ms" is how long it should take to fully fade out in milliseconds.
        /// </summary>
        public static void FadeOut(int ms)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x891B5B39AC6302AF), ms);
        }

        /// <summary>
        /// Makes the screen fade in, "ms" is how long it should take to fully fade in in milliseconds.
        /// </summary>
        public static void FadeIn(int ms)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xD4E8E24955024033), ms);
        }

        /// <summary>
        /// Developer function, can be used elsewhere but is designed for the GTAVPlayer.Teleport function.
        /// </summary>
        public static Vector3 GetGroundPosForTPFunction(uint handle, Vector2 target, Vector3 ogpos)
        {
            float[] groundheights = //i took this from alexander blade's native trainer, i don't really know if i can take this but to my knowledge other mods use this aswell so eh
            {
                100f, 150f, 50f, 0f, 200f, 250f, 300f, 400f,
                500f, 600f, 700f, 800f
            };
            OutputArgument gz = new OutputArgument();
            foreach (float h in groundheights)
            {
                Function.Call(GTAV.HexHashToNativeHashU(0x239A3351AC1DA385), handle, target.X, target.Y, h, false, false, true);
                Script.Wait(100);
                if (Function.Call<bool>(GTAV.HexHashToNativeHashU(0xC906A7DAB05C8D2B), target.X, target.Y, h, gz, false, false))
                {
                    Function.Call(GTAV.HexHashToNativeHash(0x239A3351AC1DA385), handle, ogpos.X, ogpos.Y, ogpos.Z, false, false, false);
                    return new Vector3(target.X, target.Y, gz.GetResult<float>() + 3f);
                }
            }
            Function.Call(GTAV.HexHashToNativeHash(0x239A3351AC1DA385), handle, ogpos.X, ogpos.Y, ogpos.Z, false, false, false);
            return Vector3.Zero;
        }


        /// <summary>
        /// Sets an entities coordinates from it's handle.
        /// </summary>
        public static void SetEntityCoordsFromHandle(uint handle, Vector3 coords)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x239A3351AC1DA385), handle, coords.X, coords.Y, coords.Z, false, false, false);
        }


        /// <summary>
        /// Converts a hexidecimal ulong hash to a native hash.
        /// </summary>
        public static Hash HexHashToNativeHashU(ulong hexhash)
        {
            return (Hash)hexhash;
        }

        /// <summary>
        /// Runs SHVDN's World.GetAllPeds and then converts them all into a GTAVPed and returns the list.
        /// </summary>
        public static List<GTAVPed> GetAllGTAVPeds()
        {
            List<GTAVPed> fuck = new List<GTAVPed>();
            Ped[] shvdnpeds = World.GetAllPeds();
            foreach (Ped shvdnped in shvdnpeds)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnped.Exists())
                {
                    fuck.Add(new GTAVPed((uint)shvdnped.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Runs SHVDN's World.GetNearbyPeds and then converts them all into a GTAVPed and returns the list.
        /// </summary>
        public static List<GTAVPed> GetNearbyGTAVPeds(GTAVEntity entity, float radius)
        {
            List<GTAVPed> fuck = new List<GTAVPed>();
            Ped[] shvdnpeds = World.GetNearbyPeds(entity, radius);
            foreach (Ped shvdnped in shvdnpeds)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnped.Exists())
                {
                    fuck.Add(new GTAVPed((uint)shvdnped.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Runs SHVDN's World.GetNearbyPeds and then converts them all into a GTAVPed and returns the list.
        /// </summary>
        public static List<GTAVPed> GetNearbyGTAVPeds(Vector3 pos, float radius)
        {
            List<GTAVPed> fuck = new List<GTAVPed>();
            Ped[] shvdnpeds = World.GetNearbyPeds(pos, radius);
            foreach (Ped shvdnped in shvdnpeds)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnped.Exists())
                {
                    fuck.Add(new GTAVPed((uint)shvdnped.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Runs SHVDN's World.GetClosestPed and then converts the ped to a GTAVPed, if no ped is found it will return null
        /// </summary>
        public static GTAVPed GetNearestGTAVPed(Vector3 pos, float radius)
        {
            Ped shvdnped = World.GetClosestPed(pos, radius);
            if (shvdnped != null)
            {
                return new GTAVPed((uint)shvdnped.Handle);
            }
            return null;
        }

        /// <summary>
        /// Runs SHVDN's World.GetAllVehicles and then converts all the vehicles into a GTAVVehicle and returns the list.
        /// </summary>
        public static List<GTAVVehicle> GetAllGTAVVehicles()
        {
            List<GTAVVehicle> fuck = new List<GTAVVehicle>();
            Vehicle[] shvdnvehs = World.GetAllVehicles();
            foreach (Vehicle shvdnveh in shvdnvehs)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnveh.Exists())
                {
                    fuck.Add(new GTAVVehicle((uint)shvdnveh.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Reads a string category within an INI file.
        /// </summary>
        public static List<string> ReadCategoryOfINIFile(string path, string categoryname)
        {
            List<string> strings = new List<string>();
            string[] lines = File.ReadAllLines(path);
            bool inspecifiedsection = false;
            foreach (string rl in lines)
            {
                string line = rl.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//"))
                {
                    continue;
                }
                if (line.ToLower().Equals($"[{categoryname.ToLower()}]", StringComparison.OrdinalIgnoreCase))
                {
                    inspecifiedsection = true;
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]") && inspecifiedsection)
                {
                    //reached another
                    break;
                }
                if (inspecifiedsection)
                {
                    strings.Add(line);
                }
            }
            return strings;
        }

        /// <summary>
        /// Reads a INIBool category within an INI file.
        /// </summary>
        public static List<INIBool> ReadBoolCategoryOfINIFile(string path, string categoryname)
        {
            List<INIBool> bools = new List<INIBool>();
            string[] lines = File.ReadAllLines(path);
            bool inspecifiedsection = false;
            foreach (string rl in lines)
            {
                string line = rl.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//"))
                {
                    continue;
                }
                if (line.ToLower().Equals($"[{categoryname.ToLower()}]", StringComparison.OrdinalIgnoreCase))
                {
                    inspecifiedsection = true;
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]") && inspecifiedsection)
                {
                    break;
                }

                if (inspecifiedsection)
                {
                    string[] parts = line.Split(new char[] { '=' }, 2);
                    string key = parts[0].Trim();
                    bool value = false;
                    if (parts.Length > 1)
                    {
                        bool.TryParse(parts[1].Trim(), out value);
                    }
                    bools.Add(new INIBool(key, value));
                }
            }
            return bools;
        }


        /// <summary>
        /// Runs SHVDN's World.GetNearbyVehicles and then converts all of the vehicles into a GTAVVehicle and returns the list.
        /// </summary>
        public static List<GTAVVehicle> GetNearbyGTAVVehicles(GTAVEntity entity, float radius)
        {
            List<GTAVVehicle> fuck = new List<GTAVVehicle>();
            Vehicle[] shvdnvehs = World.GetNearbyVehicles(entity, radius);
            foreach (Vehicle shvdnveh in shvdnvehs)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnveh.Exists())
                {
                    fuck.Add(new GTAVVehicle((uint)shvdnveh.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Runs SHVDN's World.GetNearbyVehicles and then converts all of the vehicles into a GTAVVehicle and returns the list.
        /// </summary>
        public static List<GTAVVehicle> GetNearbyGTAVVehicles(Vector3 pos, float radius)
        {
            List<GTAVVehicle> fuck = new List<GTAVVehicle>();
            Vehicle[] shvdnvehs = World.GetNearbyVehicles(pos, radius);
            foreach (Vehicle shvdnveh in shvdnvehs)
            {
                if (fuck.Count >= gvtpedbudget)
                {
                    break;
                }
                if (shvdnveh.Exists())
                {
                    fuck.Add(new GTAVVehicle((uint)shvdnveh.Handle));
                }
            }
            fuck.TrimExcess();
            return fuck;
        }

        /// <summary>
        /// Runs SHVDN's World.GetClosestVehicle and then converts the vehicle to a GTAVVehicle, if no ped is found it will return null
        /// </summary>
        public static GTAVVehicle GetNearestGTAVVehicle(Vector3 pos, float radius)
        {
            Vehicle shvdnveh = World.GetClosestVehicle(pos, radius);
            if (shvdnveh != null)
            {
                return new GTAVVehicle((uint)shvdnveh.Handle);
            }
            return null;
        }

        /// <summary>
        /// Forwards the function MemScanner.FindPattern from GTAVMemScanner to let scripts using V2 of the SHVDN API still be able to scan for patterns.
        /// </summary>
        public static IntPtr FindPattern(string pattern, string mask)
        {
            return MemScanner.FindPattern(pattern, mask);
        }

        /// <summary>
        /// Forwards the function MemScanner.FindPatternWithStartAddr from GTAVMemScanner to let scripts using V2 of the SHVDN API still be able to scan for patterns.
        /// </summary>
        public static IntPtr FindPattern(string pattern, string mask, IntPtr start)
        {
            return MemScanner.FindPatternWithStartAddr(pattern, mask, start);
        }

        /// <summary>
        /// If entity 1 is touching entity 2.
        /// </summary>
        public static bool IsEntityTouchingEntity(uint entityhandle1, uint entityhandle2)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHash(0x17FFC1B2BA35A494), entityhandle1, entityhandle2);
        }

        /// <summary>
        /// Gets the utilization of the control with the priority layer.
        /// </summary>
        public static bool GetCtrlUtilization(ControlPriority priority, ControlUtilization util, Ctrl control)
        {
            switch (util)
            {
                case ControlUtilization.Pressed:
                    return Function.Call<bool>(GTAV.HexHashToNativeHashU(0xE2587F8CBBD87B1D), (int)priority, (int)control);
                case ControlUtilization.JustPressed:
                    return Function.Call<bool>(GTAV.HexHashToNativeHashU(0x91AEF906BCA88877), (int)priority, (int)control);
                case ControlUtilization.Released:
                    return Function.Call<bool>(GTAV.HexHashToNativeHashU(0xFB6C4072E9A32E92), (int)priority, (int)control);
                case ControlUtilization.JustReleased:
                    return Function.Call<bool>(GTAV.HexHashToNativeHashU(0x305C8DCD79DA8B0F), (int)priority, (int)control);
            }
            return false;
        }

        /// <summary>
        /// If the specified PTFX asset is loaded.
        /// </summary>
        public static bool IsPTFXAssetLoaded(string ptfx)
        {
            return Function.Call<bool>(GTAV.HexHashToNativeHashU(0x8702416E512EC454), ptfx);
        }

        /// <summary>
        /// Requests/loads the specified PTFX (Particle FX) asset.
        /// </summary>
        public static void RequestPTFXAsset(string ptfx)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xB80D8756B4668AB6), ptfx);
            while (!Function.Call<bool>(GTAV.HexHashToNativeHashU(0x8702416E512EC454), ptfx))
            {
                Script.Yield();
            }
        }

        /// <summary>
        /// Sets the PTFX asset to be used next call.
        /// </summary>
        public static void SetPTFXAssetForNextCall(string ptfx)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x6C38AF3693A69A91), ptfx); 
        }

        /// <summary>
        /// Starts PTFX (particle fx) at specific coordinates.
        /// </summary>
        public static int StartPTFXAtCoord(string ptfxasset, string ptfxname, Vector3 pos, Vector3 rot, float scale, bool loop)
        {
            if (!IsPTFXAssetLoaded(ptfxasset))
            {
                RequestPTFXAsset(ptfxasset);
            }
            SetPTFXAssetForNextCall(ptfxasset);
            if (!loop)
            {
                Function.Call(GTAV.HexHashToNativeHash(0x25129531F77B9ED3), ptfxname, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, scale, false, false, false);
            }
            else if (loop)
            {
                int looped = Function.Call<int>(GTAV.HexHashToNativeHashU(0xE184F4F0DC5910E7), ptfxname, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, scale, false, false, false, false);
                return looped;
            }
            return 0;
        }

        /// <summary>
        /// Starts PTFX (particle fx) on a bone on a specific ped.
        /// </summary>
        public static int StartPTFXOnPedBone(string ptfxasset, string ptfxname, GTAVPed ped, int boneindex, float scale, bool loop)
        {
            if (!IsPTFXAssetLoaded(ptfxasset))
            {
                RequestPTFXAsset(ptfxasset);
            }
            SetPTFXAssetForNextCall(ptfxasset);
            if (!loop)
            {
                Function.Call(GTAV.HexHashToNativeHash(0x0E7E72961BA18619), ptfxname, ped, 0f, 0f, 0f, 0f, 0f, 0f, boneindex, scale, false, false, false);
            }
            else if (loop)
            {
                int looped = Function.Call<int>(GTAV.HexHashToNativeHashU(0xF28DA9F38CD1787C), ptfxname, ped, 0f, 0f, 0f, 0f, 0f, 0f, boneindex, scale, false, false, false);
                return looped;
            }
            return 0;
        }

        /// <summary>
        /// Starts PTFX (particle fx) on a bone on a specific ped.
        /// </summary>
        public static int StartPTFXOnPedBone(string ptfxasset, string ptfxname, GTAVPed ped, Vector3 pos, Vector3 rot, int boneindex, float scale, bool loop)
        {
            if (!IsPTFXAssetLoaded(ptfxasset))
            {
                RequestPTFXAsset(ptfxasset);
            }
            SetPTFXAssetForNextCall(ptfxasset);
            if (!loop)
            {
                Function.Call(GTAV.HexHashToNativeHash(0x0E7E72961BA18619), ptfxname, ped, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, boneindex, scale, false, false, false);
            }
            else if (loop)
            {
                int looped = Function.Call<int>(GTAV.HexHashToNativeHashU(0xF28DA9F38CD1787C), ptfxname, ped, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, boneindex, scale, false, false, false);
                return looped;
            }
            return 0;
        }

        /// <summary>
        /// Starts PTFX (particle fx) on a specific entity.
        /// </summary>
        public static int StartPTFXOnEntity(string ptfxasset, string ptfxname, uint entityhandle, float scale, bool loop)
        {
            if (!IsPTFXAssetLoaded(ptfxasset))
            {
                RequestPTFXAsset(ptfxasset);
            }
            SetPTFXAssetForNextCall(ptfxasset);
            if (!loop)
            {
                Function.Call(GTAV.HexHashToNativeHash(0x0D53A3B8DA0809D2), ptfxname, entityhandle, 0f, 0f, 0f, 0f, 0f, 0f, scale, false, false, false);
            }
            else if (loop)
            {
                int looped = Function.Call<int>(GTAV.HexHashToNativeHashU(0x1AE42C1660FD6517), ptfxname, entityhandle, 0f, 0f, 0f, 0f, 0f, 0f, scale, false, false, false);
                return looped;
            }
            return 0;
        }

        /// <summary>
        /// Plays a sound within the frontend, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFrontend(int soundid, string audiobank, string audioname)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x67C540AA08E4A6F5), soundid, audioname, audiobank, true);
            return soundid;
        }

        /// <summary>
        /// Plays a sound within the frontend and returns its sound id.
        /// </summary>
        public static int PlaySoundFrontend(string audiobank, string audioname)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0x67C540AA08E4A6F5), id, audioname, audiobank, true);
            return id;
        }

        /// <summary>
        /// Plays a sound within the frontend, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFrontend(int soundid, int audiobank, string audioname)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x67C540AA08E4A6F5), soundid, audioname, audiobank, true);
            return soundid;
        }

        /// <summary>
        /// Plays a sound within the frontend and returns its sound id.
        /// </summary>
        public static int PlaySoundFrontend(int audiobank, string audioname)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0x67C540AA08E4A6F5), id, audioname, audiobank, true);
            return id;
        }

        /// <summary>
        /// Plays a sound on the specified entity, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFromEntity(int soundid, GTAVEntity entity, string audiobank, string audioname)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xE65F427EB70AB1ED), soundid, audioname, entity.handle, audiobank, false, 0);
            return soundid;
        }

        /// <summary>
        /// Plays a sound on the specified entity and returns its sound id.
        /// </summary>
        public static int PlaySoundFromEntity(GTAVEntity entity, string audiobank, string audioname)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0xE65F427EB70AB1ED), id, audioname, entity.handle, audiobank, false, 0);
            return id;
        }

        /// <summary>
        /// Plays a sound on the specified entity and returns its sound id.
        /// </summary>
        public static int PlaySoundFromEntity(GTAVEntity entity, int audiobank, string audioname)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0xE65F427EB70AB1ED), id, audioname, entity.handle, audiobank, false, 0);
            return id;
        }

        /// <summary>
        /// Plays a sound on the specified entity, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFromEntity(int soundid, GTAVEntity entity, int audiobank, string audioname)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xE65F427EB70AB1ED), soundid, audioname, entity.handle, audiobank, false, 0);
            return soundid;
        }

        /// <summary>
        /// Ends the specified sound from its sound id and releases the sound id so it can be used for other sounds.
        /// </summary>
        public static void EndSound(int id)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0xA3B0C41BA5CC0BB5), id);
            Function.Call(GTAV.HexHashToNativeHash(0x353FC880830B88FA), id);
        }

        /// <summary>
        /// Plays the specified sound at the given coordinates and returns its sound id.
        /// </summary>
        public static int PlaySoundFromCoord(string audiobank, string audioname, Vector3 pos, int range)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0x8D8686B622B88120), id, audioname, pos.X, pos.Y, pos.Z, audiobank, false, range, false);
            return id;
        }

        /// <summary>
        /// Plays the specified sound at the given coordinates, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFromCoord(int soundid, string audiobank, string audioname, Vector3 pos, int range)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x8D8686B622B88120), soundid, audioname, pos.X, pos.Y, pos.Z, audiobank, false, range, false);
            return soundid;
        }

        /// <summary>
        /// Plays the specified sound at the given coordinates, to get a sound id use the native 0x430386FE9BF80B45 (GET_SOUND_ID).
        /// </summary>
        public static int PlaySoundFromCoord(int soundid, int audiobank, string audioname, Vector3 pos, int range)
        {
            Function.Call(GTAV.HexHashToNativeHashU(0x8D8686B622B88120), soundid, audioname, pos.X, pos.Y, pos.Z, audiobank, false, range, false);
            return soundid;
        }

        /// <summary>
        /// Plays the specified sound at the given coordinates and returns its sound id.
        /// </summary>
        public static int PlaySoundFromCoord(int audiobank, string audioname, Vector3 pos, int range)
        {
            int id = Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
            Function.Call(GTAV.HexHashToNativeHashU(0x8D8686B622B88120), id, audioname, pos.X, pos.Y, pos.Z, audiobank, false, range, false);
            return id;
        }

        /// <summary>
        /// Updates the specified sound (from sound id) coords, only compatible with PlaySoundFromCoord.
        /// </summary>
        public static void UpdateSoundCoord(int id, Vector3 pos)
        {
            Function.Call(GTAV.HexHashToNativeHash(0x7EC3C679D0E7E46B), id, pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Returns a new sound id.
        /// </summary>
        public static int GetNewSoundId()
        {
            return Function.Call<int>(GTAV.HexHashToNativeHash(0x430386FE9BF80B45));
        }

        /// <summary>
        /// Creates a new memory patcher, once created it will immediately patch, to revert a patch use the Revert() function on the patch.
        /// </summary>
        public static MemoryPatch CreateNewMemoryPatch(IntPtr address, byte[] newbytes)
        {
            try
            {
                return new MemoryPatch(address, newbytes.Length, newbytes);
            }
            catch
            {
                throw new Exception("Illegal creation of MemoryPatch, the memory might be protected, corrupted or non-existent.");
            }
        }
    }

    ///// <summary>
    ///// Utilites for running native calls.
    ///// </summary>
    //public unsafe static class NativeUtils
    //{
    //    [SuppressUnmanagedCodeSecurity]
    //    [DllImport("ScriptHookV.dll", ExactSpelling = true, EntryPoint = "?nativeInit@@YAX_K@Z")]
    //    private static extern void NativeInit(ulong hash);
    //    [SuppressUnmanagedCodeSecurity]
    //    [DllImport("ScriptHookV.dll", ExactSpelling = true, EntryPoint = "?nativePush64@@YAX_K@Z")]
    //    private static extern void NativePush64(ulong val);
    //    [SuppressUnmanagedCodeSecurity]
    //    [DllImport("ScriptHookV.dll", ExactSpelling = true, EntryPoint = "?nativeCall@@YAPEA_KXZ")]
    //    private static extern ulong* NativeCall();

    //    /// <summary>
    //    /// Converts a string into a pointer.
    //    /// </summary>
    //    internal static unsafe IntPtr String(string str)
    //    {
    //        byte[] utf8 = Encoding.UTF8.GetBytes(str);
    //        IntPtr ptr = Marshal.AllocCoTaskMem(utf8.Length + 1);
    //        if (ptr == IntPtr.Zero)
    //        {
    //            throw new Exception("Out of game memory.");
    //        }
    //        Marshal.Copy(utf8, 0, ptr, utf8.Length);
    //        byte* pointer = (byte*)ptr.ToPointer();
    //        pointer[utf8.Length] = 0;
    //        return ptr;
    //    }

    //    /// <summary>
    //    /// Pins a string.
    //    /// </summary>
    //    internal static IntPtr PinString(string str)
    //    {
    //        IntPtr handle = String(str);
    //        return handle == IntPtr.Zero ? String("") : handle;
    //    }

    //    /// <summary>
    //    /// Runs a native with the specified arguments.
    //    /// </summary>
    //    public static void Execute(ulong hash, params object[] args)
    //    {
    //        NativeInit(hash);
    //        for (int i = args.Length - 1; i >= 0; i--)
    //        {
    //            NativePush64(new NativeArg(args[i]).value);
    //        }
    //        NativeCall();
    //    }
    //}

    ///// <summary>
    ///// A native argument.
    ///// </summary>
    //public class NativeArg
    //{
    //    public ulong value { get; }

    //    public NativeArg(object arg)
    //    {
    //        switch (arg)
    //        {
    //            case int i:
    //                value = (ulong)i;
    //                break;
    //            case uint ui:
    //                value = ui;
    //                break;
    //            case bool b:
    //                value = b ? 1UL : 0UL;
    //                break;
    //            case float f:
    //                value = BitConverter.ToUInt32(BitConverter.GetBytes(f), 0);
    //                break;
    //            case double d:
    //                value = BitConverter.ToUInt64(BitConverter.GetBytes(d), 0);
    //                break;
    //            case IntPtr ptr:
    //                value = (ulong)ptr.ToInt64();
    //                break;
    //            case string s:
    //                value = (ulong)NativeUtils.PinString(s).ToInt64();
    //                break;
    //            case Enum e:
    //                value = Convert.ToUInt64(e);
    //                break;
    //            default:
    //                throw new Exception($"Unsupported argument on native: {arg.GetType()}");
    //        }
    //    }
    //}
    #endregion
}
