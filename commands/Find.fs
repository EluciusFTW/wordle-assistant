namespace Wassi.Commands

open Spectre.Console.Cli
open SpectreCoff
open System.ComponentModel
open Wassi.Domain


type FindWordSettings() =
    inherit WordListSettings()

    [<CommandOption("-n")>]
    [<Description("Number of results to show")>]
    member val numberOfResults = 10 with get, set

    [<CommandOption("-i|--including")>]
    [<Description("The letters the words need to inlcude")>]
    member val including = "" with get, set

open Wassi.Load

type FindWord() =
    inherit Command<FindWordSettings>()
    interface ICommandLimiter<FindWordSettings>

    override _.Execute(_context, settings) = 
        match getWords settings.wordList with
        | Some words -> 
            wordsContaining settings.including words 
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.map (fun word -> [C "What about:"; P word; C "?"; NL ])
            |> Seq.collect id
            |> Seq.toList
            |> Many
        | None -> E "Error: Could not load word list." 
        |> toConsole
        0
