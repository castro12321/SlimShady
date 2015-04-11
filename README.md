# SlimShady
Change your monitor(s) brightness. Because no app could manage multiple monitors or be comfortable enough for everyday use.

You can also connect SlimShady to an Arduino/RaspberryPi or other microcontroller to setup automatic brightness adjustments based on the light level in your room

The app is providing four methods of setting the brightness:

1: Hardware using WinApi and DDC/CI standard. May not work for TV screens and older monitors

2: Hardware using WinApi WmiSetBrightness. This is usually used for working with laptops and (probably) won't work on PCs

3: Gamma ramps. Efficient method to set brightness if your monitor doesn't support DDC/CI

4: Software using semi-transparent topmost window layer. Used mostly as a fallback when all above fail

Currently Windows only!
