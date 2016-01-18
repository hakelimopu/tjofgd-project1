module Assets

open Microsoft.Xna.Framework.Content
open AssetType
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

type AssetId =
    | Normal_Font
    | Miramonte_Font
    | Avatar_Texture
    | Dollar_Texture
    | Playfield_Texture
    | Coin_SoundEffect
    | Heart_SoundEffect
    | Heart_Texture//Lorc
    | SnowFlake_Texture//Lorc
    | MoodOver_SoundEffect

let loadAsset (contentManager:ContentManager) (id,asset) =
    let loadedAsset = 
        match asset with
        | Texture fileName -> AssetType.Texture (contentManager.Load<Texture2D>(fileName))
        | Font fileName -> Font (contentManager.Load<SpriteFont>(fileName))
        | Effect fileName -> AssetType.Effect (contentManager.Load<SoundEffect>(fileName))
    (id, loadedAsset)

let loadAssets (contentManager:ContentManager) =
    [(Normal_Font, Font "font");
    (Coin_SoundEffect, AssetType.Effect "coin");
    (Heart_SoundEffect, AssetType.Effect "heartsfx");
    (MoodOver_SoundEffect, AssetType.Effect "moodover");
    (Heart_Texture, AssetType.Texture "heart");
    (SnowFlake_Texture, AssetType.Texture "snowflake");
    (Miramonte_Font, Font "miramonte");
    (Avatar_Texture, AssetType.Texture "avatar");
    (Dollar_Texture, AssetType.Texture "dollar");
    (Playfield_Texture, AssetType.Texture "playfield")]
    |> Seq.map (loadAsset contentManager)
    |> Map.ofSeq



