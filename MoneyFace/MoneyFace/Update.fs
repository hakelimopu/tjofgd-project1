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
open TJoFGDGame

let clampAvatar boardState = 
    let x,y = boardState.Player
    let clamped = 
        (
        match x with
        | v when v < 0.0<px> -> 0.0<px>
        | v when v >= boardColumns * pixelsPerCell -> (boardColumns * pixelsPerCell - 1.0<cell> * pixelsPerCell)
        | _ -> x
        ,
        match y with
        | v when v < 0.0<px> -> 0.0<px>
        | v when v >= boardRows * pixelsPerCell -> (boardRows * pixelsPerCell - 1.0<cell> * pixelsPerCell)
        | _ -> y
        )
    {boardState with Player=clamped}

//TODO: please clean me up!
let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    if boardState.KeyboardState.IsKeyUp(Keys.Space) && keyboardState.IsKeyDown(Keys.Space) then
        boardState
        |> updateKeyboardState keyboardState
        |> updateGamePadState gamePadState
        |> PausedState
    else
        let newBoardState = 
            boardState
            |> clearEvents
            |> moveAvatar delta keyboardState
            |> clampAvatar 
            |> updateKeyboardState keyboardState
            |> updateGamePadState gamePadState
            |> decreaseTime delta
        if newBoardState.TimeRemaining <= 0.0<second> then
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

let updatePausedState delta boardState = 
    let keyboardState = Keyboard.GetState()
    if boardState.KeyboardState.IsKeyUp(Keys.Space) && keyboardState.IsKeyDown(Keys.Space) then
        boardState
        |> updateKeyboardState keyboardState
        |> PlayState
    else
        boardState 
        |> updateKeyboardState keyboardState
        |> PausedState

let updateTitleScreen delta =
    let keyboardState = Keyboard.GetState()
    if keyboardState.IsKeyDown(Keys.F1) then
        HelpState
    elif keyboardState.IsKeyDown(Keys.F2) then
        newGame()
    elif keyboardState.IsKeyDown(Keys.F3) then
        OptionsState
    elif keyboardState.IsKeyDown(Keys.F4) then
        HighScoreState
    else
        TitleScreen
    
let updateHelpState delta =
    let keyboardState = Keyboard.GetState()
    if keyboardState.IsKeyDown(Keys.Escape) then
        TitleScreen
    else
        HelpState
    
let updateOptionsState delta =
    let keyboardState = Keyboard.GetState()
    if keyboardState.IsKeyDown(Keys.Escape) then
        TitleScreen
    else
        OptionsState
    
let updateHighScoreState delta =
    let keyboardState = Keyboard.GetState()
    if keyboardState.IsKeyDown(Keys.Escape) then
        TitleScreen
    else
        HighScoreState
    

let updateGame delta =
    match loadGameState() with
    | TitleScreen -> updateTitleScreen delta
    | HelpState -> updateHelpState delta
    | OptionsState -> updateOptionsState delta
    | HighScoreState -> updateHighScoreState delta
    | PlayState boardState -> boardState |> updatePlayState delta
    | PausedState boardState -> boardState |> updatePausedState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
