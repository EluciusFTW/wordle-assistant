namespace Commands

module Rate =
    open Spectre.Console.Cli
    open System.ComponentModel
    open Output

    type RateSettings() =
        inherit Setting.ProgressSettings()
      
        [<CommandOption("-c|--candidate")>]
        [<Description("The word you want to get the score of.")>]
        member val candidate = "" with get, set

    let printScore words (settings: RateSettings) : unit =
        let filteredWords = Domain.filterWords words settings.progress settings.excluded

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
