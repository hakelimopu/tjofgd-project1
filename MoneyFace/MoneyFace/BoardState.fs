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
let heartCost = 5
let moodTime = 15.0<second>
let freezeCost = 10
let freezeTime = 60.0<second>

let random = new Random()

let randomBoardPosition () =
    (random.NextDouble() * boardColumns * pixelsPerCell, random.NextDouble() * boardRows * pixelsPerCell)

type BoardEvent =
    | PickUpDollar
    | PickUpHeart
    | MoodEffectOver
    | PickUpFreeze
    | FreezeEffectOver

type BoardState =
    {Player: float<px>*float<px>;
    Dollar: float<px>*float<px>;
    Heart: (float<px> * float<px>) option;
    Freeze: (float<px> * float<px>) option;
    Score: int;
    KeyboardState: KeyboardState;
    GamePadState: GamePadState;
    BoardEvents: Set<BoardEvent>;
    TimeRemaining: float<second>;
    FreezeTimeRemaining: float<second>;
    MoodTimeRemaining: float<second>}

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
    Heart = None;
    Freeze = None;
    KeyboardState = Keyboard.GetState();
    GamePadState = GamePad.GetState(PlayerIndex.One);
    BoardEvents = Set.empty;
    TimeRemaining = 60.0<second>;
    FreezeTimeRemaining = 0.0<second>;
    MoodTimeRemaining = 0.0<second>}
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

let showHeart boardState =
    if boardState.Heart.IsNone && boardState.Score >= heartCost then
        {boardState with Heart = randomBoardPosition() |> Some}
    elif boardState.Heart.IsSome && boardState.Score < heartCost then
        {boardState with Heart = None}
    else
        boardState

let showFreeze boardState =
    if boardState.FreezeTimeRemaining > 0.0<second> then
        {boardState with Freeze = None}
    elif boardState.Freeze.IsNone && boardState.Score >= freezeCost then
        {boardState with Freeze = randomBoardPosition() |> Some}
    elif boardState.Freeze.IsSome && boardState.Score < freezeCost then
        {boardState with Freeze = None}
    else
        boardState

let eatFreeze boardState = 
    if boardState.Freeze.IsSome && ((boardState.Player |> distance (boardState.Freeze |> Option.get)) / pixelsPerCell < 1.0<cell>) then
        {boardState with Freeze = None; Score = boardState.Score - freezeCost; FreezeTimeRemaining = freezeTime}
        |> addEvent PickUpFreeze
    else
        boardState

let eatHeart boardState = 
    if boardState.Heart.IsSome && ((boardState.Player |> distance (boardState.Heart |> Option.get)) / pixelsPerCell < 1.0<cell>) then
        {boardState with Heart = None; Score = boardState.Score - heartCost; MoodTimeRemaining = moodTime}
        |> addEvent PickUpHeart
    else
        boardState

let eatDollar boardState = 
    if (boardState.Player |> distance boardState.Dollar) / pixelsPerCell < 1.0<cell> then
        {boardState with Dollar = randomBoardPosition (); Score = boardState.Score + 1}
        |> addEvent PickUpDollar
    else
        boardState

let getMovementMultipler boardState =
    if boardState.MoodTimeRemaining > 0.0<second> then
        2.0
    else
        1.0

let moveAvatar (delta:float<second>) (gamePadState:GamePadState) (keyboardState :KeyboardState) boardState =
    let oldKeyboardState = boardState.KeyboardState
    let (velocityX,velocityY) = 
        if gamePadState.IsConnected then
            ((gamePadState.ThumbSticks.Left.X |> float) * 1.0<1/second>, (gamePadState.ThumbSticks.Left.Y |> float) * -1.0<1/second>)
        else
            keyboardState |> determineDelta boardState.KeyboardState
    let unitDistance = normalMovementRate * pixelsPerCell * 1.0<second> * (boardState |> getMovementMultipler)
    let velocity = (unitDistance * velocityX * delta, unitDistance * velocityY * delta)
    {boardState with Player= addLocation boardState.Player velocity}
    |> eatDollar
    |> eatHeart
    |> eatFreeze
    |> showHeart
    |> showFreeze

let decreaseFreezeTime (delta: float<second>) boardState =
    if boardState.FreezeTimeRemaining = 0.0<second> then
        boardState
    elif delta > boardState.FreezeTimeRemaining then
        {boardState with FreezeTimeRemaining = 0.0<second>}
        |> addEvent FreezeEffectOver
    else
        {boardState with FreezeTimeRemaining = boardState.FreezeTimeRemaining - delta}

let decreaseMoodTime (delta: float<second>) boardState =
    if boardState.MoodTimeRemaining = 0.0<second> then
        boardState
    elif delta > boardState.MoodTimeRemaining then
        {boardState with MoodTimeRemaining = 0.0<second>}
        |> addEvent MoodEffectOver
    else
        {boardState with MoodTimeRemaining = boardState.MoodTimeRemaining - delta}

let decreaseTimeRemaining (delta: float<second>) boardState =
    if boardState.FreezeTimeRemaining > 0.0<second> then
        boardState
    else
        {boardState with TimeRemaining = boardState.TimeRemaining - delta}

let decreaseTimes (delta: float<second>) boardState =
    boardState
    |> decreaseTimeRemaining delta
    |> decreaseMoodTime delta
    |> decreaseFreezeTime delta


let clearEvents boardState = 
    {boardState with BoardEvents = Set.empty}

