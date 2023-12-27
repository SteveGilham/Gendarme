
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.


// ----- DOS Header:
// Magic:                      0x5a4d
// Bytes on last page:         0x0090
// Pages in file:              0x0003
// Relocations:                0x0000
// Size of header (paragraphs):0x0004
// Min extra paragraphs:       0x0000
// Max extra paragraphs:       0xffff
// Initial (relative) SS:      0x0000
// Initial SP:                 0x00b8
// Checksum:                   0x0000
// Initial IP:                 0x0000
// Initial (relative) CS:      0x0000
// File addr. of reloc table:  0x0040
// Overlay number:             0x0000
// OEM identifier:             0x0000
// OEM info:                   0x0000
// File addr. of COFF header:  0x0080
// ----- COFF/PE Headers:
// Signature:                  0x00004550
// ----- COFF Header:
// Machine:                    0x014c
// Number of sections:         0x0003
// Time-date stamp:            0x8606f957
// Ptr to symbol table:        0x00000000
// Number of symbols:          0x00000000
// Size of optional header:    0x00e0
// Characteristics:            0x2022
// ----- PE Optional Header (32 bit):
// Magic:                          0x010b
// Major linker version:           0x30
// Minor linker version:           0x00
// Size of code:                   0x00003c00
// Size of init.data:              0x00000600
// Size of uninit.data:            0x00000000
// Addr. of entry point:           0x00005afe
// Base of code:                   0x00002000
// Base of data:                   0x00006000
// Image base:                     0x10000000
// Section alignment:              0x00002000
// File alignment:                 0x00000200
// Major OS version:               0x0004
// Minor OS version:               0x0000
// Major image version:            0x0000
// Minor image version:            0x0000
// Major subsystem version:        0x0004
// Minor subsystem version:        0x0000
// Size of image:                  0x0000a000
// Size of headers:                0x00000200
// Checksum:                       0x00000000
// Subsystem:                      0x0003
// DLL characteristics:            0x8560
// Size of stack reserve:          0x00100000
// Size of stack commit:           0x00001000
// Size of heap reserve:           0x00100000
// Size of heap commit:            0x00001000
// Loader flags:                   0x00000000
// Directories:                    0x00000010
// 0x00000000 [0x00000000] address [size] of Export Directory:          
// 0x00005aac [0x0000004f] address [size] of Import Directory:          
// 0x00006000 [0x00000330] address [size] of Resource Directory:        
// 0x00000000 [0x00000000] address [size] of Exception Directory:       
// 0x00000000 [0x00000000] address [size] of Security Directory:        
// 0x00008000 [0x0000000c] address [size] of Base Relocation Table:     
// 0x000059c0 [0x00000054] address [size] of Debug Directory:           
// 0x00000000 [0x00000000] address [size] of Architecture Specific:     
// 0x00000000 [0x00000000] address [size] of Global Pointer:            
// 0x00000000 [0x00000000] address [size] of TLS Directory:             
// 0x00000000 [0x00000000] address [size] of Load Config Directory:     
// 0x00000000 [0x00000000] address [size] of Bound Import Directory:    
// 0x00002000 [0x00000008] address [size] of Import Address Table:      
// 0x00000000 [0x00000000] address [size] of Delay Load IAT:            
// 0x00002008 [0x00000048] address [size] of CLR Header:                


// Image sections:
//              .text
//              0x00003b04 Virtual Size
//              0x00002000 Virtual Address
//              0x00003c00 Size of Raw Data
//              0x00000200 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x60000020 Characteristics
//                         CNT_CODE
//                         MEM_EXECUTE
//                         MEM_READ

//              .rsrc
//              0x00000330 Virtual Size
//              0x00006000 Virtual Address
//              0x00000400 Size of Raw Data
//              0x00003e00 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x40000040 Characteristics
//                         CNT_INITIALIZED_DATA
//                         MEM_READ

