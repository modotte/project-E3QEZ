import { Record, Union } from "./fable_modules/fable-library.3.6.2/Types.js";
import { option_type, record_type, int32_type, union_type, string_type } from "./fable_modules/fable-library.3.6.2/Reflection.js";
import { Cmd_none } from "./fable_modules/Fable.Elmish.3.1.0/cmd.fs.js";
import { useFeliz_React__React_useElmish_Static_78C5B8C8 } from "./fable_modules/Feliz.UseElmish.1.6.0/UseElmish.fs.js";
import { createElement } from "react";
import { ofArray } from "./fable_modules/fable-library.3.6.2/List.js";
import { Interop_reactApi } from "./fable_modules/Feliz.1.57.0/Interop.fs.js";
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
    constructor(Player, Settings) {
        super();
        this.Player = Player;
        this.Settings = Settings;
    }
}

export function Model$reflection() {
    return record_type("Main.Model", [], Model, () => [["Player", option_type(Player$reflection())], ["Settings", Settings$reflection()]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["OnAboutClicked"];
    }
}

export function Msg$reflection() {
    return union_type("Main.Msg", [], Msg, () => [[]]);
}

export function init() {
    return [new Model(void 0, new Settings(new MusicVolume(0, 50))), Cmd_none()];
}

export function update(msg, model) {
    return [model, Cmd_none()];
}

export function View_mainView() {
    const patternInput = useFeliz_React__React_useElmish_Static_78C5B8C8(init(), (msg, model) => update(msg, model), []);
    const model_1 = patternInput[0];
    const dispatch = patternInput[1];
    const children = ofArray([createElement("h1", {
        children: ["Hearties"],
    }), createElement("button", {
        children: "Start Game",
    }), createElement("button", {
        children: "Settings",
    }), createElement("button", {
        children: "About",
    })]);
    return createElement("div", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
}

ReactDOM_render_Z3D10464(() => createElement(View_mainView, null), document.getElementById("feliz-app"));

//# sourceMappingURL=Main.fs.js.map
