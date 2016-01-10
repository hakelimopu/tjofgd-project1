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
    | ScoreLabel
    | TimeLabel

let loadTextures (contentManager:ContentManager) =
    [(Avatar    , "avatar"    ); 
     (Dollar    , "dollar"    );
     (ScoreFont , "scorefont" );
     (ScoreLabel, "scorelabel");
     (TimeLabel , "timelabel" );
     (Playfield , "playfield" )]
    |> Seq.map(fun (id, filename) -> (id, contentManager.Load<Texture2D>(filename)))
    |> Map.ofSeq

let drawTexture textureId (x,y) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    let texture = textures.[textureId]
    spriteBatch.Draw (textures.[textureId], new Rectangle(x, y, texture.Width, texture.Height), Color.White)
    (textures, spriteBatch)

let drawDigit (dw,dh) (x,y) (ch: char) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    let number =
        match ch with
        | '0' -> 0
        | '1' -> 1
        | '2' -> 2
        | '3' -> 3
        | '4' -> 4
        | '5' -> 5
        | '6' -> 6
        | '7' -> 7
        | '8' -> 8
        | '9' -> 9
        | _ -> 0
    let texture = textures.[ScoreFont]
    let dst = new Rectangle(x,y,dw,dh)
    let src = new Nullable<Rectangle>(new Rectangle(number*8,0,8,8))
    spriteBatch.Draw (texture, dst, src, Color.White)

let drawScore (dw,dh) (x,y) score (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) =
    [for c in (score |> sprintf "%i") -> c]
    |> List.fold (fun acc ch -> 
        drawDigit (dw, dh) (acc,y) ch textures spriteBatch
        acc + dw) x
    |> ignore
    (textures, spriteBatch)

let drawSeconds (dw,dh) (x,y) (timeSpan:TimeSpan) (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    [for c in (timeSpan.TotalSeconds |> int |> sprintf "%i") -> c]
    |> List.fold (fun acc ch -> 
        drawDigit (dw, dh) (acc,y) ch textures spriteBatch
        acc + dw) x
    |> ignore
    (textures, spriteBatch)

let statusPanelX = 36 * BoardState.boardColumns

let drawPlayState delta boardState (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    (textures, spriteBatch)
    ||> drawTexture Playfield (0,0)
    ||> drawTexture ScoreLabel (statusPanelX,0)
    ||> drawTexture TimeLabel (statusPanelX,60)
    ||> drawTexture Avatar (36 * (boardState.Player |> fst),36 * (boardState.Player |> snd))
    ||> drawTexture Dollar (36 * (boardState.Dollar |> fst),36 * (boardState.Dollar |> snd))
    ||> drawScore (30,30) (statusPanelX, 30) boardState.Score
    ||> drawSeconds (30,30) (statusPanelX, 90) boardState.TimeRemaining
    |> ignore
    
let drawGame delta (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    match loadGameState() with
    | PlayState boardState -> drawPlayState delta boardState textures spriteBatch
    | GameOverState boardState -> drawPlayState delta boardState textures spriteBatch

let clampAvatar boardState = 
    let x,y = boardState.Player
    let clamped = 
        (
        match x with
        | v when v < 0 -> 0
        | v when v >= BoardState.boardColumns -> (boardColumns - 1)
        | _ -> x
        ,
        match y with
        | v when v < 0 -> 0
        | v when v >= BoardState.boardColumns -> (boardRows - 1)
        | _ -> y
        )
    {boardState with Player=clamped}

let updatePlayState delta boardState = 
    let keyboardState = Keyboard.GetState()
    let newBoardState = 
        boardState
        |> moveAvatar keyboardState
        |> clampAvatar 
        |> updateKeyboardState keyboardState
        |> decreaseTime delta
    if newBoardState.TimeRemaining.TotalSeconds <= 0. then
        newBoardState |> GameOverState
    else
        newBoardState |> PlayState


let updateGameOverState delta boardState = 
    let keyboardState = Keyboard.GetState()
    boardState 
    |> updateKeyboardState keyboardState
    |> if boardState.KeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2) then
            PlayState
        else
            GameOverState


let updateGame delta =
    match loadGameState() with
    | PlayState boardState -> boardState |> updatePlayState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
