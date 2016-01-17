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

let addGamePadButton (button:Buttons) (gamePadState:GamePadState) (set:Set<Buttons>) = 
    if gamePadState.IsConnected && gamePadState.IsButtonDown(button) then
        set
        |> Set.add(button)
    else
        set

let getGamePadButtons (gamePadState:GamePadState) = 
    Set.empty<Buttons>
    |> addGamePadButton Buttons.Start gamePadState
    |> addGamePadButton Buttons.A gamePadState
    |> addGamePadButton Buttons.B gamePadState
    |> addGamePadButton Buttons.X gamePadState
    |> addGamePadButton Buttons.Y gamePadState
    |> addGamePadButton Buttons.Back gamePadState

let getGamePadButtonPresses (oldGamePadState:GamePadState) (newGamePadState: GamePadState) =
    let oldButtons = oldGamePadState |> getGamePadButtons
    newGamePadState 
    |> getGamePadButtons
    |> Set.fold (fun state button->if oldButtons.Contains(button) then state else state |> Set.add(button)) Set.empty<Buttons>

//TODO: please clean me up!
let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let buttons = (boardState.GamePadState, gamePadState) ||> getGamePadButtonPresses
    if (boardState.KeyboardState.IsKeyUp(Keys.Space) && keyboardState.IsKeyDown(Keys.Space)) || buttons.Contains(Buttons.B)  then
        boardState
        |> updateKeyboardState keyboardState
        |> updateGamePadState gamePadState
        |> PausedState
    else
        let newBoardState = 
            boardState
            |> clearEvents
            |> moveAvatar delta gamePadState keyboardState
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
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let buttons = (boardState.GamePadState, gamePadState) ||> getGamePadButtonPresses
    if (boardState.KeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2)) || buttons.Contains(Buttons.Start) then
        newGame()
    else
        boardState 
        |> updateKeyboardState keyboardState
        |> updateGamePadState gamePadState
        |> GameOverState

let updatePausedState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let buttons = (boardState.GamePadState, gamePadState) ||> getGamePadButtonPresses
    if (boardState.KeyboardState.IsKeyUp(Keys.Space) && keyboardState.IsKeyDown(Keys.Space)) || buttons.Contains(Buttons.B) then
        boardState
        |> updateKeyboardState keyboardState
        |> updateGamePadState gamePadState
        |> PlayState
    else
        boardState 
        |> updateKeyboardState keyboardState
        |> updateGamePadState gamePadState
        |> PausedState

let updateTitleScreen delta =
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let buttons = gamePadState |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.F1) || buttons.Contains(Buttons.X) then
        HelpState
    elif keyboardState.IsKeyDown(Keys.F2) || buttons.Contains(Buttons.Start) then
        newGame()
    elif keyboardState.IsKeyDown(Keys.F3) || buttons.Contains(Buttons.A) then
        OptionsState
    elif keyboardState.IsKeyDown(Keys.F4) || buttons.Contains(Buttons.Y) then
        HighScoreState
    else
        TitleScreen
    
let updateHelpState delta =
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
        TitleScreen
    else
        HelpState
    
let updateOptionsState delta =
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
        TitleScreen
    else
        OptionsState
    
let updateHighScoreState delta =
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
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
