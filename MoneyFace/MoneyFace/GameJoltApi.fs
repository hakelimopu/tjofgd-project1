module GameJoltApi

open System.Security.Cryptography

let private gjGameId = 120704
let private gjPrivateKey = "ef3dafd2f890e215f079c22c4c61335e"

//BOOO! MUTABLE!
let mutable private gjUserName = ""
let mutable private gjUserToken = ""
let mutable private gjUserAuthenticated = false

let setUser userName userToken =
    gjUserName <- userName
    gjUserToken <- userToken

let private successResponse = "success:\"true\"\r\n"
let private failureResponse = "success:\"false\"\r\n"

let private makeRequest baseUrl =
    let hashMe = sprintf "%s%s" baseUrl gjPrivateKey
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

let authenticateUser () =
    let result = 
        sprintf "http://gamejolt.com/api/game/v1/users/auth/?game_id=%i&username=%s&user_token=%s" gjGameId gjUserName gjUserToken
        |> makeRequest
    gjUserAuthenticated <- result = successResponse
    gjUserAuthenticated
    
let isUserAuthenticated () =
    gjUserAuthenticated

let addAchieved trophyId =
    if isUserAuthenticated() then
        sprintf "http://gamejolt.com/api/game/v1/trophies/add-achieved/?game_id=%i&username=%s&user_token=%s&trophy_id=%i" gjGameId gjUserName gjUserToken trophyId
        |> makeRequest
        |> ignore
    else
        ()

let addScore score =
    if isUserAuthenticated() then
        sprintf "http://gamejolt.com/api/game/v1/scores/add/?game_id=%i&username=%s&user_token=%s&score=%i&sort=%i" gjGameId gjUserName gjUserToken score score
        |> makeRequest
        |> ignore
    else
        ()
