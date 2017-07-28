# Voiteq Expression Utils

A library for converting Lambda Expressions to short, consistent, human readable strings. These strings are suitable for use as cache keys or simply for debugging.

## Build instructions

### Windows

> build

The default build will run tests and deploy the package to MyGet (this can be sent to NuGet instead easily enough at some point)

To set the package version, set the baseVersion in build.fsx

> let baseVersion = "enter version number here"

### Other OS

No build file for Linux yet, but just take a look at build.bat and run equivalent commands in the shell.

The basic command sequence is:

* Install paket by running paket bootstrapper
* Restore packages by running paket restore
* Run build by calling the build.fsx build script via Fake

