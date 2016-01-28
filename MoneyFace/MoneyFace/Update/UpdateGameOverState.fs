module UpdateGameOverState

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

let updateGameOverState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (k,b,u) = boardState |> getInputState keyboardState gamePadState
    if k.Contains(Keys.F2) || b.Contains(Buttons.Start) then
        newGame()
    elif k.Contains(Keys.Escape) || b.Contains(Buttons.Back) then
        TitleScreen (keyboardState, gamePadState)
    else
        boardState 
        |> u
        |> GameOverState


