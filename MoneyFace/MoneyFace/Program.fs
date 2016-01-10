open System
open BoardState
open System.Threading

[<EntryPoint>]
let main argv = 
    let game = new TJoFGDGame.TJoFGDGame<MyGame.TextureId>(1000, 720, MyGame.loadTextures, MyGame.updateGame, MyGame.drawGame, Microsoft.Xna.Framework.Color.Black)
    game.Run()
    0
