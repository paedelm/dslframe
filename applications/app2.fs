module app2
    open Ft
    let app2 = seq {
        yield MftResource( UnixFileToQueue ({
            SrcAgent=FteAgent.LinSvcApl;
            SourceDirectory = UnixDirectory(@"c:\peter\edelman");
            FilePattern = @"*.xml";
            DstAgent=QueueAgent.LinSvcApl
        }))
        yield MqResource( TopicToQueue )
    }
