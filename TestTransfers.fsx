<<<<<<< HEAD
#define "FROMTT"
#load "FileTransfer.fs"
#load "ft.fs"
#load "process.fs"
#load "webclient.fs"
open Ft
open Ftest
open Web
open proc
do makeFileTransfers
async {
    let fd1,fd2 = runProc "cmd" "/c hoi.cmd" None
    for line in fd1 do printfn "fd1:%s" line
    for line in fd2 do printfn "fd2:%s" line
    } |> Async.RunSynchronously
try 
    let result = Async.RunSynchronously(downLoadUrl("http://googlebestaatniet.com"))
    printfn "%A" result
with
| exdl -> 
    printfn "%A" (exdl.GetBaseException())
=======
#load "FileTransfer.fs"
#load "ft.fs"
open Ft
open Ftest
do makeFileTransfers
>>>>>>> 614454b5234ef954fbaceb733e8f64dea6c75235
