module Initializer

open Feliz
open Feliz.Router
open Elmish
open Domain

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

module ShipKind =
    let private primary =
        { Id = ShipId.New()
          Name = ShipName.New(Utility.DEFAULT_SHIP_NAME)
          Size = Light
          Class = Cutter
          CargoCapacity = CargoCapacity.New(20)
          Cargo =
            { Wood = Cargo.wood
              Sugar = Cargo.sugar }
          CrewCapacity = CrewCapacity.New(20)
          Crew = ShipCrew.New(20)
          Nationality = British
          Hull = ShipHull.New(8)
          Sail = ShipSail.New(4)
          Cannon = ShipCannon.New(4) }

    let cutter = primary

    let sloop =
        { primary with
            Class = Sloop
            CargoCapacity = CargoCapacity.New(82)
            CrewCapacity = CrewCapacity.New(40)
            Crew = ShipCrew.New(40)
            Hull = ShipHull.New(16)
            Sail = ShipSail.New(11)
            Cannon = ShipCannon.New(8) }

    let junk =
        { primary with
            Class = Junk
            CargoCapacity = CargoCapacity.New(75)
            CrewCapacity = CrewCapacity.New(35)
            Crew = ShipCrew.New(35)
            Hull = ShipHull.New(14)
            Sail = ShipSail.New(14)
            Cannon = ShipCannon.New(7) }

    let galleon =
        { primary with
            Class = Galleon
            CargoCapacity = CargoCapacity.New(152)
            CrewCapacity = CrewCapacity.New(64)
            Crew = ShipCrew.New(64)
            Hull = ShipHull.New(20)
            Sail = ShipSail.New(10)
            Cannon = ShipCannon.New(11) }

    let frigate =
        { primary with
            Class = Frigate
            CargoCapacity = CargoCapacity.New(300)
            CrewCapacity = CrewCapacity.New(125)
            Crew = ShipCrew.New(125)
            Hull = ShipHull.New(38)
            Sail = ShipSail.New(17)
            Cannon = ShipCannon.New(27) }

module Port =
    let portRoyal =
        { Name = PortName.New("Port Royal")
          Description = PortDescription.New("A rich port")
          Wealth = Rich
          Nationality = British
          Cargo =
            { Wood = { Cargo.wood with Unit = CargoUnit.New(270) }
              Sugar = { Cargo.sugar with Unit = CargoUnit.New(100) } } }

    let barbados =
        { Name = PortName.New("Barbados")
          Description = PortDescription.New("A wealthy port")
          Wealth = Prosperous
          Nationality = British
          Cargo =
            { Wood = { Cargo.wood with Unit = CargoUnit.New(167) }
              Sugar = { Cargo.sugar with Unit = CargoUnit.New(82) } } }

    let nassau =
        { Name = PortName.New("Nassau")
          Description = PortDescription.New("A poor port")
          Wealth = Average
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
               Ship = ShipKind.sloop }
           Enemy = None
           State = InProgress
           Settings = { MusicVolume = MusicVolume.New(50) }
           CurrentUrl = Router.currentUrl () },
         Cmd.none)
