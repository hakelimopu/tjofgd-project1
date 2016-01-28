module UpdateTitleScreen

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
open UpdateUtility

let updateTitleScreen delta (oldKeyboardState:KeyboardState) (oldGamePadState: GamePadState)=
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (keyPresses, buttons) = getInputChanges oldKeyboardState keyboardState oldGamePadState gamePadState
    if keyPresses.Contains(Keys.F1) || buttons.Contains(Buttons.X) then
        HelpState (keyboardState,gamePadState)
    elif keyPresses.Contains(Keys.F2) || buttons.Contains(Buttons.Start) then
        newGame()
    elif keyPresses.Contains(Keys.F3) || buttons.Contains(Buttons.A) then
        OptionsState (keyboardState,gamePadState)
    elif keyPresses.Contains(Keys.F4) || buttons.Contains(Buttons.Y) then
        (GameJoltApi.getScores(), keyboardState, gamePadState) |> HighScoreState
    elif keyPresses.Contains(Keys.F5) || buttons.Contains(Buttons.B) then
        AboutState (keyboardState,gamePadState)
    else
        TitleScreen (keyboardState,gamePadState)


