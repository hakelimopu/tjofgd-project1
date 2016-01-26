module RenderPausedState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let drawPausedState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    let font = assets |> getMiramonteFont
    let pausedMeasure = font.MeasureString("-- PAUSED --")
    let unpauseMeasure = font.MeasureString("<SPACE> (B) - Unpause")
    [((500-(pausedMeasure.X |> int)/2,360),"-- PAUSED --",Color.Red,getMiramonteFont);
    ((500-(unpauseMeasure.X |> int)/2,680),"<SPACE> (B) - Unpause",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


