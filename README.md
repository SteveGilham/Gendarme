Gendarme
========

Introduction
------------

**Gendarme** is a extensible _(stand-alone)_ rule-based tool to find problems in .NET applications and libraries. Gendarme inspects programs and libraries that contain code in ECMA CIL format _(Mono and .NET)_ and looks for common problems with the code, problems that compiler do not typically check or have not historically checked. *(see: [full/original description](http://www.mono-project.com/docs/tools+libraries/tools/gendarme/))*.

This repository was forked from [mono-tools](https://github.com/mono/mono-tools) and the **Gendarme** project was separated, because there is no plan to support/update other utilities from the package, and, IMHO, having many tools *(unrelated to each other)* in one repository is not a good idea.

The main focus of this fork is to allow Windows/Visual Studio users to use/compile **Gendarme** on Windows and fixing found issues in **Gendarme**.



Using of Gendarme
-----------------

At least for now, there is only one way to use **Gendarme**. You need to download the source code and compile your own version.

### Windows compilation description

* Download the project, from [Github](https://github.com/JAD-SVK/mono-tools),
* open the project solution file `gendarme-win.sln`,
* compile the release version,
* the compiled binary files are in the `console\bin\Release\`,
* copy all `*.dll` and `*.exe` files to your selected 'program directory'.

### Using of Gendarme

There are three ways how gendarme can be used:

1. Using the GUI version: `GendarmeWizard.exe`

   *There are known bugs, but since I do not use this version, they wont be fixed anytime soon.*

2. Using the console version: `gendarme.exe`

   Basic use: `%path_to_gendarme%/gendarme.exe --config rules.xml %path_to_binary%/%binary_name%`

   The sample `rules.xml` file is in project directory `rules/rules.xml`. To import only selected rules from a certain rule set library, remove the include with `"*"` and use specific rule name.

3. Creating a own application using all the libraries.



Issues and reporting
--------------------

This is a unofficial forked project. It has no meaning to report bugs, or ask for support on the official **Gendarme** bug tracker. Use the [Github issue tracker](https://github.com/JAD-SVK/mono-tools/issues) instead.



Project updates
---------------

Unfortunately I do not have much time, so do not expect any big activity on this project. I will try to fix all problems I find *(whilst working on my own projects)*, but do not expect me to fix any problematic parts found by other developers _(at least not in short time)_.

See [TODO.md](TODO.md) to see what is on the plan *(for now)* and [KnownIssues.md](KnownIssues.md) for list of known issues.

