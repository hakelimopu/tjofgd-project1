module RenderHighScoreState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let private drawHighScore (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch x highScore =
    spriteBatch |> drawText (0,x) highScore.User Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawText (500,x) highScore.Score Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawText (750,x) highScore.Stored Color.White (assets |> getMiramonteFont)
    x - 30

let drawHighScoreState delta highScores (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "High Scores" Color.White (assets |> getMiramonteFont)
    highScores
    |> List.fold (drawHighScore assets spriteBatch) (30 * (highScores |> List.length))
    |> ignore
    spriteBatch |> drawText (0,720-45) "Esc (B) - Go Back" Color.White (assets |> getMiramonteFont)


