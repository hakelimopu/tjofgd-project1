open System
open BoardState
open System.Threading
open Microsoft.Xna.Framework.Graphics

[<EntryPoint>]
let main argv = 
    let game = new TJoFGDGame.TJoFGDGame<MyGame.AssetId, MyGame.AssetType<Texture2D,SpriteFont>>(1000, 720, MyGame.loadAssets, MyGame.updateGame, MyGame.drawGame, Microsoft.Xna.Framework.Color.Black)
    game.Run()
    0
