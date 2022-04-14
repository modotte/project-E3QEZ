module View.Root

open FSharpPlus.Lens

open Feliz
open Feliz.UseElmish
open Feliz.Router
open Fable.Core.JsInterop

open Domain

importSideEffects "../styles/global.scss"

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
                                <| OnNewCharacterEntriesUpdated(
                                    setl Player._firstName (PlayerFirstName.New(fn)) model.Player
                                )) ]
               Html.br []
               simpleLabel "Last Name"
               Html.input [ prop.required true
                            prop.onTextChange (fun ln ->
                                dispatch
                                <| OnNewCharacterEntriesUpdated(
                                    setl Player._lastName (PlayerLastName.New(ln)) model.Player
                                )) ]
               Html.br []
               simpleLabel "Age"
               Html.input [ prop.type'.range
                            prop.min Utility.PLAYER_MIN_AGE
                            prop.max Utility.PLAYER_MAX_AGE
                            prop.onChange (fun a ->
                                dispatch
                                <| OnNewCharacterEntriesUpdated(setl Player._age (PlayerAge.New(a)) model.Player)) ]

               Html.br []
               simpleLabel "Ship Name"
               Html.input [ prop.required true
                            prop.onTextChange (ShipName.New >> OnUpdateOwnedShipName >> dispatch) ]
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
    // TODO: Simplify below. Seems unnecesarily complicated
    Html.div [ simpleLabel <| Date.Formatted(model.Date)
               Html.p [ prop.text $"{Utility.currentLocation model.Location}" ]

               Html.p [ prop.text $"Coins: {PlayerCoins.Value(model.Player.Coins)}" ]
               let ship = model.Player.Ship

               Html.p [ let sn = ShipName.Value(model.Player.Ship.Name)
                        prop.text $"Current ship named {sn} of {ship.Class.ToString()} class" ]

               Html.p [ prop.text $"Your cargo: ${ship.Cargo.ToString()}" ] ]

let profilePage dispatch model =
    Html.div [ header dispatch
               backToMainNavigationButton "Back" dispatch ]

let skirmishLootPage dispatch model =
    Html.div [ header dispatch
               Html.p $"{model.Enemy.ToString()}"
               Html.hr []
               Html.p $"{model.Player.Ship.CargoCapacity}"
               Html.p $"{model.Player.Ship.Cargo}"

               backToMainNavigationButton "Continue" dispatch ]

let skirmishBoardBattlePage dispatch model =
    Html.div [ header dispatch
               match model.Enemy with
               | None -> backToMainNavigationButton "Continue" dispatch
               | Some enemy ->
                   Html.p $"{enemy.Ship.Crew.ToString()}"
                   Html.hr []
                   Html.p $"{model.Player.Ship.Crew.ToString()}"

                   if ShipCrew.Value(enemy.Ship.Crew) < Utility.SHIP_CREW_MINIMUM then
                       Html.button [ prop.text "Loot enemy ship"
                                     prop.onClick (fun _ -> dispatch OnSkirmishLootClicked) ]
                   else

                       Html.button [ prop.text "Sword"
                                     prop.onClick (fun _ -> dispatch OnSkirmishSwordClicked) ]

                       Html.button [ prop.text "Shoot Falconet"
                                     prop.onClick (fun _ -> dispatch OnSkirmishFalconetClicked) ] ]

let skirmishPage dispatch model =
    Html.div [ header dispatch
               Html.p $"{model.Enemy.ToString()}"
               Html.p $"{model.Player.Ship.ToString()}"
               Html.button [ prop.text "Evade"
                             prop.onClick (fun _ -> dispatch OnSkirmishEvadeClicked) ]
               Html.button [ prop.text "Chase"
                             prop.onClick (fun _ -> dispatch OnSkirmishCloseClicked) ]
               Html.button [ prop.text "Broadside"
                             prop.onClick (fun _ -> dispatch OnSkirmishBroadsideClicked) ] ]

