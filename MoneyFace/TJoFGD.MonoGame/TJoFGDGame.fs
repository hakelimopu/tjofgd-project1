module TJoFGDGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type TJoFGDGame<'textureKey when 'textureKey: comparison> (backBufferWidth, backBufferHeight, textureLoader: ContentManager -> Map<'textureKey, Texture2D>, gameUpdater, gameRenderer : GameTime -> Map<'textureKey,Texture2D> -> SpriteBatch -> unit, backgroundColor) as this=
    inherit Game()

    do
        this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)

    let mutable spriteBatch: SpriteBatch = null
    let mutable textures: Map<'textureKey, Texture2D> = Map.empty

    override this.Initialize() =
        graphics.PreferredBackBufferWidth <- backBufferWidth
        graphics.PreferredBackBufferHeight <- backBufferHeight
        graphics.ApplyChanges()
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        base.Initialize()

    override this.LoadContent() = 
        textures <- this.Content |> textureLoader 

    override this.Update delta =
        delta
        |> gameUpdater 

    override this.Draw delta =
        backgroundColor 
        |> this.GraphicsDevice.Clear
        spriteBatch.Begin()
        gameRenderer delta textures spriteBatch
        spriteBatch.End()


