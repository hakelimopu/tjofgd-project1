module Update

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Audio
open BoardState
open System
open AssetType
open Assets

let clampAvatar boardState = 
    let x,y = boardState.Player
    let clamped = 
        (
        match x with
        | v when v < 0 -> 0
        | v when v >= BoardState.boardColumns -> (boardColumns - 1)
        | _ -> x
        ,
        match y with
        | v when v < 0 -> 0
        | v when v >= BoardState.boardColumns -> (boardRows - 1)
        | _ -> y
        )
    {boardState with Player=clamped}

let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let newBoardState = 
        boardState
        |> clearEvents
        |> moveAvatar keyboardState
        |> clampAvatar 
        |> updateKeyboardState keyboardState
        |> decreaseTime delta
    if newBoardState.TimeRemaining.TotalSeconds <= 0. then
        newBoardState |> GameOverState
    else
        newBoardState |> PlayState


let updateGameOverState delta boardState = 
    let keyboardState = Keyboard.GetState()
    if boardState.KeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2) then
        newGame()
    else
        boardState 
        |> updateKeyboardState keyboardState
        |> GameOverState

let updateTitleScreen delta =
    let keyboardState = Keyboard.GetState()
    if keyboardState.IsKeyDown(Keys.F2) then
        newGame()
    else
        TitleScreen
    

let updateGame delta =
    match loadGameState() with
    | TitleScreen -> updateTitleScreen delta
    | PlayState boardState -> boardState |> updatePlayState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
