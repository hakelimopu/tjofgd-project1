module BoardState

open System
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework
open TJoFGDGame
open Constants

type BoardEvent =
    | PickUpDollar
    | PickUpHeart
    | MoodEffectOver
    | PickUpFreeze
    | FreezeEffectOver

type Timer = 
    | Main
    | Freeze
    | Mood

type CounterType =
    | DollarCounter
    | HeartCounter
    | FreezeCounter

type BoardState =
    {Player: float<px>*float<px>;
    Dollar: float<px>*float<px>;
    Heart: (float<px> * float<px>) option;
    Freeze: (float<px> * float<px>) option;
    Score: int;
    Counters: Map<CounterType,int>;
    KeyboardState: KeyboardState;
    GamePadState: GamePadState;
    BoardEvents: Set<BoardEvent>;
    TimesRemaining: Map<Timer,float<second>>}

type HighScore =
    {Score:string;
    User:string;
    Stored:string}

type GameState = 
    | TitleScreen
    | PlayState of BoardState
    | GameOverState of BoardState
    | HelpState
    | OptionsState
    | AboutState
    | HighScoreState of HighScore list
    | PausedState of BoardState

let newGame () = 
    {Player=Utility.randomBoardPosition ();
    Dollar=Utility.randomBoardPosition (); 
    Score=0; 
    Heart = None;
    Freeze = None;
    Counters = Map.empty;
    KeyboardState = Keyboard.GetState();
    GamePadState = GamePad.GetState(PlayerIndex.One);
    BoardEvents = Set.empty;
    TimesRemaining = Map.empty |> Map.add Main 60.0<second> |> Map.add Freeze 0.0<second> |> Map.add Mood 0.0<second>}
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

let getCounter counterType boardState =
    let value = 
        boardState.Counters
        |> Map.tryFind (counterType)
    match value with
    | Some x -> x
    | _ -> 0

let private setCounter counterType counterValue boardState =
    {boardState with Counters=boardState.Counters |> Map.add counterType counterValue}

let incrementCounter counterType boardState =
    boardState
    |> setCounter counterType ((boardState |> getCounter counterType) + 1)

let showHeart boardState =
    if boardState.Heart.IsNone && boardState.Score >= heartCost then
        {boardState with Heart = Utility.randomBoardPosition() |> Some}
    elif boardState.Heart.IsSome && boardState.Score < heartCost then
        {boardState with Heart = None}
    else
        boardState

let showFreeze boardState =
    if boardState.TimesRemaining.[Freeze] > 0.0<second> then
        {boardState with Freeze = None}
    elif boardState.Freeze.IsNone && boardState.Score >= freezeCost then
        {boardState with Freeze = Utility.randomBoardPosition() |> Some}
    elif boardState.Freeze.IsSome && boardState.Score < freezeCost then
        {boardState with Freeze = None}
    else
        boardState

let eatFreeze boardState = 
    if boardState.Freeze.IsSome && ((boardState.Player |> distance (boardState.Freeze |> Option.get)) / pixelsPerCell < 1.0<cell>) then
        {boardState with Freeze = None; Score = boardState.Score - freezeCost; TimesRemaining = boardState.TimesRemaining |> Map.add Freeze freezeTime}
        |> addEvent PickUpFreeze
        |> incrementCounter FreezeCounter
    else
        boardState

let eatHeart boardState = 
    if boardState.Heart.IsSome && ((boardState.Player |> distance (boardState.Heart |> Option.get)) / pixelsPerCell < 1.0<cell>) then
        {boardState with Heart = None; Score = boardState.Score - heartCost; TimesRemaining = boardState.TimesRemaining |> Map.add Mood moodTime}
        |> addEvent PickUpHeart
        |> incrementCounter HeartCounter
    else
        boardState

let eatDollar boardState = 
    if (boardState.Player |> distance boardState.Dollar) / pixelsPerCell < 1.0<cell> then
        {boardState with Dollar = Utility.randomBoardPosition (); Score = boardState.Score + 1}
        |> addEvent PickUpDollar
        |> incrementCounter DollarCounter
    else
        boardState

let getMovementMultipler boardState =
    if boardState.TimesRemaining.[Mood] > 0.0<second> then
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

let decreaseTime timer event (delta: float<second>) boardState = 
    if boardState.TimesRemaining.[timer] = 0.0<second> then
        boardState
    elif delta > boardState.TimesRemaining.[timer] then
        {boardState with TimesRemaining = boardState.TimesRemaining |> Map.add timer 0.0<second>}
        |> addEvent event
    else
        {boardState with TimesRemaining = boardState.TimesRemaining |> Map.add timer (boardState.TimesRemaining.[timer] - delta)}

let decreaseTimeRemaining (delta: float<second>) boardState =
    if boardState.TimesRemaining.[Freeze] > 0.0<second> then
        boardState
    else
        {boardState with TimesRemaining = boardState.TimesRemaining |> Map.add Main (boardState.TimesRemaining.[Main] - delta)}

let decreaseTimes (delta: float<second>) boardState =
    boardState
    |> decreaseTimeRemaining delta
    |> decreaseTime Mood MoodEffectOver delta
    |> decreaseTime Freeze FreezeEffectOver delta


let clearEvents boardState = 
    {boardState with BoardEvents = Set.empty}

