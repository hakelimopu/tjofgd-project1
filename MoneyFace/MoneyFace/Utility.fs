module Utility

open Constants
open System

let private random = new Random()

let randomBoardPosition () =
    (random.NextDouble() * (maximumX - minimumX) + minimumX, random.NextDouble() * (maximumY - minimumY) + minimumY)



