import clr
clr.AddReference("RingEngine")

from RingEngine.Runtime.AVGRuntime import *
from RingEngine.Runtime.AVGRuntime.Effect import *
from RingEngine.Runtime.AVGRuntime.Script import *
from RingEngine.Runtime.Storage import *

# Placements
farleft = Placement(0.0, 300.0, 0.5)
farmiddle = Placement(700.0, 300.0, 0.5)
farright = Placement(1400.0, 300.0, 0.5)
left = Placement(0.0, 300.0, 0.8)
middle = Placement(550.0, 300.0, 0.8)
right = Placement(1100.0, 300.0, 0.8)
nearleft = Placement(0.0, 300.0, 1.0)
nearmiddle = Placement(450.0, 300.0, 1.0)
nearright = Placement(900.0, 300.0, 1.0)

# Effects
transparent = SetAlpha(0)
opaque = SetAlpha(1)
dissolve = Dissolve()
fade = Fade()

# Transition
dissolveTrans = DissolveTrans()