module Domain

type Nationality =
    | British
    | Spanish
    | French

type CargoName = CargoName of string
type CargoDescription = CargoDescription of string
type CargoPrice = CargoPrice of int
type CargoUnit = CargoUnit of int

type CargoCapacity = CargoCapacity of int

type CargoItem =
    { Name: CargoName
      Description: CargoDescription
      Price: CargoPrice
      Unit: CargoUnit }

type Cargo = { Wood: CargoItem; Sugar: CargoItem }

type ShipId = ShipId of System.Guid
type ShipName = ShipName of string

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
type OwnedCrew = OwnedCrew of int

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
type PortDescription = PortDescription of string

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
type PlayerLastName = PlayerLastName of string
type PlayerCoins = PlayerCoins of int
type PlayerAge = PlayerAge of int

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Age: PlayerAge
      Coins: PlayerCoins
      OwnedShip: Ship
      CurrentLocation: Location }

type MusicVolume = MusicVolume of int
type Settings = { MusicVolume: MusicVolume }

type DaysPassed = DaysPassed of int

type Model =
    { DaysPassed: DaysPassed
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
