open System
open BoardState
open System.Threading

type GameState =
    | Play
    | Quit

let mutable gameState: GameState = Play

let eatDollar boardState = 
    if boardState.Player = boardState.Dollar then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
    else
        boardState

[<EntryPoint>]
let main argv = 
    let game = new MyGame.MyGame<MyGame.TextureId>(MyGame.loadTextures, MyGame.drawGame)
    game.Run()
    0
