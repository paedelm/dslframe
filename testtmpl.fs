#if INTERACTIVE
module Testtmpl =
#else
module Testtmpl
#endif
    open System
    open Ft
    let literalstring = @"
Regel 1
Regel 2
Regel 3"
    type Gender =
        | Man
        | Woman

    type Env =
        | TEST
        | ACPT
        | PROD

    let testtmpl name age =
        sprintf "Dit is mijn naam %s\nen mijn leeftijd %i"
            name
            age

    type testtmpl2(?name , ?age, ?gender) =
        let name = defaultArg name "Jan"
        let age = defaultArg age 22
        let gender = defaultArg gender Man
        member x.Format() =
            String.Format(
                """Dit is mijn naam {0} en dit mijn leeftijd: {1} en dit het geslacht: "{2}" """,
                name,
                age,
                gender
            )
    /// shelltemplate TEST|ACPT|PROD WinDirectory(@"c:\windir\subdir")
    let shelltemplate (env:Env) (path:WinDirectory) = 
        let script = """#!bin/bash
export Env={0}
cd "{1}"
ls"""
        String.Format(script, env, path)

    let battemplate (env:Env) (path:WinDirectory) = 
        let script = """
echo 0=%0
echo 1=%1
echo 2=%2
echo 3=%3 
set
echo env={0}
echo path={1}"""
        String.Format(script, env, path)


