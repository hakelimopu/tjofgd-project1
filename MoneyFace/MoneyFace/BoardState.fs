module BoardState

open System
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework
open TJoFGDGame

[<Measure>]type cell

let boardColumns = 20.0<cell>
let boardRows = 20.0<cell>

let pixelsPerCell = 36.0<px/cell>


let random = new Random()

let randomBoardPosition () =
    (random.NextDouble() * boardColumns * pixelsPerCell, random.NextDouble() * boardRows * pixelsPerCell)

type BoardEvent =
    | PickUpDollar

type BoardState =
    {Player: float<px>*float<px>;
    Dollar: float<px>*float<px>;
    Score: int;
    KeyboardState: KeyboardState;
    BoardEvents: Set<BoardEvent>;
    TimeRemaining: float<seconds>}

type GameState = 
    | TitleScreen
    | PlayState of BoardState
    | GameOverState of BoardState

let newGame () = 
    {Player=randomBoardPosition ();
    Dollar=randomBoardPosition (); 
    Score=0; 
    KeyboardState = Keyboard.GetState();
    BoardEvents = Set.empty;
    TimeRemaining = 60.0<seconds>}
    |> PlayState

let mutable private gameState =
    TitleScreen

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
    [(Keys.Up,    ( 0.0<px>, -pixelsPerCell * 1.0<cell>));
     (Keys.Down,  ( 0.0<px>,  pixelsPerCell * 1.0<cell>));
     (Keys.Left,  (-pixelsPerCell * 1.0<cell>,  0.0<px>));
     (Keys.Right, ( pixelsPerCell * 1.0<cell>,  0.0<px>))]
    |> Map.ofSeq

let movementKeys = 
    movementTable
    |> Map.toSeq
    |> Seq.map (fun (k,v)->k)

let lookUpDeltaForKey (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) key =
    if movementTable.ContainsKey(key) && oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key) then 
        movementTable.[key] 
    else 
        (0.0<px>,0.0<px>)

let determineDelta (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) =
    movementKeys
    |> Seq.map (lookUpDeltaForKey oldKeyboardState keyboardState)
    |> Seq.reduce addLocation

let addEvent event boardState =
    {boardState with BoardEvents = event |> boardState.BoardEvents.Add}

let distance (firstX:float<px>,firstY:float<px>) (secondX:float<px>,secondY:float<px>) =
    let deltaX = firstX - secondX
    let deltaY = firstY - secondY
    deltaX * deltaX + deltaY * deltaY
    |> sqrt

let eatDollar boardState = 
    if (boardState.Player |> distance boardState.Dollar) / pixelsPerCell < 1.0<cell> then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
        |> addEvent PickUpDollar
    else
        boardState

let moveAvatar (keyboardState :KeyboardState) boardState =
    let oldKeyboardState = boardState.KeyboardState
    let delta = keyboardState |> determineDelta boardState.KeyboardState
    {boardState with Player=delta |> addLocation boardState.Player}
    |> eatDollar

let decreaseTime (delta: float<seconds>) boardState =
    {boardState with TimeRemaining = boardState.TimeRemaining - delta}


let clearEvents boardState = 
    {boardState with BoardEvents = Set.empty}

