module RenderHelpState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame
    
let drawHelpState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    [((0,0),"Help",Color.White,getMiramonteFont);
    ((0,680),"Esc (B) - Go Back",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