//              .reloc
//              0x0000000c Virtual Size
//              0x00008000 Virtual Address
//              0x00000200 Size of Raw Data
//              0x00004200 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x42000040 Characteristics
//                         CNT_INITIALIZED_DATA
//                         MEM_DISCARDABLE
//                         MEM_READ

// Base Relocation Table
//              0x00005000 Page RVA
//              0x0000000c Block Size
//              0x00000002 Number of Entries
//              Entry 1: Type 0x3 Offset 0x00000b00
//              Entry 2: Type 0x0 Offset 0x00000000

// Import Address Table
//     DLL : mscoree.dll
//              0x00002000 Import Address Table
//              0x00005aee Import Name Table
//              0          Time Date Stamp
//              0          Index of First Forwarder Reference
//
//              0x0000  _CorDllMain

// Delay Load Import Address Table
// No data.

// Entry point code:
//FF 25 00 20 00 10 


// ----- CLR Header:
// Header size:                        0x00000048
// Major runtime version:              0x0002
// Minor runtime version:              0x0005
// 0x000038bc [0x00002104] address [size] of Metadata Directory:        
// Flags:                              0x00000001
// Entry point token:                  0x00000000
// 0x00000000 [0x00000000] address [size] of Resources Directory:       
// 0x00000000 [0x00000000] address [size] of Strong Name Signature:     
// 0x00000000 [0x00000000] address [size] of CodeManager Table:         
// 0x00000000 [0x00000000] address [size] of VTableFixups Directory:    
// 0x00000000 [0x00000000] address [size] of Export Address Table:      
// 0x00000000 [0x00000000] address [size] of Precompile Header:         

// Metadata Header
//    Storage Signature:
//              0x424a5342 Signature
//                  0x0001 Major Version
//                  0x0001 Minor Version
//              0x00000000 Extra Data Offset
//              0x0000000c Version String Length
//              'v4.0.30319' Version String
//    Storage Header:
//                    0x00 Flags
//                  0x0005 Number of Streams
//    Stream 1:
//              0x0000006c Offset
//              0x00000d70 Size
//              '#~' Name
//    Stream 2:
//              0x00000ddc Offset
//              0x00000888 Size
//              '#Strings' Name
//    Stream 3:
//              0x00001664 Offset
//              0x000003e8 Size
//              '#US' Name
//    Stream 4:
//              0x00001a4c Offset
//              0x00000010 Size
//              '#GUID' Name
//    Stream 5:
//              0x00001a5c Offset
//              0x000006a8 Size
//              '#Blob' Name

//    Metadata Stream Header:
//              0x00000000 Reserved
//                    0x02 Major
//                    0x00 Minor
//                    0x00 Heaps
//                    0x01 Rid
//      0x00001e090ba21757 MaskValid
//      0x000016003301fa00 Sorted

// Code Manager Table:
//  default


// Export Address Table Jumps:
// No data.

// File size            : 17408
// PE header size       : 512 (496 used)    ( 2.94%)
// PE additional info   : 1071              ( 6.15%)
// Num.of PE sections   : 3
// CLR header size     : 72                 ( 0.41%)
// CLR meta-data size  : 8452               (48.55%)
// CLR additional info : 0                  ( 0.00%)
// CLR method headers  : 979                ( 5.62%)
// Managed code         : 5234              (30.07%)
// Data                 : 1536              ( 8.82%)
// Unaccounted          : -448              (-2.57%)

// Num.of PE sections   : 3
//   .text    - 15360
//   .rsrc    - 1024
//   .reloc   - 512

