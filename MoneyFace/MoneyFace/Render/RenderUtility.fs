module RenderUtility

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Graphics
open TJoFGDGame
open AssetType
open Assets
open BoardState

let drawTexture (x,y) (texture:Texture2D) (spriteBatch: SpriteBatch) =
    spriteBatch.Draw (texture, new Rectangle(x - texture.Width/2, y - texture.Height/2, texture.Width, texture.Height), Color.White)

let drawText (x,y) (text:string) (color:Color) (font:SpriteFont) (spriteBatch: SpriteBatch)=
    spriteBatch.DrawString(font,text,new Vector2(x |> float32,y |> float32),color)

let drawInt xy score (font:SpriteFont) (spriteBatch: SpriteBatch) =
    let text = score |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let statusPanelX = 36.0 * Constants.boardColumns

let handleEvent (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState event =
    match event with
    | BoardState.PickUpDollar -> (assets.[Coin_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore
    | BoardState.PickUpHeart -> (assets.[Heart_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore
    | BoardState.PickUpFreeze -> (assets.[Heart_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore//GYOSFX
    | BoardState.MoodEffectOver -> (assets.[MoodOver_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore
    | BoardState.FreezeEffectOver -> (assets.[MoodOver_SoundEffect] |> getSoundEffect |> Option.get).Play() |> ignore//GYOSFX

let handleEvents (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) boardState =
    boardState.BoardEvents
    |> Set.iter (handleEvent assets boardState)

let drawTextureAsset (x,y) asset spriteBatch =
    spriteBatch |> drawTexture ((x / 1.0<px> |> int),(y / 1.0<px> |> int)) (asset |> getTexture |> Option.get)

let getMiramonteFont (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>)  =
    assets.[Miramonte_Font] |> getFont |> Option.get

let private drawTextContent assets spriteBatch (xy,text,color,fontFunc) = 
    spriteBatch |> drawText xy text color (assets |> fontFunc)

let drawTextContents (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch textContents =
    textContents
    |> List.iter(drawTextContent assets spriteBatch)
