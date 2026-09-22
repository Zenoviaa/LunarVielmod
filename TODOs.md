# Moonspiral Tower, Abyss and Ice
Finish the mini dialogue system for cutscenes
* Block player inputs during a dialogue/cutscene? only listen for control use item but don't allow using items

Simple Networking Wrapper
* Setting up with the multiplayer helper thing is a bit clunky, would be better if we could just define packets 
* Implement a Send/Receive function

Starr portrait lol
* Portrait like zuis little thing for the mini dialogue system

Cave entrance to the area, put fog over it
* Right next to the door, generate a cave, connecting to another cave somewhere in the abyss
* We can do this by storing the connection points we had when generating the abyss and picking a location that's at least decently far to connect to

Bigger Fish Rack sprite

Bunny Storm
* Make his bunnies form the shape of the attack he's doing instead of having the glow mask over top
* We could manually set the points, that'd likely be the easiest way to do it, another way is to try generating a mesh

# Big Dialogue Overhaul
* Have the same visuals as the small one but bigger, the buttons have the gradient and whatnot, and each character is going to have unique shaders for their panels

# Misc
* Add helper functions for generating edges of a trail, triangular, circular, etc, would prevent the need for making head sprites
* Merge most math helper functions into one class for ease of use
* Look into better circular primitives
* Get rid of all the other asset registry systems and just use asset refs
* Move all UI textures to Assets/Textures/UI and update asset refs, hardcode the height/width values when necessary instead of force loading the assets on load
* Move all effect textures into Assets/Textures/VFX and update asset refs
* I don't really like having both a Core and Common folder, move everything from Core to Common. Should be able to just do a global find replace of "Core" to "Common" and then move the scripts