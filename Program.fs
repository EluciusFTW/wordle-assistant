open Spectre.Console.Cli
open Commands

[<EntryPoint>]
let main argv =

    let app = CommandApp()
    app.Configure(fun config ->
        config.AddCommand<Suggest.SuggestWord>("suggest")
            .WithAlias("s")
            .WithDescription("Shows words matching the current state and excluded letters, ranked by score.")
            |> ignore

        config.AddCommand<Find.FindWord>("find")
            .WithAlias("f")
            .WithDescription("Shows words containing given letters.")
            |> ignore

        config.AddCommand<Rate.RateWord>("rate")
            .WithAlias("r")
            .WithDescription("Shows the score of a candidate word, thereby rating it.")
            |> ignore)

    app.Run(argv)
