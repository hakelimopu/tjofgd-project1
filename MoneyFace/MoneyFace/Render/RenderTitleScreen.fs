module RenderTitleScreen

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let drawTitleScreen delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    [((0,0),"MoneyFace",Color.White,getMiramonteFont);
    ((0,60),"F1 (X) - Instructions",Color.Blue,getMiramonteFont);
    ((0,120),"F2 (Start) - Start Game",Color.LightGray,getMiramonteFont);
    ((0,180),"F3 (A) - Options",Color.Green,getMiramonteFont);
    ((0,240),"F4 (Y) - High Scores",Color.Yellow,getMiramonteFont);
    ((0,300),"F5 (B) - About/Credits",Color.Red,getMiramonteFont)]
    |> drawTextContents assets spriteBatch



