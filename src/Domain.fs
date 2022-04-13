module Domain

open FSharpPlus.Lens

type Nationality =
    | British
    | Spanish
    | French

type CargoName = private CargoName of string

module CargoName =
    let New (x: string) = CargoName x
    let Value (CargoName x) = x

type CargoDescription = private CargoDescription of string

module CargoDescription =
    let New (x: string) = CargoDescription x
    let Value (CargoDescription x) = x

type CargoPrice = private CargoPrice of int

module CargoPrice =
    let New (x: int) = CargoPrice x
    let Value (CargoPrice x) = x

type CargoUnit = private CargoUnit of int

module CargoUnit =
    let New (x: int) = CargoUnit x
    let Value (CargoUnit x) = x

type CargoCapacity = private CargoCapacity of int

module CargoCapacity =
    let New (x: int) = CargoCapacity x
    let Value (CargoCapacity x) = x

type CargoItem =
    { Name: CargoName
      Description: CargoDescription
      Price: CargoPrice
      Unit: CargoUnit }

module CargoItem =
    let inline _name f p =
        f p.Name <&> fun x -> { p with Name = x }

    let inline _description f p =
        f p.Description
        <&> fun x -> { p with Description = x }

    let inline _price f p =
        f p.Price <&> fun x -> { p with Price = x }

    let inline _unit f u =
        f u.Unit <&> fun x -> { u with Unit = x }

type Cargo = { Wood: CargoItem; Sugar: CargoItem }

module Cargo =
    let inline _wood f c =
        f c.Wood <&> fun x -> { c with Wood = x }

    let inline _sugar f c =
        f c.Sugar <&> fun x -> { c with Sugar = x }

type ShipId = private ShipId of System.Guid

module ShipId =
    let New () = ShipId(System.Guid.NewGuid())
    let Value (ShipId x) = x

type ShipName = private ShipName of string

module ShipName =
    let New (x: string) = ShipName x
    let Value (ShipName x) = x

type ShipSize =
    | Light
    | Medium
    | Heavy
    | Flag

type ShipClass =
    | Cutter
    | Sloop
    | Brig
    | Junk
    | Galleon
    | Frigate

type CrewCapacity = private CrewCapacity of int

module CrewCapacity =
    let New (x: int) = CrewCapacity x
    let Value (CrewCapacity x) = x

type ShipCrew = private ShipCrew of int

module ShipCrew =
    let New (x: int) = ShipCrew x
    let Value (ShipCrew x) = x

type ShipHull = private ShipHull of int

module ShipHull =
    let New (x: int) = ShipHull x
    let Value (ShipHull x) = x

type ShipSail = private ShipSail of int

module ShipSail =
    let New (x: int) = ShipSail x
    let Value (ShipSail x) = x

type ShipCannon = private ShipCannon of int

module ShipCannon =
    let New (x: int) = ShipCannon x
    let Value (ShipCannon x) = x

type Ship =
    { Id: ShipId
      Name: ShipName
      Size: ShipSize
      Class: ShipClass
      Cargo: Cargo
      CargoCapacity: CargoCapacity
      Crew: ShipCrew
      CrewCapacity: CrewCapacity
      Nationality: Nationality
      Hull: ShipHull
      Sail: ShipSail
      Cannon: ShipCannon }

module Ship =
    let inline _id f p = f p.Id <&> fun x -> { p with Id = x }

    let inline _name f p =
        f p.Name <&> fun x -> { p with Name = x }

    let inline _size f p =
        f p.Size <&> fun x -> { p with Size = x }

    let inline _class f p =
        f p.Class <&> fun x -> { p with Class = x }

    let inline _cargo f p =
        f p.Cargo <&> fun x -> { p with Cargo = x }

    let inline _cargoCapacity f p =
        f p.CargoCapacity
        <&> fun x -> { p with CargoCapacity = x }

    let inline _crew f p =
        f p.Crew <&> fun x -> { p with Crew = x }

    let inline _crewCapacity f p =
        f p.CrewCapacity
        <&> fun x -> { p with CrewCapacity = x }

    let inline _nationality f p =
        f p.Nationality
        <&> fun x -> { p with Nationality = x }

    let inline _hull f p =
        f p.Hull <&> fun x -> { p with Hull = x }

    let inline _sail f p =
        f p.Sail <&> fun x -> { p with Sail = x }

    let inline _cannon f p =
        f p.Cannon <&> fun x -> { p with Cannon = x }

type ShipMovement =
    | Chase
    | Still
    | Evade

type ShipDistance =
    | Escape
    | Far
    | Close
    | Board

type Enemy =
    { Ship: Ship
      Movement: ShipMovement
      Distance: ShipDistance }

type PortName = private PortName of string

module PortName =
    let New (x: string) = PortName x
    let Value (PortName x) = x

