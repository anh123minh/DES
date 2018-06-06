===============================================================================
====                          R E A C T . N E T                            ====
====                          -----------------                            ====
====                                                                       ====
====            A Discrete-Event Simulation Framework for .NET             ====
===============================================================================

  $Id: README.txt 189 2006-10-23 23:53:23Z eroe $


1. INTRODUCTION
---------------

React.NET is a library (a .dll) that can be used to build discrete-event
simulations using the C# programming language and the Microsoft .NET
Framework.

This document provides information about the files and directories contained
in the distribution, instructions on building the React.NET.dll library and
example programs, and other information that may (or may not) be of interest
to developers and simulation builders.

The latest information on React.NET is available on SourceForge at
http://reactnet.sourceforge.net or http://sourceforge.net/projects/reactnet.


2. REQUIREMENTS
---------------

To build React.NET or build simulations using React.NET, you must have the
Microsoft .NET 2.0 SDK installed.  Earlier versions will not work.
Optionally, you can use Microsoft Visual Studio .NET 2005 (VS.NET 2005);
however, the VS.NET 2005 solution and project files are not included in this
distribution (I haven't got VS.NET).

To build and run the unit tests requires NUnit v2.2.5. (http://www.nunit.org)

To build the web pages requires MsXsl.exe to transform the XML sources into
HTML. (http://msdn.microsoft.com/XML/XMLDownloads/default.aspx)


3. DIRECTORIES
--------------

The React.NET project is contained in the directory hierarchy shown below.

	React
	   |
	   |-doc
	   |   |
	   |   |-html        {Pre-generated HTML documentation}
	   |
	   |-examples        {Contains sub-dirs with example simulations}
	   |   |
	   |   |-bin         {Pre-compiled example binaries}
	   |
	   |-lib             {Pre-compiled .dll files}
	   |
	   |-src
	   |   |
	   |   |-engine      {Sources for React.NET.dll}
	   |   |
	   |   |-tests       {Sources for React.Test.dll}
	   |
	   |-www
	       |
	       |-css         {CSS stylesheets}
	       |
	       |-images      {Images for web pages}
	       |
	       |-xml         {XML and XSLT files for building web pages}

In addition, when building from source, a directory named 'build' is created
that will contain all build outputs (.dll, .exe, and .html files).


4. PRE-GENERATED FILES
----------------------

The files in the 'doc\html' directory are the output of building the React.NET
local web pages from the sources in the 'www' directory.

The files in the 'lib' directory are the output of compiling the React.NET
C# sources (does not include the test library).

The files in the 'examples\bin' directory are the output of compiling all the
example simulation programs found under the 'examples' directory.


5. BUILDING
-----------

MSBuild is used to build React.NET.  Available build targets are listed at
the top of the React.proj build file.  Only the targets listed should be
invoked as other targets are used internally by the build process and might
not run correctly if executed directly.

Before attempting to run a build, you must edit the 'Build.properties' file
to provide the path to the MsXsl.exe and nunit-console.exe executables.
These are only required if you are building/running the unit tests or are
building the HTML documentation.  The build script will notify you if you're
missing one of these required programs.

To build the React.NET library (React.NET.dll) type:

   msbuild React.proj /t:Build

To build the example simulation programs type:

   msbuild React.proj /t:BuildExamples

To build all content (e.g. React.NET.dll, examples, HTML documentation) type:

   msbuild React.proj /t:BuildAll

To build and run the unit tests type:

   msbuild React.proj /t:RunTests

To remove generated outputs type:

   msbuild React.proj /t:Clean

When building using the React.proj file, all build outputs (except for the
NUnit test results) are placed in the 'build' directory.

Note that if you simply invoke 'msbuild' you'll run the default, 'Build'
target, which builds the React.NET.dll file and puts it in build\lib.

IT IS NOT CURRENTLY POSSIBLE TO BUILD THE REACT.NET API REFERENCE LIBRARY
HELP FILE (React.NET.chm) USING MSBUILD.  THIS WILL BE AVAILABLE IN A LATER
RELEASE.

                              -= END OF FILE =-
