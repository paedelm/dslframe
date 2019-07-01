#if INTERACTIVE
module files =
#else
module files
#endif
    open System.IO
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect Directory.EnumerateFiles
                  yield! dirs |> Seq.collect Directory.EnumerateDirectories |> allFiles }

    let allFilesInfo root = 
        let rec traverse (d: DirectoryInfo) = 
            seq {   for f in d.GetFiles() do
                        yield f
                    for dd in d.GetDirectories() do
                        yield! traverse dd              }
        traverse (DirectoryInfo( root ))
// hierachter alles weer commentaar maken
// open files
// open System.IO
// let peter = allFiles [@"c:\ontwikkel"] |> Seq.filter (fun (n:string) -> n.EndsWith(".fs"))
// for f in peter do printfn "%A" f
// try
    // let jan = allFilesInfo @"c:\ontwikkel"
    // jan |> Seq.filter (fun (fi:FileInfo) -> fi.Length > 300_000_000L) |> Seq.iter (fun fi -> printfn "%A %s " fi.Length fi.FullName)
// with
// | exdl -> printfn "%A" (exdl.GetBaseException()) 
// let di = new DirectoryInfo(@"c:\ontwikkel")
// let dif = di.EnumerateFiles()




