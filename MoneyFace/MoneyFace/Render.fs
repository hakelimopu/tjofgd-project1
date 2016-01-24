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
open RenderUtility

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

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,0) "Score" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 30) boardState.Score (assets.[Miramonte_Font] |> getFont |> Option.get)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,60) "Time" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 90) (boardState.TimesRemaining.[Main] |> int) (assets.[Miramonte_Font] |> getFont |> Option.get)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,120) "Mood" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 150) (boardState.TimesRemaining.[Mood] |> int) (assets.[Miramonte_Font] |> getFont |> Option.get)

    spriteBatch |> drawText (statusPanelX / 1.0<px> |> int,180) "Freeze" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawInt (statusPanelX / 1.0<px> |> int, 210) (boardState.TimesRemaining.[Freeze] |> int) (assets.[Miramonte_Font] |> getFont |> Option.get)

    
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

let drawHighScoreState delta highScores (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) spriteBatch =
    spriteBatch |> drawText (0,0) "High Scores" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    highScores
    |> List.fold (fun acc highScore -> 
                        spriteBatch |> drawText (0,acc) highScore.User Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
                        spriteBatch |> drawText (500,acc) highScore.Score Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
                        spriteBatch |> drawText (750,acc) highScore.Stored Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
                        acc - 30) (30 * (highScores |> List.length))
    |> ignore
    spriteBatch |> drawText (0,720-45) "Esc - Go Back" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)

let drawGame delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont,SoundEffect>>) (spriteBatch: SpriteBatch) = 
    match loadGameState() with
    | TitleScreen -> drawTitleScreen delta assets spriteBatch
    | PlayState boardState -> drawPlayState delta boardState assets spriteBatch
    | GameOverState boardState -> drawPlayState delta boardState assets spriteBatch
    | PausedState boardState -> drawPausedState delta boardState assets spriteBatch
    | HelpState  -> drawHelpState delta assets spriteBatch
    | OptionsState  -> drawOptionsState delta assets spriteBatch
    | HighScoreState highScores -> drawHighScoreState delta highScores assets spriteBatch



