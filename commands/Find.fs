namespace Commands

module Find =
    open Spectre.Console.Cli
    open System.ComponentModel
    
    type FindWordSettings() =
        inherit CommandSettings()

        [<CommandOption("-w|--wordlist")>]
        [<Description("Path to the word list")>]
        member val wordList = "./word-lists/words.txt" with get, set

        [<CommandOption("-n")>]
        [<Description("Number of results to show")>]
        member val numberOfResults = 10 with get, set

        [<CommandOption("-i|--including")>]
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