module MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Input
open BoardState

type TextureId = 
    | Avatar
    | Dollar
    | Playfield

let loadTextures (contentManager:ContentManager) =
    [(Avatar   , "avatar"   ); 
     (Dollar   , "dollar"   );
     (Playfield, "playfield")]
    |> Seq.map(fun (id, filename) -> (id, contentManager.Load<Texture2D>(filename)))
    |> Map.ofSeq

let drawTexture textureId (x,y) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    let texture = textures.[textureId]
    spriteBatch.Draw (textures.[textureId], new Rectangle(x, y, texture.Width, texture.Height), Color.White)
    (textures, spriteBatch)

let drawGame delta (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    let boardState = loadBoardState()
    (textures, spriteBatch)
    ||> drawTexture Playfield (0,0)
    ||> drawTexture Avatar (32 * (boardState.Player |> fst),32 * (boardState.Player |> snd))
    ||> drawTexture Dollar (32 * (boardState.Dollar |> fst),32 * (boardState.Dollar |> snd))
    |> ignore

let updateGame delta =
    let keyboardState = Keyboard.GetState()

    loadBoardState()
    |> moveAvatar keyboardState
    |> updateKeyboardState keyboardState
    |> saveBoardState
