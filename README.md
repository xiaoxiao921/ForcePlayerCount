# Force Player Count

Overrides the game's player count with a fixed value.

### Risk of Options

Risk of Options is required, an entry in the game Settings will appear to configure the player count in-game.

### Technical Notes

Overrides both `participatingPlayerCount` and `livingPlayerCount`. 

The later is only overriden when the custom player count value is less than the vanilla value.

This is because the variable is also used for things like `HoldOutZone` and Mithrix cutscene when the fight start and those can never trigger properly if you put a number greater than the number of player currently in the game.

As a sideffect of this, monster health scaling will never be greater than the amount of player currently alive.

## Changelog

### v1.0.0
- Initial release