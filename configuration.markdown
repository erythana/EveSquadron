# Configuration

## Editing the appconfig.json file

All of the app's settings can be found in the appconfig.json file.<br />
It's important to note that you shouldn't modify the Endpoints-Setting, as these are used for future tweaks and modifications.<br />
<br />
To get started, open the appconfig.json file in a text editor of your choice.<br />
This file should be located in the same directory as the executable file.

However, if you don't want to modify the default configuration file, you can create your own file in your operating systems configuration directory.<br />
This file takes precedence over the applications defaults.

Create the file `EveSquadron.appsettings.json` in your configuration directory:
* For Linux/MacOS, place the file in `/Users/<USERNAME>/.config/`
* For Windows, place the file in `C:\Users\USERNAME\AppData\Roaming`

The configuration file needs the same layout as the default one, but you only need to set the values you want to change.
For example, a file like
```
{
  "Theme": "Light"
}
```
is perfectly fine!

## List of possible settings:

#### Customizing the Theme (optional setting)

One of the main settings you might want to customize is the theme of the app.<br />
By default, the app will use the system's default theme.<br />
However, if you want to change this, you can do so by editing the Theme setting in the appconfig.json file.

To change the theme, simply set the Theme value to either `Dark`, `Light`, or leave it empty to use the system default.
By default it is set to use your systems settings.

Example:
```
"Theme": "Dark"
```

#### Changing the Clipboard Polling Interval (optional setting)

Another setting you might want to customize is the clipboard polling interval.<br />
This determines how often the app checks the clipboard for changes.<br />

To change the interval, simply set the ClipboardPollingMilliseconds value to a number between 100 and 1000.<br />
By default, the interval is set to `250` milliseconds.<br />

Example:
```
"ClipboardPollingMilliseconds": 500
```

#### Changing the Hover Color (optional setting)

It is also possible to change the color when hovering over a player.<br />
This color is also applied to corp and alliance members (with a different opacity, though).<br />
You can use any "common" colors or pick something fancy by using one of the colors listed at https://reference.avaloniaui.net/api/Avalonia.Media/Colors/ <br />

By default, the color is set to `Orange`.

Example:
```
"HoverColor": "Orange"
```

#### Show Portraits (optional setting)

Enable or disable portraits by setting the setting `ShowPortrait` to the value `True` or `False`.<br />
By default, the value is set to `True`.

Example:
```
"ShowPortrait": "True"
```

#### Changing the Grids' Row-Size (optional setting)

To change the size of each Player-Information row, you can adjust the setting "GridRowSize".
It accepts the values `Small`, `Medium` or `Big`.<br />
This color is also applied to corp and alliance members (although with a different opacity).<br />
By default, the row size is set to `Big`.

Example:
```
"GridRowSize": "Big"
```

#### Saving Changes

Once you've made your changes, be sure to save the appconfig.json file.<br />
The app will automatically pick up the new settings the next time you run it.