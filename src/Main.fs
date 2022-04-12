module Main

open Feliz
open Feliz.UseElmish
open Feliz.Router
open Elmish
open Browser.Dom
open Fable.Core.JsInterop
open Domain

importSideEffects "./styles/global.scss"

module Cargo =
    let wood =
        { Name = CargoName.New("Wood")
          Description = CargoDescription.New("Used in repairing ship hull and buildings")
          Price = CargoPrice.New(20)
          Unit = CargoUnit.New(7) }

    let sugar =
        { Name = CargoName.New("Sugar")
          Description = CargoDescription.New("Used in tea and coffee")
          Price = CargoPrice.New(57)
          Unit = CargoUnit.New(3) }

[<Literal>]
let DEFAULT_SHIP_NAME = "Heart of Ocean"

module ShipKind =
    let private primary =
        { Id = ShipId.New()
          Name = ShipName.New(DEFAULT_SHIP_NAME)
          Size = Light
          Class = Cutter
          CargoCapacity = CargoCapacity.New(20)
          OwnedCargo =
            { Wood = Cargo.wood
              Sugar = Cargo.sugar }
          CrewCapacity = CrewCapacity.New(20)
          OwnedCrew = OwnedCrew.New(20)
          Nationality = British
          Hull = ShipHull.New(5)
          Sail = ShipSail.New(4) }

    let cutter = primary

    let sloop =
        { primary with
            Class = Sloop
            CargoCapacity = CargoCapacity.New(82)
            CrewCapacity = CrewCapacity.New(40)
            OwnedCrew = OwnedCrew.New(40)
            Hull = ShipHull.New(11)
            Sail = ShipSail.New(11) }

    let junk =
        { primary with
            Class = Junk
            CargoCapacity = CargoCapacity.New(75)
            CrewCapacity = CrewCapacity.New(35)
            OwnedCrew = OwnedCrew.New(35)
            Hull = ShipHull.New(8)
            Sail = ShipSail.New(14) }

    let galleon =
        { primary with
            Class = Galleon
            CargoCapacity = CargoCapacity.New(152)
            CrewCapacity = CrewCapacity.New(64)
            OwnedCrew = OwnedCrew.New(64)
            Hull = ShipHull.New(15)
            Sail = ShipSail.New(10) }

    let frigate =
        { primary with
            Class = Frigate
            CargoCapacity = CargoCapacity.New(300)
            CrewCapacity = CrewCapacity.New(125)
            OwnedCrew = OwnedCrew.New(125)
            Hull = ShipHull.New(30)
            Sail = ShipSail.New(17) }



module Port =
    let portRoyal =
        { Name = PortName.New("Port Royal")
          Description = PortDescription.New("A rich port")
          Size = Large
          Nationality = British
          Cargo =
            { Wood = { Cargo.wood with Unit = CargoUnit.New(270) }
              Sugar = { Cargo.sugar with Unit = CargoUnit.New(100) } } }

    let barbados =
        { Name = PortName.New("Barbados")
          Description = PortDescription.New("A wealthy port")
          Size = Medium
          Nationality = British
          Cargo =
            { Wood = { Cargo.wood with Unit = CargoUnit.New(167) }
              Sugar = { Cargo.sugar with Unit = CargoUnit.New(82) } } }

    let nassau =
        { Name = PortName.New("Nassau")
          Description = PortDescription.New("A poor port")
          Size = Small
          Nationality = British
          Cargo =
            { Wood = { Cargo.wood with Unit = CargoUnit.New(60) }
              Sugar = { Cargo.sugar with Unit = CargoUnit.New(20) } } }


