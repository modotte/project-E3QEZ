module UpdateHandler

open FSharpPlus.Lens
open FunctorLens

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
            Reflection.FSharpValue.MakeUnion(Utility.Random.ofChoice (cases), [||]) :?> ShipDistance

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
        let addIntoPlayer port playerCargo =
            Utility.addIntoPlayer port Cargo._wood playerCargo model

        let removeFromPortCargo port =
            Utility.removeFromPortCargo Cargo._wood port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

    | OnSugarCargoBought location ->
        let addIntoPlayer port playerCargo =
            Utility.addIntoPlayer port Cargo._sugar playerCargo model

        let removeFromPortCargo port =
            Utility.removeFromPortCargo Cargo._sugar port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = addIntoPlayer port playerCargo
                Location = PortRoyal(removeFromPortCargo port) },
             Cmd.none)

    | OnWoodCargoSold location ->
        let removeFromPlayer port playerCargo =
            Utility.removeFromPlayer port Cargo._wood playerCargo model

        let addIntoPortCargo port =
            Utility.addIntoPortCargo Cargo._wood port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
             Cmd.none)

    | OnSugarCargoSold location ->
        let removeFromPlayer port playerCargo =
            Utility.removeFromPlayer port Cargo._sugar playerCargo model

        let addIntoPortCargo port =
            Utility.addIntoPortCargo Cargo._sugar port

        let playerCargo = model.Player.Ship.Cargo

        match location with
        | PortRoyal port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
             Cmd.none)

        | Barbados port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
             Cmd.none)
        | Nassau port ->
            ({ model with
                Player = removeFromPlayer port playerCargo
                Location = PortRoyal(addIntoPortCargo port) },
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
    | OnLocationTravel location ->
        let randomizedCargo port =
            let min = 0
            let max = 500

            port
            |> (Port._cargo << Cargo._wood << CargoItem._unit)
               .->> CargoUnit.New(Utility.Random.ofRange min max)
            |> (Port._cargo << Cargo._sugar << CargoItem._unit)
               .->> CargoUnit.New(Utility.Random.ofRange min max)

        ({ model with
            Location =
                match location with
                | PortRoyal port -> PortRoyal(randomizedCargo port)
                | Barbados port -> Barbados(randomizedCargo port)
                | Nassau port -> Nassau(randomizedCargo port)
            Date = Date.Forward(Utility.Random.ofRange 1 5, model.Date) },
         Cmd.none)
    | OnNewCharacterEntriesUpdated player -> { model with Player = player }, Cmd.none


    | OnSettingsClicked -> (model, Cmd.navigate "settingsPage")
    | OnAboutClicked -> (model, Cmd.navigate "aboutPage")
