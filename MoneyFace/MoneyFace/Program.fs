open System
open BoardState
open System.Threading
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

let private gjGameId = 120704
let private gjPrivateKey = "5797e7b6cc3992d415330e08a85c8732"

[<EntryPoint>]
let main argv = 
    GameJoltApi.initConfig gjGameId gjPrivateKey
    GameJoltApi.setUser argv.[0] argv.[1]
    GameJoltApi.authenticateUser() |> ignore
    let game = new TJoFGDGame.TJoFGDGame<Assets.AssetId, AssetType.AssetType<Texture2D,SpriteFont,SoundEffect>>(1000, 720, Assets.loadAssets, Update.updateGame, Render.drawGame, Microsoft.Xna.Framework.Color.Black)
    game.Run()
    0
