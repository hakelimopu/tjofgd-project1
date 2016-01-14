open System
open BoardState
open System.Threading
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

[<EntryPoint>]
let main argv = 
    let game = new TJoFGDGame.TJoFGDGame<Assets.AssetId, AssetType.AssetType<Texture2D,SpriteFont,SoundEffect>>(1000, 720, Assets.loadAssets, Update.updateGame, Render.drawGame, Microsoft.Xna.Framework.Color.Black)
    game.Run()
    0
