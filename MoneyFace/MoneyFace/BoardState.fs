module BoardState

open System
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

let boardColumns = 20
let boardRows = 20

let random = new Random()

let randomBoardPosition () =
    (random.Next(boardColumns), random.Next(boardRows))

type BoardState =
    {Player: int*int;
    Dollar: int*int;
    Score: int;
    KeyboardState: KeyboardState;
    TimeRemaining: TimeSpan}

type GameState = 
    | PlayState of BoardState
    | GameOverState of BoardState

let mutable private gameState =
    {Player=randomBoardPosition ();
    Dollar=randomBoardPosition (); 
    Score=0; 
    KeyboardState = Keyboard.GetState();
    TimeRemaining = new TimeSpan(0,0,60)}
    |> PlayState

let loadGameState () =
    gameState

let saveGameState newBoardState =
    gameState <- newBoardState

let updateKeyboardState keyboardState boardState =
    {boardState with KeyboardState = keyboardState}

let addLocation first second = 
    let firstX, firstY = first
    let secondX, secondY = second
    (firstX + secondX, firstY + secondY)

let movementTable =
    [(Keys.Up,    ( 0, -1));
     (Keys.Down,  ( 0,  1));
     (Keys.Left,  (-1,  0));
     (Keys.Right, ( 1,  0))]
    |> Map.ofSeq

let movementKeys = 
    movementTable
    |> Map.toSeq
    |> Seq.map (fun (k,v)->k)

let lookUpDeltaForKey (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) key =
    if movementTable.ContainsKey(key) && oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key) then 
        movementTable.[key] 
    else 
        (0,0)

let determineDelta (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) =
    movementKeys
    |> Seq.map (lookUpDeltaForKey oldKeyboardState keyboardState)
    |> Seq.reduce addLocation

let eatDollar boardState = 
    if boardState.Player = boardState.Dollar then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
    else
        boardState

let moveAvatar (keyboardState :KeyboardState) boardState =
    let oldKeyboardState = boardState.KeyboardState
    let delta = keyboardState |> determineDelta boardState.KeyboardState
    {boardState with Player=delta |> addLocation boardState.Player}
    |> eatDollar

let decreaseTime (delta: GameTime) boardState =
    {boardState with TimeRemaining = boardState.TimeRemaining - delta.ElapsedGameTime}
