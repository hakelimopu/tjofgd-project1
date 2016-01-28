module Render

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
open RenderUtility

let drawGame delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    let gameState = loadGameState()
    match gameState with
    | PlayState boardState -> RenderPlayState.drawPlayState delta boardState assets spriteBatch
    | TitleScreen _ -> RenderTitleScreen.drawTitleScreen delta assets spriteBatch
    | GameOverState boardState -> RenderPlayState.drawPlayState delta boardState assets spriteBatch
    | PausedState boardState -> RenderPausedState.drawPausedState delta boardState assets spriteBatch
    | HelpState _ -> RenderHelpState.drawHelpState delta assets spriteBatch
    | OptionsState _ -> RenderOptionsState.drawOptionsState delta assets spriteBatch
    | HighScoreState (highScores, _, _) -> RenderHighScoreState.drawHighScoreState delta highScores assets spriteBatch
    | AboutState _ -> RenderAboutState.drawAboutState delta assets spriteBatch



