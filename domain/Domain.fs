module Domain 
    open System
    open System.Collections.Generic

    let alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    let countWordsContainingLetter (words: seq<string>) (letter: char) =
        words
        |> Seq.filter (fun word -> word.Contains letter)
        |> Seq.length

    let buildOccurenceMap words = 
        alphabet
        |> Array.map (fun letter -> (letter, countWordsContainingLetter words letter))
        |> Seq.sortBy (fun pair -> -1*(snd pair))
        |> dict

    let hasSuitableCharacterAt position (word: string) (progress: string) = 
        let character = progress[position]
        match character with
        | c when Char.IsLetter(c) && Char.IsUpper(c) -> word[position] = Char.ToLower(character)
        | c when Char.IsLetter(c) && Char.IsLower(c) -> word[position] <> character && word.Contains(character)
        | _ -> true

    let wordsContaining (letters: string) (words: seq<string>) =
        words
        |> Seq.filter(fun word -> 
            letters 
            |> Seq.map(fun letter -> word.Contains(letter)) 
            |> Seq.contains false
            |> not)

    let filterWords (words: seq<string>) (progress: string) (discarded: string) =
        words
        |> Seq.filter (fun word -> 
            discarded 
            |> Seq.map(fun letter -> word.Contains(letter)) 
            |> Seq.contains true
            |> not)
        |> Seq.filter(fun word -> hasSuitableCharacterAt 0 word progress)
        |> Seq.filter(fun word -> hasSuitableCharacterAt 1 word progress)
        |> Seq.filter(fun word -> hasSuitableCharacterAt 2 word progress)
        |> Seq.filter(fun word -> hasSuitableCharacterAt 3 word progress)
        |> Seq.filter(fun word -> hasSuitableCharacterAt 4 word progress)

    let wordScore (occurenceMap: IDictionary<char, int>) word = 
        word
        |> Seq.distinct
        |> Seq.map (fun character -> occurenceMap[character])
        |> Seq.sum
