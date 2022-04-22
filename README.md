# Project E3QEZ

A naval combat & trading text game based in the Age of Sail to learn about
F# lens used in Fable + Feliz environment. I certainly won't finish this project into a complete form and
something end user could play. The primary goal to learn about basic getter and setter lens is already achieved. Anyways, I'll still release the code as an archive for my own future references especially for FSHarp+ lens and basic functional domain modelling (src/Domain.fs). 


Also, a thought came to mind while developing the domain model. Does single discriminated union carries overhead or is it zero cost similar to data declared with Haskell's `newtype`?


## Trading

|  Cargo   |  Base Price   |
|----------|:-------------:|
| Wood     |  20           |
| Sugar    |  27           |
| Tea      |  25           |
| Rum      |  26           |

## Buy Low, Sell High

Some ports will make some type of cargo
on demand (expensive) and some type of other over supplied (cheap) on random. Trade cargo between different ports to make profits.

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 6.0.0 or higher
* [node.js](https://nodejs.org) 14.0.0 or higher


## Editor

To write and edit your code, you can use either VS Code + [Ionide](http://ionide.io/), Emacs with [fsharp-mode](https://github.com/fsharp/emacs-fsharp-mode), [Rider](https://www.jetbrains.com/rider/) or Visual Studio.


## Development

Before doing anything, start with installing npm dependencies using `npm install`.

Then to start development mode with hot module reloading, run:
```bash
npm start
```
This will start the development server after compiling the project, once it is finished, navigate to http://localhost:8080 to view the application .

To build the application and make ready for production:
```
npm run build
```
This command builds the application and puts the generated files into the `deploy` directory (can be overwritten in webpack.config.js).

### Tests

The template includes a test project that ready to go which you can either run in the browser in watch mode or run in the console using node.js and mocha. To run the tests in watch mode:
```
npm run test:live
```
This command starts a development server for the test application and makes it available at http://localhost:8085.

To run the tests using the command line and of course in your CI server, you have to use the mocha test runner which doesn't use the browser but instead runs the code using node.js:
```
npm test
```

## License

This software is licensed under the MIT license. For more details, please see LICENSE file.
