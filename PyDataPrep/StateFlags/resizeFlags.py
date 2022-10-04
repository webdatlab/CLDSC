#!/usr/bin/python
from PIL import Image
import os, sys

path = sys.argv[1]
dirs = os.listdir( path )

def resize():
    for item in dirs:
        if os.path.isfile(path+item):
            im = Image.open(path+item)
            f, e = os.path.splitext(path+item)
            imResize = im.resize((128,64), Image.ANTIALIAS)
            imResize.save(f+".png", 'PNG', optimize=True)

resize()