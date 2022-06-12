open System.Collections.Generic
open System

let alphabet = "abcdefghijklmnopqrstuvwxyzu".ToCharArray();

let numberOfLettersInWord (word: string) letter =
    word 
    |> Seq.countBy (fun character -> character = letter)
    |> Seq.filter (fun pair -> fst pair)
    |> Seq.tryHead
    |> fun pair -> 
        match pair with
        | Some pair -> snd pair
        | None -> 0
    
let numberOfWordsWithletter (words: seq<string>) (letter: char) =
    words
    |> Seq.filter (fun word -> word.Contains letter)
    |> Seq.length

let occurenceMap words = 
    alphabet
    |> Array.map (fun letter -> (letter, numberOfWordsWithletter words letter))
    |> Seq.sortBy (fun pair -> -1*(snd pair))
    |> dict

let characterFilter (word: string) (state: string) position = 
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
    |> Seq.filter(fun word -> characterFilter word state 0)
    |> Seq.filter(fun word -> characterFilter word state 1)
    |> Seq.filter(fun word -> characterFilter word state 2)
    |> Seq.filter(fun word -> characterFilter word state 3)
    |> Seq.filter(fun word -> characterFilter word state 4)

let wordScore (occurenceMap: IDictionary<char, int>) word = 
    word
    |> Seq.distinct
    |> Seq.map (fun character -> occurenceMap[character])
    |> Seq.sum

printfn "Welcome to Wassi, your Wordle-Assistant!"
if Environment.GetCommandLineArgs().Length <> 3 then
    printfn "Please check your input, it must be of the format: .\\wassi.exe <STATE> <DISCARDED>"
    printfn "  - where <STATE> is you current information. Capital letters mean exact hits (green),"
    printfn "    lower case letters hits at wrong positions (yellow)."
    printfn "    To indicate nothing (gray), you can use any non-letter character."
    printfn "  - and <DISCARDED> is a list of letters you have already ruled out."
    exit -1
               
let state = Environment.GetCommandLineArgs()[1]
let discarded = Environment.GetCommandLineArgs()[2]
let words = System.IO.File.ReadLines("./word-lists/words.txt")

let filteredWords = filterWords words state discarded
let currentLetterCount = occurenceMap filteredWords

filteredWords
|> Seq.map (fun word -> (word, wordScore currentLetterCount word))
|> Seq.sortBy (fun pair -> -1*(snd pair))
|> Seq.truncate(20)
|> Seq.iter (fun pair -> printfn "The word '%s' scored '%i' points" (fst pair) (snd pair))
