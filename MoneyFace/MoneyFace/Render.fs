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

let drawTexture (x,y) (texture:Texture2D) (spriteBatch: SpriteBatch) =
    spriteBatch.Draw (texture, new Rectangle(x - texture.Width/2, y - texture.Height/2, texture.Width, texture.Height), Color.White)

let drawText (x,y) (text:string) (color:Color) (font:SpriteFont) (spriteBatch: SpriteBatch)=
    spriteBatch.DrawString(font,text,new Vector2(x |> float32,y |> float32),color)

let drawScore xy score (font:SpriteFont) (spriteBatch: SpriteBatch) =
    let text = score |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let drawSeconds xy (seconds:float<second>) (font:SpriteFont) (spriteBatch: SpriteBatch) = 
    let text = seconds |> int |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let statusPanelX = 36.0 * BoardState.boardColumns

let handleEvent (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState event =
    match event with
    | BoardState.PickUpDollar -> (assets.[Coin_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore

let handleEvents (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState =
    boardState.BoardEvents
    |> Set.iter (handleEvent assets boardState)

let drawPlayState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    boardState |> handleEvents assets
    spriteBatch |> drawTexture (boardColumns * pixelsPerCell / 2.0<px> |> int, boardRows * pixelsPerCell / 2.0<px> |> int) (assets.[Playfield_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (((boardState.Player |> fst) / 1.0<px> |> int),((boardState.Player |> snd) / 1.0<px> |> int)) (assets.[Avatar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (((boardState.Dollar |> fst) / 1.0<px> |> int),((boardState.Dollar |> snd) / 1.0<px> |> int)) (assets.[Dollar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,0) "Score" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,60) "Time" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawScore (statusPanelX / 1.0<px> |> int, 30) boardState.Score (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawSeconds (statusPanelX / 1.0<px> |> int, 90) boardState.TimeRemaining (assets.[Miramonte_Font] |> getFont |> Option.get)
    
let drawTitleScreen delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "MoneyFace" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,30) "F1 - Instructions" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,60) "F2 - Start Game" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,90) "F3 - Options" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,120) "F4 - High Scores" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawPausedState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "Paused" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,30) "<SPACE> Unpause" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawHelpState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "Help" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,30) "Esc - Go Back" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawOptionsState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "Options" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,30) "Esc - Go Back" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawHighScoreState delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "High Scores" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (0,30) "Esc - Go Back" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawGame delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    match loadGameState() with
    | TitleScreen -> drawTitleScreen delta assets spriteBatch
    | PlayState boardState -> drawPlayState delta boardState assets spriteBatch
    | GameOverState boardState -> drawPlayState delta boardState assets spriteBatch
    | PausedState boardState -> drawPausedState delta boardState assets spriteBatch
    | HelpState  -> drawHelpState delta assets spriteBatch
    | OptionsState  -> drawOptionsState delta assets spriteBatch
    | HighScoreState  -> drawHighScoreState delta assets spriteBatch



