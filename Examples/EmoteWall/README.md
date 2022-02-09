# TEABot Twitch Emote Wall Example

This directory contains a minimal example configuration for an emote wall to use with Twitch (twitch.tv). This configuration does not provide any additional chatbot features, but can be freely extended.

The emote wall works via a web document that can be opened in a web browser. It is compatible with the OBS streaming software via its browser source.

To get your emote wall running, you will need the contents of this directory and its subdirectories as well as the TEABot executable from the release page: https://github.com/LordPrevious/TEABot/releases

The TEABot executable requires the .NET framework to run. If you for whatever reason don't have that installed, ask Bing for help.

Before starting the bot, you will need to adjust the configuration files for your Twitch channel and your bot's Twitch account. See below for details.

To add the emote wall to your streaming software (e.g. OBS), add a new browser source for `emotewall.html` in the `.overlay_emotewall` subdirectory. I sugegst setting the browser source's size to the intended dimensions of your stream video.

The TEABot executable needs to be running for the emote wall to work. If the browser source cannot connect to TEABot, it will display an error message in the upper right corner.

On first startup, TEABot will ask you for a configration directory. Select the directory containing this file (and, more importantly, the `config.tss` bot configuration file). If you have adjusted the configuration files correctly, TEABot will automatically connect to Twitch, enter your channel's chat room to listen for any officially recognized Twitch emotes being used, and allow the emote wall browser source to connect and receive emote events.

# Configuration files

## `config.tss`

The global configuration file.

This file contains some `key=value` entries which tell TEABot how to behave.

The most important values you have to set yourself are:

* `superUser`: A comma-separated list of Twitch usernames with privileged bot access. Enter your own Twitch username here in lowercase.
* `self`: Your bot's own display name with which it may refer to itself. I suggest entering your bot's Twitch username with your preferred capitalization.
* `login`: Enter your bot's Twitch username here in lowercase.
* `auth`: The bot's Twitch IRC password - an oauth token. To get the token for your Twitch bot, sign in to your Twitch bot account in a web browser of your choice. Then open https://twitchapps.com/tmi/ to create an IRC login oauth token for the bot account.
* `webSocketPort`: If you have any other server program running listening to port 8080, change this value and adjust it's friend in `.overlay_emotewall/emotewall.js` to match.

### Per-channel configuration

For each channel you want the bot to join, create a subdirectory in the configuration directory with the channel's name. I suggest renaming the dummy `.your twitch channel` directory to your own Twitch username in lowercase. Make sure you remove the `.` at the start as well!

Subdirectories which do not represent a channel to join should be named with a `.` prefix, so TEABot will ignore them. That's why the emote wall's browser source files are in `.overlay_emotewall` rather than `overlay_emotewall`.

Inside the channel directory, you need another `config.tss` file for channel-specific options, e.g.:

* `join`: Set to `YES` so TEABot will join the channel — `NO` to ignore it.
* `hello`: Specify a message to send to the channel when the bot joins it.
* `maxMessageCount`: Sets the limit of messages per time interval (default 30s) to adhere to Twitch's flow control requirements. Assuming your bot is a mod on your channel, 100 is fine. If not, drop down to 20.

You can also override — or append — some values from the global configuration, e.g.:

* `superUser`: Add additional usernames which will be considered to be super users in this channel only.

## `*.tsc`

TEAScript files. They make your bot go 'wheee!'

There's only one script included in this example - `emoteWall.tsc`. It listens to Twitch emotes in chat messages and notifies the browser source that it should be displayed. You don't need to touch this file.

## `*.rtf`

These richtext files are templates for TEABot's log window. Just leave 'em be.

# Wait, what? tl;dr

* Dump this directory somewhere on your PC where you'll find it again
* Edit the `config.tss` file to contain your and your Twitch bot's information.
* Rename the `.your twitch channel` directory to your Twitch username in lowercase (drop the `.`).
* Add a browser source for `.overlay_emotewall/emotewall.html`.
* Get the TEABot executable from the current release - https://github.com/LordPrevious/TEABot/releases
* Start the bot and select the directory you dumped at the promt.
* Start typin' some sweet 'motes in your Twitch chat.