// CLR meta-data size  : 8452
//   Module        -    1 (10 bytes)
//   TypeDef       -   12 (168 bytes)     0 interfaces, 0 explicit layout
//   TypeRef       -   49 (294 bytes)
//   MethodDef     -   65 (910 bytes)     0 abstract, 0 native, 61 bodies
//   FieldDef      -   11 (66 bytes)      1 constant
//   MemberRef     -   87 (522 bytes)
//   ParamDef      -   66 (396 bytes)
//   MethodImpl    -    2 (12 bytes)
//   CustomAttribute-   67 (402 bytes)
//   StandAloneSig -   38 (76 bytes)
//   InterfaceImpl -    6 (24 bytes)
//   PropertyMap   -    2 (8 bytes)
//   Property      -    6 (36 bytes)
//   MethodSemantic-    9 (54 bytes)
//   TypeSpec      -   29 (58 bytes)
//   Assembly      -    1 (22 bytes)
//   AssemblyRef   -    2 (40 bytes)
//   NestedClass   -    3 (12 bytes)
//   GenericParam  -   21 (168 bytes)
//   MethodSpec    -    6 (24 bytes)
//   GenericParamConstraint-    7 (28 bytes)
//   Strings       -  2184 bytes
//   Blobs         -  1704 bytes
//   UserStrings   -  1000 bytes
//   Guids         -    16 bytes
//   Uncategorized -   218 bytes

// CLR method headers : 979
//   Num.of method bodies  - 61
//   Num.of fat headers    - 48
//   Num.of tiny headers   - 13
//   Num.of fat sections   - 0
//   Num.of small sections - 19

// Managed code : 5234
//   Ave method size - 85
// Classes defined in this module:
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Class EmbeddedAttribute              (auto) (ansi) (sealed) 
// Class NullableAttribute              (auto) (ansi) (sealed) 
// Class NullableContextAttribute       (auto) (ansi) (sealed) 
// Class RefSafetyRulesAttribute        (auto) (ansi) (sealed) 
// Class CollectionTools                (abstract) (auto) (ansi) (sealed) 
// Class MiscTools                      (auto) (ansi) 
// Class StringBuilderPool              (auto) (ansi) 
// Class NonEmptyCollection`1           (auto) (ansi) (sealed) 
// Class SerializeDelegate              (auto) (ansi) (sealed) (nested public) 
// Class Enumerator                     (auto) (ansi) (sealed) (nested private) 
// Class <>c                            (auto) (ansi) (sealed) (nested private) 
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


// Metadata version: v4.0.30319
.assembly extern /*23000001*/ System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
}
.assembly extern /*23000002*/ System.Collections
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
}
.assembly /*20000001*/ 'Source Code'
{
  .custom /*0C000003:0A000001*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.CompilationRelaxationsAttribute/*01000001*/::.ctor(int32) /* 0A000001 */ = ( 01 00 08 00 00 00 00 00 ) 
  .custom /*0C000004:0A000002*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute/*01000002*/::.ctor() /* 0A000002 */ = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                                                                                63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom /*0C000005:0A000003*/ instance void [System.Runtime/*23000001*/]System.Diagnostics.DebuggableAttribute/*01000003*/::.ctor(valuetype [System.Runtime/*23000001*/]System.Diagnostics.DebuggableAttribute/*01000003*//DebuggingModes/*01000004*/) /* 0A000003 */ = ( 01 00 07 01 00 00 00 00 ) 

  .custom /*0C000006:0A000004*/ instance void [System.Runtime/*23000001*/]System.Runtime.Versioning.TargetFrameworkAttribute/*01000005*/::.ctor(string) /* 0A000004 */ = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                                                                                           65 72 73 69 6F 6E 3D 76 37 2E 30 01 00 54 0E 14   // ersion=v7.0..T..
                                                                                                                                                                           46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79   // FrameworkDisplay
                                                                                                                                                                           4E 61 6D 65 08 2E 4E 45 54 20 37 2E 30 )          // Name..NET 7.0
  .custom /*0C000007:0A000005*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyCompanyAttribute/*01000006*/::.ctor(string) /* 0A000005 */ = ( 01 00 0B 53 6F 75 72 63 65 20 43 6F 64 65 00 00 ) // ...Source Code..
  .custom /*0C000008:0A000006*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyConfigurationAttribute/*01000007*/::.ctor(string) /* 0A000006 */ = ( 01 00 05 44 65 62 75 67 00 00 )                   // ...Debug..
  .custom /*0C000009:0A000007*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyFileVersionAttribute/*01000008*/::.ctor(string) /* 0A000007 */ = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom /*0C00000A:0A000008*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyInformationalVersionAttribute/*01000009*/::.ctor(string) /* 0A000008 */ = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .custom /*0C00000B:0A000009*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyProductAttribute/*0100000A*/::.ctor(string) /* 0A000009 */ = ( 01 00 0B 53 6F 75 72 63 65 20 43 6F 64 65 00 00 ) // ...Source Code..
  .custom /*0C00000C:0A00000A*/ instance void [System.Runtime/*23000001*/]System.Reflection.AssemblyTitleAttribute/*0100000B*/::.ctor(string) /* 0A00000A */ = ( 01 00 0B 53 6F 75 72 63 65 20 43 6F 64 65 00 00 ) // ...Source Code..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module 'Source Code.dll'
