open System

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
let filteredWords = Domain.filterWords words state discarded
let occurenceMap = Domain.buildOccurenceMap filteredWords

filteredWords 
|> Seq.map (fun word -> (word, Domain.wordScore occurenceMap word))
|> Seq.sortBy (fun pair -> -1*(snd pair))
|> Seq.truncate(20)
|> Seq.iter (fun pair -> printfn "The word '%s' scored '%i' points" (fst pair) (snd pair))
