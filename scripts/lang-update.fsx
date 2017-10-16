#r "System.Xml.Linq"

open System
open System.Text.RegularExpressions
open System.IO
open System.Xml.Linq;

type PropertyGroup = {
    Configuration: string
    Element: XElement
}

type Project = {
    Path: string
    Name: string
    Document: XDocument
}

type UpdateResult = Updated | UpdateNotNeeded

let solutionLocation = "/Users/takemyoxygen/dev/code/playground/Tp.All.sln"
let projectLocation = "/Users/takemyoxygen/dev/code/memo-list/src/MemoList.UI/MemoList.UI.csproj"
let targetLanguageVersion = "7.1"

let updateProject (projectLocation: string) =

    let projectDoc = XDocument.Load(projectLocation)
    let xn name = XName.Get(name, projectDoc.Root.GetDefaultNamespace().NamespaceName)
    let xa name = XName.Get(name, String.Empty)

    let flattenResults statuses = 
        if List.contains Updated statuses then Updated else UpdateNotNeeded

    let getPropertyGroups project =
        let groups = 
            project.Document.Element(xn "Project").Elements(xn "PropertyGroup")
            |> Seq.choose(fun group -> 
                match group.Attribute(xa "Condition") with
                | null -> Option.None
                | attr -> 
                    let result = Regex.Match(attr.Value, "'\$\(Configuration\)\|\$\(Platform\)' ?== ?'(.+)'")
                    if result.Success then 
                        Option.Some {Configuration = result.Groups.[1].Value; Element = group}
                    else Option.None)
            |> List.ofSeq

        printfn "\tFound %i property groups for various build configurations in project %s" groups.Length project.Name 

        groups  

    let updatePropertyGroup group =
        printfn "\tUpdating property group for \"%s\"" group.Configuration
        match group.Element.Element(xn "LangVersion") with 
        | null -> 
            group.Element.Add(XElement(xn "LangVersion", targetLanguageVersion))
            Updated
        | langVersion -> 
            if langVersion.Value = targetLanguageVersion then
                UpdateNotNeeded
            else        
                langVersion.Value <- targetLanguageVersion
                Updated


    let validate project (propertyGroups: PropertyGroup list) = 
        if propertyGroups.Length < 2 then
            (sprintf "Cannot project project %s - less than 2 configurations found" project.Name)
            |> Exception
            |> raise

        propertyGroups    


    let save project = function
        | Updated ->
            let destination = project.Path + ".updated"
            let projectName = Path.GetFileNameWithoutExtension project.Path
            printfn "\tSaving project to \"%s\"" destination

            project.Document.Save(destination)  

            printfn "\tProject was successfully saved to \"%s\"" destination
        | UpdateNotNeeded ->
            printf "Project %s already updated. Nothing to save" project.Name

    let project = 
        { Path = projectLocation
          Name = Path.GetFileNameWithoutExtension projectLocation
          Document = projectDoc }

    printfn "Updating project %s from \"%s\"" project.Name project.Path

    getPropertyGroups project
    |> validate project
    |> List.map updatePropertyGroup
    |> flattenResults
    |> save project

let getProjects (solutionLocation: string) = 
    let solutionFolder = Path.GetDirectoryName solutionLocation

    File.ReadAllLines solutionLocation
    |> Seq.choose (fun line -> 
        let m = Regex.Match(line, "\", \"(.+?\.csproj)\"")
        if m.Success then
            Option.Some m.Groups.[1].Value
        else Option.None )
    |> Seq.map (fun proj -> Path.Combine(solutionFolder, proj))
    |> Seq.filter File.Exists
    |> List.ofSeq

let projects = getProjects solutionLocation
printfn "Found %i projects" projects.Length

for project in projects do
    updateProject project

// let missing = 
//     File.ReadLines solutionLocation
//     |> Seq.filter (fun line -> line.Contains ".csproj")
//     |> Seq.filter (fun line -> projects |> List.exists line.Contains |> not)
//     |> List.ofSeq