// MVID: {70C9CF6B-8CB2-49DF-8787-56336DE26A8C}
.custom /*0C000002:06000005*/ instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute/*02000005*/::.ctor(int32) /* 06000005 */ = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000173E6040000


// =============== CLASS MEMBERS DECLARATION ===================

.class /*02000002*/ private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [System.Runtime/*23000001*/]System.Attribute/*0100000D*/
{
  .custom /*0C00000E:0A00000B*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.CompilerGeneratedAttribute/*0100000C*/::.ctor() /* 0A00000B */ = ( 01 00 00 00 ) 
  .custom /*0C00000F:06000001*/ instance void Microsoft.CodeAnalysis.EmbeddedAttribute/*02000002*/::.ctor() /* 06000001 */ = ( 01 00 00 00 ) 
  .method /*06000001*/ public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  // SIG: 20 00 01
  {
    // Method begins at RVA 0x2050
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)000010       */ call       instance void [System.Runtime/*23000001*/]System.Attribute/*0100000D*/::.ctor() /* 0A000010 */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 2A   |                  */ ret
  } // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class /*02000003*/ private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.NullableAttribute
       extends [System.Runtime/*23000001*/]System.Attribute/*0100000D*/
{
  .custom /*0C000012:0A00000B*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.CompilerGeneratedAttribute/*0100000C*/::.ctor() /* 0A00000B */ = ( 01 00 00 00 ) 
  .custom /*0C000013:06000001*/ instance void Microsoft.CodeAnalysis.EmbeddedAttribute/*02000002*/::.ctor() /* 06000001 */ = ( 01 00 00 00 ) 
  .custom /*0C000014:0A00000C*/ instance void [System.Runtime/*23000001*/]System.AttributeUsageAttribute/*0100000F*/::.ctor(valuetype [System.Runtime/*23000001*/]System.AttributeTargets/*0100000E*/) /* 0A00000C */ = ( 01 00 84 6B 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ...k....T..Allow
                                                                                                                                                                                                                          4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                                                                                                                          72 69 74 65 64 00 )                               // rited.
  .field /*04000001*/ public initonly uint8[] NullableFlags
  .method /*06000002*/ public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8 A_1) cil managed
  // SIG: 20 01 01 05
  {
    // Method begins at RVA 0x2059
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)000010       */ call       instance void [System.Runtime/*23000001*/]System.Attribute/*0100000D*/::.ctor() /* 0A000010 */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 02   |                  */ ldarg.0
    IL_0008:  /* 17   |                  */ ldc.i4.1
    IL_0009:  /* 8D   | (01)000028       */ newarr     [System.Runtime/*23000001*/]System.Byte/*01000028*/
    IL_000e:  /* 25   |                  */ dup
    IL_000f:  /* 16   |                  */ ldc.i4.0
    IL_0010:  /* 03   |                  */ ldarg.1
    IL_0011:  /* 9C   |                  */ stelem.i1
    IL_0012:  /* 7D   | (04)000001       */ stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute/*02000003*/::NullableFlags /* 04000001 */
    IL_0017:  /* 2A   |                  */ ret
  } // end of method NullableAttribute::.ctor

  .method /*06000003*/ public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8[] A_1) cil managed
  // SIG: 20 01 01 1D 05
  {
    // Method begins at RVA 0x2072
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)000010       */ call       instance void [System.Runtime/*23000001*/]System.Attribute/*0100000D*/::.ctor() /* 0A000010 */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 02   |                  */ ldarg.0
    IL_0008:  /* 03   |                  */ ldarg.1
    IL_0009:  /* 7D   | (04)000001       */ stfld      uint8[] System.Runtime.CompilerServices.NullableAttribute/*02000003*/::NullableFlags /* 04000001 */
    IL_000e:  /* 2A   |                  */ ret
  } // end of method NullableAttribute::.ctor

} // end of class System.Runtime.CompilerServices.NullableAttribute

