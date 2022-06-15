namespace Commands

module Suggest = 
    open Spectre.Console.Cli
    open System.ComponentModel
    open Output    

    type SuggestWordSettings() =
        inherit CommandSettings()

        [<CommandOption("-w|--wordlist")>]
        [<Description("Path to the word list")>]
        member val wordList = "./word-lists/words.txt" with get, set

        [<CommandOption("-n")>]
        [<Description("Number of results to show")>]
        member val numberOfResults = 10 with get, set

        [<CommandOption("-p|--progress")>]
        [<Description("Your current progress. Capital letters denote matching positions (green), lower-case letters occurences in the word (yellow)")>]
        member val state = "....." with get, set

        [<CommandOption("-e|--excluding")>]
        [<Description("The letters that have already been excluded")>]
        member val excluded = "" with get, set
    
    let printSuggestions (settings: SuggestWordSettings) words = 
        let filteredWords = Domain.filterWords words settings.state settings.excluded
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