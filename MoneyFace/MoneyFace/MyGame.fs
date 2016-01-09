module MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type TextureId = 
    | Avatar
    | Dollar

let loadTextures (contentManager:ContentManager) =
    [(Avatar,"avatar"); (Dollar, "dollar")]
    |> Seq.map(fun (id, filename) -> (id, contentManager.Load<Texture2D>(filename)))
    |> Map.ofSeq

let drawGame delta (textures:Map<TextureId,Texture2D>) (spriteBatch: SpriteBatch) = 
    spriteBatch.Draw(textures.[Avatar],new Rectangle(0,0,32,32),Color.White)
    spriteBatch.Draw(textures.[Dollar],new Rectangle(32,0,32,32),Color.White)

type MyGame<'textureKey when 'textureKey: comparison> (textureLoader: ContentManager -> Map<'textureKey, Texture2D>, gameRenderer : GameTime -> Map<'textureKey,Texture2D> -> SpriteBatch -> unit) as this=
    inherit Game()
    do
        this.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch: SpriteBatch = null
    let mutable textures: Map<'textureKey, Texture2D> = Map.empty
    override this.Initialize() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        base.Initialize()
    override this.LoadContent() = 
        textures <- this.Content |> textureLoader 
    override this.Update delta =
        ()
    override this.Draw delta =
        Color.Black 
        |> this.GraphicsDevice.Clear
        spriteBatch.Begin()
        gameRenderer delta textures spriteBatch
        spriteBatch.End()
