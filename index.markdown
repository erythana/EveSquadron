# Eve Squadron
![EveSquadron](https://user-images.githubusercontent.com/42657063/233725331-43542b46-c892-4a13-85e0-61cd9c15d49f.gif)

## Current Status
![EveSquadron-CI](https://github.com/erythana/EveSquadron/actions/workflows/EveSquadron-CI.yml/badge.svg)
![EveSquadron-CD](https://github.com/erythana/EveSquadron/actions/workflows/EveSquadron-CD.yml/badge.svg)
<br />
While this tool is still in its development-phase, it should already work without any problems.<br/>
If you encounter any bugs, please report them to me at 'Github Issues' or in-game via a message to 'Christian Gaterau'.

## How to Use

To use this tool, simply copy the name of the player you're interested in from your local- or chat window.<br />
The tool will automatically retrieve the player's information and display additional metrics in a tooltip.

### Instructions for using EveSquadron

You can use EveSquadron on all major platforms, including Windows, Linux, and MacOS.<br />
To get started, download the latest release and extract it. You can find the package in the top bar of this website.

Once you've downloaded the tool, you can run it easily on Windows by double-clicking the EveSquadron.exe file.

<br />
For Linux and MacOS, you'll need to make sure that the EveSquadron file is executable before you run it.<br />
To make it executable, run the first command. The second one will start the tool:

```
chmod +x EveSquadron
./EveSquadron
```

### How to manually compile/run the application with dotnet
This is completely optional and just here for completeness:<br />
Download the source code and extract the downloaded archive. You can find the latest version in the [Releases]({{ site.github.releases_url }}) section.<br />
Run the command<br />
```
dotnet run
```
in the directory where the solution file (.sln) is located.

## Settings
If you're using the app, you might want to customize its settings to better suit your needs.<br />
Check the [Configuration](./configuration.markdown) Page to learn more about the customization of the app.

## Additional Information

If you're interested in learning more about this tool, there are a few things you should know.<br />

### Data Sources

This tool uses two main sources of data: the official ESI API and the zKillboard API.<br />
The ESI API provides information about players and their characters, while the zKillboard API provides information about the latest Fight-Related activity for each player.

It's worth noting that this tool only accesses publicly available information and does not share any user data with anyone.<br />
So you can use it with confidence, knowing that your information is safe and secure.

### Project Inspiration

This project was inspired by "Pirates Little Helper," which is a similar tool that, unfortunately, is no longer being maintained.<br />
The goal of this project is to provide a cross-platform version of the tool that can be used on any major operating system.

If you're a fan of "Pirates Little Helper," or just looking for a helpful tool to manage your Eve Online experience, this is definitely worth checking out.<br />
It's easy to use, and provides all the data you need to stay on top of the latest developments in the game.

## Future Improvements

Here is the [Roadmap](./roadmap.markdown) for this tool.
If you have any feature requests or suggestions for improvements, please don't hesitate to reach out!<br />
You can send an in-game mail to *Christian Gaterau* with your feedback or create an issue on Github.

As of release 0.5, two capsuleers already proposed features:<br />
* Whitelisting pilots 
* Scan-Export of whole view/specific pilots

## Feedback and Donations

Additionally, if you find this tool useful and would like to support its continued development, consider donating some ISK.<br />
Your contributions will keep me from doing PvE and i have more time so this tool remains up-to-date and useful for all Eve Online players.<br />
Eve character name: *Christian Gaterau*

```
"Give me money. Money me! Money now! Me a money needing a lot now."
```
