module Main

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type FirstName = FirstName of string
type LastName = LastName of string
type Age = Age of int


type Player =
    { FirstName: FirstName
      LastName: LastName
      Age: Age }

type MusicVolume = MusicVolume of int
type Settings = { MusicVolume: MusicVolume }

type Model =
    { Player: Player option
      Settings: Settings }

type Msg =
    | OnFailure of string
    | OnAboutClicked

module Storage =
    open Browser.WebStorage
    open Thoth.Json

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
        localStorage.setItem (dbName, Encode.Auto.toString (1, model))

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


let init =
    function
    | Some oldModel -> (oldModel, Cmd.none)
    | None ->
        ({ Player = None
           Settings = { MusicVolume = MusicVolume 50 } },
         Cmd.none)

let update msg model =
    match msg with
    | OnFailure err ->
        printfn "%A" err
        (model, Cmd.none)
    | OnAboutClicked -> (model, Cmd.none)

module View =
    open Feliz.Router

    [<ReactComponent>]
    let mainView () =
        let (model, dispatch) = React.useElmish (Storage.load >> init, update, [||])

        Html.div [ Html.h1 $"Hearties"
                   Html.button [ prop.text "Start Game" ]
                   Html.button [ prop.text "Settings" ]
                   Html.button [ prop.text "About" ] ]

ReactDOM.render (View.mainView, document.getElementById "feliz-app")
