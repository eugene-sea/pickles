// include Fake lib
#r @"src\Pickles\packages\FAKE.3.36.0\tools\FakeLib.dll"
open Fake

// Properties
let cmdDir = "./build/exe/"
let guiDir = "./build/gui/"
let deployDir  = "./deploy/chocolatey/"
let packagingDir = "./packaging/"
let chocoDir = "./chocolatey/"

// version info
let version = environVar "version" // or retrieve from CI server

Target "Clean" (fun _ ->
    CleanDirs [deployDir; packagingDir]
)


Target "CreatePackage CMD" (fun _ ->
    CopyFiles packagingDir [cmdDir + "Pickles.exe"; cmdDir + "NLog.config"]
    WriteFile (packagingDir + "version.ps1") [("$version = \"" + version + "\"")]
    NuGet (fun p ->
        {p with
            OutputPath = deployDir
            WorkingDir = packagingDir
            Version = version
            Publish = false })
            (chocoDir + "Pickles.nuspec")
)


Target "CreatePackage GUI" (fun _ ->
    CopyFiles packagingDir [guiDir + "picklesui.exe"; guiDir + "NLog.config"; guiDir + "PicklesUI.exe.config"]
    WriteFile (packagingDir + "version.ps1") [("$version = \"" + version + "\"")]
    WriteFile (packagingDir + "picklesui.exe.gui") [("")]
    NuGet (fun p ->
        {p with
            OutputPath = deployDir
            WorkingDir = packagingDir
            Version = version
            Publish = false })
            (chocoDir + "picklesui.nuspec")
)


Target "Default" (fun _ ->
    trace ("Starting build of Pickles version " + version)
    DeleteDir packagingDir
)


// Dependencies
"Clean"
  ==> "CreatePackage CMD"
  ==> "CreatePackage GUI"
  ==> "Default"


// start build
RunTargetOrDefault "Default"

