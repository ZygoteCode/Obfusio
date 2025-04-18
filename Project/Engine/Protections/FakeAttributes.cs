﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfusio.Engine.Protections
{
    public class FakeAttributes
    {
        private static string[] _attributes = new string[]
        {
"YanoAttribute",
"Xenocode.Client.Attributes.AssemblyAttributes.ProcessedByXenocode",
"PoweredByAttribute",
"ObfuscatedByGoliath",
"NineRays.Obfuscator.Evaluation",
"NetGuard",
"dotNetProtector",
"DotNetPatcherPackerAttribute",
"DotNetPatcherObfuscatorAttribute",
"DotfuscatorAttribute",
"CryptoObfuscator.ProtectedWithCryptoObfuscatorAttribute",
"ProtectedWithCryptoObfuscatorAttribute",
"ProcessedByXenocode",
"BabelObfuscatorAttribute",
"BabelAttribute",
"AssemblyInfoAttribute",
"ConfusedByAttribute",
"();\t",
"EMyPID_8234_",
"ZYXDNGuarder",
"SecureTeam.Attributes.ObfuscatedByAgileDotNetAttribute",
"SmartAssembly.Attributes.PoweredByAttribute",
"Protected",
"&lt;ObfuscatedByDotNetPatcher&gt;",
"OrangeHeapAttribute",
".NETGuard",
"NineRays.Obfuscator.SoftwareWatermarkAttribute",
"SecureTeam.Attributes.ObfuscatedByCliSecureAttribute",
"Reactor",
"CryptoObfuscator",
"WTF",
"OiCuntJollyGoodDayYeHavin_____________________________________________________",
"OiCuntJollyGoodDayYeHavin",
"Borland.Vcl.Types",
"Borland.Eco.Interfaces",
"Oliviay",
"____KILL",
"CheckRuntime",
"Sixxpack",
"CodeWallTrialVersion",
"Protected_By_Attribute'00'NETSpider.Attribute",
"SkaterDotNetObfuscatorAttribute",
"SecureTeam.DotNetObfuscator",
"MaxtoCode",
"DeepSea.Obfuscation.ObfuscationAttribute",
"PhoenixProtectorAttribute",
"AssemblyTitle_Obfuscated",
"Eziriz.NetReactor",
"Eziriz.NetReactorAttribute",
"DotNetGuard.ObfuscationAttribute",
"CodeVeilObfuscationAttribute",
"ILProtector",
"ILProtectorAttribute",
"Obfusasm",
"BabelNet",
"VanguardObfuscator",
"XHEO.Deploy.ObfuscatedAttribute",
"Rummage",
"Xenocode.Client.Attributes.Obfuscated",
"yckDNGuard",
"Netz",
"ConfuserAttribute",
"ConfuserEx",
"Confuser",
"Confuser.ConfusedByAttribute",
"SecureTeam.Obfuscation.ObfuscatedByCliSecureAttribute",
"Yano",
"Eazfuscator.NET",
"Eazfuscator.NETAttribute",
"AssemblyObfuscationAttribute",
"AssemblyObfuscatedAttribute",
"AssemblySecureAttribute",
"ObfuscateAssemblyAttribute",
"AssemblyProtectionAttribute",
"OpoDotNetProtector",
"MSILDisabler",
"SecureDotNet",
"ObfuscationLevel",
"DotNetObfuscator",
"BinaryBoy.Obfuscator",
"BlackHoleObfuscator",
"BoosterAttribute",
"CrazyBoyObfuscator",
"DotNetShielder",
"DotMeltObfuscator",
"SecureTeam.NetObfuscator",
"RunPEAttribute",
"CSharpObfuscatorAttribute",
"AssemblyScrambler",
"ConfusionAttribute",
"DarkObfuscator",
"RaxorProtectorAttribute",
"Dofuscator",
"UnknownObfuscatorAttribute",
"UnknownProtectionAttribute",
"AxProtectorAttribute",
"CodeMeterAttribute",
"CodeMeterLicenseAttribute",
"CmDongleAttribute",
"CmCloudContainerAttribute",
"AgileDotNet.ObfuscatedByAgileAttribute",
"Spices.ObfuscatedBySpicesAttribute",
"CodeVeil.ObfuscationAttribute",
"VMProtect.ProtectedByVMProtectAttribute",
"EnigmaProtectedAttribute",
"BitHelmet.ObfuscatorAttribute",
"IntelliLock.LockedAttribute",
"CodeArmor.ShieldedAttribute",
"CodeFort.BastionAttribute",
"DotNetShield.ProtectedAttribute",
"Zydn.ShieldAttribute",
"SecureTeam.SecureDotNetAttribute",
"DotNetObfuscator.SealedAttribute",
"AssemblyDefender.EncryptedAttribute",
"CryptoObfuscator.MethodScramblerAttribute",
"Dotfuscator.AntiTamperAttribute",
"Eazfuscator.StringsMangledAttribute",
"NetSecure.CloakedAttribute",
"GuardIT.SecurityAttribute",
"NCloak.ObfuscationLayerAttribute",
"Obfuscar.GeneratedAttribute",
"Rethink.SecurityByObscurityAttribute",
"Salamander.TransformedAttribute",
"SmartAssembly.DynamicCodeAttribute",
"Xenocode.VirtualizedAttribute",
"Z3nus.ProtectedModuleAttribute",
"CodeWall.LicensedAttribute",
"DotNetProtector.RuntimeShieldAttribute",
"Morph.TransmutedAttribute",
"NProtect.StringEncryptionAttribute",
"QuickPack.CompressedILAttribute",
"Stunnix.NativeWrapperAttribute",
"Telerik.FakeObfuscatorAttribute",
"WinLicense.NativeProtectionAttribute",
"KoiVM.VirtualizedCodeAttribute",
"LiquidObfuscator.FluidCodeAttribute",
"Scramble.ControlFlowAttribute",
"VProtect.VirtualizationLayerAttribute",
"Babel.StringsHashedAttribute",
"DotNetReactor.SecuredAttribute",
"Skater.Net.ObfuscatedMethodAttribute",
"Goliath.AntiDumpAttribute",
"Xheo.CodeWallAttribute",
"Rummage.ScrambledMetadataAttribute",
"MaxtoCode.HardenedAttribute",
"ZLock.LicenseValidatorAttribute",
"DeepSea.Obfuscation.ControlFlowAttribute",
"Phoenix.RenamedAttributesAttribute",
"ObfuscationExcludeAttribute",
"ObfuscationIncludeAttribute",
"Yano.StringEncryptionAttribute",
"ZYXDNGuarder.HVMAttribute",
"NineRays.ILProtectionAttribute",
"Netz.CompressedAttribute",
"RedGate.SmartAssemblyAttribute",
"JSMin.MinifiedAttribute",
"CliSecure.AntiILDasmAttribute",
"Dotfuscator.Runtime.ChecksAttribute",
"Eazfuscator.NamespaceManglingAttribute",
"CryptoObfuscator.ResourceProtectionAttribute",
"Babel.MethodMergeAttribute",
"DotNetReactor.NativeEXEAttribute",
"Spices.Net.AntiReflectorAttribute",
"CodeVeil.TypeCollapsingAttribute",
"AgileDotNet.ControlFlowAttribute",
"Xenocode.FakePEFlagsAttribute",
"ILProtector.EncryptedMethodsAttribute",
"Zydn.StringsToNativeAttribute",
"Rotorz.MetadataBlindAttribute",
"CodeArmor.AntiDecompilerAttribute",
"Thumper.AntiDebugAttribute",
"Mpress.PackedDataAttribute",
"Themida.FakeSectionAttribute",
"Enigma.FakeDebugDataAttribute",
"ArmDot.MethodJittingAttribute",
"CodeFort.FakeExceptionsAttribute",
"DotNetShield.AntiTamperingAttribute",
"NecroBit.DeadCodeAttribute",
"CodeMunger.RenamedMembersAttribute",
"JumboObfuscator.FakeExportsAttribute",
"Krypton.EncryptedStringsAttribute",
"LockBox.CodeIntegrityAttribute",
"Mystic.OpaquePredicatesAttribute",
"NinjaObfuscator.StealthModeAttribute",
"Phantom.InvisibleCodeAttribute",
"QuantumObfuscator.EntangledCodeAttribute",
"TitanShield.AntiDisassemblyAttribute",
"Vanish.CodeEphemeralAttribute",
"Zircon.HardenedRuntimeAttribute",
"BlackBox.UndocumentedBehaviorAttribute",
"DarkShield.StealthAttributesAttribute",
"FrostObfuscator.ColdCodeAttribute",
"GuardianPro.RuntimeProtectionAttribute",
"IronWall.CodeIntegrityAttribute",
"JShield.JitProtectionAttribute",
"Lunar.EclipseModeAttribute",
"MirageShield.IllusionCodeAttribute",
"Nightshade.DarkCodeAttribute",
"ObfuscatePro.CommercialVersionAttribute",
"PinnacleGuard.SecureExecutionAttribute",
"RavenShield.AntiMemoryDumpAttribute",
"SteelObfuscator.TamperProofAttribute",
"UmbraGuard.ShadowExecutionAttribute",
"VortexObfuscated.SpiralCodeAttribute",
"Whiteout.ErasableCodeAttribute",
"XShield.CrossProtectionAttribute",
"Yggdrasil.WorldTreeProtectionAttribute",
"ZeroShield.UnbreakableCodeAttribute",
"AlphaObfuscator.PrimaryProtectionAttribute",
"BetaShield.ExperimentalProtectionAttribute",
"GammaGuard.RadiationShieldAttribute",
"DeltaObfuscated.StreamlinedCodeAttribute",
"EpsilonShield.AdvancedSecurityAttribute",
"ZetaGuard.FinalDefenseAttribute",
"IotaGuard.MicroProtectionAttribute",
"KappaObfuscated.CommunityEditionAttribute",
"LambdaShield.WaveProtectionAttribute",
"MuObfuscated.HiddenPatternsAttribute",
"NuGuard.NewProtectionAttribute",
"XiObfuscated.AdvancedFeaturesAttribute",
"OmicronShield.VariantProtectionAttribute",
"PiGuard.CircularProtectionAttribute",
"RhoObfuscator.DensityAttribute",
"SigmaShield.StatisticalProtectionAttribute",
"TauGuard.PersistenceAttribute",
"UpsilonObfuscated.SpecialEditionAttribute",
"PhiShield.GoldenRatioProtectionAttribute",
"ChiGuard.EnergyProtectionAttribute",
"PsiObfuscator.ParapsychologyAttribute",
"OmegaShield.UltimateProtectionAttribute",
"X9$kF!zP_AntiTamper",
"__CodeArmorV2_InternalProtection__",
"|0|\u2665|\u2605|OBFUSCATED|\u2605|\u2665|0|",
"\u202eRightToLeftAttribute\u202c",
"_____________ANTI_CRACK_____________",
"{{ThisIsNotTheRealObfuscator}}",
"[EmbeddedMalwareProtection]",
"&lt;&gt;c__ObfuscatedType",
"#=q3QU4Gm$j1p7Z8Z9w1xDw==#",
"\\uCAFE\\uBABE\\uDEAD\\uBEEF",
"UPX.CompressedPEAttribute",
"MPRESS.PackedSectionAttribute",
"ASPack.EncryptedResourcesAttribute",
"VMProtect.UltraModeAttribute",
"Enigma.FSDProtectionAttribute",
"NSA.Backdoor.QuantumInsertAttribute",
"CIA.LANGLEY.SPECIALOPS",
"GRU.MilitaryGradeEncryption",
"Unit_61398.ChinaObfuscation",
"Microsoft.VisualStudio.SpecialObfuscation",
"System.Security.Cryptography.HiddenAlgorithm",
"Windows.Win32.SecureCode.Internal",
"AntiSandbox.EnabledAttribute",
"AntiVM.HypervisorDetection",
"AntiDebug.SecureModeAttribute",
"AntiDump.MemoryProtection",
"CopyrightEvasion.Technique12",
"PiratedCopy.DoNotDistribute",
"Unlicensed.WarningAttribute",
"TrialVersion.ExpiredAttribute"
        };

        public static void Process(ModuleDefMD module)
        {
            foreach (string attribute in _attributes)
            {
                TypeRef typeRef = module.Assembly.ManifestModule.CorLibTypes.GetTypeRef("System", "Attribute");
                TypeDefUser typeDefUser = new TypeDefUser("", attribute, typeRef);
                module.Assembly.ManifestModule.Types.Add(typeDefUser);
                MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(module.Assembly.ManifestModule.CorLibTypes.Void, module.Assembly.ManifestModule.CorLibTypes.String), 0, MethodAttributes.Public);
                methodDefUser.Body = new CilBody();
                methodDefUser.Body.MaxStack = 1;
                methodDefUser.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(module.Assembly.ManifestModule, ".ctor", MethodSig.CreateInstance(module.Assembly.ManifestModule.CorLibTypes.Void), typeRef)));
                methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
                typeDefUser.Methods.Add(methodDefUser);
                CustomAttribute customAttribute = new CustomAttribute(methodDefUser);
                customAttribute.ConstructorArguments.Add(new CAArgument(module.Assembly.ManifestModule.CorLibTypes.String, ""));
                module.Assembly.ManifestModule.CustomAttributes.Add(customAttribute);
            }
        }
    }
}