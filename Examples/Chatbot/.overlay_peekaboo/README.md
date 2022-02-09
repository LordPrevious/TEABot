# TEABot Overlays

## Example overlay

Open the Peekaboo overlay file as a browser source in OBS — or in the browser of your choice: `.overlay_peekaboo/peekaboo.html`
After starting TEABot, the file must be reloaded to establish a websocket connection.
Automatic connection retries could be programmed into the overlay via peekaboo.js but I didn't bother with that for this example.

Type `!peek` in chat to trigger the overlay and have a little buddy pop up at the bottom.

For the corresponding example TEAScript, see `../peek.tsc`

## Stream Overlays with TEABot

TEABot is able to send simple JSON messages (flat hierarchy, string and integer values only) to all connected websocket clients. This can be used to create custom interactive overlays.

In OBS, you can add a HTML-based overlay by using a Browser Source. Make sure that the HTML document has it's style configured to use a transparent background!

By default, TEABot listens to websocket connections on `localhost:8080` — the port can be adjusted via the global `config.tss` by adding a value for `webSocketPort`.

### TEAScript examples

A JSON object containing a single value can be sent to any connected websocket client with a `hurl` statement:
```
store !intent "PEEK
hurl !intent
```
This results in a JSON object containing the variable name (without `!` prefix) and the given string constant (without `"` prefix).
```
{
  "intent" : "PEEK"
}
```

To send multiple values in one JSON object, wildcard variable names can be used:
```
store !data.intent "PEEK
store !data.speed 5
store !data.repetitions 3
hurl !data.*
```
The resulting JSON object then contains a value for every variable matching the common prefix — but excluding the prefix in the key names:
```
{
  "intent" : "PEEK",
  "speed" : 5,
  "repetitions" : 3
}
```

In case you have any variables with the same prefix that you do not want to send in the JSON object, you can delete them with the `drop` instruction before setting the values you do want to send:
```
drop !data.removeMe
```
Or, to delete all variables with a common prefix:
```
drop !data.*
```
