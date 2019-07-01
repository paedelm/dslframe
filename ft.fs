#if INTERACTIVE
module Ftest
#else
module Ftest
#endif
    open System
    open Ft
    let makeFileTransfers =
        let gftq:UnixFileToQueueInfo = { 
            SrcAgent=FteAgent.LinSvcApl;
            SourceDirectory = UnixDirectory(@"/peter\edelman");
            FilePattern = @"*.xml";
            DstAgent=QueueAgent.LinSvcApl  }
        printfn "%s" gftq.Generate
        printfn "%A" gftq.SrcAgent
        let agentname = sprintf "%A" gftq.SrcAgent
        let gftf = { 
            SrcAgent=FteAgent.LinSvcApl;
            SourceDirectory = @"c:\peter\edelman";
            FilePattern = @"*.xml";
            DstAgent=FteAgent.LinSvcApl
            DstDir = @"c:\dstdir"  }
    
        let wftq:WinFileToQueueInfo = { 
            SrcAgent=WinAgent.WinSvcApl;
            SourceDirectory = WinDirectory (@"c:\peter\edelman");
            FilePattern = @"*.xml";
            DstAgent=QueueAgent.LinSvcApl
            }
        printfn "%s" wftq.SourceDirectory.Value
        let transfers = seq {
            yield UnixFileToQueue(gftq)
            yield FileToFile(gftf)
            yield WinFileToQueue(wftq)
        }
        do generateAll transfers
