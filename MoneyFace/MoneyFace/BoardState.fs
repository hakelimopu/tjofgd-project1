module BoardState

open System

let boardColumns = 25
let boardRows = 25

let random = new Random()

let randomBoardPosition () =
    (random.Next(boardColumns), random.Next(boardRows))

type BoardState =
    {Player: int*int;
    Dollar: int*int;
    Score: int}

let mutable private boardState = {Player=randomBoardPosition ();Dollar=randomBoardPosition ();Score=0}

let loadBoardState () =
    boardState

let saveBoardState newBoardState =
    boardState <- newBoardState


