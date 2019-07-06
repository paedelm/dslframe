#if INTERACTIVE
module Ft
#else
module Ft
#endif
    open System.Text.RegularExpressions
    // generic string with regex validation
    let validate(value, re, title) =
        let m = Regex(re).Match(value) 
        if m.Success then value else failwith (sprintf "\"%s\" is not a valid \"%s\"" value title)
    type RegexString(value, re, title) =
        let value = validate(value, re, title)

        member x.Value with get() = value
        override x.ToString() = value

    // Domain definitions for properties
    type WinAgent =
        | WinSvcApl
    type FteAgent =
        | LinSvcApl
        | MF3
    type QueueAgent =
        | LinSvcApl
    type MainframeAgent =
        | MF1
        | MF2
        | MF3 

    type WinDirectory(dir:string) =
        /// Windows Directory like @"c:\dir\subdir\etc"
        inherit RegexString(dir, @"^[a-zA-Z]:\\[\\\S|*\S]?.*$", "Windows Directory")

    type UnixDirectory(dir:string) =
        inherit RegexString(dir,@"(\/{1,1}(((\w)|(\.)|(\\\s))+\/)*((\w)|(\.)|(\\\s))+)|\/", "Unix Directory")

    // Domain definitions for structures
    type WinFileToQueueInfo = 
        {
            /// source agent
            SrcAgent: WinAgent;
            /// WinDirectory(@"c:\dir\subdir") on the source agent
            SourceDirectory: WinDirectory;
            /// an glob or regular expression
            FilePattern: string;
            DstAgent: QueueAgent;
        }
        member x.Generate = sprintf "generate WF2Q %A" x
    type UnixFileToQueueInfo = 
        {
            /// source agent
            SrcAgent: FteAgent;
            /// sourcedirectory on the source agent
            SourceDirectory: UnixDirectory;
            /// an glob or regular expression
            FilePattern: string;
            DstAgent: QueueAgent;
        }
        member x.Generate = sprintf "generate F2Q %A" x

    type FileToFileInfo = 
        {
            /// source agent
            SrcAgent: FteAgent;
            /// sourcedirectory on the source agent
            SourceDirectory: string;
            /// an glob or regular expression
            FilePattern: string;
            DstAgent: FteAgent;
            DstDir: string
        }
        member x.Generate = sprintf "generate F2F %A" x
    /// FileTransfer        
    type FileTransfer =
        | UnixFileToQueue of UnixFileToQueueInfo
        | WinFileToQueue of WinFileToQueueInfo
        | FileToFile of FileToFileInfo
    type MessageTransfer =
        | QueuetoQueue
        | TopicToQueue

    type ApplicationResource =
        | MftResource of FileTransfer
        | MqResource of MessageTransfer
        
    let generate ft =
        match ft with
        | UnixFileToQueue ftq -> ftq.Generate
        | FileToFile ftf -> ftf.Generate
        | WinFileToQueue wftq -> wftq.Generate
    let generateAll transfers =
        for ft in transfers do
            generate ft |> printfn "%s"   

