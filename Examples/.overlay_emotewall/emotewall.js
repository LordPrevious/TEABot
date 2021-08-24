/* * * * * * * * * *
 * GLOBAL VARS
 * * * * * * * * * */

// average emote size in pixels
const gc_emoteBaseSizePx = 150;
// max emote size deviation in pixels
const gc_emoteSizeDeviationPx = 25;

// interval between animation frame processing in milliseconds
const gc_animationSpeed = 33; // ~ 30 FPS

// Set to true to connect to the TEABot websocket server
const gc_webSocketServerEnabled = true;
// websocket port, change if you use a different listening port with TEABot
const gc_webSocketPort = 8080;
// web socket URL derived from gc_webSocketPort
const gc_urlWebsocket = `ws://localhost:${gc_webSocketPort}/`;
// interval between connection attempts in milliseconds
const gc_webSocketConnectIntervalMs = 15000;

// The web socket to communicate with the backend
var g_webSocket = null;

// Array of emote IDs for an emote train
var g_trainCarts = [];
// Number of carts to reach in g_trainCarts to get a train going
var gc_trainThreshold = 10;
// Chance of an emote to be added to g_trainCarts, 1 being 100%
var gc_trainCartChance = 0.2;

/* * * * * * * * * *
 * OBJECT PROTOTYPES
 * * * * * * * * * */

const PEmoteInfo = {
	// DOM element representing the emote
	element: null,
	// top position in pixels
	top: 0,
	// left position in pixels
	left: 0,
	// emote size in pixels
	size: gc_emoteBaseSizePx,
	// last frame redraw timestamp, initially null
	lastFrameTimestamp: null,
	// animation-specific state information
	state: null,
	/*
	 * Set the position of the emoteInfo's DOM element according to the
	 * emoteInfo's position values
	 */
	setElementPosition: function () {
		this.element.style.top = this.top + 'px';
		this.element.style.left = this.left + 'px';
	},
	/*
	 * Randomize and set the size of the emote element.
	 * The size will be within
	 * [gc_emoteBaseSizePx - gc_emoteSizeDeviationPx, gc_emoteBaseSizePx + gc_emoteSizeDeviationPx]
	 */
	setEmoteSize: function(fScale = 1.0) {
		// randomize size
		this.size = Math.floor(((Math.random() * (2 * gc_emoteSizeDeviationPx)) + gc_emoteBaseSizePx - gc_emoteSizeDeviationPx) * fScale);
		// set style properties
		this.element.style.width = this.element.style.height = this.size + 'px';
	}
};


/* * * * * * * * * *
 * ANIMATION HELPER FUNCTIONS
 * * * * * * * * * */

/*
 * Plateau function.
 * @param x X axis offset
 * @param offset of plateau center
 * @param width Plateau width, from center of incline to center of decline
 * @param height Maximal height of the plateau
 * @param incline Steepness factor for the incline, larger is steeper
 * @param decline Steepness factor for the decline, larger is steeper
 * @returns Y axis offset
 */
function plateau(x, offset, width, height, incline, decline) {
	var offsetX = x - offset;
	var halfWidth = width / 2;
	var inclineFactor = incline / width;
	var declineFactor = decline / width;
	y = height /
		(
			( Math.exp(declineFactor * ( offsetX - halfWidth)) + 1 ) *
			( Math.exp(inclineFactor * (-offsetX - halfWidth)) + 1)
		);
	return y;
}

/* * * * * * * * * *
 * ANIMATION FUNCTIONS
 * * * * * * * * * */

/*
 * End the animation, discard the emote element
 * @param emoteInfo Contains all the info we need to track the emote
 */
function finish_animation(emoteInfo) {
	emoteInfo.element.remove();
}

/*
 * End the animation, stop the interval, discard the element
 * @param emoteInfo Contains all the info we need to track the emote
 * @param funAnimation Animation function to call on next frame redraw
 */
function continue_animation(emoteInfo, funAnimation) {
	requestAnimationFrame(function(timestamp) {
		var elapsedTime = (emoteInfo.lastFrameTimestamp != null)
			? (timestamp - emoteInfo.lastFrameTimestamp)
			: 0;
		emoteInfo.lastFrameTimestamp = timestamp;
		
		funAnimation(emoteInfo, elapsedTime);
	});
}

/*
 * Start an animation
 * @param emoteId Twitch emote ID to fetch the correct image
 * @param emoteScale Scale factor relative to default size
 * @param funCustomInit specific initialization, passed arguments: emoteInfo
 * @param funAnimation Animation function to call on next frame redraw, passed arguments: emoteInfo, elapsedTime
 */
