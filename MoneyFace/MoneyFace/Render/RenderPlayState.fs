module RenderPlayState

open AssetType
open Assets
open BoardState
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open RenderUtility
open TJoFGDGame

let drawPlayState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    boardState |> handleEvents assets

    //NOTE: Thou shalt not use any textures before Playfield_Texture
    spriteBatch |> drawTexture (Constants.boardColumns * Constants.pixelsPerCell / 2.0<px> |> int, Constants.boardRows * Constants.pixelsPerCell / 2.0<px> |> int) (assets.[Playfield_Texture] |> getTexture |> Option.get)

    boardState.Heart
    |> Option.iter (fun (x,y) -> spriteBatch |> drawTextureAsset (x,y) assets.[Heart_Texture])
    boardState.Freeze
    |> Option.iter (fun (x,y) -> spriteBatch |> drawTextureAsset (x,y) assets.[SnowFlake_Texture])
    spriteBatch |> drawTexture (((boardState.Player |> fst) / 1.0<px> |> int),((boardState.Player |> snd) / 1.0<px> |> int)) (assets.[Avatar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (((boardState.Dollar |> fst) / 1.0<px> |> int),((boardState.Dollar |> snd) / 1.0<px> |> int)) (assets.[Dollar_Texture] |> getTexture |> Option.get)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,0) "Score" Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 30) boardState.Score (assets |> getMiramonteFont)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,60) "Time" Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 90) (boardState.TimesRemaining.[Main] |> int) (assets |> getMiramonteFont)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,120) "Mood" Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 150) (boardState.TimesRemaining.[Mood] |> int) (assets |> getMiramonteFont)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,180) "Freeze" Color.White (assets |> getMiramonteFont)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 210) (boardState.TimesRemaining.[Freeze] |> int) (assets |> getMiramonteFont)


