module Main exposing (..)

import Browser
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Set exposing (Set)


type FirstName
    = FirstName String


type LastName
    = LastName String


type Age
    = Age Int


type alias Player =
    { firstName : FirstName
    , lastName : LastName
    , age : Age
    }


type MusicVolume
    = MusicVolume Int


type alias Settings =
    { musicVolume : MusicVolume
    }


type alias Model =
    { player : Maybe Player
    , settings : Settings
    }


type Msg
    = OnAboutClicked
    | OnError String


main : Program () Model Msg
main =
    Browser.element
        { init = init
        , view = view
        , update = update
        , subscriptions = \_ -> Sub.none
        }


init : () -> ( Model, Cmd Msg )
init _ =
    ( { player = Nothing
      , settings = { musicVolume = MusicVolume 50 }
      }
    , Cmd.none
    )


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        OnError err ->
            Debug.todo "Not implemented"

        OnAboutClicked ->
            Debug.todo "Not implemented"


mainMenuView : Model -> Html Msg
mainMenuView model =
    div []
        [ h1 [] [ text "Hearties!" ]
        , hr [] []
        , button [] [ text "Start Game" ]
        , button [] [ text "Settings" ]
        , button [ onClick OnAboutClicked ] [ text "About" ]
        ]


view : Model -> Html Msg
view model =
    div []
        [ mainMenuView model ]
