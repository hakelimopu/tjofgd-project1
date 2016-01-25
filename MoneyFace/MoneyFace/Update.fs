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
open Constants

let clampDimension minimum maximum dimension =
    if dimension < minimum then
        minimum
    elif dimension > maximum then
        maximum
    else
        dimension

let clampAvatar boardState = 
    {boardState with Player=(boardState.Player |> fst |> clampDimension minimumX maximumX, boardState.Player |> snd |> clampDimension minimumY maximumY)}

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

let getKeyboardKeys (keyboardState:KeyboardState) = 
    [Keys.Space;
    Keys.Escape;
    Keys.F2]    
    |> Seq.filter (fun k -> keyboardState.IsKeyDown(k))
    |> Set.ofSeq

let getKeyboardPresses (oldKeyboardState:KeyboardState) (newKeyboardState: KeyboardState) =
    let oldKeys = oldKeyboardState |> getKeyboardKeys
    newKeyboardState
    |> getKeyboardKeys
    |> Set.fold (fun state key->if oldKeys.Contains(key) then state else state |> Set.add(key)) Set.empty<Keys>

let getInputState keyboardState gamePadState boardState =
    let keysPressed = (boardState.KeyboardState, keyboardState) ||> getKeyboardPresses
    let buttons = (boardState.GamePadState, gamePadState) ||> getGamePadButtonPresses
    let updateInputDevices = updateKeyboardState keyboardState >> updateGamePadState gamePadState
    (keysPressed,buttons,updateInputDevices)

//start trophies

let private trophiesList =
    [(DollarCounter,25,49005);
    (DollarCounter,50,49137);
    (DollarCounter,100,49138);
    (DollarCounter,200,49139)]

let private achieveTrophy boardState (counter,goal,trophyId) =
    if boardState |> getCounter counter >= goal then
        trophyId |> GameJoltApi.addAchieved
    else
        ()

let achieveTrophies (boardState: BoardState) =
    trophiesList
    |> List.iter(achieveTrophy boardState)

//end trophies

let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (k,b,u) = boardState |> getInputState keyboardState gamePadState

    if k.Contains(Keys.Space) || b.Contains(Buttons.B)  then
        boardState
        |> u
        |> PausedState
    else
        let newBoardState = 
            boardState
            |> clearEvents
            |> moveAvatar delta gamePadState keyboardState
            |> clampAvatar 
            |> u
            |> decreaseTimes delta
        if newBoardState.TimesRemaining.[Main] <= 0.0<second> then
            newBoardState.Score |> GameJoltApi.addScore 
            newBoardState |> achieveTrophies
            newBoardState |> GameOverState
        else
            newBoardState |> PlayState


let updateGameOverState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (k,b,u) = boardState |> getInputState keyboardState gamePadState
    if k.Contains(Keys.F2) || b.Contains(Buttons.Start) then
        newGame()
    elif k.Contains(Keys.Escape) || b.Contains(Buttons.Back) then
        TitleScreen
    else
        boardState 
        |> u
        |> GameOverState

let updatePausedState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (k,b,u) = boardState |> getInputState keyboardState gamePadState
    boardState
    |> u
    |> if k.Contains(Keys.Space) || b.Contains(Buttons.B) then
        PlayState
       else
        PausedState

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
        GameJoltApi.getScores() |> HighScoreState
    elif keyboardState.IsKeyDown(Keys.F5) || buttons.Contains(Buttons.B) then
        AboutState
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
    
let updateAboutState delta =
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
        TitleScreen
    else
        AboutState
    
let updateHighScoreState delta highScores=
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
        TitleScreen
    else
        HighScoreState highScores
    

let updateGame delta =
    match loadGameState() with
    | TitleScreen -> updateTitleScreen delta
    | HelpState -> updateHelpState delta
    | OptionsState -> updateOptionsState delta
    | AboutState -> updateAboutState delta
    | HighScoreState highScores -> updateHighScoreState delta highScores
    | PlayState boardState -> boardState |> updatePlayState delta
    | PausedState boardState -> boardState |> updatePausedState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
