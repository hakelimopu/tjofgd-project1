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

let updateTitleScreen delta =
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let buttons = gamePadState |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.F1) || buttons.Contains(Buttons.X) then
        HelpState
    elif keyboardState.IsKeyDown(Keys.F2) || buttons.Contains(Buttons.Start) then
        newGame()
    elif keyboardState.IsKeyDown(Keys.F3) || buttons.Contains(Buttons.A) then
        OptionsState (keyboardState,gamePadState)
    elif keyboardState.IsKeyDown(Keys.F4) || buttons.Contains(Buttons.Y) then
        GameJoltApi.getScores() |> HighScoreState
    elif keyboardState.IsKeyDown(Keys.F5) || buttons.Contains(Buttons.B) then
        AboutState
    else
        TitleScreen


