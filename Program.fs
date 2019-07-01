// Learn more about F# at http://fsharp.org

open System
open Ftest
open proc
open Web
open files

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    do makeFileTransfers
    async {
        let fd1,fd2 = runProc "cmd" "/c hoi.cmd" None
        for line in fd1 do printfn "fd1:%s" line
        for line in fd2 do printfn "fd2:%s" line
        } |> Async.RunSynchronously
    try 
        let result = Async.RunSynchronously(downLoadUrl("http://google.com"))
        printfn "%A" result
        // let res2 = Async.RunSynchronously(downLoadUrl("http://api.worldbank.org/v2/region?format=xml"))
    with
    | exdl -> 
        printfn "%A" (exdl.GetBaseException())

    // for line in outp do printfn "%s" line
    try
        let jan = allFilesInfo @"c:\ontwikkel"
        jan |> Seq.filter (fun (fi) -> fi.Length > 300_000_000L && fi.FullName.EndsWith(@".zip") ) |> Seq.iter (fun fi -> printfn "%A %s " fi.Length fi.FullName)
    with
    | exdl -> printfn "%A" (exdl.GetBaseException()) 

    0 // return an integer exit code

