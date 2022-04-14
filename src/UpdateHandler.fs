module UpdateHandler

open Feliz
open Feliz.Router
open Elmish
open Domain
open Initializer

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

        let enemy =
            Some
            <| { Ship = { ShipKind.junk with Name = ShipName.New("Skeleton Heart") }
                 Movement = Chase
                 Distance = randomizedInitialDistance () }


        ({ model with Enemy = enemy }, Cmd.navigate "skirmishPage")
    | OnSkirmishEvadeClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigateBack ())
        | Some enemy ->
            if ShipHull.Value(model.Player.Ship.Hull) < Utility.SHIP_HULL_MINIMUM then
                ({ model with State = Lose }, Cmd.navigate "")
            // TODO: Randomize damage
            else
                match enemy.Movement with
                | Chase ->

                    let player =
                        ({ model.Player with
                            Ship =
                                { model.Player.Ship with Hull = ShipHull.New(ShipHull.Value(model.Player.Ship.Hull) - 1) } })

                    match enemy.Distance with
                    | Escape ->
                        ({ model with
                            Enemy = None
                            Player = player },
                         Cmd.navigate "mainNavigationPage")
                    | Far ->
                        ({ model with
                            Enemy = Some { enemy with Distance = Escape }
                            Player = player },
                         Cmd.none)
                    | Close ->
                        ({ model with
                            Enemy = Some { enemy with Distance = Far }
                            Player = player },
                         Cmd.none)
                    | Board -> (model, Cmd.none) // TODO: Go to board battle page
                | _ ->
                    match enemy.Distance with
                    | Escape -> ({ model with Enemy = None }, Cmd.navigate "mainNavigationPage")
                    | Far -> ({ model with Enemy = Some { enemy with Distance = Escape } }, Cmd.none)
                    | Close -> ({ model with Enemy = Some { enemy with Distance = Far } }, Cmd.none)
                    | Board -> (model, Cmd.navigate "skirmishBoardBattlePage") // TODO: Go to board battle page

    | OnSkirmishSwordClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigate "mainNavigationPage")
        | Some enemy ->
            let enemyCrew = ShipCrew.Value(enemy.Ship.Crew)

            ({ model with Enemy = Some { enemy with Ship = { enemy.Ship with Crew = ShipCrew.New(enemyCrew - 1) } } },
             Cmd.none)

    | OnSkirmishFalconetClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigate "mainNavigationPage")
        | Some enemy ->
            let enemyCrew = ShipCrew.Value(enemy.Ship.Crew)

            ({ model with Enemy = Some { enemy with Ship = { enemy.Ship with Crew = ShipCrew.New(enemyCrew - 2) } } },
             Cmd.none)

    | OnSkirmishLootClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigate "mainNavigationPage")
        | Some enemy -> (model, Cmd.navigate "skirmishLootPage")

    | OnSkirmishCloseClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigateBack ())
        | Some enemy ->
            match enemy.Distance with
            | Escape -> ({ model with Enemy = Some { enemy with Distance = Far } }, Cmd.none)
            | Far -> ({ model with Enemy = Some { enemy with Distance = Close } }, Cmd.none)
            | Close -> ({ model with Enemy = Some { enemy with Distance = Board } }, Cmd.none)
            | Board -> (model, Cmd.navigate "skirmishBoardBattlePage") // TODO: Go to board battle page

    | OnSkirmishBroadsideClicked ->
        match model.Enemy with
        | None -> (model, Cmd.navigateBack ())
        | Some enemy ->
            let enemyHull = ShipHull.Value(enemy.Ship.Hull)
            // TODO: Handle sail, crew and cannons
            let enemySail = ShipSail.Value(enemy.Ship.Sail)
            let enemyCrew = ShipCrew.Value(enemy.Ship.Crew)
            let enemyCannon = ShipCannon.Value(enemy.Ship.Cannon)

            if enemyHull < Utility.SHIP_HULL_MINIMUM then
                (model, Cmd.navigate "mainNavigationPage")
            else
                let updateEnemySail sail ship =
                    // TODO: Probably redundant check?
                    if enemySail < Utility.SHIP_SAIL_MINIMUM then
                        ship
                    else
                        { ship with Sail = ShipSail.New(enemySail - sail) }

                let updateEnemyHull hull ship =
                    if enemyHull < Utility.SHIP_HULL_MINIMUM then
                        ship
                    else
                        { ship with Hull = ShipHull.New(enemyHull - hull) }

                let updateEnemyCrew crew ship =
                    if enemyCrew < Utility.SHIP_CREW_MINIMUM then
                        ship
                    else
                        { ship with Crew = ShipCrew.New(enemyCrew - crew) }

                let updateEnemyCannon cannon ship =
                    if enemyCannon < Utility.SHIP_CANNON_MINIMUM then
                        ship
                    else
                        { ship with Cannon = ShipCannon.New(enemyCrew - cannon) }

                let updateEnemy sail hull crew cannon ship =
                    Some
                        { enemy with
                            Ship =
                                ship
                                |> updateEnemySail sail
                                |> updateEnemyHull hull
                                |> updateEnemyCrew crew
                                |> updateEnemyCannon cannon }

                match enemy.Distance with
                | Escape -> ({ model with Enemy = enemy.Ship |> updateEnemy 1 1 1 1 }, Cmd.none)
                | Far -> ({ model with Enemy = enemy.Ship |> updateEnemy 2 2 2 2 }, Cmd.none)
                | Close -> ({ model with Enemy = enemy.Ship |> updateEnemy 3 3 3 3 }, Cmd.none)
                | Board -> (model, Cmd.none)


    | OnDockClicked -> (model, Cmd.navigate "dockPage")
    | OnMarketClicked -> (model, Cmd.navigate "marketPage")

    | OnWoodCargoBought location ->
        let removeFromPortCargo port =
            Utility.removeFromPortCargo Cargo._wood port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._wood playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._wood playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._wood playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

    | OnSugarCargoBought location ->
        let removeFromPortCargo port =
            Utility.removeFromPortCargo Cargo._sugar port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._sugar playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._sugar playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = Utility.addIntoPlayer port Cargo._sugar playerCargo model
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

    | OnWoodCargoSold location ->
        let removeFromPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Wood.Price)
            let ownedWoodUnit = CargoUnit.Value(ownedCargo.Wood.Unit)

            let ownedCargo =
                { model.Player.Ship.Cargo.Wood with Unit = CargoUnit.New(ownedWoodUnit - 1) }

            let ownedCargo = { model.Player.Ship.Cargo with Wood = ownedCargo }

            let ownedShip = { model.Player.Ship with Cargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins + price)
                Ship = ownedShip }

        let addIntoPortCargo port =
            let portWoodUnit = CargoUnit.Value(port.Cargo.Wood.Unit)
            let portWood = { port.Cargo.Wood with Unit = CargoUnit.New(portWoodUnit + 1) }
            let portCargo = { port.Cargo with Wood = portWood }
            { port with Cargo = portCargo }

        match location with
        | PortRoyal p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

    | OnSugarCargoSold location ->
        let removeFromPlayerCargo ownedCargo port =
            // TODO: Handle zero coins and cargo
            let coins = PlayerCoins.Value(model.Player.Coins)
            let price = CargoPrice.Value(port.Cargo.Sugar.Price)
            let ownedSugarUnit = CargoUnit.Value(ownedCargo.Sugar.Unit)

            let ownedCargo =
                { model.Player.Ship.Cargo.Wood with Unit = CargoUnit.New(ownedSugarUnit - 1) }

            let ownedCargo = { model.Player.Ship.Cargo with Sugar = ownedCargo }

            let ownedShip = { model.Player.Ship with Cargo = ownedCargo }

            { model.Player with
                Coins = PlayerCoins.New(coins + price)
                Ship = ownedShip }

        let addIntoPortCargo port =
            let portSugarUnit = CargoUnit.Value(port.Cargo.Sugar.Unit)
            let portSugar = { port.Cargo.Sugar with Unit = CargoUnit.New(portSugarUnit + 1) }
            let portCargo = { port.Cargo with Sugar = portSugar }
            { port with Cargo = portCargo }

        match location with
        | PortRoyal p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

        | Barbados p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)
        | Nassau p ->
            ({ model with
                Player = removeFromPlayerCargo model.Player.Ship.Cargo p
                Location = PortRoyal(addIntoPortCargo p) },
             Cmd.none)

    | OnUpdateOwnedShipName name ->
        let player =
            let ship = { model.Player.Ship with Name = name }
            { model.Player with Ship = ship }

        ({ model with Player = player }, Cmd.none)

    | OnUpdateOwnedShipClass shipClass ->
        let player =
            let ship = { model.Player.Ship with Class = shipClass }

            { model.Player with Ship = ship }

        ({ model with Player = player }, Cmd.none)
    | OnUpdateLocation location ->
        ({ model with
            Location = location
            Date = Date.TomorrowAfterToday(model.Date) },
         Cmd.none)
    | OnNewCharacterEntriesUpdated player -> { model with Player = player }, Cmd.none


    | OnSettingsClicked -> (model, Cmd.navigate "settingsPage")
    | OnAboutClicked -> (model, Cmd.navigate "aboutPage")
