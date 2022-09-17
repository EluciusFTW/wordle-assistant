namespace Wassi.Commands

open Spectre.Console.Cli
open System.ComponentModel
open Wassi.Domain
open Wassi.Output    

type SuggestWordSettings() =
    inherit ProgressSettings()
    
    [<CommandOption("-n")>]
    [<Description("Number of results to show")>]
    member val numberOfResults = 10 with get, set

module Suggest = 
    let printSuggestions (settings: SuggestWordSettings) words = 
        let filteredWords = filterWords words settings.progress settings.excluded
        let occurenceMap = buildOccurenceMap filteredWords
        let results = 
            filteredWords 
            |> Seq.map (fun word -> (word, wordScore occurenceMap word))
            |> Seq.sortBy (fun pair -> -1*(snd pair))
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.toList

        match results with
        | [ ] -> warn "No words in the list match those criteria" |> printMarkedUp  
        | pairs -> pairs |> List.iter (fun pair -> printMarkedUp $"The word {emphasize (fst pair)} scored {emphasize (snd pair)} points.")

type SuggestWord() =
    inherit Command<SuggestWordSettings>()
    interface ICommandLimiter<SuggestWordSettings>

    override _.Execute(_context, settings) = 
        match Wassi.Load.getWords settings.wordList with
        | Some words -> Suggest.printSuggestions settings words
        | None ->  warn "Error: Could not load word list." |> printMarkedUp
        0