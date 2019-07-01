#if INTERACTIVE
#r @"..\..\.nuget\packages\FSharp.Data\3.1.1\lib\net45\FSharp.Data.dll"
module Web =
#else
module Web
#endif
    open FSharp.Data
    let downLoadUrl(url) =
        async {
            let! html = Http.AsyncRequestString(url)
            return html
        }
