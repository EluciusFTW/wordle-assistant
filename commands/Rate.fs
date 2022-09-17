namespace Wassi.Commands

open Spectre.Console.Cli
open System.ComponentModel
open Wassi.Domain
open Wassi.Output

type RateSettings() =
    inherit ProgressSettings()
    
    [<CommandOption("-c|--candidate")>]
    [<Description("The word you want to get the score of.")>]
    member val candidate = "" with get, set

module Print =
    let printScore words (settings: RateSettings) : unit =
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
            printMarkedUp $"The word {emphasize settings.candidate} scored {emphasize (wordScore occurenceMap settings.candidate)} points, out of possible {emphasize max}."
        | false -> printMarkedUp $"The candidate {emphasize settings.candidate} is not part of the possible words."

type RateWord() =
    inherit Command<RateSettings>()
    interface ICommandLimiter<RateSettings>

    override _.Execute(_context, settings) =
        match settings.candidate.Length with
        | 5 ->
            match Wassi.Load.getWords settings.wordList with
            | Some words -> Print.printScore words settings
            | None -> warn "Error: Could not load word list." |> printMarkedUp
        | _ -> warn "The candidate word must have exactly 5 letters to be elegible." |> printMarkedUp
        0
