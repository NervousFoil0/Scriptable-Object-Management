# Scriptable-Object-Management
This example show causes a grid based inventory system, it allows for multiple instances of the same inventory base without any extra code which fit the desired 
functions of the project.

The system was designed to utilize the power of scriptable objects. The project again layers the functionality of the items that use the scriptable object behaviour.
This allows for the use of a generic base item reducing specific scriptable object type code. The layering of the items is based on a generic type of baseItem. Then
foreach type of item such a weapon, food, crafting material and so on will then inherit from the baseItem type this is where I add custom information for the data. 
Within the project scriptable objects are also used for saving and setting the game settings such as lighting quality, sound and other options. The project then 
saves the scriptable objects with the easy save plugin allowing for AES encryption of the files for a small layer of security on the data.
