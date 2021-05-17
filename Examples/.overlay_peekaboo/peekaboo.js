/* * * * * * * * * *
 * GLOBAL VARS
 * * * * * * * * * */

// websocket port, change if you use a different listening port with TEABot
const gc_webSocketPort = 8080;

// The web socket to communicate with the backend
var g_webSocket = null;


/* * * * * * * * * *
 * MISC FUNCTIONS
 * * * * * * * * * */

/**
 * Play peeking animation
 */
function peek() {
	var peeker = document.getElementById('peeker');
	if (peeker) {
		// remove animation class if present from before
		peeker.classList.remove('peekerAnimation');
		// magic to allow restarting the animation
		peeker.offsetWidth;
		// add animation class to play animation
		peeker.classList.add('peekerAnimation');
	}
}

/*
 * Callback for g_webSocket.onmessage
 */
function cb_websocket_onmessage(event) {
	// handle JSON message
	const message = JSON.parse(event.data);
	if (message && message.intent) {
		switch (message.intent) {
		case 'HELLO':
			// respond to hello
			g_webSocket.send('Hello!');
			break;
		case 'PEEK':
			// start peeking
			peek();
			break;
		default:
			console.error("Received unknown intent.");
			break;
		}
	}
}

/*
 * Initialize the page and connect the websocket
 */
function init() {
	// set up websocket
	const urlWebsocket = `ws://localhost:${gc_webSocketPort}/`;
	console.info(urlWebsocket);
	g_webSocket = new WebSocket(urlWebsocket);
	g_webSocket.onmessage = cb_websocket_onmessage;
}
