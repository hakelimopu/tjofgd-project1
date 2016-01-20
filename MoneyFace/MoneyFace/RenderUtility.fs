module RenderUtility

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open TJoFGDGame

let drawTexture (x,y) (texture:Texture2D) (spriteBatch: SpriteBatch) =
    spriteBatch.Draw (texture, new Rectangle(x - texture.Width/2, y - texture.Height/2, texture.Width, texture.Height), Color.White)

let drawText (x,y) (text:string) (color:Color) (font:SpriteFont) (spriteBatch: SpriteBatch)=
    spriteBatch.DrawString(font,text,new Vector2(x |> float32,y |> float32),color)

let drawInt xy score (font:SpriteFont) (spriteBatch: SpriteBatch) =
    let text = score |> sprintf "%i"
    drawText xy text Color.White font spriteBatch

