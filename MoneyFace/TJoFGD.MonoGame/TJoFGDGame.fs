module TJoFGDGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type TJoFGDGame<'textureKey,'fontKey when 'textureKey: comparison and 'fontKey: comparison> 
        (backBufferWidth, backBufferHeight, textureLoader: ContentManager -> Map<'textureKey, Texture2D>, fontLoader: ContentManager -> Map<'fontKey, SpriteFont>, gameUpdater, gameRenderer : GameTime -> Map<'fontKey,SpriteFont> -> Map<'textureKey,Texture2D> -> SpriteBatch -> unit, backgroundColor) as this=
    inherit Game()

    do
        this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)

    let mutable spriteBatch: SpriteBatch = null
    let mutable textures: Map<'textureKey, Texture2D> = Map.empty
    let mutable fonts: Map<'fontKey, SpriteFont> = Map.empty

    override this.Initialize() =
        graphics.PreferredBackBufferWidth <- backBufferWidth
        graphics.PreferredBackBufferHeight <- backBufferHeight
        graphics.ApplyChanges()
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        base.Initialize()

    override this.LoadContent() = 
        textures <- this.Content |> textureLoader 
        fonts <- this.Content |> fontLoader 

    override this.Update delta =
        delta
        |> gameUpdater 

    override this.Draw delta =
        backgroundColor 
        |> this.GraphicsDevice.Clear
        spriteBatch.Begin()
        gameRenderer delta fonts textures spriteBatch
        spriteBatch.End()


