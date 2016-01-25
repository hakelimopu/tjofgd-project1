module RenderAboutState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let drawAboutState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    [((0,0),"About MoneyFace",Color.White,getMiramonteFont);
    ((0,60),"Written by Ernest Pazera",Color.Blue,getMiramonteFont);
    ((0,120),"Icons made by Lorc. Available on http://game-icons.net",Color.Blue,getMiramonteFont);
    ((0,680),"Esc (B) - Go Back",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


