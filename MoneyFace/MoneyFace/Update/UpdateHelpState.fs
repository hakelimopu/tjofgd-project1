module UpdateHelpState

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

let updateHelpState delta =
    let keyboardState = Keyboard.GetState()
    let buttons = GamePad.GetState(PlayerIndex.One) |> getGamePadButtons
    if keyboardState.IsKeyDown(Keys.Escape) || buttons.Contains(Buttons.B) || buttons.Contains(Buttons.Back) then
        TitleScreen
    else
        HelpState


