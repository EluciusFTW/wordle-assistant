namespace Wassi.Commands

open Spectre.Console.Cli
open SpectreCoff    
open System.ComponentModel
open Wassi.Domain

type SuggestWordSettings() =
    inherit ProgressSettings()
    
    [<CommandOption("-n")>]
    [<Description("Number of results to show")>]
    member val numberOfResults = 10 with get, set

module Suggest = 
    let getSuggestionOutput (settings: SuggestWordSettings) words = 
        let filteredWords = filterWords words settings.progress settings.excluded
        let occurenceMap = buildOccurenceMap filteredWords
        let results = 
            filteredWords 
            |> Seq.map (fun word -> (word, wordScore occurenceMap word))
            |> Seq.sortBy (fun pair -> -1*(snd pair))
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.toList

        match results with
        | [ ] -> E "No words in the list match those criteria" 
        | pairs -> 
            pairs 
            |> List.map (fun pair -> [ C "The word"; P (fst pair); C "scored"; P $"{snd pair}"; C "points."; NL])
            |> List.collect id
            |> Many

open Wassi.Load

type SuggestWord() =
    inherit Command<SuggestWordSettings>()
    interface ICommandLimiter<SuggestWordSettings>

    override _.Execute(_context, settings) = 
        match getWords settings.wordList with
        | Some words -> Suggest.getSuggestionOutput settings words
        | None ->  E "Error: Could not load word list." 
        |> toConsole
        0
