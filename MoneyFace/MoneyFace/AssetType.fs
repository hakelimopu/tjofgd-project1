module AssetType

type AssetType<'texture, 'font, 'effect> =
    | Texture of 'texture
    | Font of 'font
    | Effect of 'effect

let getTexture (asset:AssetType<'texture, 'font, 'effect>) =
    match asset with
    | Texture value -> Some value
    | _ -> None

let getFont (asset:AssetType<'texture, 'font, 'effect>) =
    match asset with
    | Font value -> Some value
    | _ -> None

let getSoundEffect (asset:AssetType<'texture, 'font, 'effect>) =
    match asset with
    | Effect value -> Some value
    | _ -> None



