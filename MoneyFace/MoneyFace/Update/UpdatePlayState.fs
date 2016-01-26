module UpdatePlayState

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
open UpdateUtility

//start trophies

let private trophiesList =
    [(DollarCounter,25,49005);
    (DollarCounter,50,49137);
    (DollarCounter,100,49138);
    (DollarCounter,200,49139);
    
    (HeartCounter,10,49333);
    (HeartCounter,20,49334);
    (HeartCounter,40,49335);
    (HeartCounter,80,49336);

    (FreezeCounter,5,49337);
    (FreezeCounter,10,49338);
    (FreezeCounter,20,49339);
    (FreezeCounter,40,49340)]

let private achieveTrophy boardState (counter,goal,trophyId) =
    if boardState |> getCounter counter >= goal then
        trophyId |> GameJoltApi.addAchieved
    else
        ()

let achieveTrophies (boardState: BoardState) =
    trophiesList
    |> List.iter(achieveTrophy boardState)

//end trophies

let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let gamePadState = GamePad.GetState(PlayerIndex.One)
    let (k,b,u) = boardState |> getInputState keyboardState gamePadState

    if k.Contains(Keys.Space) || b.Contains(Buttons.B)  then
        boardState
        |> u
        |> PausedState
    else
        let newBoardState = 
            boardState
            |> clearEvents
            |> moveAvatar delta gamePadState keyboardState
            |> clampAvatar 
            |> u
            |> decreaseTimes delta
        if newBoardState.TimesRemaining.[Main] <= 0.0<second> then
            newBoardState.Score |> GameJoltApi.addScore 
            newBoardState |> achieveTrophies
            newBoardState |> GameOverState
        else
            newBoardState |> PlayState


