// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.Testing.XUnit2
open Fake.OctoTools

// Properties
let buildDir = "./build/app"
let testDir  = "./build/test/"
let packagingDir = "./build/package/"
let baseVersion = "1.0"
let version = "1.7.0"
let projectName = "Voiteq.ExpressionUtils"
let zipName = packagingDir @@ (projectName + version + ".zip")
let projectPath = projectName

printfn "Using full version number: %A" version

// Targets
Target "Clean" (fun _ ->
  CleanDirs [buildDir; testDir; packagingDir]
)

Target "Build" (fun _ ->
  
  CreateCSharpAssemblyInfo "./SolutionInfo.cs"
    [Attribute.Version version
     Attribute.FileVersion version
     Attribute.Metadata("githash", Information.getCurrentHash())]

  !! (projectPath @@ projectName + ".csproj")
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
  let fileIncludes = ["", SetBaseDir buildDir (!! (buildDir @@ "*.dll")) ++ (buildDir @@ "*.json") ++ (buildDir @@ "*.exe") ++ (buildDir @@ "*.config")]
  ZipOfIncludes zipName fileIncludes
)

//Target "PushPackage" (fun _ ->

//  Octo (fun octoParams -> { octoParams with
//                              ToolPath = "./packages/OctopusTools/tools"
//                              Server = { Server = "http://athenaoctopus.northeurope.cloudapp.azure.com"; ApiKey = "API-WKR63GUVQPOSNXGG2ZM81YJYEW" }
//                              Command  = Push { Packages = [zipName]; ReplaceExisting = true }})

//)

Target "Default" (fun _ ->
  trace "Running default task"
) 

// Task dependencies
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "CreatePackage"
//  ==> "PushPackage"
  ==> "Default"

// start build
RunTargetOrDefault "Default"

