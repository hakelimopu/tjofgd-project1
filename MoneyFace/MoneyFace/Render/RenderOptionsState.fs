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
    let options = GameOptions.loadGameOptions()
    let textContents = 
        [((0,0),"Options",Color.White,getMiramonteFont);
        ((0,90),"<SPACE> (A) - Toggle SFX",Color.Green,getMiramonteFont);
        ((0,150),sprintf "Volume: %f" options.Volume,Color.White,getMiramonteFont);
        ((0,180),"<- (X) Decrease Volume",Color.Blue,getMiramonteFont);
        ((0,210),"-> (Y) Increase Volume",Color.Yellow,getMiramonteFont);
        ((0,680),"Esc (B) - Go Back",Color.Gray,getMiramonteFont)]
    if options.Sfx then
        textContents
        |> List.append [((0,60),"SFX: ON",Color.LightGreen,getMiramonteFont)]
    else
        textContents
        |> List.append [((0,60),"SFX: OFF",Color.LightPink,getMiramonteFont)]
    |> drawTextContents assets spriteBatch


