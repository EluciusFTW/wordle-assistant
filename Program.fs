open System
open Spectre.Console.Cli

[<EntryPoint>]
let main argv =

    let app = CommandApp()
    app.Configure(fun config ->
        config.AddCommand<Commands.Assist>("assist")
            .WithAlias("a")
            .WithDescription("Shows words matching the current state and excluded letters, ranked by score.")
            |> ignore

        config.AddCommand<Commands.FindWord>("find")
            .WithAlias("f")
            .WithDescription("Shows words containing given letters")
            |> ignore)

    app.Run(argv)
