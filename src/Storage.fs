module Storage

open Elmish
open Browser.WebStorage
open Thoth.Json

open Domain

let private dbName = "hearties-db"
let private decoder = Decode.Auto.generateDecoder<Model> ()

let load () =
    localStorage.getItem (dbName)
    |> unbox
    |> Option.bind (
        Decode.fromString decoder
        >> function
            | Ok model -> Some model
            | _ -> None
    )

let save (model: Model) =
    let space = 1
    localStorage.setItem (dbName, Encode.Auto.toString (space, model))

let updateStorage update (message: Msg) (model: Model) =
    let setStorage (model: Model) =
        Cmd.OfFunc.attempt save model (string >> OnFailure)

    match message with
    | OnFailure _ -> (model, Cmd.none)
    | _ ->
        let (newModel, commands) = update message model

        (newModel,
         Cmd.batch [ setStorage newModel
                     commands ])
