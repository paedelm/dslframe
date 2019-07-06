module app1
    open Ft
    open ApplicationList
    let app1 = seq {
        yield MftResource( UnixFileToQueue ({
            SrcAgent=FteAgent.LinSvcApl;
            SourceDirectory = UnixDirectory(@"/peter\edelman");
            FilePattern = @"*.xml";
            DstAgent=QueueAgent.LinSvcApl
        }))
        yield MqResource( TopicToQueue )
    }
    addApplication app1 |> ignore