.class /*02000004*/ private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.NullableContextAttribute
       extends [System.Runtime/*23000001*/]System.Attribute/*0100000D*/
{
  .custom /*0C000017:0A00000B*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.CompilerGeneratedAttribute/*0100000C*/::.ctor() /* 0A00000B */ = ( 01 00 00 00 ) 
  .custom /*0C000018:06000001*/ instance void Microsoft.CodeAnalysis.EmbeddedAttribute/*02000002*/::.ctor() /* 06000001 */ = ( 01 00 00 00 ) 
  .custom /*0C000019:0A00000C*/ instance void [System.Runtime/*23000001*/]System.AttributeUsageAttribute/*0100000F*/::.ctor(valuetype [System.Runtime/*23000001*/]System.AttributeTargets/*0100000E*/) /* 0A00000C */ = ( 01 00 4C 14 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ..L.....T..Allow
                                                                                                                                                                                                                          4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                                                                                                                          72 69 74 65 64 00 )                               // rited.
  .field /*04000002*/ public initonly uint8 Flag
  .method /*06000004*/ public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8 A_1) cil managed
  // SIG: 20 01 01 05
  {
    // Method begins at RVA 0x2082
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)000010       */ call       instance void [System.Runtime/*23000001*/]System.Attribute/*0100000D*/::.ctor() /* 0A000010 */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 02   |                  */ ldarg.0
    IL_0008:  /* 03   |                  */ ldarg.1
    IL_0009:  /* 7D   | (04)000002       */ stfld      uint8 System.Runtime.CompilerServices.NullableContextAttribute/*02000004*/::Flag /* 04000002 */
    IL_000e:  /* 2A   |                  */ ret
  } // end of method NullableContextAttribute::.ctor

} // end of class System.Runtime.CompilerServices.NullableContextAttribute

.class /*02000005*/ private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [System.Runtime/*23000001*/]System.Attribute/*0100000D*/
{
  .custom /*0C00001C:0A00000B*/ instance void [System.Runtime/*23000001*/]System.Runtime.CompilerServices.CompilerGeneratedAttribute/*0100000C*/::.ctor() /* 0A00000B */ = ( 01 00 00 00 ) 
  .custom /*0C00001D:06000001*/ instance void Microsoft.CodeAnalysis.EmbeddedAttribute/*02000002*/::.ctor() /* 06000001 */ = ( 01 00 00 00 ) 
  .custom /*0C00001E:0A00000C*/ instance void [System.Runtime/*23000001*/]System.AttributeUsageAttribute/*0100000F*/::.ctor(valuetype [System.Runtime/*23000001*/]System.AttributeTargets/*0100000E*/) /* 0A00000C */ = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                                                                                                                          4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                                                                                                                          72 69 74 65 64 00 )                               // rited.
  .field /*04000003*/ public initonly int32 Version
  .method /*06000005*/ public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  // SIG: 20 01 01 08
  {
    // Method begins at RVA 0x2092
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)000010       */ call       instance void [System.Runtime/*23000001*/]System.Attribute/*0100000D*/::.ctor() /* 0A000010 */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 02   |                  */ ldarg.0
    IL_0008:  /* 03   |                  */ ldarg.1
    IL_0009:  /* 7D   | (04)000003       */ stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute/*02000005*/::Version /* 04000003 */
    IL_000e:  /* 2A   |                  */ ret
  } // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file SourceCode.res
