﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakingTheFourth
{
    public enum Movement
    {
        Vertical,
        Horizontal,
        None
    }
    //Contributors:
    //Mike O'Donnell - Helped plan on logic for getting the block to move up and down a constant distance. Also added the beginning comments for outlining
    //Kat Weis - did inheriting from terrain and adjustments to make that work, adjusted special terrain so that disappearing platforms 
    //could inherit from it and also tweaked it so they can be different colors, horizontal moving platforms, movement enum
    //Matt Lienhard - worked on moving platforms mainly
    class SpecialTerrain : Terrain
    {
        //This class will inherit from Terrain
        //Since these will all likely be moving platforms, we will definitely need x and y coordinates and speed values for these
        //Again the same question with the Collision Detection. Do we need a method for all of them?
        //These will definitely require a parameterized constructor.
        //Separate method for spikes?

        // Constructor
        protected Texture2D image;
        private Rectangle position;
        private bool movingUp = false;
        private bool movingLeft = false;
        //max and mins
        private int maxY = -1;
        private int minY = -1;
        private int maxX = -1;
        private int minX = -1;
        private Color tint;
        Movement direction;
        FileIO movement = new FileIO();

        public SpecialTerrain(int x, int y, int width, int height, int max, int min, Movement axis, Color color) : base (x, y,width, height, color)
        {
            tint = color;
            direction = axis;
            image = base.Image;
            position = new Rectangle(x, y, width, height);
            //determines where to set max and min and where its moving
            if(axis == Movement.Horizontal)
            {
                movingLeft = true;
                maxX = max;
                minX = min;
            }
            else if (axis == Movement.Vertical)
            {
                movingUp = true;
                maxY = max;
                minY = min;
            }
        }

        // properties
        public int MaxY
        {
            get { return maxY; }
        }

        public int MinY
        {
            get { return minY; }
        }
        public int MaxX
        {
            get { return maxX; }
        }

        public int MinX
        {
            get { return minX; }
        }
        public bool MovingUp
        {
            get { return movingUp; }
        }
        public bool MovingLeft
        {
            get { return movingLeft; }
        }

        // moving platforms
        public override void Update()
        {
            //moving up
            if (movingUp == true)
            {
                Y--;
                if(Y <= maxY)
                {
                    movingUp = false;
                }
            }
            //moving left
            else if(movingLeft == true)
            {
                X--;
                if (X <= minX)
                {
                    movingLeft = false;
                }
            }
            //moving down
            else if(direction == Movement.Vertical && movingUp == false)
            {
                Y++;
                if(Y >= minY)
                {
                    movingUp = true;
                }
            }
            //moving right
            else if(movingLeft == false && direction == Movement.Horizontal)
            {
                X++;
                if (X >= maxX)
                {
                    movingLeft = true;
                }
            }
            
        }
    }
}