type PortDescription = private PortDescription of string

module PortDescription =
    let New (x: string) = PortDescription x
    let Value (PortDescription x) = x

type PortSize =
    | Small
    | Medium
    | Large

type Port =
    { Name: PortName
      Description: PortDescription
      Size: PortSize
      Nationality: Nationality
      Cargo: Cargo }

module Port =
    let inline _name f p =
        f p.Name <&> fun x -> { p with Name = x }

    let inline _description f p =
        f p.Description
        <&> fun x -> { p with Description = x }

    let inline _size f p =
        f p.Size <&> fun x -> { p with Size = x }

    let inline _cargo f p =
        f p.Cargo <&> fun x -> { p with Cargo = x }

type Location =
    | Barbados of Port
    | PortRoyal of Port
    | Nassau of Port

type PlayerFirstName = private PlayerFirstName of string

module PlayerFirstName =
    let New (x: string) = PlayerFirstName x
    let Value (PlayerFirstName x) = x

type PlayerLastName = private PlayerLastName of string

module PlayerLastName =
    let New (x: string) = PlayerLastName x
    let Value (PlayerLastName x) = x

type PlayerCoins = private PlayerCoins of int

module PlayerCoins =
    let New (x: int) = PlayerCoins x
    let Value (PlayerCoins x) = x

type PlayerAge = private PlayerAge of int

module PlayerAge =
    let New (x: int) = PlayerAge x
    let Value (PlayerAge x) = x

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Age: PlayerAge
      Coins: PlayerCoins
      Ship: Ship }

module Player =
    let inline _firstName f p =
        f p.FirstName
        <&> fun x -> { p with FirstName = x }

    let inline _lastName f p =
        f p.LastName <&> fun x -> { p with LastName = x }

    let inline _age f p = f p.Age <&> fun x -> { p with Age = x }

    let inline _coins f p =
        f p.Coins <&> fun x -> { p with Coins = x }

    let inline _ship f p =
        f p.Ship <&> fun x -> { p with Ship = x }

type MusicVolume = private MusicVolume of int

module MusicVolume =
    let New (x: int) = MusicVolume x
    let Value (MusicVolume x) = x

type Settings = { MusicVolume: MusicVolume }

module Settings =
    let inline _musicVolume f p =
        f p.MusicVolume
        <&> fun x -> { p with MusicVolume = x }

type Date = private Date of System.DateTime

module Date =
    open System
    open Fable.DateFunctions

    let New () = Date(DateTime(1650, 1, 1, 1, 0, 0))

    let Value (Date x) = x
    let TomorrowAfterToday (Date x) = Date <| x.AddDays(1.0)
    let Formatted (Date x) = string <| x.Format("dd MMMM yyyy")

/// There's no Win game state. This
/// game is practically a sandbox combat &
/// trading simulation.
type GameState =
    | InProgress
    | Lose

type Model =
    { Date: Date
      Location: Location
      Player: Player
      Enemy: Enemy option
      State: GameState
      Settings: Settings
      CurrentUrl: string list }

module Model =
    let inline _date f p =
        f p.Date <&> fun x -> { p with Date = x }

    let inline _location f p =
        f p.Location <&> fun x -> { p with Location = x }

    let inline _player f p =
        f p.Player <&> fun x -> { p with Player = x }

    let inline _enemy f p =
        f p.Enemy <&> fun x -> { p with Enemy = x }

    let inline _state f p =
        f p.State <&> fun x -> { p with State = x }

    let inline _settings f p =
        f p.Settings <&> fun x -> { p with Settings = x }

    let inline _currentUrl f p =
        f p.CurrentUrl
        <&> fun x -> { p with CurrentUrl = x }

    let inline _playerShipCargoWoodUnit x =
        _player
        << Player._ship
        << Ship._cargo
        << Cargo._wood
        << CargoItem._unit
        <| x

type Msg =
    | OnFailure of string
    | OnUrlChanged of string list
    | OnMainMenuClicked

    | OnStartGameClicked
    | OnMainNavigationClicked
    | OnProfileClicked

    | OnSkirmishClicked
    | OnSkirmishEvadeClicked
    | OnSkirmishCloseClicked
    | OnSkirmishBroadsideClicked
    | OnSkirmishSwordClicked
    | OnSkirmishFalconetClicked
    | OnSkirmishLootClicked

    | OnDockClicked
    | OnMarketClicked

    | OnWoodCargoBought of Location
    | OnWoodCargoSold of Location

    | OnSugarCargoBought of Location
    | OnSugarCargoSold of Location

    | OnUpdateOwnedShipName of ShipName
    | OnUpdateOwnedShipClass of ShipClass
    | OnUpdateLocation of Location
    | OnNewCharacterEntriesUpdated of Player

    | OnSettingsClicked
    | OnAboutClicked
