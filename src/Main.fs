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


type Msg = | OnAboutClicked

let init =
    ({ Player = None
       Settings = { MusicVolume = MusicVolume 50 } },
     Cmd.none)

let update msg model =
    match msg with
    | OnAboutClicked -> model, Cmd.none

module View =
    [<ReactComponent>]
    let mainView () =
        let model, dispatch = React.useElmish (init, update, [||])

        Html.div [ Html.h1 $"Hearties"
                   Html.button [ prop.text "Start Game" ]
                   Html.button [ prop.text "Settings" ]
                   Html.button [ prop.text "About" ] ]

ReactDOM.render (View.mainView, document.getElementById "feliz-app")
