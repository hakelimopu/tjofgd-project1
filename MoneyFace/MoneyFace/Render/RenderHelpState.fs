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
    spriteBatch |> drawTexture (18,78) (assets.[Avatar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (18,198) (assets.[Dollar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (18,258) (assets.[Heart_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (18,354) (assets.[SnowFlake_Texture] |> getTexture |> Option.get)
    [((0,0),"Help",Color.White,getMiramonteFont);
    ((36,60),"You are a light blue smiley face:",Color.Blue,getMiramonteFont);
    ((0,120),"Your goal is to get paid and be happy!",Color.Blue,getMiramonteFont);
    ((36,180),"Collect dollar signs for points!",Color.Blue,getMiramonteFont);
    ((36,240),"Hearts will improve mood and help you go faster for a",Color.Blue,getMiramonteFont);
    ((36,270),"while! (Cost: $5)",Color.Blue,getMiramonteFont);
    ((36,330),"Snowflakes will stop the clock, allowing you more time to",Color.Blue,getMiramonteFont);
    ((36,360),"collect other things! (Cost: $10)",Color.Blue,getMiramonteFont);
    ((0,450),"Try to balance between money, mood, and time extensions,",Color.Blue,getMiramonteFont);
    ((0,480),"as hearts and snowflakes cost money!",Color.Blue,getMiramonteFont);
    ((0,600),"Good luck!",Color.ForestGreen, getMiramonteFont);
    ((0,680),"Esc (B) - Go Back",Color.Gray,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


