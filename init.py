from __future__ import annotations
import clr
clr.AddReference("RingEngine")

from RingEngine.Core import *
from RingEngine.Core.General import *
from RingEngine.Core.Animation import *
from RingEngine.Core.Script import *
from RingEngine.Core.Storage import *

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

def Farleft(path: str, x: float, y: float) -> Placement:
    p = Placement(0.0, 0.0, 1.0)
    if "红叶" in path:
        p.x = 500.0
    return p


# Effects
transparent = SetAlpha.Transparent
opaque = SetAlpha.Opaque
Dissolve = OpacityEffect.Dissolve
Fade = OpacityEffect.Fade
dissolve = Dissolve(0.8)
fade = Fade(0.8)

# Transition
dissolveTrans = DissolveTrans()
slide = ImageTrans()

# Image Load Hook
def GetPlacementForImage(path: str, x: float, y: float):
    pos = Placement(0.0, 0.0, 1.0)
    if path.startswith("res://assets/images/bg"):
        pos.scale = max(1920 / x, 1080 / y)
    return pos