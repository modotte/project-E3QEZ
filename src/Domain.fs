module Domain

type ShipId = ShipId of System.Guid
type ShipName = ShipName of string

type ShipSizeKind =
    | Light
    | Medium
    | Heavy
    | Flag

type ShipSize = ShipSize of ShipSizeKind

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
      Size: ShipSizeKind
      Class: ShipClass }

type Location =
    | Barbados
    | PortRoyal
    | Nassau

type PlayerFirstName = PlayerFirstName of string
type PlayerLastName = PlayerLastName of string
type PlayerCoins = PlayerCoins of int
type PlayerAge = PlayerAge of int

type Player =
    { FirstName: PlayerFirstName
      LastName: PlayerLastName
      Coins: PlayerCoins
      Age: PlayerAge
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

    | OnNewCharacterEntriesUpdated of Player

    | OnSettingsClicked
    | OnAboutClicked