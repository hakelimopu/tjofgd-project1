module Update

open UpdateAboutState
open UpdateGameOverState
open UpdateHelpState
open UpdateHighScoreState
open UpdateOptionsState
open UpdatePausedState
open UpdatePlayState
open UpdateTitleScreen
open BoardState

let updateGame delta =
    match loadGameState() with
    | TitleScreen (k,g)-> updateTitleScreen delta k g
    | HelpState (k,g)-> updateHelpState delta k g
    | OptionsState (k,g)-> updateOptionsState delta k g
    | AboutState (k,g)-> updateAboutState delta k g
    | HighScoreState (highScores, k,g) -> updateHighScoreState delta highScores k g
    | PlayState boardState -> boardState |> updatePlayState delta
    | PausedState boardState -> boardState |> updatePausedState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
