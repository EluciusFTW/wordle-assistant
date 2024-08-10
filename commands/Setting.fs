namespace Wassi.Commands

open Spectre.Console.Cli
open System.ComponentModel

type WordListSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--wordlist")>]
    [<Description("Path to the word list")>]
    member val wordList = "../../../word-lists/words.txt" with get, set

type ProgressSettings() =
    inherit WordListSettings()

    [<CommandOption("-p|--progress")>]
    [<Description("Your current progress. Capital letters denote matching positions (green), lower-case letters occurences in the word (yellow)")>]
    member val progress = "....." with get, set

    [<CommandOption("-e|--excluding")>]
    [<Description("The letters that have already been excluded")>]
    member val excluded = "" with get, set
