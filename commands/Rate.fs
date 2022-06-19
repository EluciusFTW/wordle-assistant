namespace Commands

module Rate =
    open Spectre.Console.Cli
    open System.ComponentModel
    open Output

    type RateSettings() =
        inherit CommandSettings()

        [<CommandOption("-w|--wordlist")>]
        [<Description("Path to the word list")>]
        member val wordList = "./word-lists/words.txt" with get, set

        [<CommandOption("-p|--progress")>]
        [<Description("Your current progress. Capital letters denote matching positions (green), lower-case letters occurences in the word (yellow)")>]
        member val state = "....." with get, set

        [<CommandOption("-c|--candidate")>]
        [<Description("The word you want to get the score of.")>]
        member val candidate = "" with get, set

        [<CommandOption("-e|--excluding")>]
        [<Description("The letters that have already been excluded")>]
        member val excluded = "" with get, set

    let printScore words (settings: RateSettings) : unit =
        let filteredWords = Domain.filterWords words settings.state settings.excluded

        match (filteredWords |> Seq.exists (fun word -> word = settings.candidate)) with        
        | true ->
            let occurenceMap = Domain.buildOccurenceMap filteredWords
            let max =
                filteredWords
                |> Seq.map (fun word -> (word, Domain.wordScore occurenceMap word))
                |> Seq.sortBy (fun pair -> -1 * (snd pair))
                |> Seq.head
                |> snd
            printMarkedUp $"The word {emphasize settings.candidate} scored {emphasize (Domain.wordScore occurenceMap settings.candidate)} points, out of possible {emphasize max}."
        | false -> printMarkedUp $"The candidate {emphasize settings.candidate} is not part of the possible words."

    type RateWord() =
        inherit Command<RateSettings>()
        interface ICommandLimiter<RateSettings>

        override _.Execute(_context, settings) =
            match settings.candidate.Length with
            | 5 ->
                match Load.getWords settings.wordList with
                | Some words -> printScore words settings
                | None -> warn "Error: Could not load word list." |> printMarkedUp
            | _ -> warn "The candidate word must have exactly 5 letters to be elegible." |> printMarkedUp
            0
