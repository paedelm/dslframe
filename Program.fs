// Learn more about F# at http://fsharp.org

open System
open Ftest
open proc
open Web
open files
open FSharp.Data
open FSharp.Data.JsonExtensions

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    do makeFileTransfers
    async {
        let fd1,fd2 = runProc "cmd" "/c hoi.cmd" None
        for line in fd1 do printfn "fd1:%s" line
        for line in fd2 do printfn "fd2:%s" line
        } |> Async.RunSynchronously
    // let result = Async.RunSynchronously(downLoadUrl("http://google.com"))
    // printfn "%A" result
    async {
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
    } |> Async.RunSynchronously
    async {
        let! resx = downLoadUrl("http://api.worldbank.org/v2/region?format=xml")
        printfn "%A" resx
    } |> Async.RunSynchronously
    // Async.Parallel [| dnl1; dnl2 |] |> Async.RunSynchronously

    // for line in outp do printfn "%s" line
    try
        let jan = allFilesInfo @"c:\users\p_ede\projects"
        jan |> Seq.filter (fun (fi) -> fi.Length > 300_000_000L && fi.FullName.EndsWith(@".zip") ) |> Seq.iter (fun fi -> printfn "%A %s " fi.Length fi.FullName)
    with
    | exdl -> printfn "%A" (exdl.GetBaseException()) 

    0 // return an integer exit code

