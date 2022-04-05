module Main

open Feliz
open Feliz.UseElmish
open Feliz.Router
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type ShipId = ShipId of System.Guid
type ShipName = ShipName of string

type ShipSizeKind =
    | Light
    | Medium
    | Heavy
    | Flag

type ShipSize = ShipSize of ShipSizeKind

type ShipClass =
    | Cutter
    | Sloop
    | Brig
    | Junk
    | Galleon
    | WarGalleon
    | Frigate
    | ``Man O' War``
    | SecondRate
    | FirstRate

type Ship =
    { Id: ShipId
      Name: ShipName
      Size: ShipSizeKind
      Class: ShipClass }

type Location =
    | Barbados
    | PortRoyal
    | Nassau

type PlayerFirstName = PlayerFirstName of string
type PlayerLastName = PlayerLastName of string
type PlayerCoins = PlayerCoins of int
type PlayerAge = PlayerAge of int

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Coins: PlayerCoins
      Age: PlayerAge
      OwnedShip: Ship option
      CurrentLocation: Location }

type MusicVolume = MusicVolume of int
type Settings = { MusicVolume: MusicVolume }

type Model =
    { Player: Player option
      Settings: Settings
      CurrentUrl: string list }

type Msg =
    | OnFailure of string
    | OnUrlChanged of string list
    | OnMainMenuClicked
    | OnStartGameClicked
    | OnMainNavigationClicked
    | OnSettingsClicked
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


let init =
    function
    | Some oldModel -> (oldModel, Cmd.none)
    | None ->
        ({ Player = None
           Settings = { MusicVolume = MusicVolume 50 }
           CurrentUrl = Router.currentUrl () },
         Cmd.none)

let withOnMainNagivationClicked model =
    (model, Cmd.navigate "mainNavigationPage")

let update msg model =
    match msg with
    | OnFailure err ->
        printfn "%s" err
        (model, Cmd.none)
    | OnUrlChanged segment -> ({ model with CurrentUrl = segment }, Cmd.none)
    | OnMainMenuClicked -> (model, Cmd.navigate "")


    | OnStartGameClicked -> (model, Cmd.navigate "newCharacterPage")
    | OnMainNavigationClicked -> withOnMainNagivationClicked model


    | OnSettingsClicked -> (model, Cmd.navigate "settingsPage")
    | OnAboutClicked -> (model, Cmd.navigate "aboutPage")

module View =
    let simpleLabel (text: string) = Html.label [ prop.text text ]

    let header dispatch =
        Html.div [ Html.h1 "Hearties"
                   Html.br []
                   Html.button [ prop.text "Back to main menu"
                                 prop.onClick (fun _ -> dispatch OnMainMenuClicked) ]
                   Html.hr [] ]

    let backToMainMenuButton (text: string) dispatch =
        Html.div [ Html.button [ prop.text text
                                 prop.onClick (fun _ -> dispatch OnMainMenuClicked) ] ]

    let mainMenuPage dispatch model =
        Html.div [ header dispatch
                   Html.button [ prop.text "Start Game"
                                 prop.onClick (fun _ -> dispatch OnStartGameClicked) ]
                   Html.button [ prop.text "Settings"
                                 prop.onClick (fun _ -> dispatch OnSettingsClicked) ]
                   Html.button [ prop.text "About"
                                 prop.onClick (fun _ -> dispatch OnAboutClicked) ] ]

    let newCharacterPage dispatch model =
        Html.div [ header dispatch
                   Html.br []
                   simpleLabel "First Name"
                   Html.input [ prop.required true ]
                   Html.br []
                   simpleLabel "Last Name"
                   Html.input [ prop.required true ]
                   Html.br []
                   simpleLabel "Age"
                   Html.input [ prop.type'.range
                                prop.min 18
                                prop.min 75 ]
                   Html.br []
                   Html.button [ prop.text "Continue"
                                 prop.onClick (fun _ -> dispatch OnMainNavigationClicked) ] ]

    let settingsPage dispatch model =
        Html.div [ header dispatch
                   Html.button [ prop.text "Music On/Off" ]
                   backToMainMenuButton "Back" dispatch ]

    let aboutPage dispatch model =
        Html.div [ header dispatch
                   backToMainMenuButton "Back" dispatch ]

    let mainNavigationPage dispatch model =
        Html.div [ header dispatch
                   Html.button [ prop.text "Profile" ]
                   Html.button [ prop.text "Skirmish" ]
                   Html.button [ prop.text "Dock" ] ]

    [<ReactComponent>]
    let mainView () =
        let (model, dispatch) = React.useElmish (Storage.load >> init, update, [||])

        React.router [ router.onUrlChanged (OnUrlChanged >> dispatch)
                       router.children [ match model.CurrentUrl with
                                         | [] -> mainMenuPage dispatch model
                                         | [ "newCharacterPage" ] -> newCharacterPage dispatch model
                                         | [ "mainNavigationPage" ] -> mainNavigationPage dispatch model
                                         | [ "settingsPage" ] -> settingsPage dispatch model
                                         | [ "aboutPage" ] -> aboutPage dispatch model
                                         | _ -> Html.h1 "Not found" ] ]

ReactDOM.render (View.mainView, document.getElementById "feliz-app")