let marketCargosSection dispatch model =
    // TODO: For now, let's just print all data but let's polish it later
    match model.Location with
    | PortRoyal p ->
        Html.div [ Html.ul [ Html.li [ Html.p $"{p.Cargo.Wood}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoBought(PortRoyal p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoSold(PortRoyal p)) ]
                             Html.li [ Html.p $"{p.Cargo.Sugar}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoBought(PortRoyal p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoSold(PortRoyal p)) ] ] ]
    | Barbados p ->
        Html.div [ Html.ul [ Html.li [ Html.p $"{p.Cargo.Wood}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoBought(Barbados p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoSold(Barbados p)) ]
                             Html.li [ Html.p $"{p.Cargo.Sugar}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoBought(Barbados p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoSold(Barbados p)) ] ] ]
    | Nassau p ->
        Html.div [ Html.ul [ Html.li [ Html.p $"{p.Cargo.Wood}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoBought(Nassau p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnWoodCargoSold(Nassau p)) ]
                             Html.li [ Html.p $"{p.Cargo.Sugar}" ]
                             Html.button [ prop.text "Buy"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoBought(Nassau p)) ]
                             Html.button [ prop.text "Sell"
                                           prop.onClick (fun _ -> dispatch <| OnSugarCargoSold(Nassau p)) ] ] ]


let marketPage dispatch model =
    Html.div [ header dispatch
               Html.button [ prop.text "Back"
                             prop.onClick (fun _ -> dispatch OnDockClicked) ]
               metaInfoSection dispatch model

               Html.hr []
               marketCargosSection dispatch model ]

let dockPage dispatch model =
    Html.div [ header dispatch
               backToMainNavigationButton "Back" dispatch
               Html.hr []
               metaInfoSection dispatch model
               Html.hr []
               Html.button [ prop.text "Tavern" ]
               Html.button [ prop.text "Shipyard" ]
               Html.button [ prop.text "Market"
                             prop.onClick (fun _ -> dispatch OnMarketClicked) ]
               Html.button [ prop.text "Governor" ] ]

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

               Html.ul [ Html.li [ Html.button [ prop.text "Port Royal"
                                                 prop.onClick (fun _ ->
                                                     dispatch
                                                     <| OnLocationTravel(PortRoyal Initializer.Port.portRoyal)) ] ]
                         Html.li [ Html.button [ prop.text "Barbados"
                                                 prop.onClick (fun _ ->
                                                     dispatch
                                                     <| OnLocationTravel(Barbados Initializer.Port.barbados)) ] ]
                         Html.li [ Html.button [ prop.text "Nassau"
                                                 prop.onClick (fun _ ->
                                                     dispatch
                                                     <| OnLocationTravel(Nassau Initializer.Port.nassau)) ] ] ] ]

[<ReactComponent>]
let MainView () =
    let (model, dispatch) =
        // TODO: Turn below into manual save
        React.useElmish (Storage.load >> Initializer.init, Storage.updateStorage UpdateHandler.update, [||])

    React.router [ router.onUrlChanged (OnUrlChanged >> dispatch)
                   router.children [ match model.CurrentUrl with
                                     | [] -> mainMenuPage dispatch model
                                     | [ "newCharacterPage" ] -> newCharacterPage dispatch model
                                     | [ "mainNavigationPage" ] -> mainNavigationPage dispatch model
                                     | [ "profilePage" ] -> profilePage dispatch model
                                     | [ "skirmishPage" ] -> skirmishPage dispatch model
                                     | [ "skirmishBoardBattlePage" ] -> skirmishBoardBattlePage dispatch model
                                     | [ "skirmishLootPage" ] -> skirmishLootPage dispatch model

                                     | [ "dockPage" ] -> dockPage dispatch model
                                     | [ "marketPage" ] -> marketPage dispatch model
                                     | [ "settingsPage" ] -> settingsPage dispatch model

                                     | [ "aboutPage" ] -> aboutPage dispatch model
                                     | _ -> Html.h1 "Not found" ] ]
