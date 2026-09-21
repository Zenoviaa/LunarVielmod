# Moonspiral Tower, Abyss and Ice

Finish the mini dialogue system for cutscenes
	* Make the speech bubble scale in and out when it spawns
	* Make the speech bubble arrow bob up and down/flicker when it's waiting for an input from the player
	* Block player inputs during a dialogue/cutscene?
	* In multiplayer, the person who started the cutscene should be the one required to progress it
	* The text should pop in as it's typing (typewriter effect)
	* Ideally the typewriter effect would be perfectly wrapped, actually just do what we did in Angels Moon where it just has empty characters

Simple Networking Wrapper
	* Setting up with the multiplayer helper thing is a bit clunky, would be better if we could just define packets 
	* Implement a Send/Receive function

Starr portrait lol
	* Portrait like zuis little thing for the mini dialogue system

Cave entrance to the area, put fog over it
	* Right next to the door, generate a cave, connecting to another cave somewhere in the abyss
	* We can do this by storing the connection points we had when generating the abyss and picking a location that's at least decently far to connect to

Bigger Fish Rack sprite