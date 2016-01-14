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

let drawTexture (x,y) (texture:Texture2D) (spriteBatch: SpriteBatch) =
    spriteBatch.Draw (texture, new Rectangle(x, y, texture.Width, texture.Height), Color.White)

let drawText (x,y) (text:string) (color:Color) (font:SpriteFont) (spriteBatch: SpriteBatch)=
    spriteBatch.DrawString(font,text,new Vector2(x |> float32,y |> float32),color)

let drawScore xy score (font:SpriteFont) (spriteBatch: SpriteBatch) =
    let text = score |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let drawSeconds xy (timeSpan:TimeSpan) (font:SpriteFont) (spriteBatch: SpriteBatch) = 
    let text = timeSpan.TotalSeconds |> int |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let statusPanelX = 36 * BoardState.boardColumns

let handleEvent (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState event =
    match event with
    | BoardState.PickUpDollar -> (assets.[Coin_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore

let handleEvents (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState =
    boardState.BoardEvents
    |> Set.iter (handleEvent assets boardState)

let drawPlayState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    boardState |> handleEvents assets
    spriteBatch |> drawTexture (0,0) (assets.[Playfield_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (36 * (boardState.Player |> fst),36 * (boardState.Player |> snd)) (assets.[Avatar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (36 * (boardState.Dollar |> fst),36 * (boardState.Dollar |> snd)) (assets.[Dollar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawText (statusPanelX,0) "Score" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (statusPanelX,60) "Time" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawScore (statusPanelX, 30) boardState.Score (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawSeconds (statusPanelX, 90) boardState.TimeRemaining (assets.[Miramonte_Font] |> getFont |> Option.get)
    
let drawTitleScreen delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "MoneyFace" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawGame delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    match loadGameState() with
    | TitleScreen -> drawTitleScreen delta assets spriteBatch
    | PlayState boardState -> drawPlayState delta boardState assets spriteBatch
    | GameOverState boardState -> drawPlayState delta boardState assets spriteBatch



