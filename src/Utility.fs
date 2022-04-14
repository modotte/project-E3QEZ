[<RequireQualifiedAccess>]
module Utility

open FunctorLens
open Domain

[<Literal>]
let PLAYER_MIN_AGE = 18

[<Literal>]
let PLAYER_MAX_AGE = 60

[<Literal>]
let DEFAULT_SHIP_NAME = "Heart of Ocean"

[<Literal>]
let SHIP_HULL_MINIMUM = 4

[<Literal>]
let SHIP_SAIL_MINIMUM = 2

[<Literal>]
let SHIP_CREW_MINIMUM = 2

[<Literal>]
let SHIP_CANNON_MINIMUM = 2

let currentLocation =
    function
    | Barbados p -> PortName.Value(p.Name)
    | PortRoyal p -> PortName.Value(p.Name)
    | Nassau p -> PortName.Value(p.Name)

/// `op` should only be either + or - for adding or remove player coins
let private updatePlayerCoins op coins cargoItem port =
    let price =
        CargoPrice.Value(
            port
            ^.. (Port._cargo << cargoItem << CargoItem._price)
        )

    PlayerCoins.New((op) (PlayerCoins.Value(coins)) price)

// Buying
let private addIntoPlayerCargo cargoItem playerCargo model =
    let cargoItemUnit = CargoUnit.Value(playerCargo ^.. (cargoItem << CargoItem._unit))

    let ship =
        model.Player.Ship
        |> (Ship._cargo << cargoItem << CargoItem._unit)
           .->> (CargoUnit.New(cargoItemUnit + 1))

    { model.Player with Ship = ship }

let addIntoPlayer port cargoItem playerCargo model =
    let player = addIntoPlayerCargo cargoItem playerCargo model

    player
    |> Player._coins
       .->> (updatePlayerCoins (-) player.Coins cargoItem port)


let removeFromPortCargo (cargoItem: (CargoItem -> Functor<CargoItem>) -> Cargo -> Functor<Cargo>) (port: Port) =
    let cargoItemUnit =
        CargoUnit.Value(
            port
            ^.. (Port._cargo << cargoItem << CargoItem._unit)
        )

    port
    |> (Port._cargo << cargoItem << CargoItem._unit)
       .->> CargoUnit.New(cargoItemUnit - 1)

// Selling
let private removeFromPlayerCargo cargoItem playerCargo model =
    let cargoItemUnit = CargoUnit.Value(playerCargo ^.. (cargoItem << CargoItem._unit))

    let ship =
        model.Player.Ship
        |> (Ship._cargo << cargoItem << CargoItem._unit)
           .->> (CargoUnit.New(cargoItemUnit - 1))

    { model.Player with Ship = ship }

let removeFromPlayer port cargoItem playerCargo model =
    let player = addIntoPlayerCargo cargoItem playerCargo model

    player
    |> Player._coins
       .->> (updatePlayerCoins (+) player.Coins cargoItem port)

let addIntoPortCargo (cargoItem: (CargoItem -> Functor<CargoItem>) -> Cargo -> Functor<Cargo>) (port: Port) =
    let cargoItemUnit =
        CargoUnit.Value(
            port
            ^.. (Port._cargo << cargoItem << CargoItem._unit)
        )

    port
    |> (Port._cargo << cargoItem << CargoItem._unit)
       .->> CargoUnit.New(cargoItemUnit + 1)
