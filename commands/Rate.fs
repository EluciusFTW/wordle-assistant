namespace Wassi.Commands

open Spectre.Console.Cli
open SpectreCoff
open System.ComponentModel
open Wassi.Domain

type RateSettings() =
    inherit ProgressSettings()
    
    [<CommandOption("-c|--candidate")>]
    [<Description("The word you want to get the score of.")>]
    member val candidate = "" with get, set

module Print =
    let printScore words (settings: RateSettings) =
        let filteredWords = filterWords words settings.progress settings.excluded

        match (filteredWords |> Seq.exists (fun word -> word = settings.candidate)) with        
        | true ->
            let occurenceMap = buildOccurenceMap filteredWords
            let max =
                filteredWords
                |> Seq.map (fun word -> (word, wordScore occurenceMap word))
                |> Seq.sortBy (fun pair -> -1 * (snd pair))
                |> Seq.head
                |> snd
            Many [ 
                C "The word"
                P settings.candidate
                C"scored"
                P $"{wordScore occurenceMap settings.candidate}"
                C "points, out of possible"
                P $"{max}."
            ]
        | false -> Many [C "The candidate "; P settings.candidate; C " is not part of the possible words."] 

open Wassi.Load

type RateWord() =
    inherit Command<RateSettings>()
    interface ICommandLimiter<RateSettings>

    override _.Execute(_context, settings) =
        match settings.candidate.Length with
        | 5 ->
            match getWords settings.wordList with
            | Some words -> Print.printScore words settings
            | None -> E "Error: Could not load word list." 
        | _ -> E "The candidate word must have exactly 5 letters to be elegible." 
        |> toConsole
        0
