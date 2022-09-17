namespace Wassi.Commands

open Spectre.Console.Cli
open System.ComponentModel
open Wassi.Output
open Wassi.Domain

type FindWordSettings() =
    inherit WordListSettings()

    [<CommandOption("-n")>]
    [<Description("Number of results to show")>]
    member val numberOfResults = 10 with get, set

    [<CommandOption("-i|--including")>]
    [<Description("The letters the words need to inlcude")>]
    member val including = "" with get, set

type FindWord() =
    inherit Command<FindWordSettings>()
    interface ICommandLimiter<FindWordSettings>

    override _.Execute(_context, settings) = 
        match Wassi.Load.getWords settings.wordList with
        | Some words -> 
            wordsContaining settings.including words 
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.iter (fun word -> printMarkedUp $"What about: {emphasize word}?" )
        | None -> warn "Error: Could not load word list." |> printMarkedUp
        0
