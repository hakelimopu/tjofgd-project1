module GameOptions

type GameOptions = {Sfx:bool;Volume:float}

let mutable private gameOptions = {Sfx=true;Volume=1.0}

let loadGameOptions() =
    gameOptions

let saveGameOptions options =
    gameOptions <- options

let toggleSfx options =
    {options with Sfx=options.Sfx |> not}

let increaseVolume options =
    {options with Volume = if options.Volume >= 0.9 then 1.0 else options.Volume + 0.1}

let decreaseVolume options =
    {options with Volume = if options.Volume < 0.1 then 0.0 else options.Volume - 0.1}
    