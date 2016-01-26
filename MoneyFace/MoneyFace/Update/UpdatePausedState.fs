module UpdatePausedState

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


