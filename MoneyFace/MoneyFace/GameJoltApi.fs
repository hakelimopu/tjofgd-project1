module GameJoltApi

open System.Security.Cryptography

let private gjGameId = 120704
let private gjPrivateKey = "8fa287db58661cb6c498ca6b39cd8f58"//won't work... already reset it :)

//BOOO! MUTABLE!
let mutable private gjUserName = ""
let mutable private gjUserToken = ""

let setUser userName userToken =
    gjUserName <- userName
    gjUserToken <- userToken

let authenticateUser () =
    let baseUrl = sprintf "http://gamejolt.com/api/game/v1/users/auth/?game_id=%i&username=%s&user_token=%s" gjGameId gjUserName gjUserToken
    let hashMe = sprintf "%s%s" baseUrl gjPrivateKey
    let md5 = MD5.Create()
    let bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashMe))
    let signature =
        bytes
        |> Seq.fold (fun s b -> s+System.String.Format("{0:x2}",b)) System.String.Empty
    let finalUrl = sprintf "%s&signature=%s" baseUrl signature
    let request = System.Net.WebRequest.Create(finalUrl)
    request.Method <- "GET"
    let response = request.GetResponse()
    use stream = response.GetResponseStream()
    use reader = new System.IO.StreamReader(stream)
    let result = reader.ReadToEnd()
    result