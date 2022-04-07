module Main

open Feliz
open Feliz.UseElmish
open Feliz.Router
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

open Domain

importSideEffects "./styles/global.scss"

let init =
    function
    | Some oldModel -> (oldModel, Cmd.none)
    | None ->
        ({ DaysPassed = DaysPassed 1 // Every location change should pass at least 1 day
           Player =
             { FirstName = PlayerFirstName "Johnathan"
               LastName = PlayerLastName "Smith"
               Age = PlayerAge 18
               Coins = PlayerCoins 250
               OwnedShip =
                 Some
                     { Id = ShipId <| System.Guid.NewGuid()
                       Name = ShipName "Heart of Ocean"
                       Size = Light
                       Class = Sloop
                       CargoCapacity = CargoCapacity 350
                       OwnedCargo =
                         [| { Kind = Wood
                              BasePrice = CargoBasePrice 20
                              Unit = CargoUnit 8 } |]
                       CrewCapacity = CrewCapacity 18
                       OwnedCrew = OwnedCrew 6 }
               CurrentLocation = PortRoyal }
           Settings = { MusicVolume = MusicVolume 50 }
           CurrentUrl = Router.currentUrl () },
         Cmd.none)

let update msg model =
    match msg with
    | OnFailure err ->
        printfn "%s" err
        (model, Cmd.none)
    | OnUrlChanged segment -> ({ model with CurrentUrl = segment }, Cmd.none)
    | OnMainMenuClicked -> (model, Cmd.navigate "")


    | OnStartGameClicked -> (model, Cmd.navigate "newCharacterPage")
    | OnMainNavigationClicked -> (model, Cmd.navigate "mainNavigationPage")
    | OnProfileClicked -> (model, Cmd.navigate "profilePage")
    | OnSkirmishClicked -> (model, Cmd.navigate "skirmishPage")
    | OnDockClicked -> (model, Cmd.navigate "dockPage")

    | OnUpdateOwnedShipName name ->
        let player =
            match model.Player.OwnedShip with
            | None -> model.Player
            | Some ship ->
                let ship' = { ship with Name = name }
                { model.Player with OwnedShip = Some ship' }

        ({ model with Player = player }, Cmd.none)

    | OnUpdateOwnedShipClass shipClass ->
        let player =
            match model.Player.OwnedShip with
            | None -> model.Player
            | Some ship ->
                let ship' = { ship with Class = shipClass }

                { model.Player with OwnedShip = Some ship' }

        ({ model with Player = player }, Cmd.none)
    | OnUpdateLocation location ->
        let player = { model.Player with CurrentLocation = location }
        ({ model with Player = player }, Cmd.none)
    | OnNewCharacterEntriesUpdated player -> { model with Player = player }, Cmd.none


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
        Html.button [ prop.text text
                      prop.onClick (fun _ -> dispatch OnMainMenuClicked) ]

    let backToMainNavigationButton (text: string) dispatch =
        Html.button [ prop.text text
                      prop.onClick (fun _ -> dispatch OnMainNavigationClicked) ]

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
                   Html.input [ prop.required true
                                prop.onTextChange (fun fn ->
                                    dispatch
                                    <| OnNewCharacterEntriesUpdated { model.Player with FirstName = PlayerFirstName fn }) ]
                   Html.br []
                   simpleLabel "Last Name"
                   Html.input [ prop.required true
                                prop.onTextChange (fun ln ->
                                    dispatch
                                    <| OnNewCharacterEntriesUpdated { model.Player with LastName = PlayerLastName ln }) ]
                   Html.br []
                   simpleLabel "Age"
                   Html.input [ prop.type'.range
                                prop.min 18
                                prop.max 75
                                prop.onChange (fun a ->
                                    dispatch
                                    <| OnNewCharacterEntriesUpdated { model.Player with Age = PlayerAge a }) ]

                   Html.br []
                   simpleLabel "Ship Name"
                   Html.input [ prop.required true
                                prop.onTextChange (fun n -> dispatch <| OnUpdateOwnedShipName(ShipName n)) ]
                   Html.br []
                   simpleLabel "Ship Class"
                   Html.select [ prop.children [ Html.option [ prop.value "Sloop"
                                                               prop.text "Sloop" ]
                                                 Html.option [ prop.value "Brig"
                                                               prop.text "Brig" ]
                                                 Html.option [ prop.value "Junk"
                                                               prop.text "Junk" ] ]
                                 prop.onChange (fun (sc: string) ->
                                     dispatch
                                     <| OnUpdateOwnedShipClass(
                                         match sc with
                                         | "Brig" -> Brig
                                         | "Junk" -> Junk
                                         | _ -> Sloop
                                     )) ]

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

    let metaInfoSection dispatch model =
        Html.div [ Html.p [ prop.text $"Docked at Port of {model.Player.CurrentLocation}" ]

                   let (PlayerCoins coins) = model.Player.Coins
                   Html.p [ prop.text $"Coins: {coins}" ]

                   Html.p [ match model.Player.OwnedShip with
                            | None -> prop.text ""
                            | Some ship ->
                                let (ShipName sn) = ship.Name
                                prop.text $"Current ship named {sn} of {ship.Class.ToString()} class" ] ]

    let profilePage dispatch model =
        Html.div [ header dispatch
                   backToMainNavigationButton "Back" dispatch ]

    let skirmishPage dispatch model =
        Html.div [ header dispatch
                   backToMainNavigationButton "Back" dispatch ]

    let dockPage dispatch model =
        Html.div [ header dispatch
                   backToMainNavigationButton "Back" dispatch
                   Html.hr []
                   metaInfoSection dispatch model ]

    let mainNavigationPage dispatch model =
        Html.div [ header dispatch
                   Html.button [ prop.text "Profile"
                                 prop.onClick (fun _ -> dispatch OnProfileClicked) ]
                   Html.button [ prop.text "Skirmish"
                                 prop.onClick (fun _ -> dispatch OnSkirmishClicked) ]
                   Html.button [ prop.text "Dock"
                                 prop.onClick (fun _ -> dispatch OnDockClicked) ]
                   Html.hr []

                   metaInfoSection dispatch model

                   Html.ul [ Html.li [ Html.button [ prop.text "Barbados"
                                                     prop.onClick (fun _ -> dispatch <| OnUpdateLocation Barbados) ] ]
                             Html.li [ Html.button [ prop.text "Port Royal"
                                                     prop.onClick (fun _ -> dispatch <| OnUpdateLocation PortRoyal) ] ]
                             Html.li [ Html.button [ prop.text "Nassau"
                                                     prop.onClick (fun _ -> dispatch <| OnUpdateLocation Nassau) ] ]
                             Html.li [ Html.button [ prop.text "Havana"
                                                     prop.onClick (fun _ -> dispatch <| OnUpdateLocation Havana) ] ] ] ]

    [<ReactComponent>]
    let mainView () =
        let (model, dispatch) =
            // TODO: Turn below into manual save
            React.useElmish (Storage.load >> init, Storage.updateStorage update, [||])

        React.router [ router.onUrlChanged (OnUrlChanged >> dispatch)
                       router.children [ match model.CurrentUrl with
                                         | [] -> mainMenuPage dispatch model
                                         | [ "newCharacterPage" ] -> newCharacterPage dispatch model
                                         | [ "mainNavigationPage" ] -> mainNavigationPage dispatch model
                                         | [ "profilePage" ] -> profilePage dispatch model
                                         | [ "skirmishPage" ] -> skirmishPage dispatch model
                                         | [ "dockPage" ] -> dockPage dispatch model
                                         | [ "settingsPage" ] -> settingsPage dispatch model
                                         | [ "aboutPage" ] -> aboutPage dispatch model
                                         | _ -> Html.h1 "Not found" ] ]

ReactDOM.render (View.mainView, document.getElementById "feliz-app")
