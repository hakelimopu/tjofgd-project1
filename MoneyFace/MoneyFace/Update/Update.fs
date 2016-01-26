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
    | TitleScreen -> updateTitleScreen delta
    | HelpState -> updateHelpState delta
    | OptionsState -> updateOptionsState delta
    | AboutState -> updateAboutState delta
    | HighScoreState highScores -> updateHighScoreState delta highScores
    | PlayState boardState -> boardState |> updatePlayState delta
    | PausedState boardState -> boardState |> updatePausedState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
