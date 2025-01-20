using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;
using WT;

//This is a C# comment. Comments have no impact on compilation.

//ModName, ModVersion, ModAuthor, and ModNamespace.ModClassInheritingFromMelonMod all need changed.

[assembly: AssemblyTitle("WaltzToolz")]
[assembly: AssemblyCopyright("Created by Waltz")]

//Version numbers in C# are a set of 1 to 4 positive integers separated by periods.
//Mods typically use 3 numbers. For example: 1.2.1
//The mod version need specified in three places.
[assembly: AssemblyVersion("0")]
[assembly: AssemblyFileVersion("0")]
[assembly: MelonInfo(typeof(WT.Main), "WaltzToolz", "0", "Waltz")]
[assembly: MelonOptionalDependencies("UnityExplorerTLD", "UnityExplorer.TLD", "UnityExplorer.ML.IL2CPP.CoreCLR", "UniverseLib.ML.IL2CPP.Interop", "UniverseLib.IL2CPP.Interop.ML")]

//This tells MelonLoader that the mod is only for The Long Dark.
[assembly: MelonGame("Hinterland", "TheLongDark")]