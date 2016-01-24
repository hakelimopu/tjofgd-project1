module GameJoltApi

open System.Security.Cryptography
open BoardState

type GameJoltConfig =
    {GameId:int;
    PrivateKey:string;
    UserName:string;
    UserToken:string;
    Authenticated:bool}

let mutable private gameJoltConfig: GameJoltConfig = 
    {GameId=0;
    PrivateKey="";
    UserName="";
    UserToken="";
    Authenticated=false}

let loadConfig () =
    gameJoltConfig

let saveConfig config =
    gameJoltConfig <- config

let initConfig gameId privateKey =
    {loadConfig() with GameId=gameId; PrivateKey=privateKey}
    |> saveConfig

let setUser userName userToken =
    {loadConfig() with UserName=userName;UserToken=userToken}
    |> saveConfig

let private successResponse = "success:\"true\"\r\n"
let private failureResponse = "success:\"false\"\r\n"

let private makeRequest baseUrl =
    let config = loadConfig()
    let hashMe = sprintf "%s%s" baseUrl config.PrivateKey
    let md5 = MD5.Create()
    let bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashMe))
    let signature =
        bytes
        |> Seq.fold (fun s b -> s+System.String.Format("{0:x2}",b)) System.String.Empty
    let finalUrl = sprintf "%s&signature=%s" baseUrl signature
    let request = System.Net.WebRequest.Create(finalUrl)
    request.Method <- "GET"
    try
        let response = request.GetResponse()
        use stream = response.GetResponseStream()
        use reader = new System.IO.StreamReader(stream)
        reader.ReadToEnd()
    with
        | :? System.Net.WebException -> failureResponse

let isUserAuthenticated () =
    loadConfig().Authenticated

let authenticateUser () =
    let config = loadConfig()
    let result = 
        sprintf "http://gamejolt.com/api/game/v1/users/auth/?game_id=%i&username=%s&user_token=%s" config.GameId config.UserName config.UserToken
        |> makeRequest
    {config with Authenticated = (result = successResponse)}
    |> saveConfig
    isUserAuthenticated()
    
    
let addAchieved trophyId =
    if isUserAuthenticated() then
        let config = loadConfig()
        sprintf "http://gamejolt.com/api/game/v1/trophies/add-achieved/?game_id=%i&username=%s&user_token=%s&trophy_id=%i" config.GameId config.UserName config.UserToken trophyId
        |> makeRequest
        |> ignore
    else
        ()

let addScore score =
    if isUserAuthenticated() then
        let config = loadConfig()
        sprintf "http://gamejolt.com/api/game/v1/scores/add/?game_id=%i&username=%s&user_token=%s&score=%i&sort=%i" config.GameId config.UserName config.UserToken score score
        |> makeRequest
        |> ignore
    else
        ()

let private stripValue (text:string) =
    let quotedValue = text.Split([|':'|]).[1]
    quotedValue.Substring(1,quotedValue.Length-2)

let rec private processScores (highScores: BoardState.HighScore list) (lines:string list) = 
    match lines with
    | score :: sort :: extra_data :: user :: user_id :: guest :: stored :: tail -> tail |> processScores ({Score=score |> stripValue;User=user |> stripValue;Stored=stored |> stripValue} :: highScores)
    | _ -> highScores

let getScores () =
    let config = loadConfig()
    let scores = 
        sprintf "http://gamejolt.com/api/game/v1/scores/?game_id=%i" config.GameId
        |> makeRequest
    let lines = scores.Split([|"\r\n"|],System.StringSplitOptions.RemoveEmptyEntries) |> Array.toList
    match lines with
    | head :: tail -> if head="success:\"true\"" then tail |> processScores List.empty<BoardState.HighScore> else List.empty<BoardState.HighScore>
    | _ -> List.empty<BoardState.HighScore>
    
