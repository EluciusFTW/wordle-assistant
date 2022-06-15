namespace Commands

open Spectre.Console
module Output =

    let markup style content = $"[{style}]{content}[/]"
    let emphasize content = markup "green" content
    let warn content = markup "red" content

    let printMarkedUp content = AnsiConsole.Markup $"{content}{System.Environment.NewLine}"