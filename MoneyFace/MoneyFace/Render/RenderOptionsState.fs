module RenderOptionsState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let drawOptionsState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    [((0,0),"Options",Color.White,getMiramonteFont);
    ((0,680),"Esc (B) - Go Back",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


