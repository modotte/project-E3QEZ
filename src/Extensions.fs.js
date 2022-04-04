import { isNullOrWhiteSpace } from "./fable_modules/fable-library.3.6.2/String.js";

export function Config_variableOrDefault(key, defaultValue) {
    const foundValue = process.env[key] ? process.env[key] : '';
    if (isNullOrWhiteSpace(foundValue)) {
        return defaultValue;
    }
    else {
        return foundValue;
    }
}

//# sourceMappingURL=Extensions.fs.js.map
