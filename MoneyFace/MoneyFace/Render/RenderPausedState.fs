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
    [((0,0),"Paused",Color.White,getMiramonteFont);
    ((0,680),"<SPACE> (B) - Unpause",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


