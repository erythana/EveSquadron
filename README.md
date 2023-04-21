# Eve Squadron
![EveSquadron](https://user-images.githubusercontent.com/42657063/233725331-43542b46-c892-4a13-85e0-61cd9c15d49f.gif)

## Current Status
![EveSquadron-CI](https://github.com/erythana/EveSquadron/actions/workflows/EveSquadron-CI.yml/badge.svg)
![EveSquadron-CD](https://github.com/erythana/EveSquadron/actions/workflows/EveSquadron-CD.yml/badge.svg)
<br />
This tool is still in early development, but it should work just fine for now.<br />
However, there may be some bugs or glitches that need to be addressed, so please keep this in mind.<br />
Please report them to me if you encounter any problems.

## How to Use

To use this tool, simply copy the name of the player you're interested in from your local or chat window.<br />
The tool will automatically retrieve the player's information and display additional metrics in a tooltip.

### Instructions for using EveSquadron

You can use EveSquadron on all major platforms, including Windows, Linux, and MacOS.<br />
To get started, download the latest release and extract it. You can find the package in the top bar of this website.

Once you've downloaded the tool, you can run it easily on Windows by clicking the .exe file.

<br />
For Linux and MacOS, you'll need to make sure that the EveSquadron file is executable before you run it.<br />
To make it executable, run the first command. The second one will start the tool!:

```
chmod +x EveSquadron
./EveSquadron
```
Questions? Feel free to contact me in game.

### How to manually compile/run the application with dotnet
Download the source code and extract the downloaded ZIP. You can find the latest version in the "Releases" section.<br />
Run the command<br />
```
dotnet run
```
in the directory where the solution file (.sln) is located.

## Settings
If you're using the app, you might want to customize its settings to better suit your needs. Here's a quick guide to help you get started with configuring the app.

### Editing the appconfig.json file

All of the app's settings can be found in the appconfig.json file.<br />
It's important to note that you shouldn't modify the Endpoints-Setting, as these are used for future tweaks and modifications.<br />
<br />
To get started, open the appconfig.json file in a text editor of your choice.<br />
This file should be located in the same directory as the executable file.

#### Customizing the Theme

One of the main settings you might want to customize is the theme of the app.<br />
By default, the app will use the system's default theme.<br />
However, if you want to change this, you can do so by editing the Theme setting in the appconfig.json file.

To change the theme, simply set the Theme value to either "Dark", "Light", or leave it empty to use the system default. For example:

```
"Theme": "Dark"
```

#### Changing the Clipboard Polling Interval

Another setting you might want to customize is the clipboard polling interval.<br />
This determines how often the app checks the clipboard for changes.<br />
By default, the interval is set to 250 milliseconds.

To change the interval, simply set the ClipboardPollingMilliseconds value to a number between 100 and 1000.<br />
For example:

```
"ClipboardPollingMilliseconds": 500
```

#### Saving Changes

Once you've made your changes, be sure to save the appconfig.json file.<br />
The app will automatically pick up the new settings the next time you run it.


## Additional Information

If you're interested in learning more about this tool, there are a few things you should know.<br />

### Data Sources

This tool uses two main sources of data: the official ESI API and the zKillboard API.<br />
The ESI API provides information about players and their characters, while the zKillboard API provides information about the latest Fight-Related activity for each player.

It's worth noting that this tool only accesses publicly available information, and does not share any user data with anyone. So you can use it with confidence, knowing that your information is safe and secure.

### Project Inspiration

This project was inspired by "Pirates Little Helper," which was a similar tool that was no longer being maintained.<br />
The goal of this project is to provide a cross-platform version of the tool that can be used on any operating system.

So if you're a fan of "Pirates Little Helper," or just looking for a helpful tool to manage your Eve Online experience, this is definitely worth checking out.<br />
It's easy to use, and provides all the data you need to stay on top of the latest developments in the game.

## Future Improvements

The following improvements are currently planned for this tool:

### Caching and Cache Invalidation

At the moment, every clipboard copy query results in a query to the API.<br />
This can cause a slight delay, especially when there are a lot of people in local.<br />
To speed things up, caching and cache invalidation will be implemented.

### UI Improvements

There are several improvements planned for the user interface of this tool.<br />
In particular, the character tooltip with the details needs some work.<br />
Additionally, window sizes will be persisted after the app is closed.

### Improved Configuration

Currently, the appconfig.json file is used to configure settings.<br />
However, in the future, an appsettings.json file will be implemented that uses the platform's user configuration directories.<br />
This will make it easier to manage settings and ensure that they persist across different sessions.

Overall, these improvements will make this tool even more useful and user-friendly. So stay tuned for updates!

## Feedback and Donations

If you have any feature requests or suggestions for improvements, please don't hesitate to reach out!<br />
You can send an in-game mail to "Christian Gaterau" with your feedback.

Additionally, if you find this tool useful and would like to support its continued development, consider donating some ISK.<br />
Your contributions will keep me from doing PvE and i have more time so this tool remains up-to-date and useful for all Eve Online players.

`
"Give me money. Money me! Money now! Me a money needing a lot now."
`