let init =
    function
    | Some oldModel -> (oldModel, Cmd.none)
    | None ->
        ({ Date = Date.New() // Every location change should pass at least 1 day
           Location = PortRoyal Port.portRoyal
           Player =
             { FirstName = PlayerFirstName.New("Johnathan")
               LastName = PlayerLastName.New("Smith")
               Age = PlayerAge.New(18)
               Coins = PlayerCoins.New(650)
               OwnedShip = ShipKind.sloop }
           EnemyShip = None
           Settings = { MusicVolume = MusicVolume.New(50) }
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
    | OnSkirmishClicked ->
        // We initialize battle state
        // TODO: Randomize enemy ship data

        let randomizedInitialDistance () =
            let cases = Reflection.FSharpType.GetUnionCases(typeof<ShipDistance>)
            let index = System.Random().Next(cases.Length)
            let case = cases[index]
            Reflection.FSharpValue.MakeUnion(case, [||]) :?> ShipDistance

        let enemyShip =
            Some
            <| { Ship = { ShipKind.junk with Name = ShipName.New("Skeleton Heart") }
                 Movement = Still
                 Distance = randomizedInitialDistance () }


        ({ model with EnemyShip = enemyShip }, Cmd.navigate "skirmishPage")
    | OnSkirmishEvadeClicked ->
        match model.EnemyShip with
        | None -> (model, Cmd.navigateBack ())
        | Some enemyShip ->
            match enemyShip.Distance with
            | Escape -> ({ model with EnemyShip = None }, Cmd.navigate "mainNavigationPage")
            | Far -> ({ model with EnemyShip = Some { enemyShip with Distance = Escape } }, Cmd.none)
            | Close -> ({ model with EnemyShip = Some { enemyShip with Distance = Far } }, Cmd.none)
            | Board -> (model, Cmd.none) // TODO: Go to board battle page

    | OnDockClicked -> (model, Cmd.navigate "dockPage")
    | OnMarketClicked -> (model, Cmd.navigate "marketPage")

    | OnWoodCargoBought loc ->
        let addIntoPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Wood.Price)
            let ownedWoodUnit = CargoUnit.Value(ownedCargo.Wood.Unit)

            let ownedCargo =
                { model.Player.OwnedShip.OwnedCargo.Wood with Unit = CargoUnit.New(ownedWoodUnit + 1) }

            let ownedCargo = { model.Player.OwnedShip.OwnedCargo with Wood = ownedCargo }

            let ownedShip = { model.Player.OwnedShip with OwnedCargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins - price)
                OwnedShip = ownedShip }

        let removeFromPortCargo port =
            let portWoodUnit = CargoUnit.Value(port.Cargo.Wood.Unit)
            let portWood = { port.Cargo.Wood with Unit = CargoUnit.New(portWoodUnit - 1) }
            let portCargo = { port.Cargo with Wood = portWood }
            { port with Cargo = portCargo }

        match loc with
        | PortRoyal p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)

    | OnSugarCargoBought loc ->
        let addIntoPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Sugar.Price)
            let ownedSugarUnit = CargoUnit.Value(ownedCargo.Sugar.Unit)

            let ownedCargo =
                { model.Player.OwnedShip.OwnedCargo.Sugar with Unit = CargoUnit.New(ownedSugarUnit + 1) }

            let ownedCargo = { model.Player.OwnedShip.OwnedCargo with Sugar = ownedCargo }

            let ownedShip = { model.Player.OwnedShip with OwnedCargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins - price)
                OwnedShip = ownedShip }

        let removeFromPortCargo port =
            let portSugarUnit = CargoUnit.Value(port.Cargo.Sugar.Unit)
            let portSugar = { port.Cargo.Sugar with Unit = CargoUnit.New(portSugarUnit - 1) }
            let portCargo = { port.Cargo with Sugar = portSugar }
            { port with Cargo = portCargo }

        match loc with
        | PortRoyal p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = addIntoPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(removeFromPortCargo p) },
             Cmd.none)

    | OnWoodCargoSold loc ->
        let removeFromPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Wood.Price)
            let ownedWoodUnit = CargoUnit.Value(ownedCargo.Wood.Unit)

            let ownedCargo =
                { model.Player.OwnedShip.OwnedCargo.Wood with Unit = CargoUnit.New(ownedWoodUnit - 1) }

            let ownedCargo = { model.Player.OwnedShip.OwnedCargo with Wood = ownedCargo }

            let ownedShip = { model.Player.OwnedShip with OwnedCargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins + price)
                OwnedShip = ownedShip }

        let addIntoPortCargo port =
            let portWoodUnit = CargoUnit.Value(port.Cargo.Wood.Unit)
            let portWood = { port.Cargo.Wood with Unit = CargoUnit.New(portWoodUnit + 1) }
            let portCargo = { port.Cargo with Wood = portWood }
            { port with Cargo = portCargo }

        match loc with
        | PortRoyal p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

    | OnSugarCargoSold loc ->
        let removeFromPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Sugar.Price)
            let ownedSugarUnit = CargoUnit.Value(ownedCargo.Sugar.Unit)

            let ownedCargo =
                { model.Player.OwnedShip.OwnedCargo.Wood with Unit = CargoUnit.New(ownedSugarUnit - 1) }

            let ownedCargo = { model.Player.OwnedShip.OwnedCargo with Sugar = ownedCargo }

            let ownedShip = { model.Player.OwnedShip with OwnedCargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins + price)
                OwnedShip = ownedShip }

        let addIntoPortCargo port =
            let portSugarUnit = CargoUnit.Value(port.Cargo.Sugar.Unit)
            let portSugar = { port.Cargo.Sugar with Unit = CargoUnit.New(portSugarUnit + 1) }
            let portCargo = { port.Cargo with Sugar = portSugar }
            { port with Cargo = portCargo }

        match loc with
        | PortRoyal p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.OwnedShip.OwnedCargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

    | OnUpdateOwnedShipName name ->
        let player =
            let ship = { model.Player.OwnedShip with Name = name }
            { model.Player with OwnedShip = ship }

        ({ model with Player = player }, Cmd.none)

    | OnUpdateOwnedShipClass shipClass ->
        let player =
            let ship = { model.Player.OwnedShip with Class = shipClass }

            { model.Player with OwnedShip = ship }

        ({ model with Player = player }, Cmd.none)
    | OnUpdateLocation location ->
        ({ model with
            Location = location
            Date = Date.TomorrowAfterToday(model.Date) },
         Cmd.none)
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
                                    <| OnNewCharacterEntriesUpdated
                                        { model.Player with FirstName = PlayerFirstName.New(fn) }) ]
                   Html.br []
                   simpleLabel "Last Name"
                   Html.input [ prop.required true
                                prop.onTextChange (fun ln ->
                                    dispatch
                                    <| OnNewCharacterEntriesUpdated
                                        { model.Player with LastName = PlayerLastName.New(ln) }) ]
                   Html.br []
                   simpleLabel "Age"
                   Html.input [ prop.type'.range
                                prop.min 18
                                prop.max 75
                                prop.onChange (fun a ->
                                    dispatch
                                    <| OnNewCharacterEntriesUpdated { model.Player with Age = PlayerAge.New(a) }) ]

                   Html.br []
                   simpleLabel "Ship Name"
                   Html.input [ prop.required true
                                prop.onTextChange (fun n -> dispatch <| OnUpdateOwnedShipName(ShipName.New(n))) ]
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
                   Html.p [ prop.text $"{currentLocation model.Location}" ]

                   Html.p [ prop.text $"Coins: {PlayerCoins.Value(model.Player.Coins)}" ]
                   let ship = model.Player.OwnedShip

                   Html.p [ let sn = ShipName.Value(model.Player.OwnedShip.Name)
                            prop.text $"Current ship named {sn} of {ship.Class.ToString()} class" ]

                   Html.p [ prop.text $"Your cargo: ${ship.OwnedCargo.ToString()}" ] ]

    let profilePage dispatch model =
        Html.div [ header dispatch
                   backToMainNavigationButton "Back" dispatch ]

    let skirmishPage dispatch model =
        Html.div [ header dispatch
                   Html.p $"{model.EnemyShip.ToString()}"
                   Html.button [ prop.text "Evade"
                                 prop.onClick (fun _ -> dispatch OnSkirmishEvadeClicked) ]
                   Html.button [ prop.text "Chase" ]
                   Html.button [ prop.text "Broadside" ] ]

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
                                                         <| OnUpdateLocation(PortRoyal Port.portRoyal)) ] ]
                             Html.li [ Html.button [ prop.text "Barbados"
                                                     prop.onClick (fun _ ->
                                                         dispatch
                                                         <| OnUpdateLocation(Barbados Port.barbados)) ] ]
                             Html.li [ Html.button [ prop.text "Nassau"
                                                     prop.onClick (fun _ ->
                                                         dispatch <| OnUpdateLocation(Nassau Port.nassau)) ] ] ] ]

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
                                         | [ "marketPage" ] -> marketPage dispatch model
                                         | [ "settingsPage" ] -> settingsPage dispatch model

                                         | [ "aboutPage" ] -> aboutPage dispatch model
                                         | _ -> Html.h1 "Not found" ] ]

ReactDOM.render (View.mainView, document.getElementById "feliz-app")
