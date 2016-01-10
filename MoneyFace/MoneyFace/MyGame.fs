module MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Input
open BoardState
open System

type TextureId = 
    | Avatar
    | Dollar
    | Playfield
    | ScoreFont

let loadTextures (contentManager:ContentManager) =
    [(Avatar   , "avatar"   ); 
     (Dollar   , "dollar"   );
     (ScoreFont, "scorefont");
     (Playfield, "playfield")]
    |> Seq.map(fun (id, filename) -> (id, contentManager.Load<Texture2D>(filename)))
    |> Map.ofSeq

let drawTexture textureId (x,y) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    let texture = textures.[textureId]
    spriteBatch.Draw (textures.[textureId], new Rectangle(x, y, texture.Width, texture.Height), Color.White)
    (textures, spriteBatch)

let drawLetter (x,y) (ch: char) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    let number =
        match ch with
        | '0' -> 0
        | '1' -> 8
        | '2' -> 16
        | '3' -> 24
        | '4' -> 32
        | '5' -> 40
        | '6' -> 48
        | '7' -> 56
        | '8' -> 64
        | '9' -> 72
        | _ -> 0
    let texture = textures.[ScoreFont]
    let dst = new Rectangle(x,y,8,8)
    let src = new Nullable<Rectangle>(new Rectangle(number,0,8,8))
    spriteBatch.Draw (texture, dst, src, Color.White)

let drawScore (x,y) score (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    [for c in (score |> sprintf "%i") -> c]
    |> List.fold (fun acc ch -> 
        drawLetter (acc,y) ch textures spriteBatch
        acc + 8) x
    |> ignore
    (textures, spriteBatch)
    
let drawGame delta (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    let boardState = loadBoardState()
    (textures, spriteBatch)
    ||> drawTexture Playfield (0,0)
    ||> drawTexture Avatar (36 * (boardState.Player |> fst),36 * (boardState.Player |> snd))
    ||> drawTexture Dollar (36 * (boardState.Dollar |> fst),36 * (boardState.Dollar |> snd))
    ||> drawScore (36 * BoardState.boardColumns, 0) boardState.Score
    |> ignore

let updateGame delta =
    let keyboardState = Keyboard.GetState()

    loadBoardState()
    |> moveAvatar keyboardState
    |> updateKeyboardState keyboardState
    |> saveBoardState
