module BoardState

open System
open Microsoft.Xna.Framework.Input

let boardColumns = 22
let boardRows = 22

let random = new Random()

let randomBoardPosition () =
    (random.Next(boardColumns), random.Next(boardRows))

type BoardState =
    {Player: int*int;
    Dollar: int*int;
    Score: int;
    KeyboardState: KeyboardState}

let mutable private boardState = 
    {Player=randomBoardPosition ();
    Dollar=randomBoardPosition (); 
    Score=0; 
    KeyboardState = Keyboard.GetState()}

let loadBoardState () =
    boardState

let saveBoardState newBoardState =
    boardState <- newBoardState

let updateKeyboardState keyboardState boardState =
    {boardState with KeyboardState = keyboardState}

let moveAvatar (keyboardState :KeyboardState) boardState =
    let oldKeyboardState = boardState.KeyboardState
    let delta = 
        if oldKeyboardState.IsKeyUp(Keys.Up) && keyboardState.IsKeyDown(Keys.Up) then
            (0, -1)
        elif oldKeyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyDown(Keys.Down) then
            (0, 1)
        elif oldKeyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyDown(Keys.Left) then
            (-1, 0)
        elif oldKeyboardState.IsKeyUp(Keys.Right) && keyboardState.IsKeyDown(Keys.Right) then
            (1, 0)
        else
            (0,0)
    {boardState with Player=((boardState.Player|> fst) + (delta |> fst), (boardState.Player |> snd) + (delta |> snd))}

