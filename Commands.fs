module Commands
    open Spectre.Console.Cli
    open System.ComponentModel
    
    type AssistSettings() =
        inherit CommandSettings()

         [<CommandOption("-w|--wordlist")>]
         [<Description("Path to the word list")>]
        member val wordList = "./word-lists/words.txt" with get, set

        [<CommandOption("-n")>]
        [<Description("Number of suggestions to show")>]
        member val numberOfResults = 10 with get, set

        [<CommandOption("-s|--state")>]
        [<Description("The state of your score. Capital letters denote matching positions (green), lower-case letters occurences in the word (yellow)")>]
        member val state = "....." with get, set

        [<CommandOption("-e|--excluded")>]
        [<Description("The letters that have already been excluded")>]
        member val discarded = "" with get, set
    
    type Assist() =
        inherit Command<AssistSettings>()
        interface ICommandLimiter<AssistSettings>

        override _.Execute(_context, settings) = 
            let words = System.IO.File.ReadLines(settings.wordList) |> Seq.toArray
            let filteredWords = Domain.filterWords words settings.state settings.discarded
            let occurenceMap = Domain.buildOccurenceMap filteredWords

            filteredWords 
            |> Seq.map (fun word -> (word, Domain.wordScore occurenceMap word))
            |> Seq.sortBy (fun pair -> -1*(snd pair))
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.iter (fun pair -> 
                printfn "The word '%s' scored '%i' points" (fst pair) (snd pair))
            0

    type FindWordSettings() =
        inherit CommandSettings()

        [<CommandOption("-w|--wordlist")>]
        member val wordList = "./word-lists/words.txt" with get, set

        [<CommandOption("-n")>]
        member val numberOfResults = 10 with get, set

        [<CommandOption("-c|--contains")>]
        member val contains = "" with get, set
    
    type FindWord() =
        inherit Command<FindWordSettings>()
        interface ICommandLimiter<FindWordSettings>

        override _.Execute(_context, settings) = 
            let words = System.IO.File.ReadLines(settings.wordList) |> Seq.toArray
            let filteredWords = Domain.wordsContaining settings.contains words

            filteredWords 
            |> Seq.truncate(settings.numberOfResults)
            |> Seq.iter (fun word -> printfn "What about: %s" word)
            0

