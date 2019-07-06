// Learn more about F# at http://fsharp.org

open System
open System.IO
open Ftest
open proc
open Web
open files
open Testtmpl
open Ft
open FSharp.Data
open FSharp.Data.JsonExtensions
open System.Runtime.InteropServices
// open app1
// open app2
open System.Reflection

let isWindows = (RuntimeInformation.IsOSPlatform OSPlatform.Windows)
let isLinux   = (RuntimeInformation.IsOSPlatform OSPlatform.Linux)
// (*)
let generateAndExecute =
    let scriptBase = Path.Combine [|"runtime"; "testpe"|]
    let scriptFile = scriptBase + if isWindows then ".bat" else ".sh"
    let scriptOut  = scriptBase + ".out"
    let scriptErr  = scriptBase + ".err"
    let wind = WinDirectory(@"c:\ontwikkel")
    Directory.CreateDirectory("runtime") |> ignore
    File.WriteAllText(scriptFile, battemplate PROD wind)
    async {
        Directory.CreateDirectory("runtime") |> ignore
        if isWindows then
            let fd1,fd2 = runProc "cmd" ("/c " + scriptFile + " param1 param2 param3") None
            use tf = File.CreateText(scriptOut)
            for line in fd1 do 
                printfn "fd1:%s" line
                tf.WriteLine(line)

            use tf = File.CreateText(scriptErr)
            for line in fd2 do
                printfn "fd2:%s" line
                tf.WriteLine(line)
            } |> Async.RunSynchronously
// (*)

type wbregions = XmlProvider<"http://api.worldbank.org/v2/region?format=xml">
type WBregionsJ = JsonProvider<"http://api.worldbank.org/v2/region?format=json">
let jsonPTests =
    printfn "============================   start  Json tests ========================\n"
    let result = Async.RunSynchronously(downLoadUrl("http://api.worldbank.org/v2/region?format=json"))
    let sample = WBregionsJ.Parse(result)
    for region in sample.Array do
        printfn "%s %s" region.Code region.Name
    printfn "============================   einde  Json tests ========================\n"


let asynctests =
    let ex1 = async {
        let! resj = downLoadUrl("http://api.worldbank.org/v2/region?format=json")
        let jv = JsonValue.Parse(resj)
        // printfn "%A" jv
        match jv with
        | JsonValue.Array [| info; data |] ->
            // Print overall information
            let page, pages, total = info?page, info?pages, info?total
            printfn 
              "Showing page %d of %d. Total records %d" 
              (page.AsInteger()) (pages.AsInteger()) (total.AsInteger())
            printfn "%A" data
            // Print every non-null data point
            for record in JsonExtensions.AsArray(data) do
              if record?name <> JsonValue.Null then
                printfn "%s: %s" (record?code.AsString()) 
                                 (record?name.AsString())
        | _ -> printfn "failed"        
    } //|> Async.RunSynchronously
    let ex2 = async {
        let! resx = downLoadUrl("http://api.worldbank.org/v2/region?format=xml")
        printfn "%A" resx
    } 
    // [ex1; ex2] |> Async.Parallel |> Async.RunSynchronously |> ignore
    
    try
        let home = Environment.GetEnvironmentVariable("userprofile")
        match home with
        | null -> failwith "environment var userprofile not found!"
        | dir -> 
            printfn "search %A for *.dll's of special size" dir
            let jan = allFilesInfo (Path.Combine [|home; "projects"|])
            jan |> Seq.filter (fun (fi) -> fi.Length > 300_000_000L && fi.FullName.EndsWith(@".dll") ) |> Seq.iteri (fun i fi -> printfn "%i: %A %s " i fi.Length fi.FullName)
    with
    | exdl -> printfn "%A" (exdl.GetBaseException())

let templatetests =
    printfn "%s" (testtmpl "Peter" 60) 
    let jan = testtmpl2(name="Pieter")
    printfn "%s" (jan.Format())
    let anjo = testtmpl2(name="Anjo", age=59, gender=Woman)
    printfn "%s" (anjo.Format())
    let wind = WinDirectory(@"c:\ontwikkel")
    printfn "%s" (shelltemplate PROD wind )

let iotests =
    let wind = WinDirectory(@"c:\ontwikkel")
    Directory.CreateDirectory("runtime") |> ignore
    File.WriteAllText("runtime/file.sh", shelltemplate PROD wind)
    try
        Directory.CreateDirectory("runtime") |> ignore
        File.WriteAllText("runtime/file.sh", shelltemplate PROD wind)
        Directory.CreateDirectory("altrt") |> ignore
        File.WriteAllText("altrt/file.sh", shelltemplate PROD wind)
        if isWindows then printfn "%s" (File.ReadAllText(".gitignore"))
    with
    | exdl -> printfn "%A" (exdl.GetBaseException())
    // use tf = File.CreateText("runtime/file.sh")
    // tf.WriteLine(shelltemplate PROD wind)

let displayapp (app:seq<ApplicationResource>) =
    printfn "applicaton seq: %A" app   
[<EntryPoint>]
let main argv =
    let ass = Assembly.GetExecutingAssembly();
    let mdls = ass.DefinedTypes
    printfn "\n===================== Module List ======================\n"
    mdls |> Seq.filter (fun dt -> dt.Name.StartsWith("app") && dt.IsValueType) |>    Seq.iter (fun m -> printfn "%A" m.GetDeclaredField)
    displayapp app1.app1
    displayapp app2.app2
    printfn "Hello World from F# on OS: %s !" RuntimeInformation.OSDescription
    let result = Async.RunSynchronously(downLoadUrl("http://api.worldbank.org/v2/region?format=xml"))
    let sample = wbregions.Parse(result)
    for region in sample.Regions do
        printfn "%s %s" region.Code region.Name
    
    printfn "============================   einde  xml tests ========================"
    // parse Json via type provider
    jsonPTests

    // do makeFileTransfers
    if isWindows then
        async {
            Directory.CreateDirectory("runtime") |> ignore
            let fd1,fd2 = runProc "cmd" "/c hoi.cmd" None
            use tf = File.CreateText("runtime/hoi.out")
            for line in fd1 do 
                printfn "fd1:%s" line
                tf.WriteLine(line)

            use tf = File.CreateText("runtime/hoi.err")
            for line in fd2 do
                printfn "fd2:%s" line
                tf.WriteLine(line)
            } |> Async.RunSynchronously
    // let result = Async.RunSynchronously(downLoadUrl("http://google.com"))
    // printfn "%A" result

    // voer de asynctests uit
    // asynctests

    // voer de templatetests uit
    // templatetests

    // voer de iotests uit
    // iotests

    // generateAndExecute
    

    0 // return an integer exit code

