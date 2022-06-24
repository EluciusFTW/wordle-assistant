namespace Commands

module Suggest = 
    open Spectre.Console.Cli
    open System.ComponentModel
    open Output    

    type SuggestWordSettings() =
        inherit Setting.ProgressSettings()
        
        [<CommandOption("-n")>]
        [<Description("Number of results to show")>]
        member val numberOfResults = 10 with get, set
        
    let printSuggestions (settings: SuggestWordSettings) words = 
        let filteredWords = Domain.filterWords words settings.progress settings.excluded
        let occurenceMap = Domain.buildOccurenceMap filteredWords
        let results = 
            filteredWords 
            |> Seq.map (fun word -> (word, Domain.wordScore occurenceMap word))
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
            match Load.getWords settings.wordList with
            | Some words -> printSuggestions settings words
            | None ->  warn "Error: Could not load word list." |> printMarkedUp
            0