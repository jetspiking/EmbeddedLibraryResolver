# EmbeddedLibraryResolver

A small helper for .NET that loads embedded libraries (.dll, .so, .dylib) in an application.

It allows shipping native dependencies inside assemblies. A use-case of this is to create standalone binaries when otherwise not supported.

## Features

- Detects the current OS and selects the correct native extension:
  - Windows: .dll
  - Linux: .so
  - macOS: .dylib
- Scans the entry assembly for embedded resources whose names end with that extension
- Extracts those resources to the system temporary directory
- Loads them at runtime

## Usage

### 1. Embed native libraries as resources

In your .csproj, mark your native binaries as embedded resources:

~~~xml
<ItemGroup>
  <EmbeddedResource Include="runtimes\win-x64\native\mylib.dll" />
  <EmbeddedResource Include="runtimes\linux-x64\native\mylib.so" />
  <EmbeddedResource Include="runtimes\osx-x64\native\mylib.dylib" />
</ItemGroup>
~~~

The library file name is inferred from the end of the resource name.

Example: from

Assembly.runtimes.win-x64.native.mylib.dll

the extracted file name will be

mylib.dll

### 2. Load embedded libraries

Call EmbeddedLibraryResolver.Manager.Load once, early in application startup, before any code that depends on those native libraries.

## Notes

- Extraction directory: Uses the system temp directory.
- File name collisions: Libraries with the same file name may overwrite each other in the temp directory.
- Cleanup: Extracted files are not deleted by this helper.
- Platforms: Throws PlatformNotSupportedException on unsupported OSes.