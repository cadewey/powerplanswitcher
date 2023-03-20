Power Plan Switcher - Changelog
=========

### Version 2.0 - March 20, 2023

* Fork from the upstream repo; new icon, new namespaces, lots of refactoring
* P/Invoke `powrprof.dll` for plan switching/inspection instead of shelling out to `powrcfg`
* Add keyboard shortcuts for switching between plans and a notification when switching via a shortcut
* Remove the theme selection options
* Add a menu entry that links to the power plan settings pane
* Move CPU plans into a sub-menu
* Add an option to run the app on startup
* Add a GPU menu/sub-menu driven by a `config.json` file (only supports Nvidia cards currently)
  * This supports changing the GPUs maximum allowed power draw
  * GPU power scaling can be "linked" to CPU power plans to auto-switch the GPU's settings when the system's power plan is changed
  * Also provide a `More Info...` menu option that displays data from various GPU sensors/values
  * One GPU power profile can be configured to automatically apply on application startup

### Version 1.2 - August 6, 2021

* Support for High-DPI displays
* Added switcher for Light and Dark mode

### Version 1.1 - July 15, 2020

* Fixed wrong icon colors

### Version 1.0.1 - October 5, 2016

* Added new icons that indicate the currently active power plan
* Refactored code to conform to C# style
* Removed unused references
* Cleaned up code
* Fixed bug when checking for new update

### Version 1.0.0 - September 16, 2016

* First release