function start_animation_common(emoteId, funCustomInit, funAnimation, emoteScale = 1.0) {
	var emote = document.createElement('img');
	if (emote) {
		emote.classList.add('emote');
		emote.src = `https://static-cdn.jtvnw.net/emoticons/v2/${emoteId}/default/dark/3.0`;
		document.body.appendChild(emote);
		var emoteInfo = Object.create(PEmoteInfo);
		emoteInfo.element = emote;
		emoteInfo.setEmoteSize(emoteScale);
		funCustomInit(emoteInfo);
		continue_animation(emoteInfo, funAnimation);
	}
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_peek(emoteInfo, elapsedTime) {
	// emoteInfo.state.step corresponds to milliseconds
	
	// check if animation has finished
	if (emoteInfo.state.step > 4000) {
		finish_animation(emoteInfo);
		return;
	}
	
	// set next position
	var y = plateau(emoteInfo.state.step, 2000, 3000, emoteInfo.element.clientHeight, 20, 40);
	emoteInfo.top = document.body.clientHeight - y;
	emoteInfo.setElementPosition();
	// increase step counter
	emoteInfo.state.step += elapsedTime;
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_peek);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_peek(emoteId) {
	start_animation_common(emoteId,
		function(emoteInfo) {
			emoteInfo.top = document.body.clientHeight;
			emoteInfo.left = Math.floor(Math.random() * (document.body.clientWidth - emoteInfo.size));
			emoteInfo.setElementPosition();
			emoteInfo.state = {
				step: 0
			};
		},
		animation_peek);
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_float(emoteInfo, elapsedTime) {
	// check if animation has finished
	if ((emoteInfo.top < -emoteInfo.size) || (emoteInfo.top > document.body.clientHeight)
		|| (emoteInfo.left < -emoteInfo.size) || (emoteInfo.left > document.body.clientWidth)) {
		finish_animation(emoteInfo);
		return;
	}
	
	// set next position
	emoteInfo.top += emoteInfo.state.speedY * elapsedTime;
	emoteInfo.left += emoteInfo.state.speedX * elapsedTime;
	emoteInfo.setElementPosition();
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_float);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_float(emoteId) {
	start_animation_common(emoteId,
		function(emoteInfo) {
			// randomize direction
			var direction = (Math.random() * 2*Math.PI) - Math.PI;
			var speedFactor = (Math.random() * 0.3) + 0.2;
			emoteInfo.state = {
				speedX: Math.cos(direction) * speedFactor,
				speedY: Math.sin(direction) * speedFactor
			};
			// randomize start position
			if (Math.abs(emoteInfo.state.speedX) > Math.abs(emoteInfo.state.speedY)) {
				if (emoteInfo.state.speedX > 0) {
					// start from left
					emoteInfo.left = -emoteInfo.size;
				} else {
					// start from right
					emoteInfo.left = document.body.clientWidth;
				}
				var halfHeight = document.body.clientHeight / 2;
				var thirdHeight = document.body.clientHeight / 3;
				var yOffset = Math.random() * thirdHeight;
				if (emoteInfo.state.speedY > 0) {
					// start on top half
					emoteInfo.top = halfHeight - yOffset;
				} else {
					// start on bottom half
					emoteInfo.top = halfHeight + yOffset;
				}
			} else {
				if (emoteInfo.state.speedY > 0) {
					// start from top
					emoteInfo.top = -emoteInfo.size;
				} else {
					// start from bottom
					emoteInfo.top = document.body.clientHeight;
				}
				var halfWidth = document.body.clientWidth / 2;
				var thirdWidth = document.body.clientWidth / 3;
				var xOffset = Math.random() * thirdWidth;
				if (emoteInfo.state.speedX > 0) {
					// start on left half
					emoteInfo.left = halfWidth - xOffset;
				} else {
					// start on right half
					emoteInfo.left = halfWidth + xOffset;
				}
			}
			emoteInfo.setElementPosition();
		},
		animation_float);
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_moon(emoteInfo, elapsedTime) {
	// check if animation has finished
	if (emoteInfo.state.scalePercent > 100) {
		finish_animation(emoteInfo);
		return;
	}
	
	// increase scale
	emoteInfo.state.scalePercent += elapsedTime * emoteInfo.state.speedFactor;
	
	// update transforms
	emoteInfo.element.style.transform = `rotate(${emoteInfo.state.rotationDeg}deg) scale(${emoteInfo.state.scalePercent/100})`;
	
	// adjust opacity
	var opacity = plateau(emoteInfo.state.scalePercent, 40, 60, 75, 10, 10);
	emoteInfo.element.style.opacity = opacity/100;
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_moon);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_moon(emoteId) {
	start_animation_common(emoteId,
		function(emoteInfo) {
			// init state
			emoteInfo.state = {
				// start small, grow until filling everything
				scalePercent: 0,
				// randomized tilt
				rotationDeg: Math.floor((Math.random()-0.5)*50),
				// randomized speed
				speedFactor: (Math.random() * 0.05) + 0.02
			};
			// initialize opacity and size
			emoteInfo.element.style.opacity = 0;
			emoteInfo.size = Math.max(document.body.clientWidth, document.body.clientHeight) * 1.5;
			emoteInfo.element.style.width = emoteInfo.element.style.height = emoteInfo.size + 'px';
			// randomize start position
			emoteInfo.top = ((document.body.clientHeight - emoteInfo.size) / 2)
				+ ((Math.random()-0.5) * gc_emoteBaseSizePx);
			emoteInfo.left = ((document.body.clientWidth - emoteInfo.size) / 2)
				+ ((Math.random()-0.5) * gc_emoteBaseSizePx);
			emoteInfo.setElementPosition();
		},
		animation_moon);
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_tumble(emoteInfo, elapsedTime) {
	// check if animation has finished
	if (emoteInfo.left > (document.body.clientWidth + emoteInfo.size)) {
		finish_animation(emoteInfo);
		return;
	}
	
	// increase rotation and location
	var increment = (elapsedTime * emoteInfo.state.speedFactor);
	emoteInfo.state.rotationDeg = ((360 + emoteInfo.state.rotationDeg - increment) % 360);
	emoteInfo.left += increment;
	emoteInfo.setElementPosition();
	
	// update transform
	emoteInfo.element.style.transform = `rotate(${emoteInfo.state.rotationDeg}deg)`;
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_tumble);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_tumble(emoteId) {
	start_animation_common(emoteId,
		function(emoteInfo) {
			// init state
			emoteInfo.state = {
				// rotation degrees, random start
				rotationDeg: Math.random() * 360,
				// randomized speed
				speedFactor: (Math.random() * 0.5) + 0.2
			};
			// randomize start position
			emoteInfo.top = 0;
			emoteInfo.left = -((Math.random() + 1) * 2 * emoteInfo.size);
			emoteInfo.setElementPosition();
		},
		animation_tumble,
		0.7);
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_gonne(emoteInfo, elapsedTime) {
	// check if animation has finished
	if (emoteInfo.top > (document.body.clientHeight + emoteInfo.size)) {
		finish_animation(emoteInfo);
		return;
	}
	
	// magic physics
	var volume = Math.pow((emoteInfo.size / 2), 2) * Math.PI / 10000;
	var fX = (emoteInfo.state.speedX == 0) ? 0
		: -0.25 * volume * Math.pow(emoteInfo.state.speedX, 3) / Math.abs(emoteInfo.state.speedX);
	var fY = (emoteInfo.state.speedY == 0) ? 0
		: -0.25 * volume * Math.pow(emoteInfo.state.speedY, 3) / Math.abs(emoteInfo.state.speedY);
	// acceleration
	var mass = emoteInfo.size / 50;
	var aX = fX / mass;
	var aY = (fY / mass) + 9.81;
	// new velocity
	emoteInfo.state.speedX += aX * elapsedTime/5000;
	emoteInfo.state.speedY += aY * elapsedTime/5000;
	
	// increase rotation and location
	var rotationSpeed = Math.atan2(emoteInfo.state.speedX, emoteInfo.state.speedY);
	emoteInfo.state.rotationDeg = ((360 + (emoteInfo.state.rotationDeg + (elapsedTime * rotationSpeed))) % 360);
	emoteInfo.left += elapsedTime * emoteInfo.state.speedX;
	emoteInfo.top += elapsedTime * emoteInfo.state.speedY;
	emoteInfo.setElementPosition();
	
	// bounce off of left wall
	if (((emoteInfo.left < 0) && (emoteInfo.state.speedX < 0))
		|| ((emoteInfo.left > (document.body.clientWidth-emoteInfo.size)) && (emoteInfo.state.speedX > 0))) {
		emoteInfo.state.speedX *= -0.25;
	}
	
	// update transform
	emoteInfo.element.style.transform = `rotate(${emoteInfo.state.rotationDeg}deg)`;
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_gonne);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_gonne(emoteId) {
	// randomize base offset and common speed
	var baseTopOffset = (document.body.clientHeight/2) + ((Math.random()-0.5)*(document.body.clientHeight/4));
	var leftToRight = Math.random() > 0.7;
	var commonSpeedFactor = Math.random() + 3;
	// randomize common initial direction
	var direction = (Math.random() * Math.PI/12) - (Math.PI*23/24);
	var commonSpeedX = Math.cos(direction) * commonSpeedFactor;
	if (leftToRight) { commonSpeedX *= -1; }
	var commonSpeedY = Math.sin(direction) * commonSpeedFactor;
	// randomized delay between projectiles
	var projectileInterval = (Math.random() * 50) + 400;
	// randomized projectile count
	var projectileCount = Math.floor((Math.random() * 4)) + 6;
	// start individual animations w/ delay
	var gonneAnimationStart = function() {
		start_animation_common(emoteId,
			function(emoteInfo) {
				// init state
				emoteInfo.state = {
					// rotation degrees, random start
					rotationDeg: Math.random() * 360,
					// use the same initial speed for all projectiles
					speedX: commonSpeedX,
					speedY: commonSpeedY
				};
				// set common start position
				emoteInfo.top = baseTopOffset;
				emoteInfo.left = leftToRight ? -gc_emoteBaseSizePx : document.body.clientWidth;
				emoteInfo.setElementPosition();
			},
			animation_gonne,
			0.5);
		};
	for (var i = 0; i < projectileCount; ++i)
	{
		setTimeout(gonneAnimationStart, i * projectileInterval);
	}
}

/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_spring(emoteInfo, elapsedTime) {
	// check if animation has finished
	if (emoteInfo.top > (document.body.clientHeight + emoteInfo.size)) {
		finish_animation(emoteInfo);
		return;
	}
	
	// magic physics
	var volume = Math.pow((emoteInfo.size / 2), 2) * Math.PI / 10000;
	var fX = (emoteInfo.state.speedX == 0) ? 0
		: -0.25 * volume * Math.pow(emoteInfo.state.speedX, 3) / Math.abs(emoteInfo.state.speedX);
	var fY = (emoteInfo.state.speedY == 0) ? 0
		: -0.25 * volume * Math.pow(emoteInfo.state.speedY, 3) / Math.abs(emoteInfo.state.speedY);
	// acceleration
	var mass = emoteInfo.size / 500;
	var aX = fX / mass;
	var aY = (fY / mass) + 9.81;
	// new velocity
	emoteInfo.state.speedX += aX * elapsedTime/5000;
	emoteInfo.state.speedY += aY * elapsedTime/5000;
	
	// update location
	emoteInfo.left += elapsedTime * emoteInfo.state.speedX;
	emoteInfo.top += elapsedTime * emoteInfo.state.speedY;
	emoteInfo.setElementPosition();
	
	// update rotation
	var rotationSpeed = Math.atan2(emoteInfo.state.speedX, emoteInfo.state.speedY) * 0.4;
	emoteInfo.state.rotationDeg = ((360 + (emoteInfo.state.rotationDeg + (elapsedTime * rotationSpeed))) % 360);
	emoteInfo.element.style.transform = `rotate(${emoteInfo.state.rotationDeg}deg)`;
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_spring);
}

/*
 * Start the animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_spring(emoteId) {
	// randomize base offset and common speed
	var baseLeftOffset = (document.body.clientWidth/2) + ((Math.random()-0.5)*(document.body.clientWidth/5));
	var commonSpeedFactor = (Math.random() * 0.5) + 1.5;
	// randomized delay between projectiles
	var projectileInterval = (Math.random() * 50) + 50;
	// randomized projectile count
	var projectileCount = Math.floor((Math.random() * 10)) + 30;
	// start individual animations w/ delay
	var springAnimationStart = function() {
		start_animation_common(emoteId,
			function(emoteInfo) {
				// randomize initial direction
				var direction = ((Math.random()-0.5) * Math.PI/12) - (Math.PI/2);
				var commonSpeedX = Math.cos(direction) * commonSpeedFactor;
				var commonSpeedY = Math.sin(direction) * commonSpeedFactor;
				// init state
				emoteInfo.state = {
					// rotation degrees, random start
					rotationDeg: Math.random() * 360,
					// use the same initial speed for all projectiles
					speedX: commonSpeedX,
					speedY: commonSpeedY
				};
				// set common start position
				emoteInfo.top = document.body.clientHeight;
				emoteInfo.left = baseLeftOffset;
				emoteInfo.setElementPosition();
			},
			animation_spring,
			0.3);
		};
	for (var i = 0; i < projectileCount; ++i)
	{
		setTimeout(springAnimationStart, i * projectileInterval);
	}
}


/*
 * Perform the animation
 * @param emoteInfo Contains all the info we need to track the emote
 * @param elapsedTime Elapsed time since last frame redraw in milliseconds
 */
function animation_train(emoteInfo, elapsedTime) {
	// check if animation has finished
	if ((emoteInfo.state.speedX == 0)
		|| ((emoteInfo.state.speedX > 0) && (emoteInfo.left > document.body.clientWidth))
		|| ((emoteInfo.state.speedX < 0) && (emoteInfo.left < -emoteInfo.element.clientWidth))) {
		// train has a chance to lap
		++emoteInfo.state.lap;
		if (Math.random() < 1/emoteInfo.state.lap) {
			// invert speed
			emoteInfo.state.speedX *= -1;
			// mirror train
			if ((emoteInfo.state.lap % 2) == 1) {
				// odd lap: not mirrored
				emoteInfo.element.style.transform = 'revert';
			} else {
				// even lap: mirrored
				emoteInfo.element.style.transform = 'scaleX(-1)';
			}
		} else {
			// no extra lap, end and clean up
			finish_animation(emoteInfo);
			return;
		}
	}
	
	// increment step
	emoteInfo.state.step += elapsedTime;
	
	// update location
	emoteInfo.left += elapsedTime * emoteInfo.state.speedX;
	emoteInfo.setElementPosition();
	
	// animate individual carts
	var rotation = Math.cos(emoteInfo.state.step/400) * 5;
	var elevationOdd = (Math.sin(emoteInfo.state.step/200)-1) * 10;
	var elevationEven = -20 - elevationOdd;
	var carts = Array.from(emoteInfo.element.childNodes);
	carts.forEach(function(item, index, array)
		{
			if ((index % 2) == 1) {
				item.style.transform = `rotate(${rotation}deg) translateY(${elevationOdd}px)`;
			} else {
				item.style.transform = `rotate(${rotation}deg) translateY(${elevationEven}px)`;
			}
		}
	);
	
	// continue animation on next frame redraw
	continue_animation(emoteInfo, animation_train);
}

/*
 * Start the animation
 */
function start_train() {
	// randomize initial direction and common speed
	var leftToRight = Math.random() < 0.5;
	var commonSpeedX = (Math.random() * 0.1) + 0.3;
	if (leftToRight) { commonSpeedX *= -1; }

	// set up special animation w/ train container of multiple emotes
	var emoteInfo = Object.create(PEmoteInfo);
	var emoteContainer = document.createElement('div');
	if (emoteContainer) {
		emoteInfo.element = emoteContainer;
		emoteContainer.classList.add('train');
		document.body.appendChild(emoteContainer);
		
		// reverse list, last addition is locomotive
		if (leftToRight) { g_trainCarts.reverse(); }
		// add all carts to container
		g_trainCarts.forEach(function(item, index, array)
			{
				var emote = document.createElement('img');
				if (emote) {
					if ((leftToRight && (index == 0))
						|| (!leftToRight && (index == array.length-1))) {
						emote.classList.add('locomotive');
					} else {
						emote.classList.add('cart');
					}
					emote.src = `https://static-cdn.jtvnw.net/emoticons/v2/${item}/default/dark/3.0`;
					emote.style.left = (index * emoteInfo.size) + 'px';
					emoteContainer.appendChild(emote);
				}
			}
		);
		// clear pending cart list
		g_trainCarts = [];
		
		// set base size as height only, width is dynamic
		emoteInfo.size = gc_emoteBaseSizePx;
		emoteContainer.style.height = emoteInfo.size + 'px';
		
		// set initial position; if going right, clientWidth is not available yet, so we overestimate
		emoteInfo.top = document.body.clientHeight - emoteInfo.size;
		emoteInfo.left = leftToRight ? document.body.clientWidth : -(gc_trainThreshold*gc_emoteBaseSizePx);
		emoteInfo.setElementPosition();
		
		// initial state
		emoteInfo.state = {
			step: 0, // elapsed time counter
			speedX: commonSpeedX, // vertical train speed
			lap: 1 // lap counter for decreasing chance of extra lap
		};
		
		continue_animation(emoteInfo, animation_train);
	}
}

/*
 * Add an emote to the pending train cart list;
 * start the train if the threshold is reached.
 * @param emoteId Twitch emote ID for image URL
 */
function addTrainCart(emoteId) {
	g_trainCarts.push(emoteId);
	if (g_trainCarts.length >= gc_trainThreshold) {
		start_train();
	}
}

/*
 * Roll the wheel for a chance to add the given emote to the pending train card list!
 * @param emoteId Twitch emote ID for image URL
 */
function trainCartLottery(emoteId) {
	if (Math.random() < gc_trainCartChance) {
		addTrainCart(emoteId);
	}
}

/*
 * Known animation start functions for start_random().
 * Repetitions increase chance of some animations.
 */
const gc_animationStarters = [
		start_peek,
		start_peek,
		start_peek,
		start_peek,
		start_float,
		start_float,
		start_float,
		start_float,
		start_float,
		start_float,
		start_moon,
		start_tumble,
		start_tumble,
		start_tumble,
		start_gonne,
		start_gonne,
		start_gonne,
		start_spring
	];

/*
 * Start a randomly picked animation
 * @param emoteId Twitch emote ID to fetch the correct image
 */
function start_random(emoteId) {
	var starter = gc_animationStarters[Math.floor(Math.random() * gc_animationStarters.length)];
	if (starter) starter(emoteId);
}

/* * * * * * * * * *
 * MISC FUNCTIONS
 * * * * * * * * * */

/**
 * Test emote stuff
 */
function test() {
	//start_random('emotesv2_b7c104b7df764573b503257a8965631f');
	//start_gonne('emotesv2_b7c104b7df764573b503257a8965631f');
	
	// set up train carts
	g_trainCarts = ['302303593',
	'302303601',
	'301591742',
	'emotesv2_d7b9f42c6c2a4cffa854847170cd610d',
	'307042831',
	'emotesv2_2edd7104f35d422fa79adbd4ef2a2f5d',
	'emotesv2_f9248debce7640ec90f681d0fc5aa024',
	'307070118',
	'emotesv2_c5b61b5444334e27b4c65a37b2dc9476',
	'emotesv2_b7c104b7df764573b503257a8965631f'];
	// start train
	start_train();
}

/*
 * Start an emote wall animation
 * @param data: message data
 */
function emote_wall(data)
{
	switch (data.style.toUpperCase()) {
	case 'PEEK':
		start_peek(data.emote);
		break;
	case 'FLOAT':
		start_float(data.emote);
		break;
	case 'MOON':
		start_moon(data.emote);
		break;
	case 'TUMBLE':
		start_tumble(data.emote);
		break;
	case 'GONNE':
		start_gonne(data.emote);
		break;
	case 'SPRING':
		start_spring(data.emote);
		break;
	default:
		// play a random animation
		start_random(data.emote);
		break;
	}
	// also play the train cart lottery
	trainCartLottery(data.emote);
}

/*
 *
 */
function setNoConnectionVisible(bVisible)
{
	// set visibility of "no connection" element
	var noConnectionElement = document.getElementById('noConnection');
	if (noConnectionElement) {
		noConnectionElement.style.display = bVisible ? 'block' : 'none';
	}
}

/*
 * Callback for g_webSocket.onopen
 */
function cb_websocket_onopen(event) {
	setNoConnectionVisible(false);
}

/*
 * Callback for g_webSocket.onclose
 */
function cb_websocket_onclose(event) {
	setNoConnectionVisible(true);
	// try to reconnect
	connect_websocket();
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
		case 'EMOTE_WALL':
			// start emote wall animation
			emote_wall(message);
			break;
		default:
			console.error("Received unknown intent.");
			break;
		}
	}
}

/*
 * Connect to the websocket server
 */
function connect_websocket() {
	if (gc_webSocketServerEnabled) {
		g_webSocket = new WebSocket(gc_urlWebsocket);
		g_webSocket.onopen = cb_websocket_onopen;
		g_webSocket.onclose = cb_websocket_onclose;
		g_webSocket.onmessage = cb_websocket_onmessage;
	}
}

/*
 * Initialize the page and connect the websocket
 */
function init() {
	// set target host info
	var targetHostElement = document.getElementById('targetHost');
	if (targetHostElement) {
		if (gc_webSocketServerEnabled) {
			targetHostElement.innerHTML = gc_urlWebsocket;
		} else {
			targetHostElement.innerHTML = "WebSocket connection disabled.";
		}
	}
	// set up websocket
	connect_websocket();
}
