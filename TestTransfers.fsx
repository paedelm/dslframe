#define "FROMTT"
#load "FileTransfer.fs"
#load "ft.fs"
#load "process.fs"
#load "webclient.fs"
#load "files.fs"
open Ft
open Ftest
open Web
open proc
open files
do makeFileTransfers
async {
    let fd1,fd2 = runProc "cmd" "/c hoi.cmd" None
    for line in fd1 do printfn "fd1:%s" line
    for line in fd2 do printfn "fd2:%s" line
    } |> Async.RunSynchronously
try 
    let jan = allFilesInfo @"c:\users\p_ede\projects"
    jan |> Seq.filter (fun (fi) -> fi.Length > 300_000_000L && fi.FullName.EndsWith(@".zip") ) |> Seq.iter (fun fi -> printfn "%A %s " fi.Length fi.FullName)
    // let result = Async.RunSynchronously(downLoadUrl("http://googlebestaatniet.com"))
    // printfn "%A" result
with
| exdl -> 
    printfn "%A" (exdl.GetBaseException())
