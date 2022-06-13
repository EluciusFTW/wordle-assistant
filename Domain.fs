module Domain 
    open System
    open System.Collections.Generic

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
