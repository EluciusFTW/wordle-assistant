open System.Collections.Generic
open System

let alphabet = "abcdefghijklmnopqrstuvwxyzu".ToCharArray();

let countWordsContainingLetter (words: seq<string>) (letter: char) =
    words
    |> Seq.filter (fun word -> word.Contains letter)
    |> Seq.length

let buildOccurenceMap words = 
    alphabet
    |> Array.map (fun letter -> (letter, countWordsContainingLetter words letter))
    |> Seq.sortBy (fun pair -> -1*(snd pair))
    |> dict

let filterAtCharacterPosition position (word: string) (state: string) = 
    let character = state[position]
    match character with
    | c when Char.IsLetter(c) && Char.IsUpper(c) -> word[position] = Char.ToLower(character)
    | c when Char.IsLetter(c) && Char.IsLower(c) -> word[position] <> character && word.Contains(character)
    | _ -> true

let filterWords (words: seq<string>) (state: string) (discarded: string) =
    words
    |> Seq.filter(fun word -> 
        discarded 
        |> Seq.map(fun d -> word.Contains(d)) 
        |> Seq.exists(fun value -> value) 
        |> not)
    |> Seq.filter(fun word -> filterAtCharacterPosition 0 word state)
    |> Seq.filter(fun word -> filterAtCharacterPosition 1 word state)
    |> Seq.filter(fun word -> filterAtCharacterPosition 2 word state)
    |> Seq.filter(fun word -> filterAtCharacterPosition 3 word state)
    |> Seq.filter(fun word -> filterAtCharacterPosition 4 word state)

let wordScore (occurenceMap: IDictionary<char, int>) word = 
    word
    |> Seq.distinct
    |> Seq.map (fun character -> occurenceMap[character])
    |> Seq.sum

let args = Environment.GetCommandLineArgs();
printfn "Welcome to Wassi, your Wordle-Assistant!"
if (args.Length < 2 || args[1].Length <> 5 || args.Length > 3 ) then
    printfn "Please check your input, it must be of the format: .\\wassi.exe <STATE> <DISCARDED: optional>"
    printfn "  * where <STATE> is you current information. Capital letters mean exact hits (green),"
    printfn "    lower case letters hits at wrong positions (yellow)."
    printfn "    To indicate nothing (gray), you can use any non-letter character."
    printfn "    The value must be exactly 5 characters long."
    printfn "  * and <DISCARDED> is a list of letters you have already ruled out."
    exit -1

let state = args[1]
let discarded = 
    match args.Length with
    | 3 -> args[2]
    | _ -> String.Empty

let words = System.IO.File.ReadLines("./word-lists/words.txt") |> Seq.toArray
let filteredWords = filterWords words state discarded
let occurenceMap = buildOccurenceMap filteredWords

filteredWords 
|> Seq.map (fun word -> (word, wordScore occurenceMap word))
|> Seq.sortBy (fun pair -> -1*(snd pair))
|> Seq.truncate(20)
|> Seq.iter (fun pair -> printfn "The word '%s' scored '%i' points" (fst pair) (snd pair))
