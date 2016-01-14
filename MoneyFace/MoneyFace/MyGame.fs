module MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Input
open BoardState
open System

type AssetId =
    | Normal_Font
    | Miramonte_Font
    | Avatar_Texture
    | Dollar_Texture
    | Playfield_Texture

type AssetType<'texture,'font> =
    | Texture of 'texture
    | Font of 'font

let getTexture (asset:AssetType<'texture,'font>) =
    match asset with
    | Texture value -> Some value
    | _ -> None

let getFont (asset:AssetType<'texture,'font>) =
    match asset with
    | Font value -> Some value
    | _ -> None

let loadAsset (contentManager:ContentManager) (id,asset) =
    let loadedAsset = 
        match asset with
        | Texture fileName -> Texture (contentManager.Load<Texture2D>(fileName))
        | Font fileName -> Font (contentManager.Load<SpriteFont>(fileName))
    (id, loadedAsset)

let loadAssets (contentManager:ContentManager) =
    [(Normal_Font, Font "font");
    (Miramonte_Font, Font "miramonte");
    (Avatar_Texture, Texture "avatar");
    (Dollar_Texture, Texture "dollar");
    (Playfield_Texture, Texture "playfield")]
    |> Seq.map (loadAsset contentManager)
    |> Map.ofSeq

let drawTexture (x,y) (texture:Texture2D) (spriteBatch: SpriteBatch) =
    spriteBatch.Draw (texture, new Rectangle(x, y, texture.Width, texture.Height), Color.White)

let drawText (x,y) (text:string) (color:Color) (font:SpriteFont) (spriteBatch: SpriteBatch)=
    spriteBatch.DrawString(font,text,new Vector2(x |> float32,y |> float32),color)

let drawScore xy score (font:SpriteFont) (spriteBatch: SpriteBatch) =
    let text = score |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let drawSeconds xy (timeSpan:TimeSpan) (font:SpriteFont) (spriteBatch: SpriteBatch) = 
    let text = timeSpan.TotalSeconds |> int |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

let statusPanelX = 36 * BoardState.boardColumns

let drawPlayState delta boardState (assets:Map<AssetId,AssetType<Texture2D,SpriteFont>>) (spriteBatch: SpriteBatch) = 
    spriteBatch |> drawTexture (0,0) (assets.[Playfield_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (36 * (boardState.Player |> fst),36 * (boardState.Player |> snd)) (assets.[Avatar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawTexture (36 * (boardState.Dollar |> fst),36 * (boardState.Dollar |> snd)) (assets.[Dollar_Texture] |> getTexture |> Option.get)
    spriteBatch |> drawText (statusPanelX,0) "Score" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawText (statusPanelX,60) "Time" Color.White (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawScore (statusPanelX, 30) boardState.Score (assets.[Miramonte_Font] |> getFont |> Option.get)
    spriteBatch |> drawSeconds (statusPanelX, 90) boardState.TimeRemaining (assets.[Miramonte_Font] |> getFont |> Option.get)
    
let drawGame delta (assets:Map<AssetId,AssetType<Texture2D,SpriteFont>>) (spriteBatch: SpriteBatch) = 
    match loadGameState() with
    | PlayState boardState -> drawPlayState delta boardState assets spriteBatch
    | GameOverState boardState -> drawPlayState delta boardState assets spriteBatch

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
    if boardState.KeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2) then
        newGame()
    else
        boardState 
        |> updateKeyboardState keyboardState
        |> GameOverState


let updateGame delta =
    match loadGameState() with
    | PlayState boardState -> boardState |> updatePlayState delta
    | GameOverState boardState -> boardState |> updateGameOverState delta
    |> saveGameState
