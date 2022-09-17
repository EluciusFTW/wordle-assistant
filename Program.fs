open Spectre.Console.Cli
open Wassi.Commands

[<EntryPoint>]
let main argv =

    let app = CommandApp()
    app.Configure(fun config ->
        config.AddCommand<SuggestWord>("suggest")
            .WithAlias("s")
            .WithDescription("Shows words matching the current progress and excluded letters, ranked by score.")
            |> ignore

        config.AddCommand<FindWord>("find")
            .WithAlias("f")
            .WithDescription("Shows words containing given letters.")
            |> ignore

        config.AddCommand<RateWord>("rate")
            .WithAlias("r")
            .WithDescription("Shows the score of a candidate word, thereby rating it.")
            |> ignore)

    app.Run(argv)
