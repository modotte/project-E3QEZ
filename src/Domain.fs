module Domain

type CargoKind =
    | Wood
    | Sugar
    | Fish
    | DriedMeat

type CargoBasePrice = CargoBasePrice of int
type CargoUnit = CargoUnit of int

type CargoCapacity = CargoCapacity of int

type Cargo =
    { Kind: CargoKind
      BasePrice: CargoBasePrice
      Unit: CargoUnit }

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

type Ship =
    { Id: ShipId
      Name: ShipName
      Size: ShipSize
      Class: ShipClass
      CargoCapacity: CargoCapacity
      OwnedCargo: Cargo array }

type Location =
    | Barbados
    | PortRoyal
    | Nassau
    | Havana

type PlayerFirstName = PlayerFirstName of string
type PlayerLastName = PlayerLastName of string
type PlayerCoins = PlayerCoins of int
type PlayerAge = PlayerAge of int

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Age: PlayerAge
      Coins: PlayerCoins
      OwnedShip: Ship option
      CurrentLocation: Location }

type MusicVolume = MusicVolume of int
type Settings = { MusicVolume: MusicVolume }

type Model =
    { Player: Player
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

    | OnNewCharacterEntriesUpdated of Player

    | OnSettingsClicked
    | OnAboutClicked
