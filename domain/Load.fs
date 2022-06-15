module Load

    let getWords path = 
        match System.IO.File.Exists path with
        | true -> Some (System.IO.File.ReadLines(path) |> Seq.toArray)
        | _ -> None
