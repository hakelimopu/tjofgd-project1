open System
open BoardState
open System.Threading

let initializeConsole () =
    Console.Title <- "Money Face $ :)"
    Console.WindowWidth <- 40
    Console.WindowHeight <- 25
    Console.BufferWidth <- Console.WindowWidth
    Console.BufferHeight <- Console.WindowHeight
    Console.CursorVisible <- false

let renderBoardState (boardState:BoardState) =
    Console.Clear()
    let playerX, playerY = boardState.Player
    Console.CursorLeft <- playerX
    Console.CursorTop <- playerY
    Console.Write ("@")
    let dollarX, dollarY = boardState.Dollar
    Console.CursorLeft <- dollarX
    Console.CursorTop <- dollarY
    Console.Write ("$")
    Console.CursorLeft <- boardColumns
    Console.CursorTop <- 0
    Console.Write (boardState.Score |> sprintf "%i")
    Thread.Sleep(50)
    boardState

type GameState =
    | Play
    | Quit

let mutable gameState: GameState = Play

let readKey () =
    if Console.KeyAvailable then
        Console.ReadKey(true).Key
        |> Some
    else
        None

let eatDollar boardState = 
    if boardState.Player = boardState.Dollar then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
    else
        boardState

let processInput boardState =
    let deltaX, deltaY =
        match readKey() with
        | Some ConsoleKey.UpArrow -> (0, -1)
        | Some ConsoleKey.DownArrow -> (0, 1)
        | Some ConsoleKey.LeftArrow -> (-1 ,0)
        | Some ConsoleKey.RightArrow -> (1 ,0)
        | _ -> (0,0)
    let newPlayer = ((boardState.Player |> fst) + deltaX, (boardState.Player |> snd) + deltaY)
    {boardState with Player = newPlayer}
    |> eatDollar

[<EntryPoint>]
let main argv = 
    initializeConsole()
    while gameState <> Quit do
        loadBoardState()
        |> renderBoardState
        |> processInput
        |> saveBoardState
    0

//Avatar: smiley face \u0002
//Time attack: 60 second timer
//Goal: get points, $
//Wrinkle: manage mood. 
//Heart \u0003 will put you in a good mood
//Good mood means you move faster
//Bad mood means you move slower
//Hearts cost $
//Must rest or you will also slow down.
//Rest is represented by a Z

