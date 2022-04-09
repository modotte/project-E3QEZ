module Domain

type Nationality =
    | British
    | Spanish
    | French

type CargoName = CargoName of string

module CargoName =
    let New (x: string) = CargoName x
    let Value (CargoName x) = x

type CargoDescription = CargoDescription of string

module CargoDescription =
    let New (x: string) = CargoDescription x
    let Value (CargoDescription x) = x

type CargoPrice = CargoPrice of int

module CargoPrice =
    let New (x: int) = CargoPrice x
    let Value (CargoPrice x) = x

type CargoUnit = CargoUnit of int

module CargoUnit =
    let New (x: int) = CargoUnit x
    let Value (CargoUnit x) = x

type CargoCapacity = CargoCapacity of int

module CargoCapacity =
    let New (x: int) = CargoCapacity x
    let Value (CargoCapacity x) = x

type CargoItem =
    { Name: CargoName
      Description: CargoDescription
      Price: CargoPrice
      Unit: CargoUnit }

type Cargo = { Wood: CargoItem; Sugar: CargoItem }

type ShipId = ShipId of System.Guid

module ShipId =
    let New () = ShipId(System.Guid.NewGuid())
    let Value (ShipId x) = x

type ShipName = ShipName of string

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
    | WarGalleon
    | Frigate
    | ManOWar
    | SecondRate
    | FirstRate

type CrewCapacity = CrewCapacity of int

module CrewCapacity =
    let New (x: int) = CrewCapacity x
    let Value (CrewCapacity x) = x

type OwnedCrew = OwnedCrew of int

module OwnedCrew =
    let New (x: int) = OwnedCrew x
    let Value (OwnedCrew x) = x

type Ship =
    { Id: ShipId
      Name: ShipName
      Size: ShipSize
      Class: ShipClass
      CargoCapacity: CargoCapacity
      OwnedCargo: Cargo
      CrewCapacity: CrewCapacity
      OwnedCrew: OwnedCrew }

type PortName = PortName of string

module PortName =
    let New (x: string) = PortName x
    let Value (PortName x) = x

type PortDescription = PortDescription of string

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

type Location =
    | Barbados of Port
    | PortRoyal of Port
    | Nassau of Port

type PlayerFirstName = PlayerFirstName of string

module PlayerFirstName =
    let New (x: string) = PlayerFirstName x
    let Value (PlayerFirstName x) = x

type PlayerLastName = PlayerLastName of string

module PlayerLastName =
    let New (x: string) = PlayerLastName x
    let Value (PlayerLastName x) = x

type PlayerCoins = PlayerCoins of int

module PlayerCoins =
    let New (x: int) = PlayerCoins x
    let Value (PlayerCoins x) = x

type PlayerAge = PlayerAge of int

module PlayerAge =
    let New (x: int) = PlayerAge x
    let Value (PlayerAge x) = x

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Age: PlayerAge
      Coins: PlayerCoins
      OwnedShip: Ship }

type MusicVolume = MusicVolume of int

module MusicVolume =
    let New (x: int) = MusicVolume x
    let Value (MusicVolume x) = x

type Settings = { MusicVolume: MusicVolume }

type DaysPassed = DaysPassed of int

module DaysPassed =
    let New (x: int) = DaysPassed x
    let Value (DaysPassed x) = x

type Model =
    { DaysPassed: DaysPassed
      Location: Location
      Player: Player
      Settings: Settings
      CurrentUrl: string list }

type Msg =
    | OnFailure of string
    | OnUrlChanged of string list
    | OnMainMenuClicked

    | OnStartGameClicked
    | OnMainNavigationClicked
    | OnProfileClicked
    | OnSkirmishClicked

    | OnDockClicked
    | OnMarketClicked

    | OnWoodCargoSold of Location
    | OnWoodCargoBought of Location

    | OnUpdateOwnedShipName of ShipName
    | OnUpdateOwnedShipClass of ShipClass
    | OnUpdateLocation of Location
    | OnNewCharacterEntriesUpdated of Player

    | OnSettingsClicked
    | OnAboutClicked
