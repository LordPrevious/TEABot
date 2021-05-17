# TEABot Examples

This directory contains an example configuration and some basic TEAScripts to get you started.

On first startup, TEABot will ask you for a configration directory. Provide a path which contains your `config.tss` and scripts. You can use the files from this example directory as a basis.

## `config.tss`

The global configuration file.

This file contains some `key=value` entries which tell TEABot how to behave.

The most important values you have to set yourself are:

* `superUser`: A comma-separated list of usernames which may have special access authorization. You can check if a message sender is a super user in TEAScript via the `$isSuperUser` context value.
* `self`: Your bot's own display name with which it may refer to itself. Can be accessed in TEAScript via the `$self` context value.
* `login`: The bot's IRC nickname.
* `auth`: The bot's IRC password, e.g. an oauth token.

### Per-channel configuration

For each IRC channel you want the bot to join, create a subdirectory in the configuration directory with the channel name, but without the `#` prefix.

Subdirectories which do not represent a channel to join should be named with a `.` prefix, so TEABot will ignore them.

Inside the channel directory, create another `config.tss` file for channel-specific options, e.g.:

* `join`: Set to `YES` so TEABot will join the channel — `NO` to ignore it.
* `hello`: Specify a message to send to the channel when the bot joins it.

You can also override — or append — some values from the global configuration:

* `superUser`: Add additional usernames which will be considered to be super users in this channel only.

## `*.rtf`

These richtext files are templates for TEABot's log window. You can probably use them as they are. Modify them if you think you can make it look prettier.

## `*.tsc`

TEAScript files. They make your bot go 'wheee!'

TEAScripts can be added globally in the root directory — so they're executed for any channel the bot has joined — or per channel by adding them to the respective channel's subdirectory.

A TEAScript starts off with a block of metadata — the script's name, a description, and trigger conditions. A script may be triggered periodically or in reaction to a chat message containing a command keyword or matching a regular expression.

When a TEAScript is triggered by a chat message, required or optional parameters can be provided in the message. A parameter can either be a number, a string not containing any whitespace, or the remaining part of the message.

After the metadata and parameter blocks, the script's instruction statements follow. Take a look at the provided files for some possibilities.


A comprehensive list of supported metadata, parameter and instruction statements will follow at a later time.
