import { toString as toString_1, Record, Union } from "./fable_modules/fable-library.3.6.2/Types.js";
import { list_type, option_type, record_type, int32_type, union_type, string_type } from "./fable_modules/fable-library.3.6.2/Reflection.js";
import { fromString, Auto_generateBoxedDecoder_79988AEF } from "./fable_modules/Thoth.Json.7.0.0/Decode.fs.js";
import { bind } from "./fable_modules/fable-library.3.6.2/Option.js";
import { uncurry } from "./fable_modules/fable-library.3.6.2/Util.js";
import { toString, Auto_generateBoxedEncoder_Z20B7B430 } from "./fable_modules/Thoth.Json.7.0.0/Encode.fs.js";
import { Cmd_batch, Cmd_none, Cmd_OfFunc_attempt } from "./fable_modules/Fable.Elmish.3.1.0/cmd.fs.js";
import { isEmpty, ofArray } from "./fable_modules/fable-library.3.6.2/List.js";
import { RouterModule_router, RouterModule_urlSegments } from "./fable_modules/Feliz.Router.3.8.0/Router.fs.js";
import { printf, toConsole } from "./fable_modules/fable-library.3.6.2/String.js";
import { createElement } from "react";
import * as react from "react";
import { Interop_reactApi } from "./fable_modules/Feliz.1.57.0/Interop.fs.js";
import { useFeliz_React__React_useElmish_Static_B42E862 } from "./fable_modules/Feliz.UseElmish.1.6.0/UseElmish.fs.js";
import { singleton, delay, toList } from "./fable_modules/fable-library.3.6.2/Seq.js";
import { ReactDOM_render_Z3D10464 } from "./fable_modules/Feliz.1.57.0/ReactDOM.fs.js";
import "./styles/global.scss";


export class FirstName extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["FirstName"];
    }
}

export function FirstName$reflection() {
    return union_type("Main.FirstName", [], FirstName, () => [[["Item", string_type]]]);
}

export class LastName extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["LastName"];
    }
}

export function LastName$reflection() {
    return union_type("Main.LastName", [], LastName, () => [[["Item", string_type]]]);
}

export class Age extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Age"];
    }
}

export function Age$reflection() {
    return union_type("Main.Age", [], Age, () => [[["Item", int32_type]]]);
}

export class Player extends Record {
    constructor(FirstName, LastName, Age) {
        super();
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Age = Age;
    }
}

export function Player$reflection() {
    return record_type("Main.Player", [], Player, () => [["FirstName", FirstName$reflection()], ["LastName", LastName$reflection()], ["Age", Age$reflection()]]);
}

export class MusicVolume extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["MusicVolume"];
    }
}

export function MusicVolume$reflection() {
    return union_type("Main.MusicVolume", [], MusicVolume, () => [[["Item", int32_type]]]);
}

export class Settings extends Record {
    constructor(MusicVolume) {
        super();
        this.MusicVolume = MusicVolume;
    }
}

export function Settings$reflection() {
    return record_type("Main.Settings", [], Settings, () => [["MusicVolume", MusicVolume$reflection()]]);
}

export class Model extends Record {
    constructor(Player, Settings, CurrentUrl) {
        super();
        this.Player = Player;
        this.Settings = Settings;
        this.CurrentUrl = CurrentUrl;
    }
}

export function Model$reflection() {
    return record_type("Main.Model", [], Model, () => [["Player", option_type(Player$reflection())], ["Settings", Settings$reflection()], ["CurrentUrl", list_type(string_type)]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["OnFailure", "UrlChanged", "OnAboutClicked"];
    }
}

export function Msg$reflection() {
    return union_type("Main.Msg", [], Msg, () => [[["Item", string_type]], [["Item", list_type(string_type)]], []]);
}

const Storage_dbName = "hearties-db";

const Storage_decoder = Auto_generateBoxedDecoder_79988AEF(Model$reflection(), void 0, void 0);

export function Storage_load() {
    return bind((arg) => {
        const _arg1 = fromString(uncurry(2, Storage_decoder), arg);
        if (_arg1.tag === 0) {
            const model = _arg1.fields[0];
            return model;
        }
        else {
            return void 0;
        }
    }, localStorage.getItem(Storage_dbName));
}

export function Storage_save(model) {
    let encoder;
    localStorage.setItem(Storage_dbName, (encoder = Auto_generateBoxedEncoder_Z20B7B430(Model$reflection(), void 0, void 0, void 0), toString(1, encoder(model))));
}

export function Storage_updateStorage(update_1, message, model) {
    const setStorage = (model_1) => Cmd_OfFunc_attempt((model_2) => {
        Storage_save(model_2);
    }, model_1, (arg) => (new Msg(0, toString_1(arg))));
    if (message.tag === 0) {
        return [model, Cmd_none()];
    }
    else {
        const patternInput = update_1(message, model);
        const newModel = patternInput[0];
        const commands = patternInput[1];
        return [newModel, Cmd_batch(ofArray([setStorage(newModel), commands]))];
    }
}

export function init(_arg1) {
    if (_arg1 == null) {
        return [new Model(void 0, new Settings(new MusicVolume(0, 50)), RouterModule_urlSegments(window.location.hash, 1)), Cmd_none()];
    }
    else {
        const oldModel = _arg1;
        return [oldModel, Cmd_none()];
    }
}

export function update(msg, model) {
    switch (msg.tag) {
        case 1: {
            const segment = msg.fields[0];
            return [new Model(model.Player, model.Settings, segment), Cmd_none()];
        }
        case 2: {
            return [model, Cmd_none()];
        }
        default: {
            const err = msg.fields[0];
            toConsole(printf("%A"))(err);
            return [model, Cmd_none()];
        }
    }
}

export const View_header = (() => {
    const children = ofArray([createElement("h1", {
        children: ["Hearties"],
    }), createElement("hr", {})]);
    return createElement("div", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
})();

export function View_mainMenu(dispatch, model) {
    const children = ofArray([View_header, createElement("button", {
        children: "Start Game",
    }), createElement("button", {
        children: "Settings",
    }), createElement("button", {
        children: "About",
        onClick: (_arg1) => {
            dispatch(new Msg(2));
        },
    })]);
    return createElement("div", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
}

export function View_mainView() {
    const patternInput = useFeliz_React__React_useElmish_Static_B42E862(() => init(Storage_load()), (msg, model) => update(msg, model), []);
    const model_1 = patternInput[0];
    const dispatch = patternInput[1];
    return RouterModule_router({
        onUrlChanged: (arg_1) => {
            dispatch(new Msg(1, arg_1));
        },
        application: react.createElement(react.Fragment, {}, ...toList(delay(() => (isEmpty(model_1.CurrentUrl) ? singleton(View_mainMenu(dispatch, model_1)) : singleton(createElement("h1", {
            children: ["Not found"],
        })))))),
    });
}

ReactDOM_render_Z3D10464(() => createElement(View_mainView, null), document.getElementById("feliz-app"));

//# sourceMappingURL=Main.fs.js.map
