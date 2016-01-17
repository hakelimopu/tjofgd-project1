module BoardState

open System
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework
open TJoFGDGame

[<Measure>]type cell

let boardColumns = 20.0<cell>
let boardRows = 20.0<cell>

let pixelsPerCell = 36.0<px/cell>
let normalMovementRate = 5.0<cell/second>

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
    GamePadState: GamePadState;
    BoardEvents: Set<BoardEvent>;
    TimeRemaining: float<second>}

type GameState = 
    | TitleScreen
    | PlayState of BoardState
    | GameOverState of BoardState
    | HelpState
    | OptionsState
    | HighScoreState
    | PausedState of BoardState

let newGame () = 
    {Player=randomBoardPosition ();
    Dollar=randomBoardPosition (); 
    Score=0; 
    KeyboardState = Keyboard.GetState();
    GamePadState = GamePad.GetState(PlayerIndex.One);
    BoardEvents = Set.empty;
    TimeRemaining = 60.0<second>}
    |> PlayState

let mutable private gameState =
    TitleScreen

let loadGameState () =
    gameState

let saveGameState newBoardState =
    gameState <- newBoardState

let updateKeyboardState keyboardState boardState =
    {boardState with KeyboardState = keyboardState}

let updateGamePadState gamePadState boardState =
    {boardState with GamePadState = gamePadState}

let addLocation (firstX:float<'u>, firstY:float<'u>) (secondX:float<'u>, secondY:float<'u>) = 
    (firstX + secondX, firstY + secondY)

let movementTable =
    [(Keys.Up,    ( 0.0<1/second>, -1.0<1/second>));
     (Keys.Down,  ( 0.0<1/second>,  1.0<1/second>));
     (Keys.Left,  (-1.0<1/second>,  0.0<1/second>));
     (Keys.Right, ( 1.0<1/second>,  0.0<1/second>))]
    |> Map.ofSeq

let movementKeys = 
    movementTable
    |> Map.toSeq
    |> Seq.map (fun (k,v)->k)

let lookUpDeltaForKey (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) key =
    if movementTable.ContainsKey(key) && keyboardState.IsKeyDown(key) then 
        movementTable.[key] 
    else 
        (0.0<1/second>,0.0<1/second>)

let distance (firstX:float<'u>,firstY:float<'u>) (secondX:float<'u>,secondY:float<'u>) =
    let deltaX = firstX - secondX
    let deltaY = firstY - secondY
    deltaX * deltaX + deltaY * deltaY
    |> sqrt

let normalize (x:float<_>,y:float<_>) =
    let magnitude = 
        x * x + y * y
        |> sqrt
        |> float
    if magnitude <> 0.0 then
        (x/magnitude,y/magnitude)
    else
        (x,y)

let determineDelta (oldKeyboardState :KeyboardState) (keyboardState :KeyboardState) =
    movementKeys
    |> Seq.map (lookUpDeltaForKey oldKeyboardState keyboardState)
    |> Seq.reduce addLocation
    |> normalize

let addEvent event boardState =
    {boardState with BoardEvents = event |> boardState.BoardEvents.Add}

let eatDollar boardState = 
    if (boardState.Player |> distance boardState.Dollar) / pixelsPerCell < 1.0<cell> then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
        |> addEvent PickUpDollar
    else
        boardState

let moveAvatar (delta:float<second>) (keyboardState :KeyboardState) boardState =
    let oldKeyboardState = boardState.KeyboardState
    let (velocityX,velocityY) = keyboardState |> determineDelta boardState.KeyboardState
    let unitDistance = normalMovementRate * pixelsPerCell * 1.0<second>
    let velocity = (unitDistance * velocityX * delta, unitDistance * velocityY * delta)
    {boardState with Player= addLocation boardState.Player velocity}
    |> eatDollar

let decreaseTime (delta: float<second>) boardState =
    {boardState with TimeRemaining = boardState.TimeRemaining - delta}


let clearEvents boardState = 
    {boardState with BoardEvents = Set.empty}

