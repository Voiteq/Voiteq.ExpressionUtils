// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.Testing.XUnit2
open Fake.OctoTools

// Properties
let buildDir = "./build/app/"
let testDir  = "./build/test/"
let packagingWorkDir = "./build/package_temp/"
let packagingDir = "./build/package/"
let baseVersion = "1.7"
let version = sprintf "%s.%s" baseVersion (getBuildParamOrDefault "buildnumber" "0")
let projectName = "Voiteq.ExpressionUtils"
let zipName = packagingDir @@ (projectName + version + ".zip")
let projectPath = projectName

printfn "Using full version number: %A" version

// Targets
Target "Clean" (fun _ ->
  CleanDirs [buildDir; testDir; packagingDir]
)

Target "Build" (fun _ ->
  
  CreateFSharpAssemblyInfo "./SolutionInfo.fs"
    [Attribute.Version version
     Attribute.FileVersion version
     Attribute.Metadata("githash", Information.getCurrentHash())]

  !! (projectPath @@ projectName + ".fsproj")
  |> MSBuildRelease buildDir "Build"
  |> Log "Build app output: "

  !! "Voiteq.ExpressionUtils.Tests/Voiteq.ExpressionUtils.Tests.csproj"
  |> MSBuildRelease testDir "Build"
  |> Log "Build tests output: "

)

Target "Test" (fun _ ->
  !! (testDir @@ "*.Tests.dll")
  |> xUnit2 (fun p -> { p with XmlOutputPath = Some (testDir @@ "xunit-test-results.xml") })
)

Target "CreatePackage" (fun _ ->
    // Copy all the package files into a package folder
    let contentFiles = SetBaseDir buildDir (!!("*.dll"))
    CopyFiles packagingWorkDir contentFiles

    let nuspecFile = (!!("Voiteq.ExpressionUtils.nuspec"))
    CopyFiles packagingWorkDir contentFiles


    NuGet (fun p -> 
        {p with
            Authors = [ "Stephen Willcock" ]
            Project = projectName
            Description = "Voiteq Expression Utils"                          
            OutputPath = packagingDir
            Summary = "Voiteq Expression Utils"
            WorkingDir = packagingWorkDir
            Version = version
            AccessKey = "dc4ded01-b57e-49fe-8dde-e5eaf89e02f8"
            Publish = true
            PublishUrl = "https://www.myget.org/F/rb-public/api/v2/package"
            Files = [@"Voiteq.ExpressionUtils.dll", Some @"lib/net46", None]}) 
            "Voiteq.ExpressionUtils.nuspec"
)

Target "Default" (fun _ ->
  trace "Running default task"
) 

// Task dependencies
"Clean"
  ==> "Build"
//  ==> "Test"
  ==> "CreatePackage"
  ==> "Default"

// start build
RunTargetOrDefault "Default"

