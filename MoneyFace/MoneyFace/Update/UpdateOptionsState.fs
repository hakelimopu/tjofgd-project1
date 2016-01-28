module UpdateOptionsState

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
open GameOptions

let updateOptionsState delta (oldKeyboardState:KeyboardState) (oldGamePadState: GamePadState) =
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    if (oldKeyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyDown(Keys.Left)) || (oldGamePadState.IsButtonUp(Buttons.X) && gamePadState.IsButtonDown(Buttons.X)) then
        loadGameOptions()
        |> decreaseVolume
        |> saveGameOptions
        OptionsState (keyboardState,gamePadState)
    elif (oldKeyboardState.IsKeyUp(Keys.Right) && keyboardState.IsKeyDown(Keys.Right)) || (oldGamePadState.IsButtonUp(Buttons.Y) && gamePadState.IsButtonDown(Buttons.Y)) then
        loadGameOptions()
        |> increaseVolume
        |> saveGameOptions
        OptionsState (keyboardState,gamePadState)
    elif (oldKeyboardState.IsKeyUp(Keys.Space) && keyboardState.IsKeyDown(Keys.Space)) || (oldGamePadState.IsButtonUp(Buttons.A) && gamePadState.IsButtonDown(Buttons.A)) then
        loadGameOptions()
        |> toggleSfx
        |> saveGameOptions
        OptionsState (keyboardState,gamePadState)
    elif (oldKeyboardState.IsKeyUp(Keys.Escape) && keyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState.IsButtonUp(Buttons.B) && gamePadState.IsButtonDown(Buttons.B)) then
        TitleScreen (keyboardState,gamePadState)
    else
        OptionsState (keyboardState,gamePadState)


