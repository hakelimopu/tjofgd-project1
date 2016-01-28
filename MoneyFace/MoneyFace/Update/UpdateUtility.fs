module UpdateUtility

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
    Keys.F1;
    Keys.F2;
    Keys.F3;
    Keys.F4;
    Keys.F5;
    Keys.Left;
    Keys.Right]    
    |> Seq.filter (fun k -> keyboardState.IsKeyDown(k))
    |> Set.ofSeq

let getKeyboardPresses (oldKeyboardState:KeyboardState) (newKeyboardState: KeyboardState) =
    let oldKeys = oldKeyboardState |> getKeyboardKeys
    newKeyboardState
    |> getKeyboardKeys
    |> Set.fold (fun state key->if oldKeys.Contains(key) then state else state |> Set.add(key)) Set.empty<Keys>

let getInputChanges oldKeyboardState keyboardState oldGamePadState gamePadState =
    let keysPressed = (oldKeyboardState, keyboardState) ||> getKeyboardPresses
    let buttons = (oldGamePadState, gamePadState) ||> getGamePadButtonPresses
    (keysPressed,buttons)

let getInputState keyboardState gamePadState boardState =
    let (keysPressed,buttons) = getInputChanges boardState.KeyboardState keyboardState boardState.GamePadState gamePadState
    let updateInputDevices = updateKeyboardState keyboardState >> updateGamePadState gamePadState
    (keysPressed,buttons,updateInputDevices)


