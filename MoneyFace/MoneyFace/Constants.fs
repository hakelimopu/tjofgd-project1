module Constants

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

let minimumX = 0.5<cell> * pixelsPerCell
let minimumY = 0.5<cell> * pixelsPerCell
let maximumX = boardColumns * pixelsPerCell - minimumX
let maximumY = boardRows * pixelsPerCell - minimumY
